namespace Axh.Retro.GameBoy.Contracts.Media
{
    public interface ICartridgeHeader
    {
        byte[] EntryPoint { get; }

        bool NintendoLogoValid { get; }

        string Title { get; }

        bool IsGameBoyColour { get; }

        string NewLicenseCode { get; }

        bool IsSuperGameBoy { get; }

        CartridgeType CartridgeType { get; }

        CartridgeRomSize RomSize { get; }

        CartridgeRamSize RamSize { get; }

        DestinationCode DestinationCode { get; }

        byte OldLicenseCode { get; }

        byte RomVersion { get; }

        byte HeaderChecksum { get; }

        ushort RomChecksum { get; }
    }
}