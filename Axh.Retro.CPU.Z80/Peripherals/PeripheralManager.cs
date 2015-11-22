namespace Axh.Retro.CPU.Z80.Peripherals
{
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;

    public class PeripheralManager : IPeripheralManager
    {
        private readonly IDictionary<byte, IIOPeripheral> ioPeripherals;

        private readonly IMemoryMappedPeripheral[] memoryMappedPeripherals;

        public PeripheralManager(IPeripheralFactory peripheralFactory, IInterruptManager interruptManager)
        {
            this.ioPeripherals = peripheralFactory.GetIOMappedPeripherals(interruptManager).ToDictionary(x => x.Port);
            this.memoryMappedPeripherals = peripheralFactory.GetMemoryMappedPeripherals(interruptManager).ToArray();
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

        public void Signal(ControlSignal signal)
        {
            foreach (var peripheral in this.ioPeripherals.Values.Cast<IPeripheral>().Concat(memoryMappedPeripherals))
            {
                peripheral.Signal(signal);
            }
        }
        
        public void RegisterDma(IMmu mmu)
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

        public TPeripheral GetMemoryMappedPeripherals<TPeripheral>() where TPeripheral : IMemoryMappedPeripheral
        {
            return memoryMappedPeripherals.OfType<TPeripheral>().FirstOrDefault();
        }
    }
}
