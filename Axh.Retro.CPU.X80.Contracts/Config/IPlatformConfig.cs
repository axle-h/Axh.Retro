namespace Axh.Retro.CPU.X80.Contracts.Config
{
    using System.Collections.Generic;

    public interface IPlatformConfig
    {
        CpuMode CpuMode { get; }

        IEnumerable<IMemoryBankConfig> RandomAccessMemoryBanks { get; }
        
        double? MachineCycleSpeedMhz { get; }

        double? ThrottlingStateSpeedMhz { get; }
    }
}