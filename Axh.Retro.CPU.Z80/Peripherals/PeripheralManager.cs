using System.Collections.Generic;
using System.Linq;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;

namespace Axh.Retro.CPU.Z80.Peripherals
{
    public class PeripheralManager : IPeripheralManager
    {
        private readonly IDictionary<byte, IIOPeripheral> _ioPeripherals;

        private readonly IMemoryMappedPeripheral[] _memoryMappedPeripherals;

        private readonly ICollection<IPeripheral> _peripherals;

        public PeripheralManager(ICollection<IPeripheral> peripherals)
        {
            _peripherals = peripherals;
            _ioPeripherals = peripherals.OfType<IIOPeripheral>().ToDictionary(x => x.Port);
            _memoryMappedPeripherals = peripherals.OfType<IMemoryMappedPeripheral>().ToArray();
        }

        public byte ReadByteFromPort(byte port, byte addressMsb)
        {
            return _ioPeripherals.ContainsKey(port) ? _ioPeripherals[port].ReadByte(addressMsb) : (byte) 0;
        }

        public void WriteByteToPort(byte port, byte addressMsb, byte value)
        {
            if (!_ioPeripherals.ContainsKey(port))
            {
                return;
            }

            _ioPeripherals[port].WriteByte(addressMsb, value);
        }

        public void Signal(ControlSignal signal)
        {
            foreach (var peripheral in _peripherals)
            {
                peripheral.Signal(signal);
            }
        }

        public IEnumerable<IAddressSegment> MemoryMap => _memoryMappedPeripherals.SelectMany(x => x.AddressSegments);

        public TPeripheral PeripheralOfType<TPeripheral>() where TPeripheral : IPeripheral
            => _peripherals.OfType<TPeripheral>().FirstOrDefault();

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var peripheral in _peripherals)
            {
                peripheral.Dispose();
            }
        }
    }
}