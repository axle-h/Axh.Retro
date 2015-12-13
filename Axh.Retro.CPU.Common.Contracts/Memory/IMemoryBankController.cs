namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    using System;

    public interface IMemoryBankController : IWriteableAddressSegment
    {
        bool RamEnable { get; }

        byte RomBankNumber { get; }

        byte RamBankNumber { get; }

        event EventHandler<MemoryBankControllerEventArgs> MemoryBankSwitch;
    }
}