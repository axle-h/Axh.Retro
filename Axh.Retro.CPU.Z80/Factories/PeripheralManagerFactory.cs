namespace Axh.Retro.CPU.Z80.Factories
{
    using System;
    using System.Linq;

    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Peripherals;

    public class PeripheralManagerFactory : IPeripheralManagerFactory
    {
        private readonly IPeripheralFactory peripheralFactory;

        public PeripheralManagerFactory(IPeripheralFactory peripheralFactory)
        {
            this.peripheralFactory = peripheralFactory;
        }

        public IPeripheralManager GetPeripheralsManager(IInterruptManager interruptManager)
        {
            var ioPeripherals = this.peripheralFactory.GetIOMappedPeripherals(interruptManager).ToArray();
            var memorymappedPeripherals = this.peripheralFactory.GetMemoryMappedPeripherals(interruptManager);

            return new PeripheralManager(ioPeripherals.Cast<IPeripheral>().Concat(memorymappedPeripherals));
        }
    }
}
