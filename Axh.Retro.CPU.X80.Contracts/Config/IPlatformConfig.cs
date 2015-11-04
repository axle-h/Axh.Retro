namespace Axh.Retro.CPU.X80.Contracts.Config
{
    using System.Collections.Generic;

    public interface IPlatformConfig
    {
        CpuMode CpuMode { get; }

        IEnumerable<IMmuBankConfig> MemoryBanks { get; }
        
        double? MachineCycleSpeedMhz { get; }

        double? ThrottlingStateSpeedMhz { get; }
    }
}