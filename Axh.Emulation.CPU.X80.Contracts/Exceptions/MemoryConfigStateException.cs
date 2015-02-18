namespace Axh.Emulation.CPU.X80.Contracts.Exceptions
{
    using System;

    public class MemoryConfigStateException : Exception
    {
        public MemoryConfigStateException(ushort address, int segmentLength, int stateLength)
            : base(string.Format("Segment configured at address 0x{0:x4} - 0x{1:x4} has invalid state length: {2}", address, address + segmentLength - 1, stateLength))
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
