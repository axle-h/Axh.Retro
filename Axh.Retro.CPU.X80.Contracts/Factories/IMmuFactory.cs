namespace Axh.Retro.CPU.X80.Contracts.Factories
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;

    public interface IMmuFactory
    {
        IMmu GetMmu(IPeripheralManager peripheralManager);

        IMmuCache GetMmuCache(IMmu mmu, ushort address);
    }
}