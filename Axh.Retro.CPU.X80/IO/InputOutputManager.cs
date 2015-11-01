namespace Axh.Retro.CPU.X80.IO
{
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.X80.Contracts.IO;

    public class InputOutputManager : IInputOutputManager
    {
        private readonly IDictionary<byte, IPeripheral> peripherals;

        public InputOutputManager(IEnumerable<IPeripheral> peripherals)
        {
            this.peripherals = peripherals.ToDictionary(x => x.Port);
        }

        public byte ReadByte(byte port, byte addressMsb)
        {
            return peripherals.ContainsKey(port) ? peripherals[port].ReadByte(addressMsb) : (byte)0;
        }

        public void WriteByte(byte port, byte addressMsb, byte value)
        {
            if (!peripherals.ContainsKey(port))
            {
                return;
            }

            peripherals[port].WriteByte(addressMsb, value);
        }

        public void Halt()
        {
            foreach (var peripheral in this.peripherals.Values)
            {
                peripheral.Halt();
            }
        }

        public void Resume()
        {
            foreach (var peripheral in this.peripherals.Values)
            {
                peripheral.Resume();
            }
        }
    }
}
