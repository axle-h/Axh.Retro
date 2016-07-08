using System;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    /// <summary>
    /// An instruction block for operations decoded with <see cref="DynaRec{TRegisters}"/>.
    /// </summary>
    /// <typeparam name="TRegisters">The type of the registers.</typeparam>
    /// <seealso cref="Axh.Retro.CPU.Z80.Contracts.Core.IInstructionBlock{TRegisters}" />
    internal class DynaRecInstructionBlock<TRegisters> : IInstructionBlock<TRegisters> where TRegisters : IRegisters
    {
        private readonly Func<TRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings> _action;

        /// <summary>
        /// Static instruction timings, known at compile time
        /// </summary>
        private readonly InstructionTimings _staticTimings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynaRecInstructionBlock{TRegisters}"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="length">The length.</param>
        /// <param name="action">The action.</param>
        /// <param name="staticTimings">The static timings.</param>
        /// <param name="lastDecodeResult">The last decode result.</param>
        public DynaRecInstructionBlock(ushort address,
            ushort length,
            Func<TRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings> action,
            InstructionTimings staticTimings,
            DecodeResult lastDecodeResult)
        {
            _action = action;
            _staticTimings = staticTimings;
            Address = address;
            Length = length;
            HaltCpu = lastDecodeResult == DecodeResult.Halt;
            HaltPeripherals = lastDecodeResult == DecodeResult.Stop;
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
        public string DebugInfo { get; internal set; }

        /// <summary>
        /// Executes the instruction block.
        /// </summary>
        /// <param name="registers">The registers.</param>
        /// <param name="mmu">The mmu.</param>
        /// <param name="alu">The alu.</param>
        /// <param name="peripheralManager">The peripheral manager.</param>
        /// <returns></returns>
        public InstructionTimings ExecuteInstructionBlock(TRegisters registers,
            IMmu mmu,
            IAlu alu,
            IPeripheralManager peripheralManager)
        {
            return _action(registers, mmu, alu, peripheralManager) + _staticTimings;
        }
    }
}