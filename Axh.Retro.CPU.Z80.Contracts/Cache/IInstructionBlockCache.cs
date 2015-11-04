namespace Axh.Retro.CPU.Z80.Contracts.Cache
{
    using System;

    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    /// <summary>
    /// Doesn't need to be thread safe
    /// </summary>
    /// <typeparam name="TRegisters"></typeparam>
    public interface IInstructionBlockCache<TRegisters> where TRegisters : IRegisters
    {
        Guid CacheId { get; }

        IInstructionBlock<TRegisters> GetOrSet(ushort address, Func<IInstructionBlock<TRegisters>> getInstanceFunc);

        void InvalidateCache(ushort address, ushort length);
    }
}
