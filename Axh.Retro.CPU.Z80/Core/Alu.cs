namespace Axh.Retro.CPU.Z80.Core
{
    using Axh.Retro.CPU.Z80.Util;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public class Alu<TRegisters> : IAlu where TRegisters : IRegisters
    {
        private readonly TRegisters registers;

        private IFlagsRegister Flags => registers.AccumulatorAndFlagsRegisters.Flags;

        public Alu(TRegisters registers)
        {
            this.registers = registers;
        }

        public byte Increment(byte b)
        {
            var result = b + 1;

            var flags = Flags;
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

            var flags = Flags;
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
            return Add(a, b, Flags.Carry);
        }
        
        public ushort Add(ushort a, ushort b)
        {
            return Add(a, b, false);
        }

        public ushort AddWithCarry(ushort a, ushort b)
        {
            return Add(a, b, true);
        }

        public byte Subtract(byte a, byte b)
        {
            return Subtract(a, b, false);
        }

        public byte SubtractWithCarry(byte a, byte b)
        {
            return Subtract(a, b, Flags.Carry);
        }
        
        public ushort SubtractWithCarry(ushort a, ushort b)
        {
            var flags = Flags;
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
            Subtract(a, b, false);
            Flags.SetUndocumentedFlags(b);
        }

        public byte And(byte a, byte b)
        {
            a &= b;

            var flags = Flags;
            flags.SetParityFlags(a);
            flags.HalfCarry = true;
            flags.Carry = false;
            return a;
        }

        public byte Or(byte a, byte b)
        {
            a |= b;

            var flags = Flags;
            flags.SetParityFlags(a);
            flags.HalfCarry = false;
            flags.Carry = false;
            return a;
        }

        public byte Xor(byte a, byte b)
        {
            a ^= b;

            var flags = Flags;
            flags.SetParityFlags(a);
            flags.HalfCarry = false;
            flags.Carry = false;
            return a;
        }




        /// <summary>
        /// http://www.worldofspectrum.org/faq/reference/z80reference.htm#DAA
        ///- If the A register is greater than 0x99, OR the Carry flag is SET, then
        ///
        ///    The upper four bits of the Correction Factor are set to 6,
        ///    and the Carry flag will be SET.
        ///  Else
        ///    The upper four bits of the Correction Factor are set to 0,
        ///    and the Carry flag will be CLEARED.
        ///
        ///
        ///- If the lower four bits of the A register (A AND 0x0F) is greater than 9,
        ///  OR the Half-Carry(H) flag is SET, then
        ///
        ///   The lower four bits of the Correction Factor are set to 6.
        ///  Else
        ///    The lower four bits of the Correction Factor are set to 0.
        ///
        ///
        ///- This results in a Correction Factor of 0x00, 0x06, 0x60 or 0x66.
        ///
        ///
        ///- If the N flag is CLEAR, then
        ///
        ///    ADD the Correction Factor to the A register.
        ///  Else
        ///    SUBTRACT the Correction Factor from the A register.
        ///
        ///
        ///- The Flags are set as follows:
        ///
        ///  Carry:      Set/clear as in the first step above.
        ///
        ///  Half-Carry: Set if the correction operation caused a binary carry/borrow
        ///              from bit 3 to bit 4.
        ///              For this purpose, may be calculated as:
        ///              Bit 4 of: A(before) XOR A(after).
        ///
        ///  S,Z,P,5,3:  Set as for simple logic operations on the resultant A value.
        ///
        ///  N:          Leave.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public byte DecimalAdjust(byte a)
        {
            var flags = Flags;
            byte n = 0;
            if ((a > 0x99) || flags.Carry)
            {
                n |= 0x60;
            }
            if (((a & 0x0F) > 0x09) || flags.HalfCarry)
            {
                n |= 0x06;
            }

            flags.HalfCarry = ((a >> 4) & 1) > 0;
            if (flags.Subtract)
            {
                a -= n;
            }
            else
            {
                a += n;
            }

            flags.HalfCarry = (((flags.HalfCarry ? 1 : 0) ^ (a >> 4)) & 1) > 0;
            flags.Carry = (n >> 6) > 0;
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

            var flags = Flags;
            flags.Carry = carry;
            flags.HalfCarry = false;
            flags.Subtract = false;
            flags.SetResultFlags(result);

            return result;
        }

        /// <summary>
        /// The value a is rotated left 1 bit position through the Carry flag.
        /// The previous contents of the Carry flag are copied to bit 0. Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        public byte RotateLeft(byte a)
        {
            var flags = Flags;
            var result = unchecked((byte)((a << 1) | (flags.Carry ? 1 : 0)));
            
            flags.Carry = (a & 0x80) > 0;
            flags.HalfCarry = false;
            flags.Subtract = false;
            flags.SetResultFlags(result);

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

            var flags = Flags;
            flags.Carry = carry;
            flags.HalfCarry = false;
            flags.Subtract = false;
            flags.SetResultFlags(result);

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
            var flags = Flags;
            var result = unchecked((byte)((a >> 1) | (flags.Carry ? 0x80 : 0)));
            
            flags.Carry = (a & 1) > 0;
            flags.HalfCarry = false;
            flags.Subtract = false;
            flags.SetResultFlags(result);

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
            var flags = Flags;
            flags.Carry = (a & 0x80) > 0;
            var result = unchecked((byte)(a << 1));

            flags.SetParityFlags(result);
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
            var flags = Flags;
            flags.Carry = (a & 0x80) > 0;
            var result = unchecked((byte)((a << 1) | 0x01));

            flags.SetParityFlags(result);
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
            var flags = Flags;
            flags.Carry = (a & 0x01) > 0;
            var result = unchecked((byte)((a >> 1) | (a & 0x80)));
            
            flags.SetParityFlags(result);
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
            var flags = Flags;
            flags.Carry = (a & 0x01) > 0;
            var result = unchecked((byte)(a >> 1));

            flags.SetParityFlags(result);
            flags.HalfCarry = false;
            return result;
        }

        /// <summary>
        /// Performs a 4-bit clockwise (right) rotation of the 12-bit number whose 4 most signigifcant bits are the 4 least significant bits of accumulator, and its 8 least significant bits are b.
        /// </summary>
        /// <param name="accumulator"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public AccumulatorAndResult RotateLeftDigit(byte accumulator, byte b)
        {
            var result = new AccumulatorAndResult
                         {
                             Accumulator = (byte)((accumulator & 0xf0) | ((b & 0xf0) >> 4)),
                             Result = (byte)(((b & 0x0f) << 4) | (accumulator & 0x0f))
                         };

            var flags = Flags;
            flags.SetParityFlags(result.Accumulator);
            flags.HalfCarry = false;
            return result;
        }

        /// <summary>
        /// Performs a 4-bit anti-clockwise (left) rotation of the 12-bit number whose 4 most signigifcant bits are the 4 least significant bits of accumulator, and its 8 least significant bits are b.
        /// </summary>
        /// <param name="accumulator"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public AccumulatorAndResult RotateRightDigit(byte accumulator, byte b)
        {
            var result = new AccumulatorAndResult
                         {
                             Accumulator = (byte)((accumulator & 0xf0) | (b & 0x0f)),
                             Result = (byte)(((accumulator & 0x0f) << 4) | ((b & 0xf0) >> 4))
                         };

            var flags = Flags;
            flags.SetParityFlags(result.Accumulator);
            flags.HalfCarry = false;
            return result;
        }

        /// <summary>
        /// Tests bit 'bit' in byte a and sets the Z flag accordingly.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="bit"></param>
        public void BitTest(byte a, int bit)
        {
            var flags = Flags;
            flags.Zero = ((0x1 << bit) & a) == 0;
            flags.HalfCarry = true;
            flags.Subtract = false;
            flags.SetUndocumentedFlags(a);

            // PV as Z, S set only if n=7 and b7 of r set
            flags.ParityOverflow = flags.Zero;
            flags.Sign = bit == 7 && flags.Zero;
        }
        
        public byte BitSet(byte a, int bit)
        {
            return (byte)(a | (0x1 << bit));
        }

        public byte BitReset(byte a, int bit)
        {
            return (byte)(a & ~(0x1 << bit));
        }

        public ushort AddDisplacement(ushort a, sbyte d)
        {
            var result = a + d;

            var flags = Flags;

            var carry = a ^ d ^ result;
            flags.Carry = (carry & 0x100) > 0;
            flags.HalfCarry = ((carry << 5) & 0x200) > 0;
            flags.Zero = false;
            flags.Subtract = false;

            return unchecked ((ushort)result);
        }

        public byte Swap(byte a)
        {
            var result = (byte)(((a & 0xf) << 4) | ((a & 0xf0) >> 4));

            var flags = Flags;
            flags.Zero = result == 0;
            flags.Subtract = false;
            flags.HalfCarry = false;
            flags.Carry = false;
            return result;
        }

        private byte Add(byte a, byte b, bool addCarry)
        {
            var carry = addCarry ? 1 : 0;
            var result = a + b + carry;

            var flags = Flags;
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
            var flags = Flags;
            var carry = addCarry && flags.Carry ? 1 : 0;
            var result = a + b + carry;

            // Half carry is carry from bit 11
            flags.HalfCarry = (((a & 0x0fff) + (b & 0x0fff) + carry) & 0xf000) > 0;
            
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
                var b0 = (result & 0xff00) >> 8;
                flags.SetUndocumentedFlags((byte)b0);
                b = unchecked((ushort)result);
            }
                
            return b;
        }

        private byte Subtract(byte a, byte b, bool addCarry)
        {
            var carry = addCarry ? 1 : 0;
            var result = a - b - carry;

            var flags = Flags;

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
