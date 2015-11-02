namespace Axh.Retro.CPU.X80.Contracts.Config
{
    using System;

    public enum MemoryBankType
    {
        RandomAccessMemory,
        ReadOnlyMemory,
        Peripheral
    }

    public interface IMmuBankConfig
    {
        MemoryBankType Type { get; }

        int BankId { get; }

        ushort Address { get; }

        ushort Length { get; }
    }

    public interface IMemoryBankConfig : IMmuBankConfig
    {
        /// <summary>
        /// Initial state of the memory bank.
        /// </summary>
        byte[] State { get; }
    }

    public interface IMemoryMappedPeripheralConfig : IMmuBankConfig
    {
        Guid PeripheralId { get; }
    }
}