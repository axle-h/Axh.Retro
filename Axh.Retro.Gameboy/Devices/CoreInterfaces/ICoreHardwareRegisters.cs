using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.GameBoy.Contracts.Devices;

namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
{
    public interface ICoreHardwareRegisters : IHardwareRegisters, IReadableAddressSegment, IWriteableAddressSegment
    {
    }
}