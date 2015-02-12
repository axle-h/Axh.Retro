namespace Axh.Emulation.CPU.Z80.Contracts.Factories
{
    using Axh.Emulation.CPU.Z80.Contracts.Registers;

    public interface IRegisterSetFactory
    {
        IRegisterSet GetRegisterSet();
    }
}