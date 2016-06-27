using System;

namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    public interface IMemoryBankController : IWriteableAddressSegment
    {
        bool RamEnable { get; }

        byte RomBankNumber { get; }

        byte RamBankNumber { get; }

        event EventHandler<MemoryBankControllerEventArgs> MemoryBankSwitch;
    }
}