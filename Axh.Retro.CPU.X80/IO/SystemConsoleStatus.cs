namespace Axh.Retro.CPU.X80.IO
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.IO;

    public class SystemConsoleStatus : IPeripheral
    {
        public SystemConsoleStatus(byte port)
        {
            Port = port;
        }

        public byte Port { get; }

        public byte ReadByte(byte addressMsb)
        {
            return (byte)(Console.KeyAvailable ? 1 : 0);
        }

        public void WriteByte(byte addressMsb, byte value)
        {
            // Nothing to write
        }
        
        public void Halt()
        {
            // Don't listen.
        }

        public void Resume()
        {
            // Don't listen.
        }
    }
}
