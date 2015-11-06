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

    internal class CartridgeHeader : ICartridgeHeader
    {
        public CartridgeHeader(
            byte[] entryPoint,
            bool nintendoLogoValid,
            string title,
            bool isGameBoyColour,
            string licenseCode,
            bool isSuperGameBoy,
            CartridgeType cartridgeType,
            CartridgeRomSize romSize,
            CartridgeRamSize ramSize,
            DestinationCode destinationCode,
            byte romVersion,
            byte headerChecksum,
            ushort romChecksum)
        {
            EntryPoint = entryPoint;
            NintendoLogoValid = nintendoLogoValid;
            Title = title;
            IsGameBoyColour = isGameBoyColour;
            LicenseCode = licenseCode;
            IsSuperGameBoy = isSuperGameBoy;
            CartridgeType = cartridgeType;
            RomSize = romSize;
            RamSize = ramSize;
            DestinationCode = destinationCode;
            RomVersion = romVersion;
            HeaderChecksum = headerChecksum;
            RomChecksum = romChecksum;
        }

        public byte[] EntryPoint { get; }
        public bool NintendoLogoValid { get; }
        public string Title { get; }
        public bool IsGameBoyColour { get; }
        public string LicenseCode { get; }
        public bool IsSuperGameBoy { get; }
        public CartridgeType CartridgeType { get; }
        public CartridgeRomSize RomSize { get; }
        public CartridgeRamSize RamSize { get; }
        public DestinationCode DestinationCode { get; }
        public byte RomVersion { get; }
        public byte HeaderChecksum { get; }
        public ushort RomChecksum { get; }

        public override string ToString()
        {
            return $"Title: {Title}, IsGameBoyColour: {IsGameBoyColour}, LicenseCode: {LicenseCode}, IsSuperGameBoy: {IsSuperGameBoy}, CartridgeType: {CartridgeType}, RomSize: {RomSize}, RamSize: {RamSize}, DestinationCode: {DestinationCode}, RomVersion: {RomVersion}, HeaderChecksum: {HeaderChecksum}, RomChecksum: {RomChecksum}";
        }
    }
}
