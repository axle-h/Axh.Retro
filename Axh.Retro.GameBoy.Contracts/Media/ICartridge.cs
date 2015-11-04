namespace Axh.Retro.GameBoy.Contracts.Media
{
    public interface ICartridge
    {
        byte[][] RomBanks { get; }

        ushort[] RamBankLengths { get; }

        ICartridgeHeader Header { get; }
    }
}
