namespace Axh.Retro.GameBoy.Registers
{
    using Axh.Retro.GameBoy.Registers.Interfaces;

    public class GpuRegisters : IGpuRegisters
    {
        public GpuRegisters(ILcdControlRegister lcdControlRegister, ICurrentScanlineRegister currentScanlineRegister)
        {
            LcdControlRegister = lcdControlRegister;
            CurrentScanlineRegister = currentScanlineRegister;
            ScrollXRegister = new Interfaces.SimpleRegister(0xff43, "Background Horizontal Scrolling (SCROLLX R/W)");
            ScrollYRegister = new Interfaces.SimpleRegister(0xff42, "Background Vertical Scrolling (SCROLLY R/W)");
        }

        public IRegister ScrollXRegister { get; }

        public IRegister ScrollYRegister { get; }

        public ILcdControlRegister LcdControlRegister { get; }

        public ICurrentScanlineRegister CurrentScanlineRegister { get; }
    }
}
