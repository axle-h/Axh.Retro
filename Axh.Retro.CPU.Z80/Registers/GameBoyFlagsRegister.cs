using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Registers
{
    /// <summary>
    /// GB flags register.
    /// 7 6 5 4 3 2 1 0
    /// Z N H C 0 0 0 0
    /// </summary>
    public class GameBoyFlagsRegister : IFlagsRegister
    {
        private const byte ZeroMask = 1 << 7;
        private const byte SubtractMask = 1 << 6;
        private const byte HalfCarryMask = 1 << 5;
        private const byte CarryMask = 1 << 4;

        public byte Register
        {
            get
            {
                byte ans = 0x00;

                if (Zero)
                {
                    ans |= ZeroMask;
                }

                if (HalfCarry)
                {
                    ans |= HalfCarryMask;
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
                Zero = (value & ZeroMask) > 0;
                HalfCarry = (value & HalfCarryMask) > 0;
                Subtract = (value & SubtractMask) > 0;
                Carry = (value & CarryMask) > 0;
            }
        }

        /// <summary>
        /// Unused
        /// </summary>
        public bool Sign
        {
            get { return false; }
            set { }
        }

        public bool Zero { get; set; }

        /// <summary>
        /// Unused
        /// </summary>
        public bool Flag5
        {
            get { return false; }
            set { }
        }

        public bool HalfCarry { get; set; }

        /// <summary>
        /// Unused
        /// </summary>
        public bool Flag3
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Unused
        /// </summary>
        public bool ParityOverflow
        {
            get { return false; }
            set { }
        }

        public bool Subtract { get; set; }

        public bool Carry { get; set; }

        public void SetUndocumentedFlags(byte result)
        {
            // No undocumented flags on the GB
        }

        public void SetResultFlags(byte result)
        {
            // Set Zero flag is result = 0
            Zero = result == 0;
        }

        public void SetResultFlags(ushort result)
        {
            // Set Zero flag is result = 0
            Zero = result == 0;
        }

        public void SetParityFlags(byte result)
        {
            // No parity flag in GB so just set the results flags.
            SetResultFlags(result);
            Subtract = false;
        }

        public void ResetFlags()
        {
            Zero = false;
            HalfCarry = false;
            Subtract = false;
            Carry = false;
        }

        public void SetFlags()
        {
            Zero = true;
            HalfCarry = true;
            Subtract = true;
            Carry = true;
        }
    }
}