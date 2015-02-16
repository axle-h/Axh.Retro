namespace Axh.Emulation.CPU.X80.Factories
{
    using Axh.Emulation.CPU.X80.Contracts;

    public interface IMmuFactory
    {
        IMmu GetMmu();
    }
}