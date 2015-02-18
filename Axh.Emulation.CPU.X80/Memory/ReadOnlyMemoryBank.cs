namespace Axh.Emulation.CPU.X80.Memory
{
    using System;

    using Axh.Emulation.CPU.X80.Contracts.Config;
    using Axh.Emulation.CPU.X80.Contracts.Exceptions;
    using Axh.Emulation.CPU.X80.Contracts.Memory;

    public class ReadOnlyMemoryBank : IReadableAddressSegment
    {
        private readonly byte[] memory;

        public ReadOnlyMemoryBank(IMemoryBankConfig memoryBankConfig)
        {
            if (memoryBankConfig.State == null || memoryBankConfig.Length != memoryBankConfig.State.Length)
            {
                throw new MemoryConfigStateException(memoryBankConfig.Address, memoryBankConfig.Length, memoryBankConfig.State == null ? 0 : memoryBankConfig.State.Length);
            }

            this.memory = new byte[memoryBankConfig.Length];
            Array.Copy(memoryBankConfig.State, 0, this.memory, 0, memoryBankConfig.State.Length);

            this.Address = memoryBankConfig.Address;
            this.Length = memoryBankConfig.Length;
        }

        public ushort Address { get; private set; }

        public ushort Length { get; private set; }

        public byte ReadByte(ushort address)
        {
            return this.memory[address];
        }

        public ushort ReadWord(ushort address)
        {
            // Construct 16 bit value in little endian.
            return BitConverter.ToUInt16(memory, address);
        }

        public byte[] ReadBytes(ushort address, int length)
        {
            var bytes = new byte[length];
            Array.Copy(memory, address, bytes, 0, length);
            return bytes;
        }
    }
}
