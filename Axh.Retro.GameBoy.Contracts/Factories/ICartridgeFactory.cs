namespace Axh.Retro.GameBoy.Contracts.Factories
{
    using Axh.Retro.GameBoy.Contracts.Media;

    public interface ICartridgeFactory
    {
        ICartridge GetCartridge(byte[] cartridge);
    }
}