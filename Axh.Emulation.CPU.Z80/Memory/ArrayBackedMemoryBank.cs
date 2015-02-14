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

        public ushort[] ReadWords(ushort address, int length)
        {
            var words = new ushort[length];
            for (var i = 0; i < length; i++)
            {
                words[i] = BitConverter.ToUInt16(memory, address + i * 2);
            }

            return words;
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

        public void WriteWords(ushort address, ushort[] values)
        {
            for (ushort i = 0; i < values.Length; i++)
            {
                var bytes = BitConverter.GetBytes(values[i]);
                var wordAddress = address + 2*i;
                memory[wordAddress] = bytes[0];
                memory[wordAddress + 1] = bytes[1];
            }
        }
    }
}
