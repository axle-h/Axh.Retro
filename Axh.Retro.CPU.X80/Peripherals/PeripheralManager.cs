namespace Axh.Retro.CPU.X80.Peripherals
{
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;

    public class PeripheralManager : IPeripheralManager
    {
        private readonly IDictionary<byte, IIOPeripheral> ioPeripherals;

        private readonly IMemoryMappedPeripheral[] memoryMappedPeripherals;

        public PeripheralManager(IEnumerable<IPeripheral> peripherals)
        {
            var peripheralArray = peripherals as IPeripheral[] ?? peripherals.ToArray();

            this.ioPeripherals = peripheralArray.OfType<IIOPeripheral>().ToDictionary(x => x.Port);
            this.memoryMappedPeripherals = peripheralArray.OfType<IMemoryMappedPeripheral>().ToArray();
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
            foreach (var peripheral in this.ioPeripherals.Values.Cast<IPeripheral>().Concat(memoryMappedPeripherals))
            {
                peripheral.Halt();
            }
        }

        public void Resume()
        {
            foreach (var peripheral in this.ioPeripherals.Values.Cast<IPeripheral>().Concat(memoryMappedPeripherals))
            {
                peripheral.Resume();
            }
        }

        public void RegisterDmaForIOPeripherals(IMmu mmu)
        {
            foreach (var peripheral in this.ioPeripherals.Values)
            {
                peripheral.RegisterDma(mmu);
            }
        }

        public IEnumerable<IMemoryMappedPeripheral> GetAllMemoryMappedPeripherals()
        {
            return memoryMappedPeripherals;
        }

        public IEnumerable<TPeripheral> GetMemoryMappedPeripherals<TPeripheral>() where TPeripheral : IMemoryMappedPeripheral
        {
            return memoryMappedPeripherals.OfType<TPeripheral>();
        }
    }
}
