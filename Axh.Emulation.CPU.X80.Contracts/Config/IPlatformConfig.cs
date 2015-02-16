namespace Axh.Emulation.CPU.X80.Contracts.Config
{
    using System.Collections.Generic;

    public interface IPlatformConfig
    {
        CpuMode CpuMode { get; }

        IEnumerable<IMemoryBankConfig> RandomAccessMemoryBanks { get; }
        

    }
}