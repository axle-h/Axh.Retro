namespace Axh.Retro.CPU.X80.Contracts.Config
{
    using System;

    public class SimpleMemoryBankConfig : SimpleMmuBankConfig, IMemoryBankConfig
    {
        public SimpleMemoryBankConfig(MemoryBankType type, int bankId, ushort address, ushort length)
            : base(type, bankId, address, length)
        {
            this.State = new byte[length];
        }
        
        public byte[] State { get; }
    }

    public class SimpleMemoryMappedPeripheralConfig : SimpleMmuBankConfig, IMemoryMappedPeripheralConfig
    {
        public SimpleMemoryMappedPeripheralConfig(MemoryBankType type, int bankId, ushort address, ushort length, Guid peripheralId)
            : base(type, bankId, address, length)
        {
            PeripheralId = peripheralId;
        }

        public Guid PeripheralId { get; }
    }

    public class SimpleMmuBankConfig : IMmuBankConfig
    {
        public SimpleMmuBankConfig(MemoryBankType type, int bankId, ushort address, ushort length)
        {
            Type = type;
            BankId = bankId;
            Address = address;
            Length = length;
        }

        public MemoryBankType Type { get; }

        public int BankId { get; }

        public ushort Address { get; }

        public ushort Length { get; }
    }
}
