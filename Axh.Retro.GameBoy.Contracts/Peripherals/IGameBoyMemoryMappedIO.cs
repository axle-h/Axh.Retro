namespace Axh.Retro.GameBoy.Contracts.Peripherals
{
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Contracts.Graphics;

    public interface IGameBoyMemoryMappedIO
    {
        IHardwareRegisters HardwareRegisters { get; }
        
        IGpu Gpu { get; }
    }
}