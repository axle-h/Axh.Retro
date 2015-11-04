namespace Axh.Retro.Z80Console.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Factories;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Peripherals;

    public class PeripheralFactory : IPeripheralFactory
    {
        public IEnumerable<IIOPeripheral> GetIOMappedPeripherals(IInterruptManager interruptManager)
        {
            return new IIOPeripheral[] { new AsciiSystemConsole(0), new SystemConsoleStatus(1) };
        }

        public IEnumerable<IMemoryMappedPeripheral> GetMemoryMappedPeripherals(IInterruptManager interruptManager)
        {
            return Enumerable.Empty<IMemoryMappedPeripheral>();
        }
    }
}
