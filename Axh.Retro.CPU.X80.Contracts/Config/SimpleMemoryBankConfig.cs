namespace Axh.Retro.CPU.X80.Contracts.Config
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Memory;

    public class SimpleMemoryBankConfig : SimpleMmuBankConfig, IMemoryBankConfig
    {
        public SimpleMemoryBankConfig(MemoryBankType type, byte? bankId, ushort address, ushort length)
            : base(type, bankId, address, length)
        {
            this.State = new byte[length];
        }
        
        public byte[] State { get; }
    }

    public class SimpleMmuBankConfig : IMmuBankConfig
    {
        public SimpleMmuBankConfig(MemoryBankType type, byte? bankId, ushort address, ushort length)
        {
            Type = type;
            BankId = bankId;
            Address = address;
            Length = length;
        }

        public MemoryBankType Type { get; }

        public byte? BankId { get; }

        public ushort Address { get; }

        public ushort Length { get; }
    }
}
