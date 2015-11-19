namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Core.Decode;
    using Axh.Retro.CPU.Z80.Util;

    public partial class DynaRec<TRegisters> where TRegisters : IRegisters
    {
        private Expression ReadOperand1(Operation operation, bool is16Bit = false)
        {
            return ReadOperand(operation, operation.Operand1, is16Bit);
        }

        private Expression ReadOperand2(Operation operation, bool is16Bit = false)
        {
            return ReadOperand(operation, operation.Operand2, is16Bit);
        }

        private Expression ReadOperand(Operation operation, Operand operand, bool is16Bit)
        {
            switch (operand)
            {
                case Operand.A:
                    return A;
                case Operand.B:
                    return B;
                case Operand.C:
                    return C;
                case Operand.D:
                    return D;
                case Operand.E:
                    return E;
                case Operand.F:
                    return F;
                case Operand.H:
                    return H;
                case Operand.L:
                    return L;
                case Operand.HL:
                    return HL;
                case Operand.BC:
                    return BC;
                case Operand.DE:
                    return DE;
                case Operand.AF:
                    return AF;
                case Operand.SP:
                    return SP;
                case Operand.mHL:
                    return Expression.Call(Mmu, is16Bit ? MmuReadWord : MmuReadByte, HL);
                case Operand.mBC:
                    return Expression.Call(Mmu, is16Bit ? MmuReadWord : MmuReadByte, BC);
                case Operand.mDE:
                    return Expression.Call(Mmu, is16Bit ? MmuReadWord : MmuReadByte, DE);
                case Operand.mSP:
                    return Expression.Call(Mmu, is16Bit ? MmuReadWord : MmuReadByte, SP);
                case Operand.mnn:
                    return Expression.Call(Mmu, is16Bit ? MmuReadWord : MmuReadByte, Expression.Constant(operation.WordLiteral));
                case Operand.nn:
                    return Expression.Constant(operation.WordLiteral);
                case Operand.n:
                    return Expression.Constant(operation.ByteLiteral);
                case Operand.d:
                    return Expression.Convert(Expression.Constant((sbyte)operation.ByteLiteral), typeof(int));
                case Operand.IX:
                    return IX;
                case Operand.mIXd:
                    return Expression.Call(
                        Mmu,
                        is16Bit ? MmuReadWord : MmuReadByte,
                        Expression.Convert(Expression.Add(Expression.Convert(IX, typeof(int)), Expression.Constant((int)operation.Displacement)), typeof(ushort)));
                case Operand.IXl:
                    return IXl;
                case Operand.IXh:
                    return IXh;
                case Operand.IY:
                    return IY;
                case Operand.mIYd:
                    return Expression.Call(
                        Mmu,
                        is16Bit ? MmuReadWord : MmuReadByte,
                        Expression.Convert(Expression.Add(Expression.Convert(IY, typeof(int)), Expression.Constant((int)operation.Displacement)), typeof(ushort)));
                case Operand.IYl:
                    return IYl;
                case Operand.IYh:
                    return IYh;
                case Operand.I:
                    return I;
                case Operand.R:
                    return R;
                case Operand.mCl:
                    return Expression.Call(Mmu, is16Bit ? MmuReadWord : MmuReadByte, Expression.Add(Expression.Convert(C, typeof(ushort)), Expression.Constant((ushort)0xff00)));
                case Operand.mnl:
                    return Expression.Call(Mmu, is16Bit ? MmuReadWord : MmuReadByte, Expression.Constant((ushort)(operation.ByteLiteral + 0xff00)));
                case Operand.SPd:
                    return Expression.Call(Alu, AluAddDisplacement, SP, Expression.Constant((sbyte)operation.ByteLiteral));
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation.Operand2), operation.Operand2, null);
            }
        }

        private Expression WriteOperand1(Operation operation, Expression value, bool is16Bit = false)
        {
            return WriteOperand(operation, value, operation.Operand1, is16Bit);
        }

        private Expression WriteOperand2(Operation operation, Expression value, bool is16Bit = false)
        {
            return WriteOperand(operation, value, operation.Operand2, is16Bit);
        }

        private Expression WriteOperand(Operation operation, Expression value, Operand operand, bool is16Bit)
        {
            switch (operand)
            {
                case Operand.A:
                    return Expression.Assign(A, value);
                case Operand.B:
                    return Expression.Assign(B, value);
                case Operand.C:
                    return Expression.Assign(C, value);
                case Operand.D:
                    return Expression.Assign(D, value);
                case Operand.E:
                    return Expression.Assign(E, value);
                case Operand.F:
                    return Expression.Assign(F, value);
                case Operand.H:
                    return Expression.Assign(H, value);
                case Operand.L:
                    return Expression.Assign(L, value);
                case Operand.HL:
                    return Expression.Assign(HL, value);
                case Operand.BC:
                    return Expression.Assign(BC, value);
                case Operand.DE:
                    return Expression.Assign(DE, value);
                case Operand.SP:
                    return Expression.Assign(SP, value);
                case Operand.AF:
                    return Expression.Assign(AF, value);
                case Operand.mHL:
                    return Expression.Call(Mmu, is16Bit ? MmuWriteWord : MmuWriteByte, HL, value);
                case Operand.mBC:
                    return Expression.Call(Mmu, is16Bit ? MmuWriteWord : MmuWriteByte, BC, value);
                case Operand.mDE:
                    return Expression.Call(Mmu, is16Bit ? MmuWriteWord : MmuWriteByte, DE, value);
                case Operand.mSP:
                    return Expression.Call(Mmu, is16Bit ? MmuWriteWord : MmuWriteByte, SP, value);
                case Operand.mnn:
                    return Expression.Call(Mmu, is16Bit ? MmuWriteWord : MmuWriteByte, Expression.Constant(operation.WordLiteral), value);
                case Operand.IX:
                    return Expression.Assign(IX, value);
                case Operand.mIXd:
                    return Expression.Call(
                        Mmu,
                        is16Bit ? MmuWriteWord : MmuWriteByte,
                        Expression.Convert(Expression.Add(Expression.Convert(IX, typeof(int)), Expression.Constant((int)operation.Displacement)), typeof(ushort)),
                        value);
                case Operand.IXl:
                    return Expression.Assign(IXl, value);
                case Operand.IXh:
                    return Expression.Assign(IXh, value);
                case Operand.IY:
                    return Expression.Assign(IY, value);
                case Operand.mIYd:
                    return Expression.Call(
                        Mmu,
                        is16Bit ? MmuWriteWord : MmuWriteByte,
                        Expression.Convert(Expression.Add(Expression.Convert(IY, typeof(int)), Expression.Constant((int)operation.Displacement)), typeof(ushort)),
                        value);
                case Operand.IYl:
                    return Expression.Assign(IYl, value);
                case Operand.IYh:
                    return Expression.Assign(IYh, value);
                case Operand.I:
                    return Expression.Assign(I, value);
                case Operand.R:
                    return Expression.Assign(R, value);
                case Operand.mCl:
                    return Expression.Call(Mmu, is16Bit ? MmuWriteWord : MmuWriteByte, Expression.Add(Expression.Convert(C, typeof(ushort)), Expression.Constant((ushort)0xff00)), value);
                case Operand.mnl:
                    return Expression.Call(Mmu, is16Bit ? MmuWriteWord : MmuWriteByte, Expression.Constant((ushort)(operation.ByteLiteral + 0xff00)), value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation.Operand1), operation.Operand1, null);
            }
        }

        private Expression GetFlagTestExpression(FlagTest flagTest)
        {
            switch (flagTest)
            {
                case FlagTest.NotZero:
                    return Expression.Not(Zero);
                case FlagTest.Zero:
                    return Zero;
                case FlagTest.NotCarry:
                    return Expression.Not(Carry);
                case FlagTest.Carry:
                    return Carry;
                case FlagTest.ParityOdd:
                    return Expression.Not(ParityOverflow);
                case FlagTest.ParityEven:
                    return ParityOverflow;
                case FlagTest.Possitive:
                    return Expression.Not(Sign);
                case FlagTest.Negative:
                    return Sign;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flagTest), flagTest, null);
            }
        }

        private Expression GetDynamicTimings(int mCycles, int tStates)
        {
            return Expression.Call(DynamicTimer, DynamicTimerAdd, Expression.Constant(mCycles), Expression.Constant(tStates));
        }

        private Expression GetMemoryRefreshDeltaExpression(Expression deltaExpression)
        {
            var increment7LsbR = Expression.And(Expression.Add(Expression.Convert(R, typeof(int)), deltaExpression), Expression.Constant(0x7f));
            return Expression.Assign(R, Expression.Convert(increment7LsbR, typeof(byte)));
        }

        private Expression JumpToDisplacement(Operation operation)
        {
            return Expression.Assign(PC, Expression.Convert(Expression.Add(Expression.Convert(PC, typeof(int)), ReadOperand1(operation, true)), typeof(ushort)));
        }

        private IEnumerable<Expression> GetLdExpressions(bool decrement = false)
        {
            yield return Expression.Call(Mmu, MmuTransferByte, HL, DE);
            yield return decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL);
            yield return decrement ? Expression.PreDecrementAssign(DE) : Expression.PreIncrementAssign(DE);
            yield return Expression.PreDecrementAssign(BC);
            yield return Expression.Assign(HalfCarry, Expression.Constant(false));
            yield return Expression.Assign(ParityOverflow, Expression.NotEqual(BC, Expression.Constant((ushort)0)));
            yield return Expression.Assign(Subtract, Expression.Constant(false));
        }

        private IEnumerable<Expression> GetLdrExpressions(bool decrement = false)
        {
            var breakLabel = Expression.Label();
            yield return
                Expression.Loop(
                    Expression.Block(
                        Expression.Call(Mmu, MmuTransferByte, HL, DE),
                        decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL),
                        decrement ? Expression.PreDecrementAssign(DE) : Expression.PreIncrementAssign(DE),
                        Expression.PreDecrementAssign(BC),
                        Expression.IfThen(Expression.Equal(BC, Expression.Constant((ushort)0)), Expression.Break(breakLabel)),
                        GetDynamicTimings(5, 21),
                        GetMemoryRefreshDeltaExpression(Expression.Constant(2))), // This function actually decreases the PC by two for each 'loop' hence need more refresh cycles.
                    breakLabel);

            yield return Expression.Assign(HalfCarry, Expression.Constant(false));
            yield return Expression.Assign(ParityOverflow, Expression.Constant(false));
            yield return Expression.Assign(Subtract, Expression.Constant(false));
        }

        private IEnumerable<Expression> GetCpExpressions(bool decrement = false)
        {
            yield return Expression.Call(Alu, AluCompare, A, Expression.Call(Mmu, MmuReadByte, HL));
            yield return decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL);
            yield return Expression.PreDecrementAssign(BC);
        }

        private Expression GetCprExpression(bool decrement = false)
        {
            var breakLabel = Expression.Label();
            var expressions = GetCpExpressions(decrement);
            var iterationExpressions = new[]
                                       {
                                           Expression.IfThen(Expression.OrElse(Expression.Equal(BC, Expression.Constant((ushort)0)), Zero), Expression.Break(breakLabel)), GetDynamicTimings(5, 21),
                                           GetMemoryRefreshDeltaExpression(Expression.Constant(2))
                                       };
            return Expression.Loop(Expression.Block(expressions.Concat(iterationExpressions).ToArray()), breakLabel);
        }

        private IEnumerable<Expression> GetInExpressions(bool decrement = false)
        {
            yield return Expression.Call(Mmu, MmuWriteByte, HL, Expression.Call(IO, IoReadByte, C, B));
            yield return decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL);
            yield return Expression.Assign(B, Expression.Convert(Expression.Subtract(Expression.Convert(B, typeof(int)), Expression.Constant(1)), typeof(byte)));
            yield return Expression.Assign(Subtract, Expression.Constant(true));
            yield return Expression.Call(Flags, SetResultFlags, B);
        }

        private Expression GetInrExpression(bool decrement = false)
        {
            var breakLabel = Expression.Label();

            var expressions = GetInExpressions(decrement);
            var iterationExpressions = new[]
                                       {
                                           Expression.IfThen(Expression.Equal(B, Expression.Constant((byte)0)), Expression.Break(breakLabel)), GetDynamicTimings(5, 21),
                                           GetMemoryRefreshDeltaExpression(Expression.Constant(2))
                                       };

            return Expression.Loop(Expression.Block(expressions.Concat(iterationExpressions).ToArray()), breakLabel);
        }

        private IEnumerable<Expression> GetOutExpressions(bool decrement = false)
        {
            yield return Expression.Call(IO, IoWriteByte, C, B, ReadByteAtHL);
            yield return decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL);
            yield return Expression.Assign(B, Expression.Convert(Expression.Subtract(Expression.Convert(B, typeof(int)), Expression.Constant(1)), typeof(byte)));
            yield return Expression.Assign(Subtract, Expression.Constant(true));
            yield return Expression.Call(Flags, SetResultFlags, B);
        }

        private Expression GetOutrExpression(bool decrement = false)
        {
            var breakLabel = Expression.Label();

            var expressions = GetOutExpressions(decrement);
            var iterationExpressions = new[]
                                       {
                                           Expression.IfThen(Expression.Equal(B, Expression.Constant((byte)0)), Expression.Break(breakLabel)), GetDynamicTimings(5, 21),
                                           GetMemoryRefreshDeltaExpression(Expression.Constant(2))
                                       };

            return Expression.Loop(Expression.Block(expressions.Concat(iterationExpressions).ToArray()), breakLabel);
        }

        private static Expression GetDebugExpression(string text)
        {
            var document = Expression.SymbolDocument(text);
            return Expression.DebugInfo(document, DebugViewWriter.OperationDebugOperationStartLine, 1, DebugViewWriter.OperationDebugOperationStartLine + 1, 1);
        }
    }
}
