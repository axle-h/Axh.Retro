using Axh.Retro.CPU.X80.Contracts.Memory;

namespace Axh.Retro.CPU.X80.Contracts.Factories
{
    using Retro.CPU.X80.Contracts.Memory;

    public interface IMmuFactory
    {
        IMmu GetMmu();

        IMmuCache GetMmuCache(IMmu mmu, ushort address);
    }
}