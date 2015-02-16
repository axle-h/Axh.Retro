namespace Axh.Emulation.CPU.X80.Memory
{
    using System;

    using Axh.Emulation.CPU.X80.Contracts.Config;
    using Axh.Emulation.CPU.X80.Contracts.Memory;

    public class ArrayBackedMemoryBank : IReadableAddressSegment, IWriteableAddressSegment
    {
        private readonly byte[] memory;

        public ArrayBackedMemoryBank(IMemoryBankConfig memoryBankConfig)
        {
            this.memory = new byte[memoryBankConfig.Length];
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

        public void WriteByte(ushort address, byte value)
        {
            this.memory[address] = value;
        }

        public void WriteWord(ushort address, ushort word)
        {
            var bytes = BitConverter.GetBytes(word);
            memory[address] = bytes[0];
            memory[address + 1] = bytes[1];
        }

        public void WriteBytes(ushort address, byte[] values)
        {
            Array.Copy(values, 0, memory, address, values.Length);
        }
    }
}
