using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Contracts.Graphics;

namespace Axh.Retro.GameBoy.Contracts.Peripherals
{
    public interface IGameBoyMemoryMappedIO : IMemoryMappedPeripheral
    {
        IHardwareRegisters HardwareRegisters { get; }

        IGpu Gpu { get; }
    }
}