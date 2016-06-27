namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.GameBoy.Contracts.Devices;

    public interface ICoreHardwareRegisters : IHardwareRegisters, IReadableAddressSegment, IWriteableAddressSegment
    {
        
    }
}
