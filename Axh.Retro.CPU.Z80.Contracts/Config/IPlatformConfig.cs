using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Config;

namespace Axh.Retro.CPU.Z80.Contracts.Config
{
    public interface IPlatformConfig
    {
        CpuMode CpuMode { get; }

        IEnumerable<IMemoryBankConfig> MemoryBanks { get; }

        double MachineCycleSpeedMhz { get; }

        InstructionTimingSyncMode InstructionTimingSyncMode { get; }

        bool LockOnUndefinedInstruction { get; }
    }
}