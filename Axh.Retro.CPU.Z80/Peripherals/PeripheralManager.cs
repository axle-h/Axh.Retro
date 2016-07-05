using System.Collections.Generic;
using System.Linq;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;

namespace Axh.Retro.CPU.Z80.Peripherals
{
    public class PeripheralManager : IPeripheralManager
    {
        private readonly IDictionary<byte, IIOPeripheral> ioPeripherals;

        private readonly IMemoryMappedPeripheral[] memoryMappedPeripherals;

        private readonly ICollection<IPeripheral> peripherals;

        public PeripheralManager(ICollection<IPeripheral> peripherals)
        {
            this.peripherals = peripherals;
            this.ioPeripherals = peripherals.OfType<IIOPeripheral>().ToDictionary(x => x.Port);
            this.memoryMappedPeripherals = peripherals.OfType<IMemoryMappedPeripheral>().ToArray();
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
            foreach (var peripheral in peripherals)
            {
                peripheral.Signal(signal);
            }
        }

        public IEnumerable<IAddressSegment> MemoryMap => memoryMappedPeripherals.SelectMany(x => x.AddressSegments);

        public TPeripheral PeripheralOfType<TPeripheral>() where TPeripheral : IPeripheral =>  peripherals.OfType<TPeripheral>().FirstOrDefault();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var peripheral in peripherals)
            {
                peripheral.Dispose();
            }
        }
    }
}