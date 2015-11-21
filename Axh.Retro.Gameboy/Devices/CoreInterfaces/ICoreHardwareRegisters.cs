namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.GameBoy.Contracts.Devices;

    internal interface ICoreHardwareRegisters : IHardwareRegisters, IReadableAddressSegment, IWriteableAddressSegment
    {

    }
}
