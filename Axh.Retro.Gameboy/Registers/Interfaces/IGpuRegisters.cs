namespace Axh.Retro.GameBoy.Registers.Interfaces
{
    /// <summary>
    /// GameBoy GPU registers.
    /// </summary>
    public interface IGpuRegisters
    {
        /// <summary>
        /// Gets the scroll x register.
        /// </summary>
        /// <value>
        /// The scroll x register.
        /// </value>
        IRegister ScrollXRegister { get; }

        /// <summary>
        /// Gets the scroll y register.
        /// </summary>
        /// <value>
        /// The scroll y register.
        /// </value>
        IRegister ScrollYRegister { get; }

        /// <summary>
        /// Gets the LCD control register.
        /// </summary>
        /// <value>
        /// The LCD control register.
        /// </value>
        ILcdControlRegister LcdControlRegister { get; }

        /// <summary>
        /// Gets the current scanline register.
        /// </summary>
        /// <value>
        /// The current scanline register.
        /// </value>
        ICurrentScanlineRegister CurrentScanlineRegister { get; }

        /// <summary>
        /// Gets the LCD monochrome palette register.
        /// </summary>
        /// <value>
        /// The LCD monochrome palette register.
        /// </value>
        ILcdMonochromePaletteRegister LcdMonochromePaletteRegister { get; }

        /// <summary>
        /// Gets the LCD status register.
        /// </summary>
        /// <value>
        /// The LCD status register.
        /// </value>
        ILcdStatusRegister LcdStatusRegister { get; }

        /// <summary>
        /// Gets the window X position register.
        /// This is the upper left corner of the window minus 7.
        /// Values can be 0 - 166.
        /// WX=7 locates the window at upper left of the screen, it is then completely covering normal background in the horizontal direction.
        /// </summary>
        /// <value>
        /// The window X position register.
        /// </value>
        IRegister WindowXPositionRegister { get; }

        /// <summary>
        /// Gets the window Y position register.
        /// This is the upper left corner of the window.
        /// Values can be 0 - 143.
        /// WY=0 locates the window at upper left of the screen, it is then completely covering normal background in the vertical direction.
        /// </summary>
        /// <value>
        /// The window Y position register.
        /// </value>
        IRegister WindowYPositionRegister { get; }
    }
}