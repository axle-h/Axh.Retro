using System;

namespace Axh.Retro.CPU.Z80.Util
{
    internal static class ByteExtensions
    {
        private static readonly bool[] EvenParityTable = P8(true);

        public static string ToHex(this byte b)
        {
            return "0x" + b.ToString("X2");
        }

        public static bool IsEvenParity(this byte b)
        {
            return EvenParityTable[b];
        }

        private static bool[] P2(bool n)
        {
            return new[] {n, !n, !n, n};
        }

        private static bool[] P4(bool n)
        {
            return P0110(P2(n), P2(!n));
        }

        private static bool[] P6(bool n)
        {
            return P0110(P4(n), P4(!n));
        }

        private static bool[] P8(bool n)
        {
            return P0110(P6(n), P6(!n));
        }

        private static bool[] P0110(bool[] p0, bool[] p1)
        {
            var ans = new bool[p0.Length * 4];

            Array.Copy(p0, 0, ans, 0, p0.Length);
            Array.Copy(p1, 0, ans, p0.Length, p0.Length);
            Array.Copy(p1, 0, ans, p0.Length * 2, p0.Length);
            Array.Copy(p0, 0, ans, p0.Length * 3, p0.Length);

            return ans;
        }
    }
}