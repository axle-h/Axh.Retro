namespace Axh.Retro.CPU.X80.Contracts.Core
{
    public interface IArithmeticLogicUnit
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

        byte DecimalAdjust(byte a);

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
    }
}