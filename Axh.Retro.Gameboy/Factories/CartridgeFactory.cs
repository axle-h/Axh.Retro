using System;
using System.IO;
using System.Linq;
using Axh.Retro.GameBoy.Contracts.Factories;
using Axh.Retro.GameBoy.Contracts.Media;
using Axh.Retro.GameBoy.Media;
using Axh.Retro.GameBoy.Util;

namespace Axh.Retro.GameBoy.Factories
{
    public class CartridgeFactory : ICartridgeFactory
    {
        private const int RomBankLength = 0x4000;
        private const ushort HeaderStart = 0x0100;

        private static readonly byte[] ExpectedNintendoLogo =
        {
            0xCE,
            0xED,
            0x66,
            0x66,
            0xCC,
            0x0D,
            0x00,
            0x0B,
            0x03,
            0x73,
            0x00,
            0x83,
            0x00,
            0x0C,
            0x00,
            0x0D,
            0x00,
            0x08,
            0x11,
            0x1F,
            0x88,
            0x89,
            0x00,
            0x0E,
            0xDC,
            0xCC,
            0x6E,
            0xE6,
            0xDD,
            0xDD,
            0xD9,
            0x99,
            0xBB,
            0xBB,
            0x67,
            0x63,
            0x6E,
            0x0E,
            0xEC,
            0xCC,
            0xDD,
            0xDC,
            0x99,
            0x9F,
            0xBB,
            0xB9,
            0x33,
            0x3E
        };

        public ICartridge GetCartridge(byte[] cartridge)
        {
            var header = GetCartridgeHeader(cartridge);
            var numberOfBanks = cartridge.Length / RomBankLength;

            if (cartridge.Length % RomBankLength != 0)
            {
                throw new Exception("Invalid cartridge length: " + cartridge.Length);
            }

            if (numberOfBanks % 2 != 0)
            {
                throw new Exception("Invalid number of banks: " + numberOfBanks);
            }

            // EntryPoint should not be all 0's or ff's
            if (header.EntryPoint.All(x => x == 0 || x == 0xff))
            {
                throw new Exception("No valid entrypoint in cartridge");
            }

            if (!header.NintendoLogoValid)
            {
                // Should we do this?!
                throw new Exception("Nintendo logo is not valid");
            }

            if (header.RomSize.NumberOfBanks() != numberOfBanks)
            {
                throw new Exception($"Cartridge type {header.RomSize} does not match ROM banks: " + numberOfBanks);
            }

            var banks = new byte[numberOfBanks][];
            for (var n = 0; n < numberOfBanks; n++)
            {
                var bank = new byte[RomBankLength];
                Array.Copy(cartridge, n * RomBankLength, bank, 0, RomBankLength);
                banks[n] = bank;
            }

            var ramBanks = Enumerable.Range(0, header.RamSize.NumberOfBanks()).Select(i => header.RamSize.BankSize()).ToArray();

            return new Cartridge(banks, ramBanks, header);
        }

        private static ICartridgeHeader GetCartridgeHeader(byte[] cartridgeBytes)
        {
            using (var stream = new MemoryStream(cartridgeBytes))
            {
                stream.Seek(HeaderStart, SeekOrigin.Begin);

                var entryPoint = stream.ReadBuffer(4);
                var nintendoLogoValid = stream.ReadBuffer(0x30).SequenceEqual(ExpectedNintendoLogo);
                var title = stream.ReadAscii(15).Trim();
                var isGameBoyColour = stream.ReadByte() == 0x80;
                var newLicenseCode = new string(stream.ReadAscii(2).Reverse().ToArray());
                var isSuperGameBoy = stream.ReadByte() == 0x03;
                var cartridgeType = stream.ReadEnum<CartridgeType>();
                var romSize = stream.ReadEnum<CartridgeRomSize>();
                var ramSize = stream.ReadEnum<CartridgeRamSize>();
                var destinationCode = stream.ReadEnum<DestinationCode>();
                var oldLicenseCode = (byte) stream.ReadByte();
                var romVersion = (byte) stream.ReadByte();
                var headerChecksum = (byte) stream.ReadByte();
                var romChecksum = stream.ReadBigEndianUInt16();

                string licenseCode = null;
                switch (oldLicenseCode)
                {
                    case 0x79:
                        licenseCode = "Accolade";
                        break;
                    case 0xa4:
                        licenseCode = "Konami";
                        break;
                    case 0x33:
                        licenseCode = newLicenseCode;
                        break;
                }

                return new CartridgeHeader(entryPoint,
                                           nintendoLogoValid,
                                           title,
                                           isGameBoyColour,
                                           licenseCode,
                                           isSuperGameBoy,
                                           cartridgeType,
                                           romSize,
                                           ramSize,
                                           destinationCode,
                                           romVersion,
                                           headerChecksum,
                                           romChecksum);
            }
        }
    }
}