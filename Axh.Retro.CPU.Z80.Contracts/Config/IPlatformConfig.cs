namespace Axh.Retro.CPU.Z80.Contracts.Config
{
    using System.Collections.Generic;

    using Axh.Retro.CPU.Common.Contracts.Config;

    public interface IPlatformConfig
    {
        CpuMode CpuMode { get; }

        IEnumerable<IMmuBankConfig> MemoryBanks { get; }
        
        double MachineCycleSpeedMhz { get; }

        InstructionTimingSyncMode InstructionTimingSyncMode { get; }
    }
}