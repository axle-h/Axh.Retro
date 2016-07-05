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
    ///     Z80 MMU builds a segment MMU taking memory bank configs from IPlatformConfig and IPeripheralManager
    /// </summary>
    public class Z80Mmu<TRegisters> : SegmentMmu where TRegisters : IRegisters
    {
        private readonly IInstructionBlockCache<TRegisters> instructionBlockCache;

        public Z80Mmu(IPeripheralManager peripheralManager,
                      IPlatformConfig platformConfig,
                      IMemoryBankController memoryBankController,
                      IInstructionBlockCache<TRegisters> instructionBlockCache,
                      IDmaController dmaController,
                      IInstructionTimer instructionTimer)
            : base(
                GetAddressSegments(peripheralManager, platformConfig, memoryBankController),
                dmaController,
                instructionTimer)
        {
            this.instructionBlockCache = instructionBlockCache;
        }

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

        protected override void OnAddressWrite(ushort address, ushort length)
        {
            instructionBlockCache.InvalidateCache(address, length);
        }
    }
}