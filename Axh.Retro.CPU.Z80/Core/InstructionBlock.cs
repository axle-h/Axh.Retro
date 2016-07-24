using System;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Core
{
    /// <summary>
    /// An instruction block.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Z80.Contracts.Core.IInstructionBlock" />
    internal class InstructionBlock : IInstructionBlock
    {
        private readonly Func<IRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings> _action;

        /// <summary>
        /// Static instruction timings, known at compile time
        /// </summary>
        private readonly InstructionTimings _staticTimings;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionBlock"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="length">The length.</param>
        /// <param name="action">The action.</param>
        /// <param name="staticTimings">The static timings.</param>
        /// <param name="halt">if set to <c>true</c> [halt].</param>
        /// <param name="stop">if set to <c>true</c> [stop].</param>
        /// <param name="debugInfo">The debug information.</param>
        public InstructionBlock(ushort address,
            ushort length,
            Func<IRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings> action,
            InstructionTimings staticTimings,
            bool halt,
            bool stop,
            string debugInfo = null)
        {
            _action = action;
            _staticTimings = staticTimings;
            Address = address;
            Length = length;
            HaltCpu = halt || stop;
            HaltPeripherals = stop;
            DebugInfo = debugInfo;
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public ushort Address { get; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public ushort Length { get; }

        /// <summary>
        /// Gets a value indicating whether [to halt the cpu at the end of the block].
        /// </summary>
        /// <value>
        /// <c>true</c> if [the cpu should be halted at the end of this instruction block]; otherwise, <c>false</c>.
        /// </value>
        public bool HaltCpu { get; }

        /// <summary>
        /// Gets a value indicating whether [to halt peripherals at the end of the block].
        /// </summary>
        /// <value>
        /// <c>true</c> if [peripherals should be halted at the end of this instruction block]; otherwise, <c>false</c>.
        /// </value>
        public bool HaltPeripherals { get; }

        /// <summary>
        /// Gets the debug information.
        /// This is only populated when debug mode is enabled in config.
        /// </summary>
        /// <value>
        /// The debug information.
        /// </value>
        public string DebugInfo { get; }

        /// <summary>
        /// Executes the instruction block.
        /// </summary>
        /// <param name="registers">The registers.</param>
        /// <param name="mmu">The mmu.</param>
        /// <param name="alu">The alu.</param>
        /// <param name="peripheralManager">The peripheral manager.</param>
        /// <returns></returns>
        public InstructionTimings ExecuteInstructionBlock(IRegisters registers,
            IMmu mmu,
            IAlu alu,
            IPeripheralManager peripheralManager) => _action(registers, mmu, alu, peripheralManager) + _staticTimings;
    }
}