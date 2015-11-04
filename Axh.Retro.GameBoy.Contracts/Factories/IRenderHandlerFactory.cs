namespace Axh.Retro.GameBoy.Contracts.Factories
{
    using Axh.Retro.GameBoy.Contracts.Peripherals;

    public interface IRenderHandlerFactory
    {
        IRenderHandler GetIRenderHandler();
    }
}
