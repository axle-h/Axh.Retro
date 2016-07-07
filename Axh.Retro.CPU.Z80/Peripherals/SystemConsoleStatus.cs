using System;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;

namespace Axh.Retro.CPU.Z80.Peripherals
{
    public class SystemConsoleStatus : IIOPeripheral
    {
        public byte Port => 1;

        public byte ReadByte(byte addressMsb)
        {
            return (byte) (Console.KeyAvailable ? 1 : 0);
        }

        public void WriteByte(byte addressMsb, byte value)
        {
            // Nothing to write
        }

        public void Signal(ControlSignal signal)
        {
            // Don't listen.
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose.
        }
    }
}