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
                    throw new Exception(string.Format("Gap in configured address segments from 0x{0:x4} to 0x{1:x4}", lastAddress, segment.Address));
                }

                if (segment.Address < lastAddress)
                {
                    throw new Exception(string.Format("Overlapping configured address segments from 0x{0:x4} to 0x{1:x4}", segment.Address, lastAddress));
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
            var addressSegment = this.GetAddressSegmentForAddress(address, out segmentAddress);
            return addressSegment.ReadWord(segmentAddress);
        }

        public void WriteByte(ushort address, byte value)
        {
            ushort segmentAddress;
            var addressSegment = this.GetAddressSegmentForAddressWrite(address, out segmentAddress);

            addressSegment.WriteByte(address, value);
        }

        public void WriteWord(ushort address, ushort word)
        {
            ushort segmentAddress;
            var addressSegment = this.GetAddressSegmentForAddressWrite(address, out segmentAddress);
            addressSegment.WriteWord(address, word);
        }

        public void WriteBytes(ushort address, byte[] values)
        {
            ushort segmentAddress;
            var addressSegment = this.GetAddressSegmentForAddressWrite(address, out segmentAddress);
            addressSegment.WriteBytes(address, values);
        }

        private IAddressSegment GetAddressSegmentForAddressWrite(ushort address, out ushort segmentAddress)
        {
            var segment = GetAddressSegmentForAddress(address, out segmentAddress);

            if (!segment.IsWriteable && this.platformConfig.WriteFaultMode == WriteFaultMode.SegmentationFault)
            {
                throw new SegmentationFaultException(address);
            }

            return segment;
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
            
            segmentAddress = (ushort)(segment.Length - address);

            return segment;
        }

    }
}
