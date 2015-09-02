namespace Axh.Retro.CPU.X80.Core
{
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Registers;

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
            return Add(a, b, 0);
        }
        
        public byte AddWithCarry(byte a, byte b)
        {
            return Add(a, b, flags.Carry ? 1 : 0);
        }

        public byte Subtract(byte a, byte b)
        {
            return Subtract(a, b, 0);
        }

        public byte SubtractWithCarry(byte a, byte b)
        {
            return Subtract(a, b, flags.Carry ? 1 : 0);
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
            return a;
        }

        public byte Or(byte a, byte b)
        {
            a |= b;
            flags.SetParityFlags(a);
            return a;
        }

        public byte Xor(byte a, byte b)
        {
            a ^= b;
            flags.SetParityFlags(a);
            return a;
        }

        private byte Add(byte a, byte b, int carry)
        {
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

        private byte Subtract(byte a, byte b, int carry)
        {
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
