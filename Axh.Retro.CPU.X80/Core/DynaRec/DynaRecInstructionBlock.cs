namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    internal class DynaRecInstructionBlock<TRegisters> : IInstructionBlock<TRegisters>
        where TRegisters : IRegisters
    {
        private readonly Func<TRegisters, IMmu, IArithmeticLogicUnit, InstructionTimings> action;

        /// <summary>
        /// Static instruction timings, known at compile time
        /// </summary>
        private readonly InstructionTimings staticTimings;

        public DynaRecInstructionBlock(ushort address, ushort length, Func<TRegisters, IMmu, IArithmeticLogicUnit, InstructionTimings> action, InstructionTimings staticTimings)
        {
            this.action = action;
            this.staticTimings = staticTimings;
            Address = address;
            Length = length;
        }

        public ushort Address { get; }

        public ushort Length { get; }

        public InstructionTimings ExecuteInstructionBlock(TRegisters registers, IMmu mmu, IArithmeticLogicUnit alu)
        {
            return action(registers, mmu, alu) + staticTimings;
        }
    }
}
