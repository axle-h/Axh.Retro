using System;
using System.Collections.Generic;
using Axh.Retro.CPU.Z80.Contracts.Core;

namespace Axh.Retro.CPU.Z80.Wiring
{
    /// <summary>
    /// A context to keep and maintain a collection of <see cref="ICpuCore"/> instances.
    /// </summary>
    public interface ICpuCoreContext : IDisposable
    {
        /// <summary>
        /// Gets the core created with the specified id, or null if no core exists with the id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        ICpuCore GetCore(Guid id);

        /// <summary>
        /// Constructs a new core and returns it.
        /// </summary>
        /// <returns></returns>
        ICpuCore GetNewCore();

        /// <summary>
        /// Stops the core with the specified id and removes it from the context.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        bool StopCore(Guid id);

        /// <summary>
        /// Gets all core ids.
        /// </summary>
        /// <returns></returns>
        ICollection<Guid> GetAllCoreIds();
    }
}