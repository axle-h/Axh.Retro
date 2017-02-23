using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Axh.Retro.CPU.Z80.Contracts.Core;

namespace Axh.Retro.CPU.Z80.Wiring
{
    /// <summary>
    /// A context to keep and maintain a collection of <see cref="ICpuCore"/> instances.
    /// </summary>
    /// <seealso cref="ICpuCoreContext" />
    public class CpuCoreContext : ICpuCoreContext
    {
        private readonly ConcurrentDictionary<Guid, ICpuCore> _cores;
        private readonly IWiredCpuCoreFactory _coreFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CpuCoreContext"/> class.
        /// </summary>
        /// <param name="coreFactory">The core factory.</param>
        public CpuCoreContext(IWiredCpuCoreFactory coreFactory)
        {
            _coreFactory = coreFactory;
            _cores = new ConcurrentDictionary<Guid, ICpuCore>();
        }

        /// <summary>
        /// Gets the core created with the specified id, or null if no core exists with the id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ICpuCore GetCore(Guid id)
        {
            ICpuCore core;
            return _cores.TryGetValue(id, out core) ? core : null;
        }

        /// <summary>
        /// Constructs a new core and returns it.
        /// </summary>
        /// <returns></returns>
        public ICpuCore GetNewCore()
        {
            var core = _coreFactory.GetNewCore();
            _cores.TryAdd(core.CoreId, core);
            return core;
        }

        /// <summary>
        /// Stops the core with the specified id and removes it from the context.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool StopCore(Guid id)
        {
            ICpuCore core;
            if (!_cores.TryRemove(id, out core))
            {
                return false;
            }
            
            core.Dispose();
            return true;
        }

        /// <summary>
        /// Gets all core ids.
        /// </summary>
        /// <returns></returns>
        public ICollection<Guid> GetAllCoreIds() => _cores.Keys;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var kvp in _cores)
            {
                kvp.Value.Dispose();
            }

            _coreFactory.Dispose();
        }
    }
}
