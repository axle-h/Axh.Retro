namespace Axh.Emulation.CPU.Z80.Contracts.Config
{
    using System.Collections.Generic;

    public interface IZ80PlatformConfig
    {
        ushort AddressSpace { get; }

        WriteFaultMode WriteFaultMode { get; }

        IEnumerable<IMemoryBankConfig> RandomAccessMemoryBanks { get; }
        

    }
}