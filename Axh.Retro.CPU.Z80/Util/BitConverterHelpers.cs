using System;

namespace Axh.Retro.CPU.Z80.Util
{
    internal static class BitConverterHelpers
    {
        public static ushort To16Bit(byte rH, byte rL)
        {
            return (ushort) ((rH << 8) | rL);
        }

        public static byte[] To8Bit(ushort r0)
        {
            return BitConverter.GetBytes(r0);
        }

        public static byte GetLowOrderByte(ushort r0)
        {
            return (byte) (r0 & 0xff);
        }

        public static byte GetHighOrderByte(ushort r0)
        {
            return (byte) (r0 >> 8);
        }

        public static ushort SetLowOrderByte(ushort r0, byte b)
        {
            return (ushort) ((r0 & 0xff00) | b);
        }

        public static ushort SetHighOrderByte(ushort r0, byte b)
        {
            return (ushort) ((r0 & 0xff) | (b << 8));
        }
    }
}