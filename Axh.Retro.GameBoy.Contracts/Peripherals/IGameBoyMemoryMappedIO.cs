namespace Axh.Retro.GameBoy.Contracts.Peripherals
{
    using Axh.Retro.GameBoy.Contracts.Devices;

    public interface IGameBoyMemoryMappedIO
    {
        IHardwareRegisters HardwareRegisters { get; }

        IInterruptRegister InterruptRegister { get; }

        IFrameBuffer FrameBuffer { get; }
    }
}