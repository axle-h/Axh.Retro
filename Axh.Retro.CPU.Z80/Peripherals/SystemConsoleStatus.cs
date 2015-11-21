namespace Axh.Retro.CPU.Z80.Peripherals
{
    using System;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;

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

        public void Signal(ControlSignal signal)
        {
            // Don't listen.
        }

        public void RegisterDma(IMmu mmu)
        {
            // No DMA.
        }
    }
}
