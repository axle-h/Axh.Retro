namespace Axh.Retro.GameBoy.Media
{
    using Axh.Retro.GameBoy.Contracts.Media;

    internal class Cartridge : ICartridge
    {
        public Cartridge(byte[][] romBanks, ushort[] ramBankLengths, ICartridgeHeader header)
        {
            RomBanks = romBanks;
            RamBankLengths = ramBankLengths;
            Header = header;
        }

        public byte[][] RomBanks { get; }

        public ushort[] RamBankLengths { get; }

        public ICartridgeHeader Header { get; }
    }
}
