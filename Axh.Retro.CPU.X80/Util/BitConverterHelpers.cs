namespace Axh.Retro.CPU.X80.Util
{
    using System;

    internal static class BitConverterHelpers
    {
        public static ushort To16Bit(byte rH, byte rL)
        {
            return (ushort)((rH << 8) | rL);
        }

        public static byte[] To8Bit(ushort r0)
        {
            return BitConverter.GetBytes(r0);
        }
    }
}
