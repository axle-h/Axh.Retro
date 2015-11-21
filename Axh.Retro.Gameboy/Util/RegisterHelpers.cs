namespace Axh.Retro.GameBoy.Util
{
    internal static class RegisterHelpers
    {
        public static byte GetRegister(bool bit7, bool bit6, bool bit5, bool bit4, bool bit3, bool bit2, bool bit1, bool bit0)
        {
            byte register = 0;
            if (bit7)
            {
                register |= 0x80;
            }

            if (bit6)
            {
                register |= 0x40;
            }

            if (bit5)
            {
                register |= 0x20;
            }

            if (bit4)
            {
                register |= 0x10;
            }

            if (bit3)
            {
                register |= 0x8;
            }

            if (bit2)
            {
                register |= 0x4;
            }

            if (bit1)
            {
                register |= 0x2;
            }

            if (bit0)
            {
                register |= 0x1;
            }

            return register;
        }

        public static bool GetBit(byte register, int bit)
        {
            return (register & (0x1 << bit)) > 0;
        }
    }
}
