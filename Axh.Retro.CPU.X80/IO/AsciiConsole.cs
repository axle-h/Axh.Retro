namespace Axh.Retro.CPU.X80.IO
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.IO;

    public class AsciiConsole : IPeripheral
    {
        public AsciiConsole(byte port)
        {
            this.Port = port;
        }

        public byte Port { get; }

        public byte ReadByte(byte addressMsb)
        {
            var c = Console.Read();
            return unchecked((byte)c);
        }

        public void WriteByte(byte addressMsb, byte value)
        {
            var c = (char)value;
            Console.Write(c);
        }
    }
}
