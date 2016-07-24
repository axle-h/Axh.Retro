using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Core.Decode;
using Axh.Retro.CPU.Z80.Timing;

namespace Axh.Retro.CPU.Z80.Core.Interpreted
{
    /// <summary>
    /// Simple, interpreted instruction block decoder.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Z80.Contracts.Core.IInstructionBlockDecoder" />
    public class Interpreter : IInstructionBlockDecoder
    {
        private readonly CpuMode _cpuMode;
        private readonly bool _debug;
        private readonly IOpCodeDecoder _decoder;
        private readonly IPrefetchQueue _prefetch;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Interpreter"/> class.
        /// </summary>
        /// <param name="platformConfig">The platform configuration.</param>
        /// <param name="runtimeConfig">The runtime configuration.</param>
        /// <param name="prefetch">The prefetch.</param>
        /// <param name="decoder">The decoder.</param>
        public Interpreter(IPlatformConfig platformConfig, IRuntimeConfig runtimeConfig, IPrefetchQueue prefetch, IOpCodeDecoder decoder)
        {
            _prefetch = prefetch;
            _decoder = decoder;
            _cpuMode = platformConfig.CpuMode;
            _debug = runtimeConfig.DebugMode;
        }

        /// <summary>
        /// Gets a value indicating whether this instruction block decoder [supports instruction block caching].
        /// </summary>
        /// <value>
        /// <c>true</c> if this instruction block decoder [supports instruction block caching]; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsInstructionBlockCaching => false;

        /// <summary>
        /// Decodes the next block.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IInstructionBlock DecodeNextBlock(ushort address)
        {
            var decodedBlock = _decoder.DecodeNextBlock(address);
            var debugInfo = _debug ? $"{string.Join("\n", decodedBlock.Operations.Select(x => x.ToString()))}" : null;

            return new InstructionBlock(address,
                                        (ushort) _prefetch.TotalBytesRead,
                                        (registers, mmu, alu, peripherals) => Run(registers, mmu, alu, peripherals, decodedBlock),
                                        decodedBlock.Timings,
                                        decodedBlock.Halt,
                                        decodedBlock.Stop,
                                        debugInfo);
        }

        private InstructionTimings Run(IRegisters registers,
            IMmu mmu,
            IAlu alu,
            IPeripheralManager peripherals,
            DecodedBlock block)
        {
            var helper = new InterpreterHelper(registers, mmu, alu, peripherals);
            var result = block.Operations.Select(b => Run(registers, mmu, alu, peripherals, helper, b)).Aggregate((t0, t1) => t0 + t1);

            if (_cpuMode == CpuMode.Z80)
            {
                // Add the block length to the 7 lsb of memory refresh register.
                registers.R = (byte) ((registers.R + _prefetch.TotalBytesRead) & 0x7f);
            }

            return result;
        }

        private void SyncProgramCounter(IRegisters registers) => registers.ProgramCounter = (ushort)(registers.ProgramCounter + _prefetch.TotalBytesRead);

        private InstructionTimings Run(IRegisters registers, IMmu mmu, IAlu alu, IPeripheralManager peripherals, InterpreterHelper helper, Operation operation)
        {
            helper.Operation = operation;
            var timer = new InstructionTimingsBuilder();

            switch (operation.OpCode)
            {
                case OpCode.NoOperation:
                    break;
                case OpCode.Stop:
                case OpCode.Halt:
                    SyncProgramCounter(registers);
                    break;

                case OpCode.Load:
                    if (operation.Operand1 == operation.Operand2)
                    {
                        break;
                    }

                    helper.Operand1 = helper.Operand2;
                    if (operation.Operand2 == Operand.I || operation.Operand2 == Operand.R)
                    {
                        // LD A, R & LD A, I also reset H & N and copy IFF2 to P/V
                        registers.AccumulatorAndFlagsRegisters.Flags.SetResultFlags(registers.AccumulatorAndFlagsRegisters.A);
                        registers.AccumulatorAndFlagsRegisters.Flags.HalfCarry = false;
                        registers.AccumulatorAndFlagsRegisters.Flags.Subtract = false;
                        registers.AccumulatorAndFlagsRegisters.Flags.ParityOverflow = registers.InterruptFlipFlop2;
                    }
                    break;
                    
                case OpCode.Load16:
                    helper.WordOperand1 = helper.WordOperand2;
                    break;

                case OpCode.Push:
                    helper.PushStackPointer();
                    mmu.WriteWord(registers.StackPointer, helper.WordOperand1);
                    break;

                case OpCode.Pop:
                    helper.WordOperand1 = mmu.ReadWord(registers.StackPointer);
                    helper.PopStackPointer();
                    break;

                case OpCode.Add:
                    helper.Alu8BitOperation(alu.Add);
                    break;

                case OpCode.AddCarry:
                    helper.Alu8BitOperation(alu.AddWithCarry);
                    break;

                case OpCode.Subtract:
                    helper.Alu8BitOperation(alu.Subtract);
                    break;

                case OpCode.SubtractCarry:
                    helper.Alu8BitOperation(alu.SubtractWithCarry);
                    break;

                case OpCode.And:
                    helper.Alu8BitOperation(alu.And);
                    break;

                case OpCode.Or:
                    helper.Alu8BitOperation(alu.Or);
                    break;

                case OpCode.Xor:
                    helper.Alu8BitOperation(alu.Xor);
                    break;

                case OpCode.Compare:
                    alu.Compare(registers.AccumulatorAndFlagsRegisters.A, helper.Operand1);
                    break;

                case OpCode.Increment:
                    helper.Operand1 = alu.Increment(helper.Operand1);
                    break;

                case OpCode.Decrement:
                    helper.Operand1 = alu.Decrement(helper.Operand1);
                    break;

                case OpCode.Add16:
                    helper.Alu16BitOperation(alu.Add);
                    break;

                case OpCode.AddCarry16:
                    helper.Alu16BitOperation(alu.AddWithCarry);
                    break;

                case OpCode.SubtractCarry16:
                    helper.Alu16BitOperation(alu.SubtractWithCarry);
                    break;

                case OpCode.Increment16:
                    // INC ss (no flags changes so implemented directly)
                    helper.WordOperand1 = (ushort) (helper.WordOperand1 + 1);
                    break;

                case OpCode.Decrement16:
                    // DEC ss (no flags changes so implemented directly)
                    helper.WordOperand1 = (ushort) (helper.WordOperand1 - 1);
                    break;

                case OpCode.Exchange:
                    {
                        var w = helper.WordOperand2;
                        helper.WordOperand2 = helper.WordOperand1;
                        helper.WordOperand1 = w;
                    }
                    break;

                case OpCode.ExchangeAccumulatorAndFlags:
                    registers.SwitchToAlternativeAccumulatorAndFlagsRegisters();
                    break;

                case OpCode.ExchangeGeneralPurpose:
                    registers.SwitchToAlternativeGeneralPurposeRegisters();
                    break;

                case OpCode.Jump:
                    if (operation.FlagTest == FlagTest.None || helper.DoFlagTest())
                    {
                        registers.ProgramCounter = helper.WordOperand1;
                    }
                    else
                    {
                        SyncProgramCounter(registers);
                    }
                    break;

                case OpCode.JumpRelative:
                    if (operation.FlagTest == FlagTest.None || helper.DoFlagTest())
                    {
                        helper.JumpToDisplacement();

                        if (operation.FlagTest != FlagTest.None)
                        {
                            timer.Add(1, 5);
                        }
                    }
                    SyncProgramCounter(registers);
                    break;

                case OpCode.DecrementJumpRelativeIfNonZero:
                    registers.GeneralPurposeRegisters.B--;
                    if (registers.GeneralPurposeRegisters.B != 0)
                    {
                        helper.JumpToDisplacement();
                        timer.Add(1, 5);
                    }
                    SyncProgramCounter(registers);
                    break;

                case OpCode.Call:
                    SyncProgramCounter(registers);

                    if (operation.FlagTest == FlagTest.None || helper.DoFlagTest())
                    {
                        helper.PushStackPointer();
                        mmu.WriteWord(registers.StackPointer, registers.ProgramCounter);
                        registers.ProgramCounter = helper.WordOperand1;

                        if (operation.FlagTest != FlagTest.None)
                        {
                            timer.Add(2, 7);
                        }
                    }
                    break;

                case OpCode.Return:
                    if (operation.FlagTest == FlagTest.None || helper.DoFlagTest())
                    {
                        registers.ProgramCounter = mmu.ReadWord(registers.StackPointer);
                        helper.PopStackPointer();

                        if (operation.FlagTest != FlagTest.None)
                        {
                            timer.Add(2, 6);
                        }
                    }
                    else
                    {
                        SyncProgramCounter(registers);
                    }
                    break;

                case OpCode.ReturnFromInterrupt:
                    registers.ProgramCounter = mmu.ReadWord(registers.StackPointer);
                    helper.PopStackPointer();
                    registers.InterruptFlipFlop1 = true;
                    break;

                case OpCode.ReturnFromNonmaskableInterrupt:
                    registers.ProgramCounter = mmu.ReadWord(registers.StackPointer);
                    helper.PopStackPointer();
                    registers.InterruptFlipFlop1 = registers.InterruptFlipFlop2;
                    break;

                case OpCode.Reset:
                    SyncProgramCounter(registers);
                    helper.PushStackPointer();
                    mmu.WriteWord(registers.StackPointer, registers.ProgramCounter);
                    registers.ProgramCounter = helper.WordOperand1;
                    break;

                case OpCode.Input:
                    helper.Operand1 = peripherals.ReadByteFromPort(helper.Operand2,
                                                                   operation.Operand2 == Operand.n
                                                                       ? registers.AccumulatorAndFlagsRegisters.A
                                                                       : registers.GeneralPurposeRegisters.B);
                    break;

                case OpCode.Output:
                    peripherals.WriteByteToPort(helper.Operand2,
                                                operation.Operand2 == Operand.n
                                                    ? registers.AccumulatorAndFlagsRegisters.A
                                                    : registers.GeneralPurposeRegisters.B,
                                                helper.Operand1);
                    break;

                case OpCode.RotateLeftWithCarry:
                    helper.Operand1 = alu.RotateLeftWithCarry(helper.Operand1);
                    if (operation.OpCodeMeta.HasFlag(OpCodeMeta.UseAlternativeFlagAffection))
                    {
                        registers.AccumulatorAndFlagsRegisters.Flags.Zero = false;
                        registers.AccumulatorAndFlagsRegisters.Flags.Sign = false;
                    }
                    break;

                case OpCode.RotateLeft:
                    helper.Operand1 = alu.RotateLeft(helper.Operand1);
                    if (operation.OpCodeMeta.HasFlag(OpCodeMeta.UseAlternativeFlagAffection))
                    {
                        registers.AccumulatorAndFlagsRegisters.Flags.Zero = false;
                        registers.AccumulatorAndFlagsRegisters.Flags.Sign = false;
                    }
                    break;

                case OpCode.RotateRightWithCarry:
                    helper.Operand1 = alu.RotateRightWithCarry(helper.Operand1);
                    if (operation.OpCodeMeta.HasFlag(OpCodeMeta.UseAlternativeFlagAffection))
                    {
                        registers.AccumulatorAndFlagsRegisters.Flags.Zero = false;
                        registers.AccumulatorAndFlagsRegisters.Flags.Sign = false;
                    }
                    break;

                case OpCode.RotateRight:
                    helper.Operand1 = alu.RotateRight(helper.Operand1);
                    if (operation.OpCodeMeta.HasFlag(OpCodeMeta.UseAlternativeFlagAffection))
                    {
                        registers.AccumulatorAndFlagsRegisters.Flags.Zero = false;
                        registers.AccumulatorAndFlagsRegisters.Flags.Sign = false;
                    }
                    break;

                case OpCode.RotateLeftDigit:
                    {
                        var result = alu.RotateLeftDigit(registers.AccumulatorAndFlagsRegisters.A,
                                                         mmu.ReadByte(registers.GeneralPurposeRegisters.HL));
                        registers.AccumulatorAndFlagsRegisters.A = result.Accumulator;
                        mmu.WriteByte(registers.GeneralPurposeRegisters.HL, result.Result);
                    }
                    break;

                case OpCode.RotateRightDigit:
                    {
                        var result = alu.RotateRightDigit(registers.AccumulatorAndFlagsRegisters.A,
                                                          mmu.ReadByte(registers.GeneralPurposeRegisters.HL));
                        registers.AccumulatorAndFlagsRegisters.A = result.Accumulator;
                        mmu.WriteByte(registers.GeneralPurposeRegisters.HL, result.Result);
                    }
                    break;

                case OpCode.ShiftLeft:
                    helper.Operand1 = alu.ShiftLeft(helper.Operand1);
                    break;

                case OpCode.ShiftLeftSet:
                    helper.Operand1 = alu.ShiftLeftSet(helper.Operand1);
                    break;

                case OpCode.ShiftRight:
                    helper.Operand1 = alu.ShiftRight(helper.Operand1);
                    break;

                case OpCode.ShiftRightLogical:
                    helper.Operand1 = alu.ShiftRightLogical(helper.Operand1);
                    break;

                case OpCode.BitTest:
                    alu.BitTest(helper.Operand1, operation.ByteLiteral);
                    break;

                case OpCode.BitSet:
                    helper.Operand1 = alu.BitSet(helper.Operand1, operation.ByteLiteral);
                    break;

                case OpCode.BitReset:
                    helper.Operand1 = alu.BitReset(helper.Operand1, operation.ByteLiteral);
                    break;

                case OpCode.TransferIncrement:
                    helper.BlockTransfer();
                    break;

                case OpCode.TransferIncrementRepeat:
                    helper.BlockTransferRepeat(timer);
                    break;

                case OpCode.TransferDecrement:
                    helper.BlockTransfer(true);
                    break;

                case OpCode.TransferDecrementRepeat:
                    helper.BlockTransferRepeat(timer, true);
                    break;

                case OpCode.SearchIncrement:
                    helper.BlockSearch();
                    break;

                case OpCode.SearchIncrementRepeat:
                    helper.BlockSearchRepeat(timer);
                    break;

                case OpCode.SearchDecrement:
                    helper.BlockSearch(true);
                    break;

                case OpCode.SearchDecrementRepeat:
                    helper.BlockSearchRepeat(timer, true);
                    break;

                case OpCode.InputTransferIncrement:
                    helper.InputTransfer();
                    break;

                case OpCode.InputTransferIncrementRepeat:
                    helper.InputTransferRepeat(timer);
                    break;

                case OpCode.InputTransferDecrement:
                    helper.InputTransfer(true);
                    break;

                case OpCode.InputTransferDecrementRepeat:
                    helper.InputTransferRepeat(timer, true);
                    break;

                case OpCode.OutputTransferIncrement:
                    helper.OutputTransfer();
                    break;

                case OpCode.OutputTransferIncrementRepeat:
                    helper.OutputTransferRepeat(timer);
                    break;

                case OpCode.OutputTransferDecrement:
                    helper.OutputTransfer(true);
                    break;

                case OpCode.OutputTransferDecrementRepeat:
                    helper.OutputTransferRepeat(timer, true);
                    break;

                case OpCode.DecimalArithmeticAdjust:
                    registers.AccumulatorAndFlagsRegisters.A = alu.DecimalAdjust(registers.AccumulatorAndFlagsRegisters.A,
                                                                                 _cpuMode == CpuMode.Z80);
                    break;

                case OpCode.NegateOnesComplement:
                    registers.AccumulatorAndFlagsRegisters.A = (byte) ~registers.AccumulatorAndFlagsRegisters.A;
                    registers.AccumulatorAndFlagsRegisters.Flags.SetUndocumentedFlags(registers.AccumulatorAndFlagsRegisters.A);
                    registers.AccumulatorAndFlagsRegisters.Flags.HalfCarry = true;
                    registers.AccumulatorAndFlagsRegisters.Flags.Subtract = true;
                    break;

                case OpCode.NegateTwosComplement:
                    registers.AccumulatorAndFlagsRegisters.A = alu.Subtract(0, registers.AccumulatorAndFlagsRegisters.A);
                    break;

                case OpCode.InvertCarryFlag:
                    registers.AccumulatorAndFlagsRegisters.Flags.SetUndocumentedFlags(registers.AccumulatorAndFlagsRegisters.A);
                    registers.AccumulatorAndFlagsRegisters.Flags.HalfCarry = _cpuMode != CpuMode.GameBoy &&
                                                                             registers.AccumulatorAndFlagsRegisters.Flags.Carry;
                    registers.AccumulatorAndFlagsRegisters.Flags.Subtract = false;
                    registers.AccumulatorAndFlagsRegisters.Flags.Carry = !registers.AccumulatorAndFlagsRegisters.Flags.Carry;
                    break;

                case OpCode.SetCarryFlag:
                    registers.AccumulatorAndFlagsRegisters.Flags.SetUndocumentedFlags(registers.AccumulatorAndFlagsRegisters.A);
                    registers.AccumulatorAndFlagsRegisters.Flags.HalfCarry = false;
                    registers.AccumulatorAndFlagsRegisters.Flags.Subtract = false;
                    registers.AccumulatorAndFlagsRegisters.Flags.Carry = true;
                    break;

                case OpCode.DisableInterrupts:
                    registers.InterruptFlipFlop1 = false;
                    registers.InterruptFlipFlop2 = false;
                    break;

                case OpCode.EnableInterrupts:
                    registers.InterruptFlipFlop1 = true;
                    registers.InterruptFlipFlop2 = true;
                    break;

                case OpCode.InterruptMode0:
                    registers.InterruptMode = InterruptMode.InterruptMode0;
                    break;

                case OpCode.InterruptMode1:
                    registers.InterruptMode = InterruptMode.InterruptMode1;
                    break;

                case OpCode.InterruptMode2:
                    registers.InterruptMode = InterruptMode.InterruptMode2;
                    break;

                case OpCode.Swap:
                    helper.Operand1 = alu.Swap(helper.Operand1);
                    break;

                case OpCode.LoadIncrement:
                    helper.Operand1 = helper.Operand2;
                    registers.GeneralPurposeRegisters.HL++;
                    break;

                case OpCode.LoadDecrement:
                    helper.Operand1 = helper.Operand2;
                    registers.GeneralPurposeRegisters.HL--;
                    break;

                    default:
                        throw new ArgumentOutOfRangeException();
            }

            if (operation.OpCodeMeta.HasFlag(OpCodeMeta.AutoCopy))
            {
                // Autocopy for DD/FD prefix
                helper.Operand2 = helper.Operand1;
            }

            return timer.GetInstructionTimings();
        }
    }
}