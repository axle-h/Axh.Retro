namespace Axh.Retro.CPU.X80.Peripherals
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;

    public class AsciiSystemConsole : IIOPeripheral
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

        public void Halt()
        {
            // Don't listen.
        }

        public void Resume()
        {
            // Don't listen.
        }

        public void RegisterMmuForDma(IMmu mmu)
        {
            // No DMA.
        }
    }
}
