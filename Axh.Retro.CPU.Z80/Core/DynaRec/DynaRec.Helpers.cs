namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using System;
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

        private HighLowExpressionPair ToHighLowExpressionPair(Operand operand)
        {
            switch (operand)
            {
                case Operand.HL:
                    return new HighLowExpressionPair(Xpr.H, Xpr.L);
                case Operand.BC:
                    return new HighLowExpressionPair(Xpr.B, Xpr.C);
                case Operand.DE:
                    return new HighLowExpressionPair(Xpr.D, Xpr.E);
                case Operand.AF:
                    return new HighLowExpressionPair(Xpr.A, Xpr.F);
                case Operand.IX:
                    return new HighLowExpressionPair(Xpr.IXh, Xpr.IXl);
                case Operand.IY:
                    return new HighLowExpressionPair(Xpr.IYh, Xpr.IYl);
                default:
                    throw new ArgumentOutOfRangeException(nameof(operand), operand, null);
            }
        }

        private Expression Read(Operation operation, bool is16Bit = false)
        {
            switch (operation.Operand2)
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
                case Operand.mnn:
                    return Expression.Call(Xpr.Mmu, is16Bit ? Xpr.MmuReadWord : Xpr.MmuReadByte, Expression.Constant(operation.WordLiteral));
                case Operand.nn:
                    return Expression.Constant(operation.WordLiteral);
                case Operand.n:
                    return Expression.Constant(operation.ByteLiteral);
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

        private Expression Write(Operation operation, Expression value, bool is16Bit = false)
        {
            switch (operation.Operand1)
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
        
    }
}
