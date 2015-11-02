namespace Axh.Retro.CPU.X80.Peripherals
{
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;

    public class PeripheralManager : IPeripheralManager
    {
        private readonly IDictionary<byte, IIOPeripheral> ioPeripherals;

        private readonly IList<IPeripheral> peripherals;

        public PeripheralManager(IEnumerable<IIOPeripheral> ioPeripherals)
        {
            this.ioPeripherals = ioPeripherals.ToDictionary(x => x.Port);
            this.peripherals = new List<IPeripheral>();
        }

        public byte ReadByteFromPort(byte port, byte addressMsb)
        {
            return ioPeripherals.ContainsKey(port) ? ioPeripherals[port].ReadByte(addressMsb) : (byte)0;
        }

        public void WriteByteToPort(byte port, byte addressMsb, byte value)
        {
            if (!ioPeripherals.ContainsKey(port))
            {
                return;
            }

            ioPeripherals[port].WriteByte(addressMsb, value);
        }

        public void Halt()
        {
            foreach (var peripheral in this.ioPeripherals.Values.Concat(peripherals))
            {
                peripheral.Halt();
            }
        }

        public void Resume()
        {
            foreach (var peripheral in this.ioPeripherals.Values.Concat(peripherals))
            {
                peripheral.Resume();
            }
        }

        public void RegisterPeripheral(IPeripheral peripheral)
        {
            this.peripherals.Add(peripheral);
        }

        public void RegisterMmuForDma(IMmu mmu)
        {
            foreach (var peripheral in this.ioPeripherals.Values.Concat(peripherals))
            {
                peripheral.RegisterMmuForDma(mmu);
            }
        }
    }
}
