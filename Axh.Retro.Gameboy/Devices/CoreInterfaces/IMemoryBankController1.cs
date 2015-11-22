namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
{
    using Axh.Retro.CPU.Common.Contracts.Memory;

    public interface IMemoryBankController1 : IWriteableAddressSegment
    {
        bool RamEnable { get; }

        byte RomBankNumber { get; }

        byte RamBankNumber { get; }
    }
}