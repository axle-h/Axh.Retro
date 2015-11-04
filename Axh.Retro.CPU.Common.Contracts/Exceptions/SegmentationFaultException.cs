namespace Axh.Retro.CPU.Common.Contracts.Exceptions
{
    using System;

    public class SegmentationFaultException : Exception
    {
        private readonly ushort address;

        public SegmentationFaultException(ushort address) : base("Cannot write to address: " + address)
        {
            this.address = address;
        }
    }
}
