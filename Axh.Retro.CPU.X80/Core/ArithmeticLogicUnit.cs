namespace Axh.Retro.CPU.X80.Core
{
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Util;

    public class ArithmeticLogicUnit : IArithmeticLogicUnit
    {
        private readonly IFlagsRegister flags;

        public ArithmeticLogicUnit(IFlagsRegister flags)
        {
            this.flags = flags;
        }

        public byte Increment(byte b)
        {
            var result = b + 1;

            flags.HalfCarry = (b & 0x0f) == 0x0f;
            flags.ParityOverflow = b == 0x7f;
            flags.Subtract = false;

            b = unchecked((byte)result);
            flags.SetResultFlags(b);
            return b;
        }
        
        public byte Decrement(byte b)
        {
            var result = b - 1;

            flags.HalfCarry = (b & 0x0f) == 0;
            flags.ParityOverflow = b == 0x80;
            flags.Subtract = true;

            b = unchecked((byte)result);
            flags.SetResultFlags(b);
            return b;
        }

        public byte Add(byte a, byte b)
        {
            return Add(a, b, false);
        }

        public byte AddWithCarry(byte a, byte b)
        {
            return Add(a, b, flags.Carry);
        }
        
        public ushort Add(ushort a, ushort b)
        {
            return Add(a, b, false);
        }

        public ushort AddWithCarry(ushort a, ushort b)
        {
            return Add(a, b, flags.Carry);
        }

        public byte Subtract(byte a, byte b)
        {
            return Subtract(a, b, false);
        }

        public byte SubtractWithCarry(byte a, byte b)
        {
            return Subtract(a, b, flags.Carry);
        }
        
        public ushort SubtractWithCarry(ushort a, ushort b)
        {
            var carry = flags.Carry ? 1 : 0;
            var result = a - b - carry;

            flags.HalfCarry = (a & 0x0f00) < (b & 0x0f00) + carry;

            // Carry = result > ushort.MinValue;
            flags.Carry = result < 0;
            flags.Subtract = true;

            // Overflow = (added signs are same) && (result sign differs from the sign of either of operands)
            flags.ParityOverflow = (((a ^ ~b) & 0x8000) == 0) && (((result ^ a) & 0x8000) != 0);
            b = unchecked((ushort)result);
            flags.SetResultFlags(b);

            return b;
        }

        public void Compare(byte a, byte b)
        {
            var result = a - b;

            flags.HalfCarry = (a & 0x0f) < (b & 0x0f);

            // Overflow = (added signs are same) && (result sign differs from the sign of either of operands)
            flags.ParityOverflow = (((a ^ ~b) & 0x80) == 0) && (((result ^ a) & 0x80) != 0);
            
            flags.Subtract = true;

            b = unchecked((byte)result);
            flags.SetResultFlags(b);
        }

        public byte And(byte a, byte b)
        {
            a &= b;
            flags.SetParityFlags(a);
            flags.HalfCarry = true;
            flags.Carry = false;
            return a;
        }

        public byte Or(byte a, byte b)
        {
            a |= b;
            flags.SetParityFlags(a);
            flags.HalfCarry = true;
            flags.Carry = false;
            return a;
        }

        public byte Xor(byte a, byte b)
        {
            a ^= b;
            flags.SetParityFlags(a);
            flags.HalfCarry = true;
            flags.Carry = false;
            return a;
        }

        public byte DecimalAdjust(byte a)
        {
            var a0 = a;

            if ((a & 0x0f) > 9 || flags.HalfCarry)
            {
                if (flags.Subtract)
                {
                    if (a - 0x06 < 0) flags.Carry = true;
                    a = unchecked((byte)(a - 0x06));
                }
                else
                {
                    if (a + 0x06 > 0x99) flags.Carry = true;
                    a = unchecked((byte)(a + 0x06));
                }
            }

            if (((a & 0xf0) >> 4) > 9 || flags.Carry)
            {
                if (flags.Subtract)
                {
                    if (a - 0x60 < 0) flags.Carry = true;
                    a = unchecked((byte)(a - 0x60));
                }
                else
                {
                    if (a + 0x60 > 0x99) flags.Carry = true;
                    a = unchecked((byte)(a + 0x60));
                }
            }

            flags.HalfCarry = ((a ^ a0) & 0x10) > 0;
            flags.ParityOverflow = a.IsEvenParity();
            flags.SetResultFlags(a);
            return a;
        }

        /// <summary>
        /// The value a is rotated left 1 bit position.
        /// The sign bit (bit 7) is copied to the Carry flag and also to bit 0. Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public byte RotateLeftWithCarry(byte a)
        {
            var carry = (a & 0x80) > 0;
            var result = unchecked((byte)((a << 1) | (carry ? 1 : 0)));

            flags.Carry = carry;
            flags.HalfCarry = false;
            flags.Subtract = false;
            flags.SetUndocumentedFlags(result);

            return result;
        }

        /// <summary>
        /// The value a is rotated left 1 bit position through the Carry flag.
        /// The previous contents of the Carry flag are copied to bit 0. Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        public byte RotateLeft(byte a)
        {
            var result = unchecked((byte)((a << 1) | (flags.Carry ? 1 : 0)));
            
            flags.Carry = (a & 0x80) > 0;
            flags.HalfCarry = false;
            flags.Subtract = false;
            flags.SetUndocumentedFlags(result);

            return result;
        }

        /// <summary>
        /// The value a is rotated right 1 bit position.
        /// Bit 0 is copied to the Carry flag and also to bit 7. Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public byte RotateRightWithCarry(byte a)
        {
            var carry = (a & 1) > 0;
            var result = unchecked((byte)((a >> 1) | (carry ? 0x80 : 0)));

            flags.Carry = carry;
            flags.HalfCarry = false;
            flags.Subtract = false;
            flags.SetUndocumentedFlags(result);

            return result;
        }

        /// <summary>
        /// The value a is rotated right 1 bit position through the Carry flag.
        /// The previous contents of the Carry flag are copied to bit 7. Bit 0 is the leastsignificant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public byte RotateRight(byte a)
        {
            var result = unchecked((byte)((a >> 1) | (flags.Carry ? 0x80 : 0)));

            flags.Carry = (a & 1) > 0;
            flags.HalfCarry = false;
            flags.Subtract = false;
            flags.SetUndocumentedFlags(result);

            return result;
        }

        /// <summary>
        /// An arithmetic shift left 1 bit position is performed on the contents of operand m.
        /// The contents of bit 7 are copied to the Carry flag.
        /// Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public byte ShiftLeft(byte a)
        {
            flags.Carry = (a & 0x80) > 0;
            var result = unchecked((byte)(a << 1));

            this.flags.SetParityFlags(result);
            flags.HalfCarry = false;
            return result;
        }

        /// <summary>
        /// Undocumented Z80 instruction known as SLS, SLL or SL1.
        /// An arithmetic shift left 1 bit position is performed on the contents of operand m.
        /// The contents of bit 7 are copied to the Carry flag, and bit 0 is set.
        /// Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public byte ShiftLeftSet(byte a)
        {
            flags.Carry = (a & 0x80) > 0;
            var result = unchecked((byte)((a << 1) | 0x01));

            this.flags.SetParityFlags(result);
            flags.HalfCarry = false;
            return result;
        }

        /// <summary>
        /// An arithmetic shift right 1 bit position is performed on the contents of operand m.
        /// The contents of bit 0 are copied to the Carry flag and the previous contents of bit 7 remain unchanged.
        /// Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public byte ShiftRight(byte a)
        {
            flags.Carry = (a & 0x01) > 0;
            var result = unchecked((byte)((a >> 1) | (a & 0x80)));
            
            this.flags.SetParityFlags(result);
            flags.HalfCarry = false;
            return result;
        }

        /// <summary>
        /// The contents of operand m are shifted right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag, and bit 7 is reset.
        /// Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public byte ShiftRightLogical(byte a)
        {
            flags.Carry = (a & 0x01) > 0;
            var result = unchecked((byte)(a >> 1));

            this.flags.SetParityFlags(result);
            flags.HalfCarry = false;
            return result;
        }

        private byte Add(byte a, byte b, bool addCarry)
        {
            var carry = addCarry ? 1 : 0;
            var result = a + b + carry;

            flags.HalfCarry = (((a & 0x0f) + (b & 0x0f) + (carry & 0x0f)) & 0xf0) > 0;

            // Overflow = (added signs are same) && (result sign differs from the sign of either of operands)
            flags.ParityOverflow = (((a ^ b) & 0x80) == 0) && (((result ^ a) & 0x80) != 0);

            // Carry = result > byte.MaxValue;
            flags.Carry = (result & 0x100) == 0x100;

            flags.Subtract = false;

            b = unchecked((byte)result);
            flags.SetResultFlags(b);
            return b;
        }

        private ushort Add(ushort a, ushort b, bool addCarry)
        {
            var carry = addCarry ? 1 : 0;
            var result = a + b + carry;

            // Half carry is carry from bit 11
            flags.HalfCarry = (((a & 0x0f00) + (b & 0x0f00) + (carry & 0x0f00)) & 0xf000) > 0;

            // Carry = result > ushort.MaxValue;
            flags.Carry = (result & 0x10000) == 0x10000;

            flags.Subtract = false;

            if (addCarry)
            {
                // Overflow = (added signs are same) && (result sign differs from the sign of either of operands)
                flags.ParityOverflow = (((a ^ b) & 0x8000) == 0) && (((result ^ a) & 0x8000) != 0);
                b = unchecked((ushort)result);
                flags.SetResultFlags(b);
            }
            else
            {
                // S & Z are unaffected so we're only setting the undocumented flags from the last 8-bit addition
                var b0 = unchecked((byte)((result & 0xff00) >> 8));
                flags.SetUndocumentedFlags(b0);
                b = unchecked((ushort)result);
            }
            
            return b;
        }

        private byte Subtract(byte a, byte b, bool addCarry)
        {
            var carry = addCarry ? 1 : 0;
            var result = a - b - carry;

            flags.HalfCarry = (a & 0x0f) < (b & 0x0f) + carry;

            // Overflow = (added signs are same) && (result sign differs from the sign of either of operands)
            flags.ParityOverflow = (((a ^ ~b) & 0x80) == 0) && (((result ^ a) & 0x80) != 0);

            // Carry = result > byte.MinValue;
            flags.Carry = result < 0;
            flags.Subtract = true;

            b = unchecked((byte)result);
            flags.SetResultFlags(b);
            return b;
        }
    }
}
