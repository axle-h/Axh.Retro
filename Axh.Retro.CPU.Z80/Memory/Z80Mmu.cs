using System;
using System.Collections.Generic;
using System.Linq;
using Axh.Retro.CPU.Common.Contracts.Config;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Common.Memory;
using Axh.Retro.CPU.Z80.Contracts.Cache;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Memory
{
    /// <summary>
    /// Z80 MMU builds a segment MMU taking memory bank configs from IPlatformConfig and IPeripheralManager
    /// </summary>
    public class Z80Mmu<TRegisters> : SegmentMmu where TRegisters : IRegisters
    {
        private readonly IInstructionBlockCache<TRegisters> _instructionBlockCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="Z80Mmu{TRegisters}"/> class.
        /// </summary>
        /// <param name="peripheralManager">The peripheral manager.</param>
        /// <param name="platformConfig">The platform configuration.</param>
        /// <param name="memoryBankController">The memory bank controller.</param>
        /// <param name="instructionBlockCache">The instruction block cache.</param>
        /// <param name="dmaController">The dma controller.</param>
        /// <param name="instructionTimer">The instruction timer.</param>
        public Z80Mmu(IPeripheralManager peripheralManager,
            IPlatformConfig platformConfig,
            IMemoryBankController memoryBankController,
            IInstructionBlockCache<TRegisters> instructionBlockCache,
            IDmaController dmaController,
            IInstructionTimer instructionTimer)
            : base(GetAddressSegments(peripheralManager, platformConfig, memoryBankController), dmaController, instructionTimer)
        {
            _instructionBlockCache = instructionBlockCache;
        }

        /// <summary>
        /// Gets the address segments.
        /// </summary>
        /// <param name="peripheralManager">The peripheral manager.</param>
        /// <param name="platformConfig">The platform configuration.</param>
        /// <param name="memoryBankController">The memory bank controller.</param>
        /// <returns></returns>
        private static IEnumerable<IAddressSegment> GetAddressSegments(IPeripheralManager peripheralManager,
            IPlatformConfig platformConfig,
            IMemoryBankController memoryBankController)
        {
            var memoryBanks =
                platformConfig.MemoryBanks.GroupBy(x => x.Address)
                              .Select(x => GetAddressSegment(x.ToArray(), memoryBankController))
                              .ToArray();
            return memoryBanks.Concat(peripheralManager.MemoryMap);
        }

        /// <summary>
        /// Gets the address segment.
        /// </summary>
        /// <param name="configs">The configs.</param>
        /// <param name="memoryBankController">The memory bank controller.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// </exception>
        /// <exception cref="System.NotImplementedException">Banked RAM</exception>
        private static IAddressSegment GetAddressSegment(ICollection<IMemoryBankConfig> configs,
            IMemoryBankController memoryBankController)
        {
            var config = configs.First();
            if (configs.Count == 1)
            {
                switch (config.Type)
                {
                    case MemoryBankType.RandomAccessMemory:
                        return new ArrayBackedMemoryBank(config);
                    case MemoryBankType.ReadOnlyMemory:
                        return new ReadOnlyMemoryBank(config);
                    case MemoryBankType.Unused:
                        return new NullMemoryBank(config);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            switch (config.Type)
            {
                case MemoryBankType.RandomAccessMemory:
                    throw new NotImplementedException("Banked RAM");
                case MemoryBankType.ReadOnlyMemory:
                    return new BankedReadOnlyMemoryBank(configs, memoryBankController);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, registers an address write event.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        protected override void OnAddressWrite(ushort address, ushort length) => _instructionBlockCache.InvalidateCache(address, length);
    }
}