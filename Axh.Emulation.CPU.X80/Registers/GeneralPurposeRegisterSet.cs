namespace Axh.Emulation.CPU.X80.Registers
{
    using System;

    using Axh.Emulation.CPU.X80.Contracts.Registers;
    using Axh.Emulation.CPU.X80.Contracts.State;

    public class GeneralPurposeRegisterSet : IGeneralPurposeRegisterSet
    {
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte H { get; set; }
        public byte L { get; set; }

        public IFlagsRegister Flags { get; private set; }

        //16 Bit Registers
        public ushort AF
        {
            get
            {
                return To16Bit(this.A, this.Flags.Register);
            }
            set
            {
                var bytes = To8Bit(value);
                this.A = bytes[1];
                this.Flags.Register = bytes[0];
            }
        }

        public ushort BC
        {
            get
            {
                return To16Bit(this.B, this.C);
            }
            set
            {
                var bytes = To8Bit(value);
                this.B = bytes[1];
                this.C = bytes[0];
            }
        }

        public ushort DE
        {
            get
            {
                return To16Bit(this.D, this.E);
            }
            set
            {
                var bytes = To8Bit(value);
                this.D = bytes[1];
                this.E = bytes[0];
            }
        }

        public ushort HL
        {
            get
            {
                return To16Bit(this.H, this.L);
            }
            set
            {
                var bytes = To8Bit(value);
                this.H = bytes[1];
                this.L = bytes[0];
            }
        }

        public GeneralPurposeRegisterSet(IFlagsRegister flagsRegister)
        {
            this.Flags = flagsRegister;
        }

        public void Reset()
        {
            A = B = C = D = E = H = L = 0;
            this.Flags.ResetFlags();
        }

        public void ResetToState(GeneralPurposeRegisterState state)
        {
            A = state.A;
            B = state.B;
            C = state.C;
            D = state.D;
            E = state.E;
            Flags.Register = state.F;
            H = state.H;
            L = state.L;
        }

        public GeneralPurposeRegisterState GetRegisterState()
        {
            return new GeneralPurposeRegisterState
            {
                A = A,
                B = B,
                C = C,
                D = D,
                E = E,
                F = Flags.Register,
                H = H,
                L = L
            };
        }

        private static ushort To16Bit(byte rH, byte rL)
        {
            return (ushort)((rH << 8) | rL);
        }

        private static byte[] To8Bit(ushort r0)
        {
            return BitConverter.GetBytes(r0);
        }
    }
}
