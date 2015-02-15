namespace Axh.Emulation.CPU.Z80
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Emulation.CPU.Z80.Contracts;
    using Axh.Emulation.CPU.Z80.Contracts.Config;
    using Axh.Emulation.CPU.Z80.Contracts.Exceptions;
    using Axh.Emulation.CPU.Z80.Contracts.Memory;

    public class Z80Mmu : IMmu
    {
        private readonly IZ80PlatformConfig platformConfig;

        private readonly ushort[] segmentAddresses;

        private readonly IAddressSegment[] segments;

        public Z80Mmu(IZ80PlatformConfig platformConfig, IEnumerable<IAddressSegment> addressSegments)
        {
            this.platformConfig = platformConfig;

            this.segments = addressSegments.OrderBy(x => x.Address).ToArray();
            this.segmentAddresses = segments.Select(x => x.Address).ToArray();

            // Check for gaps.
            ushort lastAddress = 0x0000;
            foreach (var segment in segments)
            {
                if (segment.Address > lastAddress)
                {
                    throw new Z80ConfigurationException(string.Format("Gap in configured address segments from 0x{0:x4} to 0x{1:x4}", lastAddress, segment.Address));
                }

                if (segment.Address < lastAddress)
                {
                    throw new Z80ConfigurationException(string.Format("Overlapping configured address segments from 0x{0:x4} to 0x{1:x4}", segment.Address, lastAddress));
                }

                lastAddress += (ushort)(segment.Length + 1);
            }
        }
        
        public byte ReadByte(ushort address)
        {
            ushort segmentAddress;
            var addressSegment = this.GetAddressSegmentForAddress(address, out segmentAddress);
            return addressSegment.ReadByte(segmentAddress);
        }

        public ushort ReadWord(ushort address)
        {
            ushort segmentAddress;
            int segmentIndex;
            if (this.TryGetSegmentIndexForAddress(address, sizeof(ushort), out segmentIndex, out segmentAddress))
            {
                return segments[segmentIndex].ReadWord(segmentAddress);
            }

            // Read one byte from the end of the returned segment index and another from the start of the next
            var lsb = segments[segmentIndex].ReadByte(segmentAddress);
            var msb = segments[(segmentIndex + 1)%segments.Length].ReadByte(0);
            return BitConverter.ToUInt16(new[] {lsb, msb}, 0);
        }

        public byte[] ReadBytes(ushort address, int length)
        {
            ushort segmentAddress;
            int segmentIndex;
            if (this.TryGetSegmentIndexForAddress(address, length, out segmentIndex, out segmentAddress))
            {
                return segments[segmentIndex].ReadBytes(segmentAddress, length);
            }

            var bytes = new byte[length];

            // Read from first segment
            var nextSegment = segments[segmentIndex];
            var segmentLength = nextSegment.Length - segmentAddress;
            var nextBytes = nextSegment.ReadBytes(segmentAddress, segmentLength);
            Array.Copy(nextBytes, 0, bytes, 0, segmentLength);
            length -= segmentLength;

            // Read from consecutive segments until all bytes have been read.
            while (length > 0)
            {
                segmentIndex = (segmentIndex + 1)%segments.Length;
                nextSegment = segments[segmentIndex];
                segmentLength = Math.Min(length, nextSegment.Length);
                nextBytes = nextSegment.ReadBytes(0, segmentLength);

                Array.Copy(nextBytes, 0, bytes, length, segmentLength);
                length -= segmentLength;
            }
            
            return bytes;
        }
        
        public void WriteByte(ushort address, byte value)
        {
            ushort segmentAddress;
            var segment = GetAddressSegmentForAddress(address, out segmentAddress);

            if (segment.IsWriteable)
            {
                segment.WriteByte(address, value);
            }
            WriteFault(address);
        }

        public void WriteWord(ushort address, ushort word)
        {
            ushort segmentAddress;
            int segmentIndex;
            if (this.TryGetSegmentIndexForAddress(address, sizeof(ushort), out segmentIndex, out segmentAddress))
            {
                var segment = segments[segmentIndex];

                if (segment.IsWriteable)
                {
                    segment.WriteWord(address, word);
                }
                WriteFault(address);
                return;
            }

            // Write one byte to the end of the returned segment index and another to the start of the next
            var bytes = BitConverter.GetBytes(word);
            var segment0 = segments[segmentIndex];
            var segment1 = segments[(segmentIndex + 1) % segments.Length];

            if (segment0.IsWriteable && segment1.IsWriteable)
            {
                segment0.WriteByte(segmentAddress, bytes[0]);
                segment1.WriteByte(0, bytes[1]);
                return;
            }

            WriteFault(address);
        }

        public void WriteBytes(ushort address, byte[] bytes)
        {
            ushort segmentAddress;
            int segmentIndex;
            if (this.TryGetSegmentIndexForAddress(address, bytes.Length, out segmentIndex, out segmentAddress))
            {
                var segment = segments[segmentIndex];

                if (segment.IsWriteable)
                {
                    segment.WriteBytes(address, bytes);
                }
                WriteFault(address);
                return;
            }

            var nextSegment = segments[segmentIndex];

            if (!nextSegment.IsWriteable)
            {
                this.WriteFault(address);
                return;
            }

            // Write to first segment
            var length = bytes.Length;
            var segmentLength = nextSegment.Length - segmentAddress;
            var nextBytes = new byte[segmentLength];
            Array.Copy(bytes, 0, nextBytes, 0, segmentLength);
            nextSegment.WriteBytes(segmentAddress, nextBytes);
            length -= segmentLength;

            // Write to consecutive segments until all bytes have been written.
            while (length > 0)
            {
                segmentIndex = (segmentIndex + 1) % segments.Length;
                nextSegment = segments[segmentIndex];

                if (!nextSegment.IsWriteable)
                {
                    this.WriteFault(address);
                }

                segmentLength = Math.Min(length, nextSegment.Length);
                nextBytes = new byte[segmentLength];
                Array.Copy(bytes, length, nextBytes, 0, segmentLength);
                length -= segmentLength;
            }
        }

        private void WriteFault(ushort address)
        {

            if (this.platformConfig.WriteFaultMode == WriteFaultMode.SegmentationFault)
            {
                throw new SegmentationFaultException(address);
            }

            // Otherwise do nothing.
        }
        
        private IAddressSegment GetAddressSegmentForAddress(ushort address, out ushort segmentAddress)
        {
            var index = Array.BinarySearch(segmentAddresses, address);

            // If the index is negative, it represents the bitwise 
            // complement of the next larger element in the array. 
            if (index < 0)
            {
                index = ~index - 1;
            }

            var segment = segments[index];
            
            segmentAddress = (ushort)(segment.Address - address);

            return segment;
        }

        private bool TryGetSegmentIndexForAddress(ushort address, int length, out int segmentIndex, out ushort segmentAddress)
        {
            segmentIndex = Array.BinarySearch(segmentAddresses, address);

            // If the index is negative, it represents the bitwise 
            // complement of the next larger element in the array. 
            if (segmentIndex < 0)
            {
                segmentIndex = ~segmentIndex - 1;
            }

            var segment = segments[segmentIndex];

            segmentAddress = (ushort)(segment.Address - address);

            return segmentAddress + length < segment.Length;
        }
    }
}
