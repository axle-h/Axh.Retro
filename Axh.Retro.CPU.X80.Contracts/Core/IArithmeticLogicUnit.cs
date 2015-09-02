namespace Axh.Retro.CPU.X80.Contracts.Core
{
    public interface IArithmeticLogicUnit
    {
        byte Increment(byte b);

        byte Decrement(byte b);

        byte Add(byte a, byte b);

        byte AddWithCarry(byte a, byte b);

        byte Subtract(byte a, byte b);

        byte SubtractWithCarry(byte a, byte b);

        void Compare(byte a, byte b);

        byte And(byte a, byte b);

        byte Or(byte a, byte b);

        byte Xor(byte a, byte b);

        byte DecimalAdjust(byte a);
    }
}