namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    using System.Threading;
    using System.Threading.Tasks;

    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public interface ICpuCore<TRegisters, TRegisterState>
        where TRegisters : IStateBackedRegisters<TRegisterState>
        where TRegisterState : struct
    {
        Task StartCoreProcessAsync(CancellationToken cancellationToken);

        ICoreContext<TRegisters, TRegisterState> Context { get; }
    }
}