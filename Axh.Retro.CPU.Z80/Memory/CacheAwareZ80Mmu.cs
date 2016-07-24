using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Memory.Dma;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Cache;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;

namespace Axh.Retro.CPU.Z80.Memory
{
    /// <summary>
    /// Z80 MMU builds a segment MMU taking memory bank configs from <see cref="IPlatformConfig"/> and <see cref="IPeripheralManager"/>.
    /// This implementation is instruction block cache aware so will call <see cref="IInstructionBlockCache.InvalidateCache"/> for every address write.
    /// </summary>
    public class CacheAwareZ80Mmu : Z80Mmu
    {
        private readonly IInstructionBlockCache _instructionBlockCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheAwareZ80Mmu"/> class.
        /// </summary>
        /// <param name="peripheralManager">The peripheral manager.</param>
        /// <param name="platformConfig">The platform configuration.</param>
        /// <param name="memoryBankController">The memory bank controller.</param>
        /// <param name="dmaController">The dma controller.</param>
        /// <param name="instructionTimer">The instruction timer.</param>
        public CacheAwareZ80Mmu(IPeripheralManager peripheralManager,
            IPlatformConfig platformConfig,
            IMemoryBankController memoryBankController,
            IDmaController dmaController,
            IInstructionTimer instructionTimer,
            IInstructionBlockCache instructionBlockCache)
            : base(peripheralManager, platformConfig, memoryBankController, dmaController, instructionTimer)
        {
            _instructionBlockCache = instructionBlockCache;
        }

        /// <summary> 
        /// When overridden in a derived class, registers an address write event.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        protected override void OnAddressWrite(ushort address, ushort length) => _instructionBlockCache.InvalidateCache(address, length);
    }
}