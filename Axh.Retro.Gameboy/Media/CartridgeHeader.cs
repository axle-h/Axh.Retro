namespace Axh.Retro.GameBoy.Media
{
    using System.IO;
    using System.Linq;

    using Axh.Retro.GameBoy.Contracts.Media;
    using Axh.Retro.GameBoy.Util;

    internal class CartridgeHeader : ICartridgeHeader
    {
        private const ushort HeaderStart = 0x0100;
        private const ushort HeaderLength = 0x50;

        private static readonly byte[] ExpectedNintendoLogo =
        {
            0xCE, 0xED, 0x66, 0x66, 0xCC, 0x0D, 0x00, 0x0B, 0x03, 0x73, 0x00, 0x83, 0x00, 0x0C, 0x00, 0x0D, 0x00, 0x08, 0x11, 0x1F, 0x88, 0x89, 0x00, 0x0E,
            0xDC, 0xCC, 0x6E, 0xE6, 0xDD, 0xDD, 0xD9, 0x99, 0xBB, 0xBB, 0x67, 0x63, 0x6E, 0x0E, 0xEC, 0xCC, 0xDD, 0xDC, 0x99, 0x9F, 0xBB, 0xB9, 0x33, 0x3E
        };

        public CartridgeHeader(byte[] cartridgeBytes)
        {
            using (var stream = new MemoryStream(cartridgeBytes.Skip(HeaderStart).Take(HeaderLength).ToArray()))
            {
                EntryPoint = stream.ReadBuffer(4);
                NintendoLogoValid = stream.ReadBuffer(0x30).SequenceEqual(ExpectedNintendoLogo);
                Title = stream.ReadAscii(15).Trim();
                IsGameBoyColour = stream.ReadByte() == 0x80;
                NewLicenseCode = new string(stream.ReadAscii(2).Reverse().ToArray());
                IsSuperGameBoy = stream.ReadByte() == 0x03;
                CartridgeType = stream.ReadEnum<CartridgeType>();
                RomSize = stream.ReadEnum<CartridgeRomSize>();
                RamSize = stream.ReadEnum<CartridgeRamSize>();
                DestinationCode = stream.ReadEnum<DestinationCode>();
                OldLicenseCode = (byte)stream.ReadByte();
                RomVersion = (byte)stream.ReadByte();
                HeaderChecksum = (byte)stream.ReadByte();
                RomChecksum = stream.ReadBigEndianUInt16();
            }
        }

        public byte[] EntryPoint { get; }
        public bool NintendoLogoValid { get; }
        public string Title { get; }
        public bool IsGameBoyColour { get; }
        public string NewLicenseCode { get; }
        public bool IsSuperGameBoy { get; }
        public CartridgeType CartridgeType { get; }
        public CartridgeRomSize RomSize { get; }
        public CartridgeRamSize RamSize { get; }
        public DestinationCode DestinationCode { get; }
        public byte OldLicenseCode { get; }
        public byte RomVersion { get; }
        public byte HeaderChecksum { get; }
        public ushort RomChecksum { get; }

        public override string ToString()
        {
            return $"Title: {Title}, IsGameBoyColour: {IsGameBoyColour}, NewLicenseCode: {NewLicenseCode}, IsSuperGameBoy: {IsSuperGameBoy}, CartridgeType: {CartridgeType}, RomSize: {RomSize}, RamSize: {RamSize}, DestinationCode: {DestinationCode}, OldLicenseCode: {OldLicenseCode}, RomVersion: {RomVersion}, HeaderChecksum: {HeaderChecksum}, RomChecksum: {RomChecksum}";
        }
    }
}
