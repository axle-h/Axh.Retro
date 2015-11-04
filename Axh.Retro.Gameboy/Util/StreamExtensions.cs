namespace Axh.Retro.GameBoy.Util
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    internal static class StreamExtensions
    {
        public static string ReadAscii(this Stream stream, int len)
        {
            return 0 != len ? Encoding.ASCII.GetString(stream.ReadBuffer(len).TakeWhile(b => 0 != b).ToArray()) : string.Empty;
        }

        public static byte[] ReadBuffer(this Stream stream, int len)
        {
            var read = 0;
            var buffer = new byte[len];
            while (read != len)
            {
                read += stream.Read(buffer, read, len - read);
            }
            return buffer;
        }

        public static ushort ReadBigEndianUInt16(this Stream stream)
        {
            var msb = stream.ReadByte();
            var lsb = stream.ReadByte();
            return (ushort)((msb << 8) | lsb);
        }

        public static TEnum ReadEnum<TEnum>(this Stream stream)
        {
            var value = (byte)stream.ReadByte();
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                throw new Exception($"Bad {typeof(TEnum)}: 0x{value:x2}");
            }

            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }
    }
}
