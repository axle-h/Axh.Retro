namespace Axh.Emulation.CPU.Z80.Factories
{
    using Axh.Emulation.CPU.Z80.Contracts;

    public interface IMmuFactory
    {
        IMmu GetMmu();
    }
}