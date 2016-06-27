using System;

namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    public class AddressWriteEventArgs : EventArgs
    {
        public AddressWriteEventArgs(ushort address, ushort length)
        {
            Address = address;
            Length = length;
        }

        public ushort Address { get; }
        public ushort Length { get; }
    }
}