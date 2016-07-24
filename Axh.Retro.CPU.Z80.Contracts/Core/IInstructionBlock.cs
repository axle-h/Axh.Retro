using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    /// <summary>
    /// An instruction block.
    /// </summary>
    public interface IInstructionBlock
    {
        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        ushort Address { get; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        ushort Length { get; }

        /// <summary>
        /// Gets a value indicating whether [to halt the cpu at the end of the block].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [the cpu should be halted at the end of this instruction block]; otherwise, <c>false</c>.
        /// </value>
        bool HaltCpu { get; }

        /// <summary>
        /// Gets a value indicating whether [to halt peripherals at the end of the block].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [peripherals should be halted at the end of this instruction block]; otherwise, <c>false</c>.
        /// </value>
        bool HaltPeripherals { get; }

        /// <summary>
        /// Gets the debug information.
        /// This is only populated when debug mode is enabled in config.
        /// </summary>
        /// <value>
        /// The debug information.
        /// </value>
        string DebugInfo { get; }

        /// <summary>
        /// Executes the instruction block.
        /// </summary>
        /// <param name="registers">The registers.</param>
        /// <param name="mmu">The mmu.</param>
        /// <param name="alu">The alu.</param>
        /// <param name="peripheralManager">The peripheral manager.</param>
        /// <returns></returns>
        InstructionTimings ExecuteInstructionBlock(IRegisters registers, IMmu mmu, IAlu alu, IPeripheralManager peripheralManager);
    }
}