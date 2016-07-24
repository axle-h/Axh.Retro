using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Config;

namespace Axh.Retro.CPU.Z80.Contracts.Config
{
    /// <summary>
    /// The platform configuration.
    /// </summary>
    public interface IPlatformConfig
    {
        /// <summary>
        /// Gets the cpu mode.
        /// </summary>
        /// <value>
        /// The cpu mode.
        /// </value>
        CpuMode CpuMode { get; }

        /// <summary>
        /// Gets the memory banks.
        /// </summary>
        /// <value>
        /// The memory banks.
        /// </value>
        IEnumerable<IMemoryBankConfig> MemoryBanks { get; }

        /// <summary>
        /// Gets the machine cycle speed in MHZ.
        /// </summary>
        /// <value>
        /// The machine cycle speed in MHZ.
        /// </value>
        double MachineCycleFrequencyMhz { get; }

        /// <summary>
        /// Gets the instruction timing synchronize mode.
        /// </summary>
        /// <value>
        /// The instruction timing synchronize mode.
        /// </value>
        InstructionTimingSyncMode InstructionTimingSyncMode { get; }

        /// <summary>
        /// Gets a value indicating whether [lock on undefined instruction].
        /// </summary>
        /// <value>
        /// <c>true</c> if [lock on undefined instruction]; otherwise, <c>false</c>.
        /// </value>
        bool LockOnUndefinedInstruction { get; }
    }
}