namespace Axh.Retro.CPU.Z80.Peripherals
{
    using System;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;

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
        
        public void RegisterDma(IMmu mmu)
        {
            // No DMA.
        }

        public void Signal(ControlSignal signal)
        {
            // Don't listen.
        }
    }
}
