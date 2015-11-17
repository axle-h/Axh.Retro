namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Core.Decode;

    internal partial class DynaRec<TRegisters> where TRegisters : IRegisters
    {
        private struct HighLowExpressionPair
        {
            public HighLowExpressionPair(Expression high, Expression low) : this()
            {
                High = high;
                Low = low;
            }

            public Expression High { get; }
            public Expression Low { get; }
        }
        
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
                    return Xpr.A;
                case Operand.B:
                    return Xpr.B;
                case Operand.C:
                    return Xpr.C;
                case Operand.D:
                    return Xpr.D;
                case Operand.E:
                    return Xpr.E;
                case Operand.F:
                    return Xpr.F;
                case Operand.H:
                    return Xpr.H;
                case Operand.L:
                    return Xpr.L;
                case Operand.HL:
                    return Xpr.HL;
                case Operand.BC:
                    return Xpr.BC;
                case Operand.DE:
                    return Xpr.DE;
                case Operand.SP:
                    return Xpr.SP;
                case Operand.mHL:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuReadWord : Xpr.MmuReadByte, Xpr.HL);
                case Operand.mBC:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuReadWord : Xpr.MmuReadByte, Xpr.BC);
                case Operand.mDE:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuReadWord : Xpr.MmuReadByte, Xpr.DE);
                case Operand.mSP:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuReadWord : Xpr.MmuReadByte, Xpr.SP);
                case Operand.mnn:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuReadWord : Xpr.MmuReadByte, Expression.Constant(operation.WordLiteral));
                case Operand.nn:
                    return Expression.Constant(operation.WordLiteral);
                case Operand.n:
                    return Expression.Constant(operation.ByteLiteral);
                case Operand.d:
                    return Expression.Convert(Expression.Constant((sbyte)operation.ByteLiteral), typeof(int));
                case Operand.IX:
                    return Xpr.IX;
                case Operand.mIXd:
                    return Expression.Call(
                        Xpr.Mmu,
                        is16Bit ? Xpr.MmuReadWord : Xpr.MmuReadByte,
                        Expression.Convert(Expression.Add(Expression.Convert(Xpr.IX, typeof(int)), Expression.Constant((int)operation.Displacement)), typeof(ushort)));
                case Operand.IXl:
                    return Xpr.IXl;
                case Operand.IXh:
                    return Xpr.IXh;
                case Operand.IY:
                    return Xpr.IY;
                case Operand.mIYd:
                    return Expression.Call(
                        Xpr.Mmu,
                        is16Bit ? Xpr.MmuReadWord : Xpr.MmuReadByte,
                        Expression.Convert(Expression.Add(Expression.Convert(Xpr.IY, typeof(int)), Expression.Constant((int)operation.Displacement)), typeof(ushort)));
                case Operand.IYl:
                    return Xpr.IYl;
                case Operand.IYh:
                    return Xpr.IYh;
                case Operand.I:
                    return Xpr.I;
                case Operand.R:
                    return Xpr.R;
                case Operand.mCl:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuReadWord : Xpr.MmuReadByte, Expression.Add(Expression.Convert(Xpr.C, typeof(ushort)), Expression.Constant((ushort)0xff00)));
                case Operand.mnl:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuReadWord : Xpr.MmuReadByte, Expression.Constant((ushort)(operation.ByteLiteral + 0xff00)));
                case Operand.SPd:
                    return Expression.Call(Xpr.Alu, Xpr.AluAddDisplacement, Xpr.SP, Expression.Constant((sbyte)operation.ByteLiteral));
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
                    return Expression.Assign(Xpr.A, value);
                case Operand.B:
                    return Expression.Assign(Xpr.B, value);
                case Operand.C:
                    return Expression.Assign(Xpr.C, value);
                case Operand.D:
                    return Expression.Assign(Xpr.D, value);
                case Operand.E:
                    return Expression.Assign(Xpr.E, value);
                case Operand.F:
                    return Expression.Assign(Xpr.F, value);
                case Operand.H:
                    return Expression.Assign(Xpr.H, value);
                case Operand.L:
                    return Expression.Assign(Xpr.L, value);
                case Operand.HL:
                    return Expression.Assign(Xpr.HL, value);
                case Operand.BC:
                    return Expression.Assign(Xpr.BC, value);
                case Operand.DE:
                    return Expression.Assign(Xpr.DE, value);
                case Operand.SP:
                    return Expression.Assign(Xpr.SP, value);
                case Operand.mHL:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuWriteWord : Xpr.MmuWriteByte, Xpr.HL, value);
                case Operand.mBC:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuWriteWord : Xpr.MmuWriteByte, Xpr.BC, value);
                case Operand.mDE:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuWriteWord : Xpr.MmuWriteByte, Xpr.DE, value);
                case Operand.mSP:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuWriteWord : Xpr.MmuWriteByte, Xpr.SP, value);
                case Operand.mnn:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuWriteWord : Xpr.MmuWriteByte, Expression.Constant(operation.WordLiteral), value);
                case Operand.IX:
                    return Expression.Assign(Xpr.IX, value);
                case Operand.mIXd:
                    return Expression.Call(
                        Xpr.Mmu,
                        is16Bit ? Xpr.MmuWriteWord : Xpr.MmuWriteByte,
                        Expression.Convert(Expression.Add(Expression.Convert(Xpr.IX, typeof(int)), Expression.Constant((int)operation.Displacement)), typeof(ushort)),
                        value);
                case Operand.IXl:
                    return Expression.Assign(Xpr.IXl, value);
                case Operand.IXh:
                    return Expression.Assign(Xpr.IXh, value);
                case Operand.IY:
                    return Expression.Assign(Xpr.IY, value);
                case Operand.mIYd:
                    return Expression.Call(
                        Xpr.Mmu,
                        is16Bit ? Xpr.MmuWriteWord : Xpr.MmuWriteByte,
                        Expression.Convert(Expression.Add(Expression.Convert(Xpr.IY, typeof(int)), Expression.Constant((int)operation.Displacement)), typeof(ushort)),
                        value);
                case Operand.IYl:
                    return Expression.Assign(Xpr.IYl, value);
                case Operand.IYh:
                    return Expression.Assign(Xpr.IYh, value);
                case Operand.I:
                    return Expression.Assign(Xpr.I, value);
                case Operand.R:
                    return Expression.Assign(Xpr.R, value);
                case Operand.mCl:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuWriteWord : Xpr.MmuWriteByte, Expression.Add(Expression.Convert(Xpr.C, typeof(ushort)), Expression.Constant((ushort)0xff00)), value);
                case Operand.mnl:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuWriteWord : Xpr.MmuWriteByte, Expression.Constant((ushort)(operation.ByteLiteral + 0xff00)), value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation.Operand1), operation.Operand1, null);
            }
        }

        private Expression GetFlagTestExpression(FlagTest flagTest)
        {
            switch (flagTest)
            {
                case FlagTest.NotZero:
                    return Expression.Not(Xpr.Zero);
                case FlagTest.Zero:
                    return Xpr.Zero;
                case FlagTest.NotCarry:
                    return Expression.Not(Xpr.Carry);
                case FlagTest.Carry:
                    return Xpr.Carry;
                case FlagTest.ParityOdd:
                    return Expression.Not(Xpr.ParityOverflow);
                case FlagTest.ParityEven:
                    return Xpr.ParityOverflow;
                case FlagTest.Possitive:
                    return Expression.Not(Xpr.Sign);
                case FlagTest.Negative:
                    return Xpr.Sign;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flagTest), flagTest, null);
            }
        }

        public Expression GetDynamicTimings(int mCycles, int tStates)
        {
            return Expression.Call(Xpr.DynamicTimer, Xpr.DynamicTimerAdd, Expression.Constant(mCycles), Expression.Constant(tStates));
        }

        public Expression GetMemoryRefreshDeltaExpression(Expression deltaExpression)
        {
            var increment7LsbR = Expression.And(Expression.Add(Expression.Convert(Xpr.R, typeof(int)), deltaExpression), Expression.Constant(0x7f));
            return Expression.Assign(Xpr.R, Expression.Convert(increment7LsbR, typeof(byte)));
        }

        private Expression JumpToDisplacement(Operation operation)
        {
            return Expression.Assign(Xpr.PC, Expression.Convert(Expression.Add(Expression.Convert(Xpr.PC, typeof(int)), ReadOperand1(operation, true)), typeof(ushort)));
        }

        private IEnumerable<Expression> GetLdExpressions(bool decrement = false)
        {
            yield return Expression.Call(Xpr.Mmu, Xpr.MmuTransferByte, Xpr.HL, Xpr.DE);
            yield return decrement ? Expression.PreDecrementAssign(Xpr.HL) : Expression.PreIncrementAssign(Xpr.HL);
            yield return decrement ? Expression.PreDecrementAssign(Xpr.DE) : Expression.PreIncrementAssign(Xpr.DE);
            yield return Expression.PreDecrementAssign(Xpr.BC);
            yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(false));
            yield return Expression.Assign(Xpr.ParityOverflow, Expression.NotEqual(Xpr.BC, Expression.Constant((ushort)0)));
            yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
        }

        private IEnumerable<Expression> GetLdrExpressions(bool decrement = false)
        {
            var breakLabel = Expression.Label();
            yield return
                Expression.Loop(
                    Expression.Block(
                        Expression.Call(Xpr.Mmu, Xpr.MmuTransferByte, Xpr.HL, Xpr.DE),
                        decrement ? Expression.PreDecrementAssign(Xpr.HL) : Expression.PreIncrementAssign(Xpr.HL),
                        decrement ? Expression.PreDecrementAssign(Xpr.DE) : Expression.PreIncrementAssign(Xpr.DE),
                        Expression.PreDecrementAssign(Xpr.BC),
                        Expression.IfThen(Expression.Equal(Xpr.BC, Expression.Constant((ushort)0)), Expression.Break(breakLabel)),
                        GetDynamicTimings(5, 21),
                        GetMemoryRefreshDeltaExpression(Expression.Constant(2))), // This function actually decreases the PC by two for each 'loop' hence need more refresh cycles.
                    breakLabel);

            yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(false));
            yield return Expression.Assign(Xpr.ParityOverflow, Expression.Constant(false));
            yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
        }

        public IEnumerable<Expression> GetCpExpressions(bool decrement = false)
        {
            yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.HL));
            yield return decrement ? Expression.PreDecrementAssign(Xpr.HL) : Expression.PreIncrementAssign(Xpr.HL);
            yield return Expression.PreDecrementAssign(Xpr.BC);
        }

        public Expression GetCprExpression(bool decrement = false)
        {
            var breakLabel = Expression.Label();
            var expressions = GetCpExpressions(decrement);
            var iterationExpressions = new[]
                                       {
                                           Expression.IfThen(Expression.OrElse(Expression.Equal(Xpr.BC, Expression.Constant((ushort)0)), Xpr.Zero), Expression.Break(breakLabel)), GetDynamicTimings(5, 21),
                                           GetMemoryRefreshDeltaExpression(Expression.Constant(2))
                                       };
            return Expression.Loop(Expression.Block(expressions.Concat(iterationExpressions).ToArray()), breakLabel);
        }

        public IEnumerable<Expression> GetInExpressions(bool decrement = false)
        {
            yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Expression.Call(Xpr.IO, Xpr.IoReadByte, Xpr.C, Xpr.B));
            yield return decrement ? Expression.PreDecrementAssign(Xpr.HL) : Expression.PreIncrementAssign(Xpr.HL);
            yield return Expression.Assign(Xpr.B, Expression.Convert(Expression.Subtract(Expression.Convert(Xpr.B, typeof(int)), Expression.Constant(1)), typeof(byte)));
            yield return Expression.Assign(Xpr.Subtract, Expression.Constant(true));
            yield return Expression.Call(Xpr.Flags, Xpr.SetResultFlags, Xpr.B);
        }

        public Expression GetInrExpression(bool decrement = false)
        {
            var breakLabel = Expression.Label();

            var expressions = GetInExpressions(decrement);
            var iterationExpressions = new[]
                                       {
                                           Expression.IfThen(Expression.Equal(Xpr.B, Expression.Constant((byte)0)), Expression.Break(breakLabel)), GetDynamicTimings(5, 21),
                                           GetMemoryRefreshDeltaExpression(Expression.Constant(2))
                                       };

            return Expression.Loop(Expression.Block(expressions.Concat(iterationExpressions).ToArray()), breakLabel);
        }

        public IEnumerable<Expression> GetOutExpressions(bool decrement = false)
        {
            yield return Expression.Call(Xpr.IO, Xpr.IoWriteByte, Xpr.C, Xpr.B, Xpr.ReadByteAtHL);
            yield return decrement ? Expression.PreDecrementAssign(Xpr.HL) : Expression.PreIncrementAssign(Xpr.HL);
            yield return Expression.Assign(Xpr.B, Expression.Convert(Expression.Subtract(Expression.Convert(Xpr.B, typeof(int)), Expression.Constant(1)), typeof(byte)));
            yield return Expression.Assign(Xpr.Subtract, Expression.Constant(true));
            yield return Expression.Call(Xpr.Flags, Xpr.SetResultFlags, Xpr.B);
        }

        public Expression GetOutrExpression(bool decrement = false)
        {
            var breakLabel = Expression.Label();

            var expressions = GetOutExpressions(decrement);
            var iterationExpressions = new[]
                                       {
                                           Expression.IfThen(Expression.Equal(Xpr.B, Expression.Constant((byte)0)), Expression.Break(breakLabel)), GetDynamicTimings(5, 21),
                                           GetMemoryRefreshDeltaExpression(Expression.Constant(2))
                                       };

            return Expression.Loop(Expression.Block(expressions.Concat(iterationExpressions).ToArray()), breakLabel);
        }
    }
}
