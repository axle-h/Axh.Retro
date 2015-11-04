namespace Axh.Retro.CPU.Z80.Contracts.Factories
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;

    public interface IMmuFactory
    {
        IMmu GetMmu(IPeripheralManager peripheralManager);

        IMmuCache GetMmuCache(IMmu mmu, ushort address);
    }
}