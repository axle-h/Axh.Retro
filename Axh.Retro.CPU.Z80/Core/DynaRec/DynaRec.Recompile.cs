﻿namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Core.Decode;

    internal partial class DynaRec<TRegisters> where TRegisters : IRegisters
    {
        private IEnumerable<Expression> Recompile(Operation operation)
        {
            switch (operation.Opcode)
            {
                case Opcode.NoOperation:
                    break;
                case Opcode.Halt:
                    LastDecodeResult = DecodeResult.Halt;
                    break;

                case Opcode.Load:
                    if (operation.Operand1 == operation.Operand2)
                    {
                        break;
                    }

                    yield return WriteOperand1(operation, ReadOperand2(operation));

                    if (operation.Operand2 == Operand.I || operation.Operand2 == Operand.R)
                    {
                        // LD A, R & LD A, I also reset H & N and copy IFF2 to P/V
                        yield return Expression.Call(Xpr.Flags, Xpr.SetResultFlags, Xpr.A);
                        yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(false));
                        yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                        yield return Expression.Assign(Xpr.ParityOverflow, Xpr.IFF2);
                    }
                    break;

                case Opcode.Load16:
                    yield return WriteOperand1(operation, ReadOperand2(operation, true), true);
                    break;

                case Opcode.Push:
                    yield return Xpr.PushSP;
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, ReadOperand1(operation, true));
                    break;

                case Opcode.Pop:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, Xpr.SP), true);
                    yield return Xpr.PopSP;
                    break;

                case Opcode.Add:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, ReadOperand2(operation)));
                    break;

                case Opcode.AddCarry:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, ReadOperand2(operation)));
                    break;

                case Opcode.Subtract:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, ReadOperand2(operation)));
                    break;

                case Opcode.SubtractCarry:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, ReadOperand2(operation)));
                    break;

                case Opcode.And:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, ReadOperand2(operation)));
                    break;

                case Opcode.Or:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, ReadOperand2(operation)));
                    break;

                case Opcode.Xor:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, ReadOperand2(operation)));
                    break;

                case Opcode.Compare:
                    yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, ReadOperand2(operation));
                    break;

                case Opcode.Increment:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluIncrement, ReadOperand1(operation)));
                    break;

                case Opcode.Decrement:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluDecrement, ReadOperand1(operation)));
                    break;

                case Opcode.Add16:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluAdd16, ReadOperand1(operation, true), ReadOperand2(operation, true)), true);
                    break;

                case Opcode.AddCarry16:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, ReadOperand1(operation, true), ReadOperand2(operation, true)), true);
                    break;

                case Opcode.SubtractCarry16:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluSubtract16WithCarry, ReadOperand1(operation, true), ReadOperand2(operation, true)), true);
                    break;

                case Opcode.Increment16:
                    // INC ss (no flags changes so implemented directly)
                    yield return Expression.PreIncrementAssign(ReadOperand1(operation, true));
                    break;

                case Opcode.Decrement16:
                    // DEC ss (no flags changes so implemented directly)
                    yield return Expression.PreDecrementAssign(ReadOperand1(operation, true));
                    break;

                case Opcode.Exchange:
                    this.usesLocalWord = true;
                    yield return Expression.Assign(Xpr.LocalWord, ReadOperand2(operation, true));
                    yield return WriteOperand2(operation, ReadOperand1(operation, true), true);
                    yield return WriteOperand1(operation, Xpr.LocalWord, true);
                    break;

                case Opcode.ExchangeAccumulatorAndFlags:
                    yield return Xpr.SwitchToAlternativeAccumulatorAndFlagsRegisters;
                    break;

                case Opcode.ExchangeGeneralPurpose:
                    yield return Xpr.SwitchToAlternativeGeneralPurposeRegisters;
                    break;

                case Opcode.Jump:
                    if (operation.FlagTest == FlagTest.None)
                    {
                        yield return Expression.Assign(Xpr.PC, ReadOperand1(operation, true));
                    }
                    else
                    {
                        yield return Expression.IfThenElse(GetFlagTestExpression(operation.FlagTest), Expression.Assign(Xpr.PC, ReadOperand1(operation, true)), SyncProgramCounter);
                    }
                    LastDecodeResult = DecodeResult.Finalize;
                    break;

                case Opcode.JumpRelative:
                    if (operation.FlagTest == FlagTest.None)
                    {
                        yield return JumpToDisplacement(operation);
                    }
                    else
                    {
                        this.usesDynamicTimings = true;
                        yield return Expression.IfThen(GetFlagTestExpression(operation.FlagTest), Expression.Block(JumpToDisplacement(operation), GetDynamicTimings(1, 5)));
                    }
                    LastDecodeResult = DecodeResult.FinalizeAndSync;
                    break;

                case Opcode.DecrementJumpRelativeIfNonZero:
                    this.usesDynamicTimings = true;
                    yield return Expression.Assign(Xpr.B, Expression.Convert(Expression.Decrement(Expression.Convert(Xpr.B, typeof(int))), typeof(byte)));
                    yield return Expression.IfThen(Expression.NotEqual(Xpr.B, Expression.Constant((byte)0)), Expression.Block(JumpToDisplacement(operation), GetDynamicTimings(1, 5)));
                    LastDecodeResult = DecodeResult.FinalizeAndSync;
                    break;

                case Opcode.Call:
                    yield return SyncProgramCounter;

                    if (operation.FlagTest == FlagTest.None)
                    {
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, Xpr.PC);
                        yield return Expression.Assign(Xpr.PC, ReadOperand1(operation));
                    }
                    else
                    {
                        this.usesDynamicTimings = true;
                        yield return
                            Expression.IfThen(
                                GetFlagTestExpression(operation.FlagTest),
                                Expression.Block(Xpr.PushSP, Xpr.WritePCToStack, Expression.Assign(Xpr.PC, ReadOperand1(operation)), GetDynamicTimings(2, 7)));
                    }
                    LastDecodeResult = DecodeResult.Finalize;
                    break;

                case Opcode.Return:
                    if (operation.FlagTest == FlagTest.None)
                    {
                        yield return Xpr.ReadPCFromStack;
                        yield return Xpr.PopSP;
                    }
                    else
                    {
                        this.usesDynamicTimings = true;
                        yield return Expression.IfThenElse(GetFlagTestExpression(operation.FlagTest), Expression.Block(Xpr.ReadPCFromStack, Xpr.PopSP, GetDynamicTimings(2, 6)), SyncProgramCounter);
                    }
                    LastDecodeResult = DecodeResult.Finalize;
                    break;

                case Opcode.ReturnFromInterrupt:
                    yield return Xpr.ReadPCFromStack;
                    yield return Xpr.PopSP;
                    LastDecodeResult = DecodeResult.Finalize;
                    break;

                case Opcode.ReturnFromNonmaskableInterrupt:
                    yield return Xpr.ReadPCFromStack;
                    yield return Xpr.PopSP;
                    yield return Expression.Assign(Xpr.IFF1, Xpr.IFF2);
                    LastDecodeResult = DecodeResult.Finalize;
                    break;

                case Opcode.Reset:
                    yield return SyncProgramCounter;
                    yield return Xpr.PushSP;
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, Xpr.PC);
                    yield return Expression.Assign(Xpr.PC, ReadOperand1(operation, true));
                    LastDecodeResult = DecodeResult.Finalize;
                    break;

                case Opcode.Input:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.IO, Xpr.IoReadByte, ReadOperand2(operation), operation.Operand2 == Operand.n ? Xpr.A : Xpr.B));
                    break;

                case Opcode.Output:
                    yield return Expression.Call(Xpr.IO, Xpr.IoWriteByte, ReadOperand2(operation), operation.Operand2 == Operand.n ? Xpr.A : Xpr.B, ReadOperand1(operation));
                    break;

                case Opcode.RotateLeftWithCarry:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftWithCarry, ReadOperand1(operation)));
                    break;

                case Opcode.RotateLeft:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluRotateLeft, ReadOperand1(operation)));
                    break;

                case Opcode.RotateRightWithCarry:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluRotateRightWithCarry, ReadOperand1(operation)));
                    break;

                case Opcode.RotateRight:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluRotateRight, ReadOperand1(operation)));
                    break;

                case Opcode.RotateLeftDigit:
                    this.usesAccumulatorAndResult = true;
                    yield return Expression.Assign(Xpr.AccumulatorAndResult, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftDigit, Xpr.A, Xpr.ReadByteAtHL));
                    yield return Expression.Assign(Xpr.A, Xpr.AccumulatorAndResult_Accumulator);
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Xpr.AccumulatorAndResult_Result);
                    break;

                case Opcode.RotateRightDigit:
                    this.usesAccumulatorAndResult = true;
                    yield return Expression.Assign(Xpr.AccumulatorAndResult, Expression.Call(Xpr.Alu, Xpr.AluRotateRightDigit, Xpr.A, Xpr.ReadByteAtHL));
                    yield return Expression.Assign(Xpr.A, Xpr.AccumulatorAndResult_Accumulator);
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Xpr.AccumulatorAndResult_Result);
                    break;

                case Opcode.ShiftLeft:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluShiftLeft, ReadOperand1(operation)));
                    break;

                case Opcode.ShiftLeftSet:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluShiftLeftSet, ReadOperand1(operation)));
                    break;

                case Opcode.ShiftRight:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluShiftRight, ReadOperand1(operation)));
                    break;

                case Opcode.ShiftRightLogical:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluShiftRightLogical, ReadOperand1(operation)));
                    break;

                case Opcode.BitTest:
                    yield return Expression.Call(Xpr.Alu, Xpr.AluBitTest, ReadOperand1(operation), Expression.Constant((int)operation.ByteLiteral));
                    break;

                case Opcode.BitSet:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluBitSet, ReadOperand1(operation), Expression.Constant((int)operation.ByteLiteral)));
                    break;

                case Opcode.BitReset:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluBitReset, ReadOperand1(operation), Expression.Constant((int)operation.ByteLiteral)));
                    break;

                case Opcode.TransferIncrement:
                    yield return Expression.Block(GetLdExpressions());
                    break;

                case Opcode.TransferIncrementRepeat:
                    this.usesDynamicTimings = true;
                    yield return Expression.Block(GetLdrExpressions());
                    break;

                case Opcode.TransferDecrement:
                    yield return Expression.Block(GetLdExpressions(true));
                    break;

                case Opcode.TransferDecrementRepeat:
                    this.usesDynamicTimings = true;
                    yield return Expression.Block(GetLdrExpressions(true));
                    break;

                case Opcode.SearchIncrement:
                    yield return Expression.Block(GetCpExpressions());
                    break;

                case Opcode.SearchIncrementRepeat:
                    this.usesDynamicTimings = true;
                    yield return GetCprExpression();
                    break;

                case Opcode.SearchDecrement:
                    yield return Expression.Block(GetCpExpressions(true));
                    break;

                case Opcode.SearchDecrementRepeat:
                    this.usesDynamicTimings = true;
                    yield return GetCprExpression(true);
                    break;

                case Opcode.InputTransferIncrement:
                    yield return Expression.Block(GetInExpressions());
                    break;

                case Opcode.InputTransferIncrementRepeat:
                    this.usesDynamicTimings = true;
                    yield return GetInrExpression();
                    break;

                case Opcode.InputTransferDecrement:
                    yield return Expression.Block(GetInExpressions(true));
                    break;

                case Opcode.InputTransferDecrementRepeat:
                    this.usesDynamicTimings = true;
                    yield return GetInrExpression(true);
                    break;

                case Opcode.OutputTransferIncrement:
                    yield return Expression.Block(GetOutExpressions());
                    break;

                case Opcode.OutputTransferIncrementRepeat:
                    this.usesDynamicTimings = true;
                    yield return GetOutrExpression();
                    break;

                case Opcode.OutputTransferDecrement:
                    yield return Expression.Block(GetOutExpressions(true));
                    break;

                case Opcode.OutputTransferDecrementRepeat:
                    this.usesDynamicTimings = true;
                    yield return GetOutrExpression(true);
                    break;

                case Opcode.DecimalArithmeticAdjust:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecimalAdjust, Xpr.A));
                    break;

                case Opcode.NegateOnesComplement:
                    yield return Expression.Assign(Xpr.A, Expression.Not(Xpr.A));
                    yield return Expression.Call(Xpr.Flags, Xpr.SetUndocumentedFlags, Xpr.A);
                    yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(true));
                    yield return Expression.Assign(Xpr.Subtract, Expression.Constant(true));
                    break;

                case Opcode.NegateTwosComplement:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Expression.Constant((byte)0), Xpr.A));
                    break;

                case Opcode.InvertCarryFlag:
                    yield return Expression.Assign(Xpr.HalfCarry, Xpr.Carry);
                    yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.Carry, Expression.Not(Xpr.Carry));
                    break;

                case Opcode.SetCarryFlag:
                    yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.Carry, Expression.Constant(true));
                    break;

                case Opcode.DisableInterrupts:
                    yield return Expression.Assign(Xpr.IFF1, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.IFF2, Expression.Constant(false));
                    break;

                case Opcode.EnableInterrupts:
                    yield return Expression.Assign(Xpr.IFF1, Expression.Constant(true));
                    yield return Expression.Assign(Xpr.IFF2, Expression.Constant(true));
                    break;

                case Opcode.InterruptMode0:
                    yield return Expression.Assign(Xpr.IM, Expression.Constant(InterruptMode.InterruptMode0));
                    break;

                case Opcode.InterruptMode1:
                    yield return Expression.Assign(Xpr.IM, Expression.Constant(InterruptMode.InterruptMode1));
                    break;

                case Opcode.InterruptMode2:
                    yield return Expression.Assign(Xpr.IM, Expression.Constant(InterruptMode.InterruptMode2));
                    break;

                case Opcode.Swap:
                    yield return WriteOperand1(operation, Expression.Call(Xpr.Alu, Xpr.AluSwap, ReadOperand1(operation)));
                    break;

                case Opcode.LoadIncrement:
                    yield return WriteOperand1(operation, ReadOperand2(operation));
                    yield return Expression.PreIncrementAssign(Xpr.HL); // No support for indexes but GB doesnt have them
                    break;

                case Opcode.LoadDecrement:
                    yield return WriteOperand1(operation, ReadOperand2(operation));
                    yield return Expression.PreDecrementAssign(Xpr.HL); // No support for indexes but GB doesnt have them
                    break;

                case Opcode.Stop:
                    LastDecodeResult = DecodeResult.Stop;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (operation.OpCodeMeta.HasFlag(OpCodeMeta.AutoCopy))
            {
                // Autocopy for DD/FD prefix
                yield return WriteOperand2(operation, ReadOperand1(operation));
            }
        }
    }
}