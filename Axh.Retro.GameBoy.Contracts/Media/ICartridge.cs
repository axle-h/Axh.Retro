namespace Axh.Retro.GameBoy.Contracts.Media
{
    public interface ICartridge
    {
        byte[][] RomBanks { get; }

        ushort[] RamBankLengths { get; }

        ICartridgeHeader Header { get; }
    }

    public interface ICartridgeHeader
    {
        byte[] EntryPoint { get; }

        bool NintendoLogoValid { get; }

        string Title { get; }

        bool IsGameBoyColour { get; }

        string LicenseCode { get; }

        bool IsSuperGameBoy { get; }

        CartridgeType CartridgeType { get; }

        CartridgeRomSize RomSize { get; }

        CartridgeRamSize RamSize { get; }

        DestinationCode DestinationCode { get; }

        byte RomVersion { get; }

        byte HeaderChecksum { get; }

        ushort RomChecksum { get; }
    }
}