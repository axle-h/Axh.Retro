namespace Axh.Retro.GameBoy.Contracts.Factories
{
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Contracts.Graphics;

    public interface IRenderHandlerFactory
    {
        IRenderHandler GetRenderHandler();
    }
}
