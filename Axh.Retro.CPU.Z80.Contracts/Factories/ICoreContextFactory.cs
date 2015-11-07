namespace Axh.Retro.CPU.Z80.Contracts.Factories
{
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public interface ICoreContextFactory<TRegisters, TRegisterState>
        where TRegisters : IStateBackedRegisters<TRegisterState>
        where TRegisterState : struct
    {
        ICoreContext<TRegisters, TRegisterState> GetContext();
    }
}
