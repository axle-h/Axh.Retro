namespace Axh.Emulation.CPU.X80.Contracts.Factories
{
    using Axh.Emulation.CPU.X80.Contracts.Memory;

    public interface IMmuFactory
    {
        IMmu GetMmu();

        IMmuCache GetMmuCache(IMmu mmu, ushort address);
    }
}