namespace Axh.Retro.CPU.Z80.Contracts.Registers
{
    using Axh.Retro.CPU.Z80.Contracts.State;

    public interface IIntel8080Registers : IRegisters, IStateBackedRegisters<Intel8080RegisterState>
    {
    }
}
