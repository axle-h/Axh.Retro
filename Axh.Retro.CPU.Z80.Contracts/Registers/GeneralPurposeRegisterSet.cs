using Axh.Retro.CPU.Common.Contracts.Util;
using Axh.Retro.CPU.Z80.Contracts.State;

namespace Axh.Retro.CPU.Z80.Contracts.Registers
{
    public class GeneralPurposeRegisterSet
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
            get { return BitConverterHelpers.To16Bit(B, C); }
            set
            {
                var bytes = BitConverterHelpers.To8Bit(value);
                B = bytes[1];
                C = bytes[0];
            }
        }

        public ushort DE
        {
            get { return BitConverterHelpers.To16Bit(D, E); }
            set
            {
                var bytes = BitConverterHelpers.To8Bit(value);
                D = bytes[1];
                E = bytes[0];
            }
        }

        public ushort HL
        {
            get { return BitConverterHelpers.To16Bit(H, L); }
            set
            {
                var bytes = BitConverterHelpers.To8Bit(value);
                H = bytes[1];
                L = bytes[0];
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
            return new GeneralPurposeRegisterState { B = B, C = C, D = D, E = E, H = H, L = L };
        }
    }
}