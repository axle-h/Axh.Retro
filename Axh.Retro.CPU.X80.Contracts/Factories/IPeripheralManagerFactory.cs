namespace Axh.Retro.CPU.X80.Contracts.Factories
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;

    public interface IPeripheralManagerFactory
    {
        IPeripheralManager GetPeripheralsManager(IInterruptManager interruptManager);
    }
}
