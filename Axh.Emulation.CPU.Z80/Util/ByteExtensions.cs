namespace Axh.Emulation.CPU.Z80.Util
{
    internal static class ByteExtensions
    {
        public static string ToHex(this byte b)
        {
            return "0x" + b.ToString("X2");
        }

        public static bool IsEvenParity(this byte b)
        {
            int accum = b;
            byte count = 0;
            while (accum > 0)
            {
                accum &= (accum - 1);
                count++;
            }
            return count % 2 == 0;
        }
    }
}
