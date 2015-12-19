namespace Axh.Retro.GameBoy.Registers.Interfaces
{
    public interface IGpuRegisters
    {
        /// <summary>
        /// Scroll X register.
        /// </summary>
        IRegister ScrollXRegister { get; }

        /// <summary>
        /// Scroll Y register.
        /// </summary>
        IRegister ScrollYRegister { get; }

        /// <summary>
        /// LCD controller register.
        /// </summary>
        ILcdControlRegister LcdControlRegister { get; }

        /// <summary>
        /// Current scanline register.
        /// </summary>
        ICurrentScanlineRegister CurrentScanlineRegister { get; }
    }
}
