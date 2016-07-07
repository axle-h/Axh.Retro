using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Cache;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Cache
{
    /// <summary>
    /// An instruction block cache.
    /// </summary>
    /// <typeparam name="TRegisters">The type of the registers.</typeparam>
    /// <seealso cref="Axh.Retro.CPU.Z80.Contracts.Cache.IInstructionBlockCache{TRegisters}" />
    public class InstructionBlockCache<TRegisters> : IInstructionBlockCache<TRegisters> where TRegisters : IRegisters
    {
        private readonly ConcurrentDictionary<ushort, ICachedInstructionBlock<TRegisters>> _cache;
        private readonly TimeSpan _garbageCollectionInterval = TimeSpan.FromMinutes(10);
        private readonly Timer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionBlockCache{TRegisters}"/> class.
        /// </summary>
        public InstructionBlockCache()
        {
            _cache = new ConcurrentDictionary<ushort, ICachedInstructionBlock<TRegisters>>();

            // Pseudo garbage collection. Meh... will create a proper implementation another day.
            _timer = new Timer(_garbageCollectionInterval.TotalMilliseconds);
            _timer.Elapsed += (sender, args) => GarbageCollection();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() =>_timer?.Dispose();

        /// <summary>
        /// Get an instruction block from the cache at address. If not present then call getInstanceFunc and add to the cache.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="getInstanceFunc"></param>
        /// <returns></returns>
        public IInstructionBlock<TRegisters> GetOrSet(ushort address, Func<IInstructionBlock<TRegisters>> getInstanceFunc)
        {
            ICachedInstructionBlock<TRegisters> cachedInstructionBlock;
            if (_cache.TryGetValue(address, out cachedInstructionBlock))
            {
                cachedInstructionBlock.AccessedCount++;
            }
            else
            {
                var block = getInstanceFunc();
                var ranges = AddressRange.GetRanges(block.Address, block.Length).ToArray();
                if (ranges.Length == 1)
                {
                    cachedInstructionBlock = new NormalCachedInstructionBlock<TRegisters>(ranges[0], block);
                }
                else
                {
                    cachedInstructionBlock = new CachedInstructionBlock<TRegisters>(ranges, block);
                }

                _cache.TryAdd(block.Address, cachedInstructionBlock);
            }

            return cachedInstructionBlock.InstructionBlock;
        }

        /// <summary>
        /// Invalidates all cache from address for length
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        public void InvalidateCache(ushort address, ushort length)
        {
            var ranges = AddressRange.GetRanges(address, length).ToArray();
            if (ranges.Length == 1)
            {
                var range = ranges[0];
                // TODO: this is a hot path. Need a better way of invalidating the cache.
                foreach (var kvp in _cache.Where(x => x.Value.Intersects(range)).ToArray())
                {
                    ICachedInstructionBlock<TRegisters> dummy;
                    _cache.TryRemove(kvp.Key, out dummy);
                }
            }
            else
            {
                foreach (var key in _cache.Keys)
                {
                    var cacheItem = _cache[key];
                    if (ranges.Any(range => cacheItem.Intersects(range)))
                    {
                        _cache.TryRemove(key, out cacheItem);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes stale cache entries.
        /// TODO: is this necessary?
        /// </summary>
        private void GarbageCollection()
        {
            foreach (var key in _cache.Keys)
            {
                var cacheItem = _cache[key];
                if (cacheItem.AccessedCount == 0)
                {
                    _cache.TryRemove(key, out cacheItem);
                }
            }
        }
    }
}