namespace Axh.Retro.GameBoy.Contracts.Media
{
    using System;

    public static class MediaExtensions
    {


        public static int NumberOfBanks(this CartridgeRomSize cartridgeRomSize)
        {
            switch (cartridgeRomSize)
            {
                case CartridgeRomSize.Fixed32kB:
                    return 2;
                case CartridgeRomSize.Banked64kB:
                    return 4;
                case CartridgeRomSize.Banked128kB:
                    return 8;
                case CartridgeRomSize.Banked256kB:
                    return 16;
                case CartridgeRomSize.Banked512kB:
                    return 32;
                case CartridgeRomSize.Banked1024kB:
                    return 64;
                case CartridgeRomSize.Banked2048kB:
                    return 128;
                case CartridgeRomSize.Banked4096kB:
                    return 256;
                case CartridgeRomSize.Banked1152kB:
                    return 72;
                case CartridgeRomSize.Banked1280kB:
                    return 80;
                case CartridgeRomSize.Banked1536kB:
                    return 96;
                default:
                    throw new NotSupportedException("RomSize: " + cartridgeRomSize);
            }
        }

        public static int NumberOfBanks(this CartridgeRamSize cartridgeRamSize)
        {
            switch (cartridgeRamSize)
            {
                case CartridgeRamSize.None:
                    return 0;
                case CartridgeRamSize.Fixed2kB:
                    return 1;
                case CartridgeRamSize.Fixed8kB:
                    return 1;
                case CartridgeRamSize.Banked32kB:
                    return 4;
                case CartridgeRamSize.Banked128kB:
                    return 16;
                default:
                    throw new NotSupportedException("RamSize: " + cartridgeRamSize);
            }
        }

        public static ushort BankSize(this CartridgeRamSize cartridgeRamSize)
        {
            switch (cartridgeRamSize)
            {
                case CartridgeRamSize.None:
                    return 0;
                case CartridgeRamSize.Fixed2kB:
                    return 0x800;
                case CartridgeRamSize.Fixed8kB:
                case CartridgeRamSize.Banked32kB:
                case CartridgeRamSize.Banked128kB:
                    return 0x2000;
                default:
                    throw new NotSupportedException("RamSize: " + cartridgeRamSize);
            }
        }
    }
}
