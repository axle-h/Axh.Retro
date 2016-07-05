using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axh.Retro.CPU.Common.Contracts.Exceptions;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;

namespace Axh.Retro.CPU.Common.Memory
{
    public class SegmentMmu : IMmu
    {
        private readonly IDmaController _dmaController;
        private readonly IInstructionTimer _instructionTimer;

        private readonly List<AddressRange> _lockedAddressRanges; // TODO: respect these.
        private readonly ushort[] _readSegmentAddresses;
        private readonly IReadableAddressSegment[] _readSegments;

        private readonly ushort[] _writeSegmentAddresses;
        private readonly IWriteableAddressSegment[] _writeSegments;

        private bool _disposed;

        public SegmentMmu(IEnumerable<IAddressSegment> addressSegments,
            IDmaController dmaController,
            IInstructionTimer instructionTimer)
        {
            _dmaController = dmaController;
            _instructionTimer = instructionTimer;
            var sortedSegments = addressSegments.OrderBy(x => x.Address).ToArray();

            _readSegments = sortedSegments.OfType<IReadableAddressSegment>().ToArray();
            _readSegmentAddresses = _readSegments.Select(x => x.Address).ToArray();

            _writeSegments = sortedSegments.OfType<IWriteableAddressSegment>().ToArray();
            _writeSegmentAddresses = _writeSegments.Select(x => x.Address).ToArray();

            CheckSegments(_readSegments);
            CheckSegments(_writeSegments);

            _lockedAddressRanges = new List<AddressRange>();

            // Dma task.
            Task.Factory.StartNew(DmaTask, TaskCreationOptions.LongRunning);
        }

        public byte ReadByte(ushort address)
        {
            ushort segmentAddress;
            var addressSegment = GetAddressSegmentForAddress(_readSegmentAddresses,
                _readSegments,
                address,
                out segmentAddress);
            return addressSegment.ReadByte(segmentAddress);
        }

        public ushort ReadWord(ushort address)
        {
            ushort segmentAddress;
            int segmentIndex;
            IReadableAddressSegment segment;
            if (TryGetSegmentIndexForAddress(_readSegmentAddresses,
                _readSegments,
                address,
                sizeof (ushort),
                out segment,
                out segmentIndex,
                out segmentAddress))
            {
                return _readSegments[segmentIndex].ReadWord(segmentAddress);
            }

            // Read one byte from the end of the returned segment index and another from the start of the next
            var lsb = segment.ReadByte(segmentAddress);
            var msb = _readSegments[(segmentIndex + 1) % _readSegments.Length].ReadByte(0);
            return BitConverter.ToUInt16(new[] {lsb, msb}, 0);
        }

        public byte[] ReadBytes(ushort address, int length)
        {
            ushort segmentAddress;
            int segmentIndex;
            IReadableAddressSegment segment;
            if (TryGetSegmentIndexForAddress(_readSegmentAddresses,
                _readSegments,
                address,
                length,
                out segment,
                out segmentIndex,
                out segmentAddress))
            {
                return _readSegments[segmentIndex].ReadBytes(segmentAddress, length);
            }

            var bytes = new byte[length];

            // Read from first segment
            var nextSegment = _readSegments[segmentIndex];
            var segmentLength = nextSegment.Length - segmentAddress;
            var nextBytes = nextSegment.ReadBytes(segmentAddress, segmentLength);
            Array.Copy(nextBytes, 0, bytes, 0, segmentLength);
            var lengthRemaining = length - segmentLength;

            // Read from consecutive segments until all bytes have been read.
            while (lengthRemaining > 0)
            {
                segmentIndex = (segmentIndex + 1) % _readSegments.Length;
                nextSegment = _readSegments[segmentIndex];
                segmentLength = Math.Min(lengthRemaining, nextSegment.Length);
                nextBytes = nextSegment.ReadBytes(0, segmentLength);

                Array.Copy(nextBytes, 0, bytes, length - lengthRemaining, segmentLength);
                lengthRemaining -= segmentLength;
            }

            return bytes;
        }

        public void WriteByte(ushort address, byte value)
        {
            ushort segmentAddress;
            var segment = GetAddressSegmentForAddress(_writeSegmentAddresses, _writeSegments, address,
                out segmentAddress);
            segment.WriteByte(segmentAddress, value);

            if (TriggerWriteEventForMemoryBankType(segment.Type))
            {
                OnAddressWrite(address, 1);
            }
        }

        public void WriteWord(ushort address, ushort word)
        {
            ushort segmentAddress;
            int segmentIndex;
            IWriteableAddressSegment segment;
            if (TryGetSegmentIndexForAddress(_writeSegmentAddresses,
                _writeSegments,
                address,
                sizeof (ushort),
                out segment,
                out segmentIndex,
                out segmentAddress))
            {
                segment.WriteWord(segmentAddress, word);
                if (TriggerWriteEventForMemoryBankType(segment.Type))
                {
                    OnAddressWrite(address, 2);
                }
                return;
            }

            // Write one byte to the end of the returned segment index and another to the start of the next
            var bytes = BitConverter.GetBytes(word);
            segment.WriteByte(segmentAddress, bytes[0]);
            var nextSegment = _writeSegments[(segmentIndex + 1) % _writeSegments.Length];

            if (TriggerWriteEventForMemoryBankType(segment.Type) || TriggerWriteEventForMemoryBankType(nextSegment.Type))
            {
                OnAddressWrite(address, 2);
            }

            nextSegment.WriteByte(0, bytes[1]);
        }

        public void WriteBytes(ushort address, byte[] bytes)
        {
            ushort segmentAddress;
            int segmentIndex;
            IWriteableAddressSegment segment;
            if (TryGetSegmentIndexForAddress(_writeSegmentAddresses,
                _writeSegments,
                address,
                bytes.Length,
                out segment,
                out segmentIndex,
                out segmentAddress))
            {
                segment.WriteBytes(segmentAddress, bytes);
                if (TriggerWriteEventForMemoryBankType(segment.Type))
                {
                    OnAddressWrite(address, (ushort) bytes.Length);
                }
                return;
            }

            // Write to first segment
            var segmentLength = segment.Length - segmentAddress;
            var nextBytes = new byte[segmentLength];
            Array.Copy(bytes, 0, nextBytes, 0, segmentLength);
            segment.WriteBytes(segmentAddress, nextBytes);
            var nextIndex = segmentLength;
            var lengthRemaining = bytes.Length - segmentLength;

            var triggerWriteEvent = TriggerWriteEventForMemoryBankType(segment.Type);

            // Write to consecutive segments until all bytes have been written.
            while (lengthRemaining > 0)
            {
                segmentIndex = (segmentIndex + 1) % _writeSegments.Length;
                segment = _writeSegments[segmentIndex];

                segmentLength = Math.Min(lengthRemaining, segment.Length);
                nextBytes = new byte[segmentLength];
                Array.Copy(bytes, nextIndex, nextBytes, 0, segmentLength);
                segment.WriteBytes(0, nextBytes);

                triggerWriteEvent |= TriggerWriteEventForMemoryBankType(segment.Type);

                lengthRemaining -= segmentLength;
                nextIndex += segmentLength;
            }

            if (triggerWriteEvent)
            {
                OnAddressWrite(address, (ushort) bytes.Length);
            }
        }

        public void TransferByte(ushort addressFrom, ushort addressTo)
        {
            var b = ReadByte(addressFrom);
            WriteByte(addressTo, b);
        }

        public void TransferBytes(ushort addressFrom, ushort addressTo, int length)
        {
            var bytes = ReadBytes(addressFrom, length);
            WriteBytes(addressTo, bytes);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
        }

        private void DmaTask()
        {
            while (!_disposed)
            {
                IDmaOperation operation;
                if (!_dmaController.TryGet(out operation))
                {
                    continue;
                }

                // Check if we need lock any address ranges.
                _lockedAddressRanges.AddRange(operation.LockedAddressesRanges);

                // Execute the operation.
                operation.Execute(this);

                _instructionTimer.SyncToTimings(operation.Timings).Wait();

                // Unlock the locked address ranges.
                _lockedAddressRanges.Clear();
            }
        }

        /// <summary>
        ///     When overridden in a derived class, registers an address write event.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        protected virtual void OnAddressWrite(ushort address, ushort length)
        {
        }

        private static TAddressSegment GetAddressSegmentForAddress<TAddressSegment>(ushort[] segmentAddresses,
            IList<TAddressSegment> segments,
            ushort address,
            out ushort segmentAddress)
            where TAddressSegment : IAddressSegment
        {
            var index = Array.BinarySearch(segmentAddresses, address);

            // If the index is negative, it represents the bitwise 
            // complement of the next larger element in the array. 
            if (index < 0)
            {
                index = ~index - 1;
            }

            var segment = segments[index];

            segmentAddress = (ushort) (address - segment.Address);

            return segment;
        }

        private static bool TryGetSegmentIndexForAddress<TAddressSegment>(ushort[] segmentAddresses,
            IList<TAddressSegment> segments,
            ushort address,
            int length,
            out TAddressSegment segment,
            out int segmentIndex,
            out ushort segmentAddress)
            where TAddressSegment : IAddressSegment
        {
            segmentIndex = Array.BinarySearch(segmentAddresses, address);

            // If the index is negative, it represents the bitwise 
            // complement of the next larger element in the array. 
            if (segmentIndex < 0)
            {
                segmentIndex = ~segmentIndex - 1;
            }

            segment = segments[segmentIndex];

            segmentAddress = (ushort) (address - segment.Address);

            return segmentAddress + length < segment.Length;
        }


        private static void CheckSegments(IEnumerable<IAddressSegment> addressSegments)
        {
            uint lastAddress = 0x0000;
            foreach (var segment in addressSegments)
            {
                if (segment.Length < 1)
                {
                    throw new PlatformConfigurationException($"Segment length is less than 1 at 0x{segment.Address:x4}");
                }

                if (segment.Address > lastAddress)
                {
                    throw new MmuAddressSegmentGapException((ushort) lastAddress, segment.Address);
                }

                if (segment.Address < lastAddress)
                {
                    throw new MmuAddressSegmentOverlapException(segment.Address, (ushort) lastAddress);
                }

                lastAddress += segment.Length;
            }

            if (lastAddress < ushort.MaxValue + 1)
            {
                throw new MmuAddressSegmentGapException((ushort) lastAddress, ushort.MaxValue);
            }
        }

        /// <summary>
        ///     Determines whether to trigger an address write event when writing to the specified memory bank type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool TriggerWriteEventForMemoryBankType(MemoryBankType type)
            => type == MemoryBankType.RandomAccessMemory;
    }
}