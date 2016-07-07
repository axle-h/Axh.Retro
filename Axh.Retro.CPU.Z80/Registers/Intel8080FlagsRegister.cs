using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Util;

namespace Axh.Retro.CPU.Z80.Registers
{
    /// <summary>
    /// Flags register used by the Z80 & Intel 8080 (I think)
    /// </summary>
    public class Intel8080FlagsRegister : IFlagsRegister
    {
        private const byte SignMask = 1 << 7;
        private const byte ZeroMask = 1 << 6;
        private const byte Flag5Mask = 1 << 5;
        private const byte HalfCarryMask = 1 << 4;
        private const byte Flag3Mask = 1 << 3;
        private const byte ParityOverflowMask = 1 << 2;
        private const byte SubtractMask = 1 << 1;
        private const byte CarryMask = 1;

        public Intel8080FlagsRegister()
        {
            ResetFlags();
        }

        public byte Register
        {
            get
            {
                byte ans = 0x00;

                if (Sign)
                {
                    ans |= SignMask;
                }

                if (Zero)
                {
                    ans |= ZeroMask;
                }

                if (Flag5)
                {
                    ans |= Flag5Mask;
                }

                if (HalfCarry)
                {
                    ans |= HalfCarryMask;
                }

                if (Flag3)
                {
                    ans |= Flag3Mask;
                }

                if (ParityOverflow)
                {
                    ans |= ParityOverflowMask;
                }

                if (Subtract)
                {
                    ans |= SubtractMask;
                }

                if (Carry)
                {
                    ans |= CarryMask;
                }

                return ans;
            }
            set
            {
                Sign = (value & SignMask) > 0;
                Zero = (value & ZeroMask) > 0;
                Flag5 = (value & Flag5Mask) > 0;
                HalfCarry = (value & HalfCarryMask) > 0;
                Flag3 = (value & Flag3Mask) > 0;
                ParityOverflow = (value & ParityOverflowMask) > 0;
                Subtract = (value & SubtractMask) > 0;
                Carry = (value & CarryMask) > 0;
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

        public void SetUndocumentedFlags(byte result)
        {
            // Undocumented flags are set from corresponding result bits.
            Flag5 = (result & Flag5Mask) > 0;
            Flag3 = (result & Flag3Mask) > 0;
        }

        public void SetResultFlags(byte result)
        {
            // Sign flag is a copy of the sign bit.
            Sign = (result & SignMask) > 0;

            // Set Zero flag is result = 0
            Zero = result == 0;

            SetUndocumentedFlags(result);
        }

        public void SetResultFlags(ushort result)
        {
            // Sign flag is a copy of the sign bit.
            Sign = (result & (SignMask << 8)) > 0;

            // Set Zero flag is result = 0
            Zero = result == 0;

            // Flag is affected by the high - byte addition.
            SetUndocumentedFlags((byte) (result >> 8));
        }

        public void SetParityFlags(byte result)
        {
            SetResultFlags(result);
            ParityOverflow = result.IsEvenParity();
            Subtract = false;
        }

        public void ResetFlags()
        {
            Sign = false;
            Zero = false;
            Flag5 = false;
            HalfCarry = false;
            Flag3 = false;
            ParityOverflow = false;
            Subtract = false;
            Carry = false;
        }

        public void SetFlags()
        {
            Sign = true;
            Zero = true;
            Flag5 = true;
            HalfCarry = true;
            Flag3 = true;
            ParityOverflow = true;
            Subtract = true;
            Carry = true;
        }

        public void SetCompareFlags(byte result, ushort byteCounter)
        {
            SetResultFlags(result);
            ParityOverflow = byteCounter != 0;
            Subtract = true;
        }
    }
}