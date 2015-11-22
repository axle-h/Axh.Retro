namespace Axh.Retro.CPU.Common.Config
{
    using System;

    using Axh.Retro.CPU.Common.Contracts.Config;
    using Axh.Retro.CPU.Common.Contracts.Memory;

    public class SimpleMemoryBankConfig : IMemoryBankConfig
    {
        public SimpleMemoryBankConfig(MemoryBankType type, byte? bankId, ushort address, ushort length)
            : this(type, bankId, address, length, new byte[length])
        {
        }

        public SimpleMemoryBankConfig(MemoryBankType type, byte? bankId, ushort address, ushort length, byte[] state)
        {
            Type = type;
            BankId = bankId;
            Address = address;
            Length = length;
            State = new byte[length];
            Array.Copy(state, 0, this.State, 0, length);
        }

        public MemoryBankType Type { get; }

        public byte? BankId { get; }

        public ushort Address { get; }

        public ushort Length { get; }

        public byte[] State { get; }
    }
}
