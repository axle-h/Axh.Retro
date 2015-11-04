namespace Axh.Retro.GameBoy.Peripherals
{
    using System;
    using System.Collections.Generic;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;

    public class InterruptRegister : IMemoryMappedPeripheral, IReadableAddressSegment, IWriteableAddressSegment
    {
        private byte registerValue;

        public InterruptRegister()
        {
            this.registerValue = 0;
        }

        public void Halt()
        {
            // Do nothing
        }

        public void Resume()
        {
            // Do nothing
        }

        public IEnumerable<IAddressSegment> AddressSegments => new[] { this };

        public MemoryBankType Type => MemoryBankType.RandomAccessMemory;

        public ushort Address => 0xffff;

        public ushort Length => 1;

        public byte ReadByte(ushort address)
        {
            return this.registerValue;
        }

        public ushort ReadWord(ushort address)
        {
            throw new NotSupportedException();
        }

        public byte[] ReadBytes(ushort address, int length)
        {
            if (length > 1)
            {
                throw new NotSupportedException();
            }
            return new[] { this.registerValue };
        }

        public void WriteByte(ushort address, byte value)
        {
            this.registerValue = value;
        }

        public void WriteWord(ushort address, ushort word)
        {
            throw new NotSupportedException();
        }

        public void WriteBytes(ushort address, byte[] values)
        {
            if (values.Length > 1)
            {
                throw new NotSupportedException();
            }

            this.registerValue = values[0];
        }
    }
}
