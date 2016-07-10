using System;
using System.Collections.Generic;
using Axh.Retro.CPU.Common.Config;
using Axh.Retro.CPU.Common.Contracts.Config;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.Z80Console.Properties;

namespace Axh.Retro.Z80Console.Config
{
    /// <summary>
    /// Configuration for the for the Z80 console platform.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Z80.Contracts.Config.IPlatformConfig" />
    internal class Z8064KBootstrappedConfig : IPlatformConfig
    {
        private const ushort MemoryStart = 0x0000;
        private const ushort MemoryLength = 0xffff;

        /// <summary>
        /// Initializes a new instance of the <see cref="Z8064KBootstrappedConfig"/> class.
        /// </summary>
        public Z8064KBootstrappedConfig()
        {
            var ramConfig = new SimpleMemoryBankConfig(MemoryBankType.RandomAccessMemory, null, MemoryStart, MemoryLength);

            // Bootstrap from Resources/code.bin
            // This is a simple hello world program that was built using sdcc
            var code = Resources.code;
            Array.Copy(code, 0, ramConfig.InitialState, 0, code.Length);

            MemoryBanks = new[] { ramConfig };
        }

        /// <summary>
        /// Gets the cpu mode.
        /// </summary>
        /// <value>
        /// The cpu mode.
        /// </value>
        public CpuMode CpuMode => CpuMode.Z80;

        /// <summary>
        /// Gets the memory banks.
        /// </summary>
        /// <value>
        /// The memory banks.
        /// </value>
        public IEnumerable<IMemoryBankConfig> MemoryBanks { get; }

        /// <summary>
        /// Gets the machine cycle speed in MHZ.
        /// </summary>
        /// <value>
        /// The machine cycle speed in MHZ.
        /// </value>
        public double MachineCycleSpeedMhz => 4;

        /// <summary>
        /// Gets the instruction timing synchronize mode.
        /// </summary>
        /// <value>
        /// The instruction timing synchronize mode.
        /// </value>
        public InstructionTimingSyncMode InstructionTimingSyncMode => InstructionTimingSyncMode.Null;

        /// <summary>
        /// Gets a value indicating whether [lock on undefined instruction].
        /// </summary>
        /// <value>
        /// <c>true</c> if [lock on undefined instruction]; otherwise, <c>false</c>.
        /// </value>
        public bool LockOnUndefinedInstruction => false;
    }
}