namespace Axh.Emulation.CPU.X80.Registers
{
    using Axh.Emulation.CPU.X80.Contracts.Registers;
    using Axh.Emulation.CPU.X80.Util;

    public class Z80FlagsRegister : IFlagsRegister
    {
        private const byte SignMask = 1 << 7;
        private const byte ZeroMask = 1 << 6;
        private const byte Flag5Mask = 1 << 5;
        private const byte HalfCarryMask = 1 << 4;
        private const byte Flag3Mask = 1 << 3;
        private const byte ParityOverflowMask = 1 << 2;
        private const byte SubtractMask = 1 << 1;
        private const byte CarryMask = 1;

        public byte Register
        {
            get
            {
                byte ans = 0x00;

                if (this.Sign)
                {
                    ans |= SignMask;
                }

                if (this.Zero)
                {
                    ans |= ZeroMask;
                }

                if (this.Flag5)
                {
                    ans |= Flag5Mask;
                }

                if (this.HalfCarry)
                {
                    ans |= HalfCarryMask;
                }

                if (this.Flag3)
                {
                    ans |= Flag3Mask;
                }

                if (this.ParityOverflow)
                {
                    ans |= ParityOverflowMask;
                }

                if (this.Subtract)
                {
                    ans |= SubtractMask;
                }

                if (this.Carry)
                {
                    ans |= CarryMask;
                }

                return ans;
            }
            set
            {
                this.Sign = (value & SignMask) > 0;
                this.Zero = (value & ZeroMask) > 0;
                this.Flag5 = (value & Flag5Mask) > 0;
                this.HalfCarry = (value & HalfCarryMask) > 0;
                this.Flag3 = (value & Flag3Mask) > 0;
                this.ParityOverflow = (value & ParityOverflowMask) > 0;
                this.Subtract = (value & SubtractMask) > 0;
                this.Carry = (value & CarryMask) > 0;
            }
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

        public Z80FlagsRegister()
        {
            ResetFlags();
        }

        public void SetUndocumentedFlags(byte res)
        {
            // Undocumented flags are set from corresponding result bits.
            this.Flag5 = (res & Flag5Mask) > 0;
            this.Flag3 = (res & Flag3Mask) > 0;
        }

        public void SetResultFlags(byte res)
        {
            // Sign flag is a copy of the sign bit.
            this.Sign = (res & SignMask) > 0;
            
            // Set Zero flag is result = 0
            this.Zero = res == 0;

            this.SetUndocumentedFlags(res);
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
