namespace Axh.Retro.GameBoy.Contracts.Media
{
    public enum CartridgeRamSize
    {
        /// <summary>
        /// 0 - No cartridge RAM
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 1 - 16kBit = 2kB = 1 bank
        /// </summary>
        Fixed2kB = 0x01,

        /// <summary>
        /// 2 - 64kBit = 8kB = 1 bank
        /// </summary>
        Fixed8kB = 0x02,

        /// <summary>
        /// 3 - 256kBit = 32kB = 4 banks
        /// </summary>
        Banked32kB = 0x03,

        /// <summary>
        /// 4 - 1MBit =128kB = 16 banks
        /// </summary>
        Banked128kB = 0x04,
    }
}
