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
                    yield break;

                case Opcode.Load:
                    if (operation.Operand1 == operation.Operand2)
                    {
                        break;
                    }

                    yield return Write(operation, Read(operation));

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
                    yield return Write(operation, Read(operation, true), true);
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
                    yield return Write(operation, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, Read(operation)));
                    break;
                case Opcode.AddCarry:
                    yield return Write(operation, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, Read(operation)));
                    break;
                case Opcode.Subtract:
                    yield return Write(operation, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, Read(operation)));
                    break;
                case Opcode.SubtractCarry:
                    yield return Write(operation, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, Read(operation)));
                    break;
                case Opcode.And:
                    yield return Write(operation, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, Read(operation)));
                    break;
                case Opcode.Or:
                    yield return Write(operation, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, Read(operation)));
                    break;
                case Opcode.Xor:
                    yield return Write(operation, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, Read(operation)));
                    break;
                case Opcode.Compare:
                    yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, Read(operation));
                    break;
                case Opcode.Increment:
                    yield return Write(operation, Expression.Call(Xpr.Alu, Xpr.AluIncrement, Read(operation)));
                    break;
                case Opcode.Decrement:
                    yield return Write(operation, Expression.Call(Xpr.Alu, Xpr.AluDecrement, Read(operation)));
                    break;
                case Opcode.Add16:
                    break;
                case Opcode.AddCarry16:
                    break;
                case Opcode.SubtractCarry16:
                    break;
                case Opcode.Increment16:
                    break;
                case Opcode.Decrement16:
                    break;
                case Opcode.Exchange:
                    break;
                case Opcode.ExchangeAccumulatorAndFlags:
                    break;
                case Opcode.ExchangeGeneralPurpose:
                    break;
                case Opcode.Jump:
                    break;
                case Opcode.JumpRelative:
                    break;
                case Opcode.DecrementJumpRelativeIfNonZero:
                    break;
                case Opcode.Call:
                    break;
                case Opcode.Return:
                    break;
                case Opcode.ReturnFromInterrupt:
                    break;
                case Opcode.ReturnFromNonmaskableInterrupt:
                    break;
                case Opcode.Input:
                    break;
                case Opcode.Output:
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
