namespace Axh.Retro.CPU.X80.Peripherals
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;

    public class SystemConsoleStatus : IIOPeripheral
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

        public void RegisterDma(IMmu mmu)
        {
            // No DMA.
        }
    }
}
