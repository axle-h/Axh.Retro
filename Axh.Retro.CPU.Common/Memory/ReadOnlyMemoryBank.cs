using System;
using Axh.Retro.CPU.Common.Contracts.Config;
using Axh.Retro.CPU.Common.Contracts.Exceptions;
using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.CPU.Common.Memory
{
    public class ReadOnlyMemoryBank : IReadableAddressSegment
    {
        private readonly byte[] _memory;

        public ReadOnlyMemoryBank(IMemoryBankConfig memoryBankConfig)
        {
            if (memoryBankConfig.State == null || memoryBankConfig.Length != memoryBankConfig.State.Length)
            {
                throw new MemoryConfigStateException(memoryBankConfig.Address,
                    memoryBankConfig.Length,
                    memoryBankConfig.State?.Length ?? 0);
            }

            _memory = new byte[memoryBankConfig.Length];
            Array.Copy(memoryBankConfig.State, 0, _memory, 0, memoryBankConfig.State.Length);

            Type = memoryBankConfig.Type;
            Address = memoryBankConfig.Address;
            Length = memoryBankConfig.Length;
        }

        public MemoryBankType Type { get; }

        public ushort Address { get; }

        public ushort Length { get; }

        public byte ReadByte(ushort address)
        {
            return _memory[address];
        }

        public ushort ReadWord(ushort address)
        {
            // Construct 16 bit value in little endian.
            return BitConverter.ToUInt16(_memory, address);
        }

        public byte[] ReadBytes(ushort address, int length)
        {
            var bytes = new byte[length];
            Array.Copy(_memory, address, bytes, 0, length);
            return bytes;
        }

        public void ReadBytes(ushort address, byte[] buffer)
        {
            Array.Copy(_memory, address, buffer, 0, buffer.Length);
        }

        public override string ToString()
        {
            return $"{Type}: 0x{Address:x4} - 0x{Address + Length - 1:x4}";
        }
    }
}