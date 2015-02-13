namespace Axh.Emulation.CPU.Z80.Memory
{
    using System;

    using Axh.Emulation.CPU.Z80.Contracts.Config;
    using Axh.Emulation.CPU.Z80.Contracts.Memory;

    public class ArrayBackedMemoryBank : IAddressSegment
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

        public bool IsWriteable
        {
            get
            {
                return true;
            } 
        }

        public byte ReadByte(ushort address)
        {
            return this.memory[address];
        }

        public void WriteByte(ushort address, byte value)
        {
            this.memory[address] = value;
        }

        public ushort ReadWord(ushort address)
        {
            // Construct 16 bit value in little endian.
            return BitConverter.ToUInt16(memory, address);
        }

        public void WriteWord(ushort address, ushort word)
        {
            var bytes = BitConverter.GetBytes(word);
            Array.Copy(bytes, 0, memory, address, sizeof(ushort));
        }

        public void WriteBytes(ushort address, byte[] values)
        {
            Array.Copy(values, 0, memory, address, values.Length);
        }
    }
}
