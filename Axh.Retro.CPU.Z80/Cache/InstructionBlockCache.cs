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
        private readonly ConcurrentDictionary<ushort, ICacheItem> _cache;
        private readonly TimeSpan _garbageCollectionInterval = TimeSpan.FromMinutes(10);

        private readonly Timer _timer;

        public InstructionBlockCache()
        {
            _cache = new ConcurrentDictionary<ushort, ICacheItem>();

            // Psuedo garbage collection. Meh... will create a proper implementation another day.
            _timer = new Timer(_garbageCollectionInterval.TotalMilliseconds);
            _timer.Elapsed += (sender, args) => GarbageCollection();
        }

        public void Dispose()
        {
            _timer?.Dispose();
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
            if (_cache.TryGetValue(address, out cacheItem))
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

                _cache.TryAdd(block.Address, cacheItem);
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
                foreach (var kvp in _cache.Where(x => x.Value.Intersects(range)).ToArray())
                {
                    ICacheItem dummy;
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

        private void GarbageCollection()
        {
            foreach (var key in _cache.Keys)
            {
                var cacheItem = _cache[key];
                if (cacheItem.Accessed == 0)
                {
                    _cache.TryRemove(key, out cacheItem);
                }
            }
        }

        /// <summary>
        ///     Instruction block cache wrapper with a single normal range.
        /// </summary>
        private class NormalInstructionBlockCacheItem : ICacheItem
        {
            private readonly AddressRange _addressRange;

            public NormalInstructionBlockCacheItem(AddressRange range, IInstructionBlock<TRegisters> instructionBlock)
            {
                InstructionBlock = instructionBlock;
                _addressRange = range;
            }

            public uint Accessed { get; set; }

            public IInstructionBlock<TRegisters> InstructionBlock { get; }

            public bool Intersects(AddressRange range)
            {
                return range.Intersects(_addressRange);
            }
        }

        /// <summary>
        ///     Instruction block cache wrapper with two ranges.
        /// </summary>
        private class InstructionBlockCacheItem : ICacheItem
        {
            private readonly AddressRange _addressRange0;
            private readonly AddressRange _addressRange1;

            public InstructionBlockCacheItem(IReadOnlyList<AddressRange> addressRanges,
                IInstructionBlock<TRegisters> instructionBlock)
            {
                InstructionBlock = instructionBlock;
                _addressRange0 = addressRanges[0];
                _addressRange1 = addressRanges[1];
            }

            public IInstructionBlock<TRegisters> InstructionBlock { get; }

            public uint Accessed { get; set; }

            public bool Intersects(AddressRange range)
            {
                return range.Intersects(_addressRange0) || range.Intersects(_addressRange1);
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