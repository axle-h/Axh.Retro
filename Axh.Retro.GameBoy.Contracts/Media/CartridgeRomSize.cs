namespace Axh.Retro.GameBoy.Contracts.Media
{
    public enum CartridgeRomSize : byte
    {
        /// <summary>
        ///     0 - 256Kbit = 32KByte = 2 banks
        /// </summary>
        Fixed32kB = 0x00,

        /// <summary>
        ///     1 - 512Kbit = 64KByte = 4 banks
        /// </summary>
        Banked64kB = 0x01,

        /// <summary>
        ///     2 - 1Mbit = 128KByte = 8 banks
        /// </summary>
        Banked128kB = 0x02,

        /// <summary>
        ///     3 - 2Mbit = 256KByte = 16 banks
        /// </summary>
        Banked256kB = 0x03,

        /// <summary>
        ///     4 - 4Mbit = 512KByte = 32 banks
        /// </summary>
        Banked512kB = 0x04,

        /// <summary>
        ///     5 - 8Mbit = 1MByte = 64 banks
        /// </summary>
        Banked1024kB = 0x05,

        /// <summary>
        ///     6 - 16Mbit = 2MByte = 128 banks
        /// </summary>
        Banked2048kB = 0x06,

        /// <summary>
        ///     7 - 32Mbit = 4MByte = 256 banks
        /// </summary>
        Banked4096kB = 0x07,

        /// <summary>
        ///     $52 - 9Mbit = 1.1MByte = 72 banks
        /// </summary>
        Banked1152kB = 0x52,

        /// <summary>
        ///     $53 - 10Mbit = 1.2MByte = 80 banks
        /// </summary>
        Banked1280kB = 0x53,

        /// <summary>
        ///     $54 - 12Mbit = 1.5MByte = 96 banks
        /// </summary>
        Banked1536kB = 0x54
    }
}