namespace Axh.Retro.CPU.X80.IO
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.IO;

    public class AsciiSystemConsole : IPeripheral
    {
        public AsciiSystemConsole(byte port)
        {
            this.Port = port;
        }

        public byte Port { get; }

        public byte ReadByte(byte addressMsb)
        {
            var key = Console.ReadKey();
            return unchecked((byte)key.KeyChar);
        }

        public void WriteByte(byte addressMsb, byte value)
        {
            var c = (char)value;
            Console.Write(c);
        }
    }
}
