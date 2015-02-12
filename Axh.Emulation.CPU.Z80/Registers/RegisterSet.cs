namespace Axh.Emulation.CPU.Z80.Registers
{
    using System;
    using Axh.Emulation.CPU.Z80.Contracts.Registers;

    public class RegisterSet : IRegisterSet
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

        public RegisterSet(IFlagsRegister flagsRegister)
        {
            this.Flags = flagsRegister;
        }

        private static ushort To16Bit(byte rH, byte rL)
        {
            return (ushort)((rH << 8) | rL); //this is big endian for some reason
        }

        private static byte[] To8Bit(ushort r0)
        {
            return BitConverter.GetBytes(r0);
        }
    }
}
