namespace Axh.Retro.CPU.X80.Contracts.Registers
{
    using Axh.Retro.CPU.X80.Contracts.State;

    public interface IIntel8080Registers : IRegisters, IStateBackedRegisters<Intel8080RegisterState>
    {
    }
}
