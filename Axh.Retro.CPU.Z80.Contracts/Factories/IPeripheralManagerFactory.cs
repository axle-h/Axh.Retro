namespace Axh.Retro.CPU.Z80.Contracts.Factories
{
    using System;

    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;

    public interface IPeripheralManagerFactory
    {
        IPeripheralManager GetPeripheralsManager(IInterruptManager interruptManager);
    }
}
