namespace Axh.Retro.CPU.X80.Cache
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Axh.Retro.CPU.X80.Contracts.Cache;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public class InstructionBlockCache<TRegisters> : IInstructionBlockCache<TRegisters> where TRegisters : IRegisters
    {
        private readonly bool slidingExpirationEnabled;
        private readonly TimeSpan slidingExpiration;

        private readonly ConcurrentDictionary<ushort, InstructionBlockCacheItem> cache;

        public InstructionBlockCache(IRuntimeConfig runtimeConfig)
        {
            this.CacheId = Guid.NewGuid();
            this.slidingExpirationEnabled = runtimeConfig.InstructionCacheSlidingExpiration.HasValue;
            this.slidingExpiration = runtimeConfig.InstructionCacheSlidingExpiration.GetValueOrDefault();
            this.cache = new ConcurrentDictionary<ushort, InstructionBlockCacheItem>();
        }

        public Guid CacheId { get; }

        public IInstructionBlock<TRegisters> GetOrSet(ushort address, Func<IInstructionBlock<TRegisters>> getInstanceFunc)
        {
            InstructionBlockCacheItem cacheItem;
            if (this.cache.TryGetValue(address, out cacheItem))
            {
                if (slidingExpirationEnabled)
                {
                    cacheItem.RegisterAccess();
                }
            }
            else
            {
                var block = getInstanceFunc();
                cacheItem = this.slidingExpirationEnabled ? new InstructionBlockCacheItem(block, slidingExpiration, () => Remove(address)) : new InstructionBlockCacheItem(block);
                this.cache.TryAdd(address, cacheItem);
            }
            
            
            return cacheItem.InstructionBlock;
        }

        public void InvalidateCache(ushort address, ushort length)
        {
            var ranges = Range.GetRanges(address, length).ToArray();

            foreach (var cacheItem in cache.Where(cacheItem => ranges.Any(range => cacheItem.Value.Intersects(range))))
            {
                Remove(cacheItem.Key);
            }
        }

        private void Remove(ushort address)
        {
            InstructionBlockCacheItem cacheItem;
            cache.TryRemove(address, out cacheItem);
            cacheItem?.Dispose();
        }

        private struct Range
        {
            private readonly ushort min;
            private readonly ushort max;

            private Range(ushort min, ushort max) : this()
            {
                if (min > max)
                {
                    throw new Exception("Bad range");
                }
                this.min = min;
                this.max = max;
            }

            public bool Intersects(Range range)
            {
                return Math.Max(range.min, this.min) <= Math.Min(range.max, this.max);
            }

            public static IEnumerable<Range> GetRanges(ushort address, ushort length)
            {
                var maxAddress = unchecked((ushort)(address + length - 1));
                if (maxAddress >= address)
                {
                    yield return new Range(address, maxAddress);
                }
                else
                {
                    yield return new Range(ushort.MinValue, maxAddress);
                    yield return new Range(address, ushort.MaxValue);
                }
            }

            public override string ToString()
            {
                return $"({min}, {max})";
            }
        }

        private class InstructionBlockCacheItem : IDisposable
        {
            private readonly Range[] addressRanges;
            private readonly Action removeTask;
            private readonly TimeSpan slidingExpiration;
            private readonly CancellationTokenSource cancellationTokenSource;

            private long? lastAccessed;


            public InstructionBlockCacheItem(IInstructionBlock<TRegisters> instructionBlock)
            {
                this.InstructionBlock = instructionBlock;
                this.addressRanges = Range.GetRanges(this.InstructionBlock.Address, this.InstructionBlock.Length).ToArray();
            }

            public InstructionBlockCacheItem(IInstructionBlock<TRegisters> instructionBlock, TimeSpan slidingExpiration, Action removeTask) : this(instructionBlock)
            {
                this.slidingExpiration = slidingExpiration;
                this.removeTask = removeTask;
                this.cancellationTokenSource = new CancellationTokenSource();

                Task.Delay(slidingExpiration, cancellationTokenSource.Token).ContinueWith(CheckSlidingExpiration, cancellationTokenSource.Token);
            }

            public IInstructionBlock<TRegisters> InstructionBlock { get; }

            public bool Intersects(Range range)
            {
                return addressRanges.Any(x => x.Intersects(range));
            }

            public void RegisterAccess()
            {
                this.lastAccessed = DateTime.UtcNow.Ticks;
            }

            private void CheckSlidingExpiration(Task task)
            {
                if (task.IsCanceled)
                {
                    return;
                }

                if (lastAccessed.HasValue)
                {
                    var ticks = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - lastAccessed.Value);
                    if (ticks >= slidingExpiration)
                    {
                        this.removeTask();
                    }
                    else
                    {
                        Task.Delay(slidingExpiration.Subtract(ticks), cancellationTokenSource.Token).ContinueWith(CheckSlidingExpiration, cancellationTokenSource.Token);
                        this.lastAccessed = null;
                    }
                }
                else
                {
                    this.removeTask();
                }
            }
            
            public void Dispose()
            {
                if (this.cancellationTokenSource == null)
                {
                    return;
                }

                this.cancellationTokenSource.Cancel();
                this.cancellationTokenSource.Dispose();
            }
        }
    }
}
