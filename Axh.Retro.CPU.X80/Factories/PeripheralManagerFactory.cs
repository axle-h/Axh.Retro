namespace Axh.Retro.CPU.X80.Factories
{
    using System;
    using System.Linq;

    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;
    using Axh.Retro.CPU.X80.Peripherals;

    public class PeripheralManagerFactory : IPeripheralManagerFactory
    {
        private readonly IPeripheralFactory peripheralFactory;

        private readonly IPlatformConfig platformConfig;

        public PeripheralManagerFactory(IPeripheralFactory peripheralFactory, IPlatformConfig platformConfig)
        {
            this.peripheralFactory = peripheralFactory;
            this.platformConfig = platformConfig;
        }

        public IPeripheralManager GetPeripheralsManager()
        {
            var peripherals = this.platformConfig.IOPorts.Select(this.peripheralFactory.GetIoMappedPeripheral).ToArray();
            return new PeripheralManager(peripherals);
        }
    }
}
