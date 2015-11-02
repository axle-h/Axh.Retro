﻿namespace Axh.Retro.CPU.X80.Contracts.Factories
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Peripherals;

    public interface IPeripheralFactory
    {
        IIOPeripheral GetIoMappedPeripheral(byte port);

        IMemoryMappedPeripheral GetMemoryMappedPeripheral(Guid peripheralId);
    }
}
