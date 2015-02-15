namespace Axh.Emulation.CPU.Z80.Contracts.Config
{
    using System.Collections.Generic;

    public interface IZ80PlatformConfig
    {
        WriteFaultMode WriteFaultMode { get; }

        IEnumerable<IMemoryBankConfig> RandomAccessMemoryBanks { get; }
        

    }
}