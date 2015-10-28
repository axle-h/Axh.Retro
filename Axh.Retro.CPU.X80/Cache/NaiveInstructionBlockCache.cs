namespace Axh.Retro.CPU.X80.Cache
{
    using System;
    using System.Runtime.Caching;
    using System.Runtime.CompilerServices;

    using Axh.Retro.CPU.X80.Contracts.Cache;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    /// <summary>
    /// Implements System.Runtime.Caching.
    /// Naive because it does no bounds checking as Memory cache uses string key, bleh.
    /// </summary>
    /// <typeparam name="TRegisters"></typeparam>
    public class NaiveInstructionBlockCache<TRegisters> : IInstructionBlockCache<TRegisters> where TRegisters : IRegisters
    {
        private static readonly TimeSpan SlidingExpiration = TimeSpan.FromMinutes(10);

        private readonly MemoryCache cache;

        private readonly CacheItemPolicy policy;

        public NaiveInstructionBlockCache()
        {
            this.CacheId = Guid.NewGuid();
            this.cache = new MemoryCache($"{this.GetType().FullName}({this.CacheId})");
            this.policy = new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration, SlidingExpiration = SlidingExpiration };
        }

        public Guid CacheId { get; }

        public IInstructionBlock<TRegisters> GetOrSet(ushort address, Func<IInstructionBlock<TRegisters>> getInstanceFunc)
        {
            var key = address.ToString();
            var block = this.cache.Get(address.ToString()) as IInstructionBlock<TRegisters>;
            if (block != null)
            {
                return block;
            }

            block = getInstanceFunc();
            this.cache.Add(key, block, policy);

            return block;
        }
        
        public void InvalidateCache(ushort address, int length)
        {
            // Do nothing
            // It would be far too inefficient to parse all the cached keys
        }
    }
}
