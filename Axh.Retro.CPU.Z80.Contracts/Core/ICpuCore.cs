using System;
using System.Threading;
using System.Threading.Tasks;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    /// <summary>
    /// A Z80 CPU core.
    /// </summary>
    /// <typeparam name="TRegisters">The type of the registers.</typeparam>
    /// <typeparam name="TRegisterState">The type of the register state.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface ICpuCore<TRegisters, TRegisterState> : IDisposable
        where TRegisters : IStateBackedRegisters<TRegisterState>
        where TRegisterState : struct
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        ICoreContext<TRegisters, TRegisterState> Context { get; }

        /// <summary>
        /// Starts the core process.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task StartCoreProcessAsync(CancellationToken cancellationToken);
    }
}