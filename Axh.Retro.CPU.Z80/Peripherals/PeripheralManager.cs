using System.Collections.Generic;
using System.Linq;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;

namespace Axh.Retro.CPU.Z80.Peripherals
{
    public class PeripheralManager : IPeripheralManager
    {
        private readonly IDictionary<byte, IIOPeripheral> ioPeripherals;

        private readonly IMemoryMappedPeripheral[] memoryMappedPeripherals;

        public PeripheralManager(IEnumerable<IIOPeripheral> ioPeripherals,
                                 IEnumerable<IMemoryMappedPeripheral> memoryMappedPeripherals)
        {
            this.ioPeripherals = ioPeripherals.ToDictionary(x => x.Port);
            this.memoryMappedPeripherals = memoryMappedPeripherals.ToArray();
        }

        public byte ReadByteFromPort(byte port, byte addressMsb)
        {
            return ioPeripherals.ContainsKey(port) ? ioPeripherals[port].ReadByte(addressMsb) : (byte) 0;
        }

        public void WriteByteToPort(byte port, byte addressMsb, byte value)
        {
            if (!ioPeripherals.ContainsKey(port))
            {
                return;
            }

            ioPeripherals[port].WriteByte(addressMsb, value);
        }

        public void Signal(ControlSignal signal)
        {
            foreach (var peripheral in ioPeripherals.Values.Cast<IPeripheral>().Concat(memoryMappedPeripherals))
            {
                peripheral.Signal(signal);
            }
        }

        public IEnumerable<IMemoryMappedPeripheral> GetAllMemoryMappedPeripherals()
        {
            return memoryMappedPeripherals;
        }

        public TPeripheral GetMemoryMappedPeripherals<TPeripheral>() where TPeripheral : IMemoryMappedPeripheral
        {
            return memoryMappedPeripherals.OfType<TPeripheral>().FirstOrDefault();
        }
    }
}