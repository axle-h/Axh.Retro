﻿using Axh.Retro.CPU.Common.Contracts.Timing;

namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using System;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    internal class DynaRecInstructionBlock<TRegisters> : IInstructionBlock<TRegisters>
        where TRegisters : IRegisters
    {
        private readonly Func<TRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings> action;

        /// <summary>
        /// Static instruction timings, known at compile time
        /// </summary>
        private readonly InstructionTimings staticTimings;

        public DynaRecInstructionBlock(ushort address, ushort length, Func<TRegisters, IMmu, IAlu, IPeripheralManager, InstructionTimings> action, InstructionTimings staticTimings, DecodeResult lastDecodeResult)
        {
            this.action = action;
            this.staticTimings = staticTimings;
            this.Address = address;
            this.Length = length;
            this.HaltCpu = lastDecodeResult == DecodeResult.Halt;
            this.HaltPeripherals = lastDecodeResult == DecodeResult.Stop;
        }
        
        public ushort Address { get; }

        public ushort Length { get; }

        public bool HaltCpu { get; }

        public bool HaltPeripherals { get; }

        public string DebugInfo { get; internal set; }

        public InstructionTimings ExecuteInstructionBlock(TRegisters registers, IMmu mmu, IAlu alu, IPeripheralManager peripheralManager)
        {
            return action(registers, mmu, alu, peripheralManager) + staticTimings;
        }
    }
}
