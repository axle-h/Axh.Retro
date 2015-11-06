namespace Axh.Retro.GameBoy.Contracts.Peripherals
{
    using Axh.Retro.GameBoy.Contracts.Devices;

    public interface IGameBoyRegisters
    {
        IHardwareRegisters HardwareRegisters { get; }

        IInterruptRegister InterruptRegister { get; }
    }
}