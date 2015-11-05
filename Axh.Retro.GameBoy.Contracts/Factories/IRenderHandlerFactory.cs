namespace Axh.Retro.GameBoy.Contracts.Factories
{
    using Axh.Retro.GameBoy.Contracts.Devices;

    public interface IRenderHandlerFactory
    {
        IRenderHandler GetRenderHandler();
    }
}
