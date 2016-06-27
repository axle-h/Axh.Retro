using System;

namespace Axh.Retro.CPU.Common.Contracts.Exceptions
{
    public class MemoryConfigStateException : Exception
    {
        public MemoryConfigStateException(ushort address, int segmentLength, int stateLength)
            : base(
                $"Segment configured at address 0x{address:x4} - 0x{address + segmentLength - 1:x4} has invalid state length: {stateLength}"
                )
        {
            Adddress = address;
            SegmentLength = segmentLength;
            StateLength = stateLength;
        }

        public ushort Adddress { get; private set; }

        public int SegmentLength { get; private set; }

        public int StateLength { get; private set; }
    }
}