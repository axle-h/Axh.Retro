namespace Axh.Retro.CPU.Z80.Cache
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Axh.Retro.CPU.Z80.Contracts.Cache;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    using Timer = System.Timers.Timer;

    public class InstructionBlockCache<TRegisters> : IInstructionBlockCache<TRegisters>, IDisposable where TRegisters : IRegisters
    {
        private readonly TimeSpan garbageCollectionInterval = TimeSpan.FromMinutes(10);

        private readonly ConcurrentDictionary<ushort, ICacheItem> cache;
        
        private readonly Timer timer;

        public InstructionBlockCache()
        {
            this.CacheId = Guid.NewGuid();
            this.cache = new ConcurrentDictionary<ushort, ICacheItem>();

            // Psuedo garbage collection. Meh... will create a proper implementation another day.
            timer = new Timer(garbageCollectionInterval.TotalMilliseconds);
            timer.Elapsed += (sender, args) => GarbageCollection();
        }

        public Guid CacheId { get; }

        public IInstructionBlock<TRegisters> GetOrSet(ushort address, Func<IInstructionBlock<TRegisters>> getInstanceFunc)
        {
            ICacheItem cacheItem;
            if (this.cache.TryGetValue(address, out cacheItem))
            {
                cacheItem.Accessed++;
            }
            else
            {
                var block = getInstanceFunc();
                var ranges = NormalRange.GetRanges(block.Address, block.Length).ToArray();
                if (ranges.Length == 1)
                {
                    cacheItem = new NormalInstructionBlockCacheItem(ranges[0], block);
                }
                else
                {
                    cacheItem = new InstructionBlockCacheItem(ranges, block);
                }
                
                this.cache.TryAdd(block.Address, cacheItem);
            }
            
            return cacheItem.InstructionBlock;
        }

        public void InvalidateCache(ushort address, ushort length)
        {
            var ranges = NormalRange.GetRanges(address, length).ToArray();
            if (ranges.Length == 1)
            {
                var range = ranges[0];
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
        /// Normal: min > max due to mod ushort
        /// </summary>
        private struct NormalRange
        {
            private readonly ushort address;
            private readonly ushort maxAddress;

            private NormalRange(ushort address, ushort maxAddress) : this()
            {
                if (address > maxAddress)
                {
                    throw new ArgumentException($"Cannot create normal range: {maxAddress} > {address}");
                }
                this.address = address;
                this.maxAddress = maxAddress;
            }
            
            public bool Intersects(NormalRange range)
            {
                return Math.Max(range.address, this.address) <= Math.Min(range.maxAddress, this.maxAddress);
            }

            public static IEnumerable<NormalRange> GetRanges(ushort address, ushort length)
            {
                var maxAddress = unchecked((ushort)(address + length - 1));
                if (maxAddress >= address)
                {
                    yield return new NormalRange(address, maxAddress);
                }
                else
                {
                    yield return new NormalRange(ushort.MinValue, maxAddress);
                    yield return new NormalRange(address, ushort.MaxValue);
                }
            }

            public override string ToString()
            {
                return $"({address}, {maxAddress})";
            }
        }

        private class NormalInstructionBlockCacheItem : ICacheItem
        {
            private readonly NormalRange addressRange;

            public uint Accessed { get; set; }
            
            public NormalInstructionBlockCacheItem(NormalRange range, IInstructionBlock<TRegisters> instructionBlock)
            {
                this.InstructionBlock = instructionBlock;
                this.addressRange = range;
            }

            public IInstructionBlock<TRegisters> InstructionBlock { get; }

            public bool Intersects(NormalRange range)
            {
                return range.Intersects(addressRange);
            }
        }

        private class InstructionBlockCacheItem : ICacheItem
        {
            private readonly NormalRange addressRange0;
            private readonly NormalRange addressRange1;

            public InstructionBlockCacheItem(IReadOnlyList<NormalRange> addressRanges, IInstructionBlock<TRegisters> instructionBlock)
            {
                InstructionBlock = instructionBlock;
                this.addressRange0 = addressRanges[0];
                this.addressRange1 = addressRanges[1];
            }

            public IInstructionBlock<TRegisters> InstructionBlock { get; }

            public uint Accessed { get; set; }

            public bool Intersects(NormalRange range)
            {
                return range.Intersects(addressRange0) || range.Intersects(addressRange1);
            }
        }

        private interface ICacheItem
        {
            IInstructionBlock<TRegisters> InstructionBlock { get; }

            uint Accessed { get; set; }

            bool Intersects(NormalRange range);
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
