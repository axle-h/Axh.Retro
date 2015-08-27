namespace Axh.Retro.CPU.X80.Contracts.Core
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public class InstructionBlock<TRegisters> where TRegisters : IRegisters
    {
        private readonly Func<TRegisters, IMmu, InstructionTimings> action;

        /// <summary>
        /// Static instruction timings, known at compile time
        /// </summary>
        private readonly InstructionTimings staticTimings;

        public InstructionBlock(ushort address, ushort length, Func<TRegisters, IMmu, InstructionTimings> action, InstructionTimings staticTimings)
        {
            this.action = action;
            this.staticTimings = staticTimings;
            Address = address;
            Length = length;
        }

        public ushort Address { get; }

        public ushort Length { get; }

        public InstructionTimings ExecuteInstructionBlock(TRegisters registers, IMmu mmu)
        {
            return action(registers, mmu) + staticTimings;
        }
    }
}
