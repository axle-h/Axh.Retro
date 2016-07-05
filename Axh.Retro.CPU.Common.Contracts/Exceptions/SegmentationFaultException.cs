using System;

namespace Axh.Retro.CPU.Common.Contracts.Exceptions
{
    public class SegmentationFaultException : Exception
    {
        public SegmentationFaultException(ushort address) : base("Cannot write to address: " + address)
        {
            Address = address;
        }

        public ushort Address { get; }
    }
}