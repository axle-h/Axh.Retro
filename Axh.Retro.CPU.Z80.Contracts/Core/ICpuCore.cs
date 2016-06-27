using System.Threading;
using System.Threading.Tasks;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    public interface ICpuCore<TRegisters, TRegisterState> where TRegisters : IStateBackedRegisters<TRegisterState>
                                                          where TRegisterState : struct
    {
        ICoreContext<TRegisters, TRegisterState> Context { get; }
        Task StartCoreProcessAsync(CancellationToken cancellationToken);
    }
}