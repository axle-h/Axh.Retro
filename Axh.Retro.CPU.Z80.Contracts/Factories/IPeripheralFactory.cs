namespace Axh.Retro.CPU.Z80.Contracts.Factories
{
    using System;
    using System.Collections.Generic;

    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;

    public interface IPeripheralFactory
    {
        IEnumerable<IIOPeripheral> GetIOMappedPeripherals(IInterruptManager interruptManager);
        
        IEnumerable<IMemoryMappedPeripheral> GetMemoryMappedPeripherals(IInterruptManager interruptManager);
    }
}
