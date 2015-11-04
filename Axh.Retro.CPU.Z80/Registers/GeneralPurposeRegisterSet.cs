namespace Axh.Retro.CPU.Z80.Registers
{
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;
    using Axh.Retro.CPU.Z80.Util;

    public class GeneralPurposeRegisterSet : IGeneralPurposeRegisterSet
    {
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte H { get; set; }
        public byte L { get; set; }

        //16 Bit Registers
        public ushort BC
        {
            get
            {
                return BitConverterHelpers.To16Bit(this.B, this.C);
            }
            set
            {
                var bytes = BitConverterHelpers.To8Bit(value);
                this.B = bytes[1];
                this.C = bytes[0];
            }
        }

        public ushort DE
        {
            get
            {
                return BitConverterHelpers.To16Bit(this.D, this.E);
            }
            set
            {
                var bytes = BitConverterHelpers.To8Bit(value);
                this.D = bytes[1];
                this.E = bytes[0];
            }
        }

        public ushort HL
        {
            get
            {
                return BitConverterHelpers.To16Bit(this.H, this.L);
            }
            set
            {
                var bytes = BitConverterHelpers.To8Bit(value);
                this.H = bytes[1];
                this.L = bytes[0];
            }
        }
        
        public void Reset()
        {
            B = C = D = E = H = L = 0;
        }

        public void ResetToState(GeneralPurposeRegisterState state)
        {
            B = state.B;
            C = state.C;
            D = state.D;
            E = state.E;
            H = state.H;
            L = state.L;
        }

        public GeneralPurposeRegisterState GetRegisterState()
        {
            return new GeneralPurposeRegisterState
            {
                B = B,
                C = C,
                D = D,
                E = E,
                H = H,
                L = L
            };
        }
    }
}
