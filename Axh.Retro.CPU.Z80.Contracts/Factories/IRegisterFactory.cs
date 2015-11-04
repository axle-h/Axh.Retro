namespace Axh.Retro.CPU.Z80.Contracts.Factories
{
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public interface IRegisterFactory<out TRegisters> where TRegisters : IRegisters
    {
        TRegisters GetInitialRegisters();
    }
}