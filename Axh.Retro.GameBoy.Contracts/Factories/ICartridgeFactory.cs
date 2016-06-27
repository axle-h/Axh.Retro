using Axh.Retro.GameBoy.Contracts.Media;

namespace Axh.Retro.GameBoy.Contracts.Factories
{
    public interface ICartridgeFactory
    {
        ICartridge GetCartridge(byte[] cartridge);
    }
}