namespace Axh.Retro.CPU.Z80.Peripherals
{
    using System;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;

    public class AsciiSystemConsole : IIOPeripheral
    {
        public byte Port => 0;

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
