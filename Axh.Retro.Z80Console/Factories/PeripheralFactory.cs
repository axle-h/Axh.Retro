namespace Axh.Retro.Z80Console.Factories
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;
    using Axh.Retro.CPU.X80.Peripherals;

    public class PeripheralFactory : IPeripheralFactory
    {
        public IIOPeripheral GetIoMappedPeripheral(byte port)
        {
            switch (port)
            {
                case 0:
                    return new AsciiSystemConsole(port);
                case 1:
                    return new SystemConsoleStatus(port);
                default:
                    throw new ArgumentOutOfRangeException(port.ToString());
            }
        }

        public IMemoryMappedPeripheral GetMemoryMappedPeripheral(Guid peripheralId)
        {
            throw new NotImplementedException();
        }
    }
}
