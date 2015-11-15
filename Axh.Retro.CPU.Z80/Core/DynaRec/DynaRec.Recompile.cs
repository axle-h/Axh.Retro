namespace Axh.Retro.CPU.Z80.Core.DynaRec
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
                    {
                        var pair = ToHighLowExpressionPair(operation.Operand1);
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, pair.High);
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, pair.Low);
                    }
                    break;

                case Opcode.Pop:
                    {
                        var pair = ToHighLowExpressionPair(operation.Operand1);
                        yield return Expression.Assign(pair.Low, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        yield return Expression.Assign(pair.High, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                    }
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
                        yield return Expression.IfThen(GetFlagTestExpression(operation.FlagTest), Expression.Block(JumpToDisplacement(operation), Xpr.GetDynamicTimings(1, 5)));
                    }
                    LastDecodeResult = DecodeResult.FinalizeAndSync;
                    break;

                case Opcode.DecrementJumpRelativeIfNonZero:
                    this.usesDynamicTimings = true;
                    yield return Expression.Assign(Xpr.B, Expression.Convert(Expression.Decrement(Expression.Convert(Xpr.B, typeof(int))), typeof(byte)));
                    yield return Expression.IfThen(Expression.NotEqual(Xpr.B, Expression.Constant((byte)0)), Expression.Block(JumpToDisplacement(operation), Xpr.GetDynamicTimings(1, 5)));
                    LastDecodeResult = DecodeResult.FinalizeAndSync;
                    break;

                case Opcode.Call:
                    yield return SyncProgramCounter;

                    if (operation.FlagTest == FlagTest.None)
                    {
                        yield return Xpr.PushPushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, Xpr.SP, Xpr.PC);
                        yield return Expression.Assign(Xpr.PC, ReadOperand1(operation));
                    }
                    else
                    {
                        this.usesDynamicTimings = true;
                        yield return
                            Expression.IfThen(
                                GetFlagTestExpression(operation.FlagTest),
                                Expression.Block(Xpr.PushPushSP, Xpr.WritePCToStack, Expression.Assign(Xpr.PC, ReadOperand1(operation)), Xpr.GetDynamicTimings(2, 7)));
                    }
                    LastDecodeResult = DecodeResult.Finalize;
                    break;

                case Opcode.Return:
                    if (operation.FlagTest == FlagTest.None)
                    {
                        yield return Xpr.ReadPCFromStack;
                        yield return Xpr.PopPopSP;
                    }
                    else
                    {
                        this.usesDynamicTimings = true;
                        yield return Expression.IfThenElse(GetFlagTestExpression(operation.FlagTest), Expression.Block(Xpr.ReadPCFromStack, Xpr.PopPopSP, Xpr.GetDynamicTimings(2, 6)), SyncProgramCounter);
                    }
                    LastDecodeResult = DecodeResult.Finalize;
                    break;

                case Opcode.ReturnFromInterrupt:
                    yield return Xpr.ReadPCFromStack;
                    yield return Xpr.PopPopSP;
                    LastDecodeResult = DecodeResult.Finalize;
                    break;

                case Opcode.ReturnFromNonmaskableInterrupt:
                    yield return Xpr.ReadPCFromStack;
                    yield return Xpr.PopPopSP;
                    yield return Expression.Assign(Xpr.IFF1, Xpr.IFF2);
                    LastDecodeResult = DecodeResult.Finalize;
                    break;

                case Opcode.Reset:
                    yield return SyncProgramCounter;
                    yield return Xpr.PushPushSP;
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
                    break;
                case Opcode.RotateLeft:
                    break;
                case Opcode.RotateRightWithCarry:
                    break;
                case Opcode.RotateRight:
                    break;
                case Opcode.RotateLeftDigit:
                    break;
                case Opcode.RotateRightDigit:
                    break;
                case Opcode.ShiftLeft:
                    break;
                case Opcode.ShiftLeftSet:
                    break;
                case Opcode.ShiftRight:
                    break;
                case Opcode.ShiftRightLogical:
                    break;
                case Opcode.BitTest:
                    break;
                case Opcode.BitSet:
                    break;
                case Opcode.BitReset:
                    break;
                case Opcode.TransferIncrement:
                    break;
                case Opcode.TransferIncrementRepeat:
                    break;
                case Opcode.TransferDecrement:
                    break;
                case Opcode.TransferDecrementRepeat:
                    break;
                case Opcode.SearchIncrement:
                    break;
                case Opcode.SearchIncrementRepeat:
                    break;
                case Opcode.SearchDecrement:
                    break;
                case Opcode.SearchDecrementRepeat:
                    break;
                case Opcode.InputTransferIncrement:
                    break;
                case Opcode.InputTransferIncrementRepeat:
                    break;
                case Opcode.InputTransferDecrement:
                    break;
                case Opcode.InputTransferDecrementRepeat:
                    break;
                case Opcode.OutputTransferIncrement:
                    break;
                case Opcode.OutputTransferIncrementRepeat:
                    break;
                case Opcode.OutputTransferDecrement:
                    break;
                case Opcode.OutputTransferDecrementRepeat:
                    break;
                case Opcode.DecimalArithmeticAdjust:
                    break;
                case Opcode.NegateOnesComplement:
                    break;
                case Opcode.NegateTwosComplement:
                    break;
                case Opcode.InvertCarryFlag:
                    break;
                case Opcode.SetCarryFlag:
                    break;
                case Opcode.DisableInterrupts:
                    break;
                case Opcode.EnableInterrupts:
                    break;
                case Opcode.InterruptMode0:
                    break;
                case Opcode.InterruptMode1:
                    break;
                case Opcode.InterruptMode2:
                    break;
                case Opcode.Swap:
                    break;
                case Opcode.LoadIncrement:
                    break;
                case Opcode.LoadDecrement:
                    break;
                case Opcode.Stop:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
