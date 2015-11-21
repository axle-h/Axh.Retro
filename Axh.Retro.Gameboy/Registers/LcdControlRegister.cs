namespace Axh.Retro.GameBoy.Registers
{
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;
    using Axh.Retro.GameBoy.Registers.Interfaces;
    using Axh.Retro.GameBoy.Util;

    internal class LcdControlRegister : ILcdControlRegister
    {
        public ushort Address => 0xff40;

        public string Name => "LCD Control (LCDCONT R/W)";

        /// <summary>
        /// FF40 -- LCDCONT [RW] LCD Control             | when set to 1 | when set to 0
        /// Bit7 LCD operation                           | ON            | OFF
        /// Bit6 Window Tile Map address                 | 9C00-9FFF     | 9800-9BFF
        /// Bit5 Window display                          | ON            | OFF
        /// Bit4 Tile Pattern Table address              | 8000-8FFF     | 8800-97FF
        /// Bit3 Background Tile Map address             | 9C00-9FFF     | 9800-9BFF
        /// Bit2 Sprite size                             | 8x16          | 8x8
        /// Bit1 Color #0 transparency in the window     | SOLID         | TRANSPARENT
        /// Bit0 Background display                      | ON            | OFF
        /// </summary>
        public byte Register
        {
            get
            {
                return RegisterHelpers.GetRegister(LcdOperation, WindowTileMap, WindowDisplay, TilePatternTable, BackgroundTileMap, SpriteSize, WindowColor0Transparent, BackgroundDisplay);
            }
            set
            {
                LcdOperation = RegisterHelpers.GetBit(value, 7);
                WindowTileMap = RegisterHelpers.GetBit(value, 6);
                WindowDisplay = RegisterHelpers.GetBit(value, 5);
                TilePatternTable = RegisterHelpers.GetBit(value, 4);
                BackgroundTileMap = RegisterHelpers.GetBit(value, 3);
                SpriteSize = RegisterHelpers.GetBit(value, 2);
                WindowColor0Transparent = RegisterHelpers.GetBit(value, 1);
                BackgroundDisplay = RegisterHelpers.GetBit(value, 0);
            }
        }

        public string DebugView => this.ToString();

        /// <summary>
        /// LCD status
        /// True: On
        /// False: Off
        /// </summary>
        public bool LcdOperation { get; private set; }

        /// <summary>
        /// Sets which tile map the window uses
        /// True: 9C00-9FFF (1)
        /// False: 9800-9BFF (0)
        /// </summary>
        public bool WindowTileMap { get; private set; }

        /// <summary>
        /// Window status
        /// True: On
        /// False: Off
        /// </summary>
        public bool WindowDisplay { get; private set; }

        /// <summary>
        /// Sets which tile pattern table to use
        /// True: 8000-8FFF (1)
        /// False: 8800-97FF (0)
        /// </summary>
        public bool TilePatternTable { get; private set; }

        /// <summary>
        /// Sets which tile map the background uses
        /// True: 9C00-9FFF (1)
        /// False: 9800-9BFF (0)
        /// </summary>
        public bool BackgroundTileMap { get; private set; }

        /// <summary>
        /// Sets the sprite size
        /// True: 8x16
        /// False: 8x8
        /// </summary>
        public bool SpriteSize { get; private set; }

        /// <summary>
        /// Sets the transparency of colour 0 on the window
        /// True: SOLID
        /// False: TRANSPARENT
        /// </summary>
        public bool WindowColor0Transparent { get; private set; }

        /// <summary>
        /// Background status
        /// True: On
        /// False: Off
        /// </summary>
        public bool BackgroundDisplay { get; private set; }

        public override string ToString()
        {
            return $"{Name} ({Address}) = {Register}\nLcdOperation: {LcdOperation}\nWindowTileMap: {WindowTileMap}\nWindowDisplay: {WindowDisplay}\nTilePatternTable: {TilePatternTable}\nBackgroundTileMap: {BackgroundTileMap}\nSpriteSize: {SpriteSize}\nWindowColor0Transparent: {WindowColor0Transparent}\nBackgroundDisplay: {BackgroundDisplay}";
        }
    }
    
}
