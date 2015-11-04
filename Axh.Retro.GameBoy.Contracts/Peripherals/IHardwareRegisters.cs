namespace Axh.Retro.GameBoy.Contracts.Peripherals
{
    using Axh.Retro.GameBoy.Contracts.Devices;

    public interface IHardwareRegisters
    {
        IJoyPad JoyPad { get; }
    }
}