namespace Axh.Retro.CPU.Z80.Core.Decode
{
    internal enum FlagTest : byte
    {
        None = 0,
        NotZero = 0x01,
        Zero = 0x02,
        NotCarry = 0x03,
        Carry = 0x04,
        ParityOdd = 0x05,
        ParityEven = 0x06,
        Possitive = 0x07,
        Negative = 0x08
    }
}
