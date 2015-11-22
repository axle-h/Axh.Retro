namespace Axh.Retro.GameBoy.Registers
{
    using System;

    using Axh.Retro.GameBoy.Devices.CoreInterfaces;
    using Axh.Retro.GameBoy.Registers.Interfaces;
    using Axh.Retro.GameBoy.Util;

    internal class LcdControlRegister : ILcdControlRegister
    {
        private LcdControl lcdControl;

        public LcdControlRegister()
        {
            this.lcdControl = LcdControl.None;
        }

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
                return (byte)lcdControl;
            }
            set
            {
                lcdControl = (LcdControl)value;
            }
        }

        [Flags]
        private enum LcdControl : byte
        {
            None = 0,
            BackgroundDisplay = 0x01,
            WindowColor0Transparent = 0x02,
            SpriteSize = 0x04,
            BackgroundTileMap = 0x08,
            TilePatternTable = 0x10,
            WindowDisplay = 0x20,
            WindowTileMap = 0x40,
            LcdOperation = 0x80
        }

        public string DebugView => this.ToString();

        /// <summary>
        /// LCD status
        /// True: On
        /// False: Off
        /// </summary>
        public bool LcdOperation => this.lcdControl.HasFlag(LcdControl.LcdOperation);

        /// <summary>
        /// Sets which tile map the window uses
        /// True: 9C00-9FFF (1)
        /// False: 9800-9BFF (0)
        /// </summary>
        public bool WindowTileMap => this.lcdControl.HasFlag(LcdControl.WindowTileMap);

        /// <summary>
        /// Window status
        /// True: On
        /// False: Off
        /// </summary>
        public bool WindowDisplay => this.lcdControl.HasFlag(LcdControl.WindowDisplay);

        /// <summary>
        /// Sets which tile pattern table to use
        /// True: 8000-8FFF (1)
        /// False: 8800-97FF (0)
        /// </summary>
        public bool TilePatternTable => this.lcdControl.HasFlag(LcdControl.TilePatternTable);

        /// <summary>
        /// Sets which tile map the background uses
        /// True: 9C00-9FFF (1)
        /// False: 9800-9BFF (0)
        /// </summary>
        public bool BackgroundTileMap => this.lcdControl.HasFlag(LcdControl.BackgroundTileMap);

        /// <summary>
        /// Sets the sprite size
        /// True: 8x16
        /// False: 8x8
        /// </summary>
        public bool SpriteSize => this.lcdControl.HasFlag(LcdControl.SpriteSize);

        /// <summary>
        /// Sets the transparency of colour 0 on the window
        /// True: SOLID
        /// False: TRANSPARENT
        /// </summary>
        public bool WindowColor0Transparent => this.lcdControl.HasFlag(LcdControl.WindowColor0Transparent);

        /// <summary>
        /// Background status
        /// True: On
        /// False: Off
        /// </summary>
        public bool BackgroundDisplay => this.lcdControl.HasFlag(LcdControl.BackgroundDisplay);

        public override string ToString()
        {
            return $"{Name} ({Address}) = {Register}\n\tLcdOperation: {LcdOperation}\n\tWindowTileMap: {WindowTileMap}\n\tWindowDisplay: {WindowDisplay}\n\tTilePatternTable: {TilePatternTable}\n\tBackgroundTileMap: {BackgroundTileMap}\n\tSpriteSize: {SpriteSize}\n\tWindowColor0Transparent: {WindowColor0Transparent}\n\tBackgroundDisplay: {BackgroundDisplay}";
        }
    }
    
}
