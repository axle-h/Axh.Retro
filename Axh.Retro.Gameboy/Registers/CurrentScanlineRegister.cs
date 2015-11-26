namespace Axh.Retro.GameBoy.Registers
{
    using Axh.Retro.GameBoy.Registers.Interfaces;

    public class CurrentScanlineRegister : ICurrentScanlineRegister
    {
        public CurrentScanlineRegister()
        {
            this.Scanline = 0;
        }

        public ushort Address => 0xff44;

        public string Name => "Current Scanline (LY R)";

        public byte Register
        {
            get
            {
                return Scanline;
            }
            set
            {
                Scanline = 0x00;
            }
        }

        public string DebugView => this.ToString();

        public override string ToString()
        {
            return $"{Name} ({Address}) = {Register}"; ;
        }
        
        public void IncrementScanline()
        {
            Scanline = (byte)((Scanline + 1) % 154);   
        }

        public byte Scanline { get; private set; }
    }
}
