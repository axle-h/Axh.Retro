namespace Axh.Emulation.CPU.Z80.Registers
{
    using Axh.Emulation.CPU.Z80.Contracts.Registers;
    using Axh.Emulation.CPU.Z80.Util;

    public class FlagsRegister : IFlagsRegister
    {
        private const byte SignMask = 0x80;
        private const byte ZeroMask = 0x40;
        private const byte Flag5Mask = 0x20;
        private const byte HalfCarryMask = 0x10;
        private const byte Flag3Mask = 0x08;
        private const byte ParityOverflowMask = 0x04;
        private const byte SubtractMask = 0x02;
        private const byte CarryMask = 0x01;

        public byte Register
        {
            get
            {
                return (byte)(0x00
                               | (this.Sign ? 1 : 0) << 7
                               | (this.Zero ? 1 : 0) << 6
                               | (this.Flag5 ? 1 : 0) << 5
                               | (this.HalfCarry ? 1 : 0) << 4
                               | (this.Flag3 ? 1 : 0) << 3
                               | (this.ParityOverflow ? 1 : 0) << 2
                               | (this.Subtract ? 1 : 0) << 1
                               | (this.Carry ? 1 : 0));
            }
            set
            {
                this.Sign = (value & (1 << 7)) != 0;
                this.Zero = (value & (1 << 6)) != 0;
                this.Flag5 = (value & (1 << 5)) != 0;
                this.HalfCarry = (value & (1 << 4)) != 0;
                this.Flag3 = (value & (1 << 3)) != 0;
                this.ParityOverflow = (value & (1 << 2)) != 0;
                this.Subtract = (value & (1 << 1)) != 0;
                this.Carry = (value & 1) != 0;
            }
        }

        private static void SetFlag(byte flagMask, bool value)
        {
            
        }

        // Flags
        public bool Sign { get; set; }
        public bool Zero { get; set; }
        public bool Flag5 { get; set; }
        public bool HalfCarry { get; set; }
        public bool Flag3 { get; set; }
        public bool ParityOverflow { get; set; }
        public bool Subtract { get; set; }
        public bool Carry { get; set; }

        public void SetResultFlags(byte res)
        {
            this.Sign = (res & 0x80) != 0;
            this.Zero = res == 0;
            this.SetUndocumentedFlags(res);
        }

        public void SetUndocumentedFlags(byte res)
        {
            this.Flag5 = (res & 0x20) != 0;
            this.Flag3 = (res & 0x08) != 0;
        }

        public void SetParityFlags(byte res)
        {
            this.SetResultFlags(res);
            this.HalfCarry = true;
            this.ParityOverflow = res.IsEvenParity();
            this.Subtract = false;
            this.Carry = false;
        }

        public void ResetFlags()
        {
            this.Sign = false;
            this.Zero = false;
            this.Flag5 = false;
            this.HalfCarry = false;
            this.Flag3 = false;
            this.ParityOverflow = false;
            this.Subtract = false;
            this.Carry = false;
        }

        public void SetFlags()
        {
            this.Sign = true;
            this.Zero = true;
            this.Flag5 = true;
            this.HalfCarry = true;
            this.Flag3 = true;
            this.ParityOverflow = true;
            this.Subtract = true;
            this.Carry = true;
        }
    }
}
