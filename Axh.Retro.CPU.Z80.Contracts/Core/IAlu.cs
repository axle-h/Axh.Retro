namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    public interface IAlu
    {
        byte Increment(byte b);

        byte Decrement(byte b);

        byte Add(byte a, byte b);

        byte AddWithCarry(byte a, byte b);

        ushort Add(ushort a, ushort b);

        ushort AddWithCarry(ushort a, ushort b);

        byte Subtract(byte a, byte b);

        byte SubtractWithCarry(byte a, byte b);

        ushort SubtractWithCarry(ushort a, ushort b);

        void Compare(byte a, byte b);

        byte And(byte a, byte b);

        byte Or(byte a, byte b);

        byte Xor(byte a, byte b);

        byte DecimalAdjust(byte a, bool setHalfCarry);

        /// <summary>
        /// The value a is rotated left 1 bit position.
        /// The sign bit (bit 7) is copied to the Carry flag and also to bit 0. Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        byte RotateLeftWithCarry(byte a);

        /// <summary>
        /// The value a is rotated left 1 bit position through the Carry flag.
        /// The previous contents of the Carry flag are copied to bit 0. Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        byte RotateLeft(byte a);

        /// <summary>
        /// The value a is rotated right 1 bit position.
        /// Bit 0 is copied to the Carry flag and also to bit 7. Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        byte RotateRightWithCarry(byte a);

        /// <summary>
        /// The value a is rotated right 1 bit position through the Carry flag.
        /// The previous contents of the Carry flag are copied to bit 7. Bit 0 is the leastsignificant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        byte RotateRight(byte a);

        /// <summary>
        /// An arithmetic shift left 1 bit position is performed on the contents of operand m.
        /// The contents of bit 7 are copied to the Carry flag.
        /// Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        byte ShiftLeft(byte a);

        /// <summary>
        /// Undocumented Z80 instruction known as SLS, SLL or SL1.
        /// An arithmetic shift left 1 bit position is performed on the contents of operand m.
        /// The contents of bit 7 are copied to the Carry flag, and bit 0 is set.
        /// Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        byte ShiftLeftSet(byte a);

        /// <summary>
        /// An arithmetic shift right 1 bit position is performed on the contents of operand m.
        /// The contents of bit 0 are copied to the Carry flag and the previous contents of bit 7 remain unchanged.
        /// Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        byte ShiftRight(byte a);

        /// <summary>
        /// The contents of operand m are shifted right 1 bit position.
        /// The contents of bit 0 are copied to the Carry flag, and bit 7 is reset.
        /// Bit 0 is the least-significant bit.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        byte ShiftRightLogical(byte a);

        /// <summary>
        /// Performs a 4-bit clockwise (right) rotation of the 12-bit number whose 4 most signigifcant bits are the 4 least significant bits of accumulator, and its 8 least significant bits are b.
        /// </summary>
        /// <param name="accumulator"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        AccumulatorAndResult RotateLeftDigit(byte accumulator, byte b);

        /// <summary>
        /// Performs a 4-bit anti-clockwise (left) rotation of the 12-bit number whose 4 most signigifcant bits are the 4 least significant bits of accumulator, and its 8 least significant bits are b.
        /// </summary>
        /// <param name="accumulator"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        AccumulatorAndResult RotateRightDigit(byte accumulator, byte b);

        /// <summary>
        /// Tests bit 'bit' in byte a and sets the Z flag accordingly.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="bit"></param>
        void BitTest(byte a, int bit);

        byte BitSet(byte a, int bit);

        byte BitReset(byte a, int bit);

        /// <summary>
        /// Add signed displacement to 16-bit register.
        /// Specific to GB ALU.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        ushort AddDisplacement(ushort a, sbyte d);

        /// <summary>
        /// Swap lower and upper nibbles.
        /// Specific to GB ALU.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        byte Swap(byte a);
    }
}