namespace Axh.Retro.CPU.X80.Contracts.Cache
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public interface IInstructionBlockCache<TRegisters> where TRegisters : IRegisters
    {
        Guid CacheId { get; }

        IInstructionBlock<TRegisters> GetOrSet(ushort address, Func<IInstructionBlock<TRegisters>> getInstanceFunc);

        void InvalidateCache(ushort address, int length);
    }
}
