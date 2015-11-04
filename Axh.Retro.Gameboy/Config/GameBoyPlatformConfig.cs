﻿namespace Axh.Retro.GameBoy.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.Common.Config;
    using Axh.Retro.CPU.Common.Contracts.Config;
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.GameBoy.Contracts;
    using Axh.Retro.GameBoy.Contracts.Config;
    using Axh.Retro.GameBoy.Contracts.Factories;
    using Axh.Retro.GameBoy.Contracts.Media;

    public class GameBoyPlatformConfig : IPlatformConfig
    {
        private const double CpuFrequency = 4.194304;
        private const ushort SystemMemoryBank1Address = 0xd000;
        private const ushort SystemMemoryBank1Length = 0x1000;
        private const int CgbSystemMemoryBanks = 8;

        private const ushort CartridgeRomBank0Address = 0x0000;
        private const ushort CartridgeRomBank1Address = 0x4000;
        private const ushort CartridgeRomBankLength = 0x4000;

        private const ushort CartridgeRamAddress = 0xa000;
        private const ushort CartridgeRamLength = 0x2000;
        
        /// <summary>
        /// $FF80-$FFFE	Zero Page - 127 bytes
        /// </summary>
        private static readonly IMemoryBankConfig ZeroPageConfig = new SimpleMemoryBankConfig(MemoryBankType.RandomAccessMemory, null, 0xff80, 0x7f);

        /// <summary>
        /// $FEA0-$FEFF	Unusable Memory
        /// </summary>
        private static readonly IMemoryBankConfig UnusedConfig = new SimpleMemoryBankConfig(MemoryBankType.Unused, null, 0xfea0, 0x60);
        
        /// <summary>
        /// $C000-$CFFF	Internal RAM - Bank 0 (fixed)
        /// </summary>
        private static readonly IMemoryBankConfig SystemMemoryBank0Config = new SimpleMemoryBankConfig(MemoryBankType.RandomAccessMemory, null, 0xc000, 0x1000);
        
        public GameBoyPlatformConfig(IGameBoyConfig gameBoyConfig, ICartridgeFactory cartridgeFactory)
        {
            var cartridge = cartridgeFactory.GetCartridge(gameBoyConfig.CartridgeData);
            this.MemoryBanks = GetMemoryBankConfigs(cartridge, gameBoyConfig.GameBoyType).ToArray();
        }

        private static IEnumerable<IMemoryBankConfig> GetMemoryBankConfigs(ICartridge cartridge, GameBoyType gameBoyType)
        {
            // TODO: error checks
            if (cartridge.RomBanks.Length < 2)
            {
                throw new Exception("All cartridges must have 2 rom banks");
            }

            yield return new SimpleMemoryBankConfig(MemoryBankType.ReadOnlyMemory, null, CartridgeRomBank0Address, CartridgeRomBankLength, cartridge.RomBanks[0]);
            for (byte i = 1; i < cartridge.RomBanks.Length; i++)
            {
                yield return new SimpleMemoryBankConfig(MemoryBankType.ReadOnlyMemory, i, CartridgeRomBank1Address, CartridgeRomBankLength, cartridge.RomBanks[i]);
            }
            
            if (cartridge.RamBankLengths.Any())
            {
                var ramBanks = cartridge.RamBankLengths.Length > 1
                    ? cartridge.RamBankLengths.SelectMany((length, id) => GetCartridgeRamBankConfig(id, length))
                    : GetCartridgeRamBankConfig(null, cartridge.RamBankLengths.First());
                
                foreach (var bank in ramBanks)
                {
                    yield return bank;
                }
            }
            else
            {
                yield return new SimpleMemoryBankConfig(MemoryBankType.Unused, null, CartridgeRamAddress, CartridgeRamLength);
            }

            yield return SystemMemoryBank0Config;

            switch (gameBoyType)
            {
                case GameBoyType.GameBoy:
                    yield return new SimpleMemoryBankConfig(MemoryBankType.RandomAccessMemory, null, SystemMemoryBank1Address, SystemMemoryBank1Length);
                    break;
                case GameBoyType.GameBoyColour:
                    for(byte i = 1; i < CgbSystemMemoryBanks; i++)
                    {
                        yield return new SimpleMemoryBankConfig(MemoryBankType.RandomAccessMemory, i, SystemMemoryBank1Address, SystemMemoryBank1Length);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameBoyType), gameBoyType, null);
            }

            yield return UnusedConfig;
            yield return ZeroPageConfig;
        }

        private static IEnumerable<IMemoryBankConfig> GetCartridgeRamBankConfig(int? bankId, ushort length)
        {
            yield return new SimpleMemoryBankConfig(MemoryBankType.RandomAccessMemory, bankId.HasValue ? (byte?)bankId.Value : null, CartridgeRamAddress, length);
            if (length >= CartridgeRamLength)
            {
                yield break;
            }

            if (bankId.HasValue)
            {
                throw new Exception("Banked cartridge RAM must be " + CartridgeRamLength + " bytes");
            }

            yield return new SimpleMemoryBankConfig(MemoryBankType.Unused, null, (ushort)(CartridgeRamAddress + length), (ushort)(CartridgeRamLength - length));
        }

        public CpuMode CpuMode => CpuMode.GameBoy;

        public IEnumerable<IMmuBankConfig> MemoryBanks { get; }

        /// <summary>
        /// GB rounds all machine cycles to 4 throttling states. I.e. we need to run timing based on machine cycles.
        /// </summary>
        public double? MachineCycleSpeedMhz => CpuFrequency;

        public double? ThrottlingStateSpeedMhz => null;
    }
}
