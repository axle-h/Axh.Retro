namespace Axh.Retro.GameBoy.Registers
{
    using Axh.Retro.GameBoy.Registers.Interfaces;

    public class CurrentScanlineRegister : ICurrentScanlineRegister
    {
        private byte register;

        public CurrentScanlineRegister()
        {
            this.register = 0;
        }

        public ushort Address => 0xff44;

        public string Name => "Current Scanline (LY R)";

        public byte Register
        {
            get
            {
                return register;
            }
            set
            {
                register = 0x00;
            }
        }

        public string DebugView => this.ToString();

        public override string ToString()
        {
            return $"{Name} ({Address}) = {Register}"; ;
        }

        public void SetCurrentScanline(int scanline)
        {
            if (scanline > 153)
            {
                this.register = 153;
            }
            else
            {
                this.register = (byte)scanline;
            }   
        }
    }
}
