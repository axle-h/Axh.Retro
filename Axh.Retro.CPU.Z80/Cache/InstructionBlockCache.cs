using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Cache;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Cache
{
    public class InstructionBlockCache<TRegisters> : IInstructionBlockCache<TRegisters>
        where TRegisters : IRegisters
    {
        private readonly ConcurrentDictionary<ushort, ICacheItem> cache;
        private readonly TimeSpan garbageCollectionInterval = TimeSpan.FromMinutes(10);

        private readonly Timer timer;

        public InstructionBlockCache()
        {
            cache = new ConcurrentDictionary<ushort, ICacheItem>();
            
            // Psuedo garbage collection. Meh... will create a proper implementation another day.
            timer = new Timer(garbageCollectionInterval.TotalMilliseconds);
            timer.Elapsed += (sender, args) => GarbageCollection();
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        /// <summary>
        ///     Get an instruction block from the cache at address. If not present then call getInstanceFunc and add to the cache.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="getInstanceFunc"></param>
        /// <returns></returns>
        public IInstructionBlock<TRegisters> GetOrSet(ushort address,
                                                      Func<IInstructionBlock<TRegisters>> getInstanceFunc)
        {
            ICacheItem cacheItem;
            if (cache.TryGetValue(address, out cacheItem))
            {
                cacheItem.Accessed++;
            }
            else
            {
                var block = getInstanceFunc();
                var ranges = AddressRange.GetRanges(block.Address, block.Length).ToArray();
                if (ranges.Length == 1)
                {
                    cacheItem = new NormalInstructionBlockCacheItem(ranges[0], block);
                }
                else
                {
                    cacheItem = new InstructionBlockCacheItem(ranges, block);
                }

                cache.TryAdd(block.Address, cacheItem);
            }

            return cacheItem.InstructionBlock;
        }

        /// <summary>
        ///     Invalidates all cache from address for length
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
                foreach (var kvp in cache.Where(x => x.Value.Intersects(range)).ToArray())
                {
                    ICacheItem dummy;
                    cache.TryRemove(kvp.Key, out dummy);
                }
            }
            else
            {
                foreach (var key in cache.Keys)
                {
                    var cacheItem = cache[key];
                    if (ranges.Any(range => cacheItem.Intersects(range)))
                    {
                        cache.TryRemove(key, out cacheItem);
                    }
                }
            }
        }

        private void GarbageCollection()
        {
            foreach (var key in cache.Keys)
            {
                var cacheItem = cache[key];
                if (cacheItem.Accessed == 0)
                {
                    cache.TryRemove(key, out cacheItem);
                }
            }
        }

        /// <summary>
        ///     Instruction block cache wrapper with a single normal range.
        /// </summary>
        private class NormalInstructionBlockCacheItem : ICacheItem
        {
            private readonly AddressRange addressRange;

            public NormalInstructionBlockCacheItem(AddressRange range, IInstructionBlock<TRegisters> instructionBlock)
            {
                InstructionBlock = instructionBlock;
                addressRange = range;
            }

            public uint Accessed { get; set; }

            public IInstructionBlock<TRegisters> InstructionBlock { get; }

            public bool Intersects(AddressRange range)
            {
                return range.Intersects(addressRange);
            }
        }

        /// <summary>
        ///     Instruction block cache wrapper with two ranges.
        /// </summary>
        private class InstructionBlockCacheItem : ICacheItem
        {
            private readonly AddressRange addressRange0;
            private readonly AddressRange addressRange1;

            public InstructionBlockCacheItem(IReadOnlyList<AddressRange> addressRanges,
                                             IInstructionBlock<TRegisters> instructionBlock)
            {
                InstructionBlock = instructionBlock;
                addressRange0 = addressRanges[0];
                addressRange1 = addressRanges[1];
            }

            public IInstructionBlock<TRegisters> InstructionBlock { get; }

            public uint Accessed { get; set; }

            public bool Intersects(AddressRange range)
            {
                return range.Intersects(addressRange0) || range.Intersects(addressRange1);
            }
        }

        private interface ICacheItem
        {
            IInstructionBlock<TRegisters> InstructionBlock { get; }

            uint Accessed { get; set; }

            bool Intersects(AddressRange range);
        }
    }
}