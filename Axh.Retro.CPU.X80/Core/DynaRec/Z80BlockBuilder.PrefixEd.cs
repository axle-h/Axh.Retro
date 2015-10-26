namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    using Xpr = Z80Expressions;

    internal partial class Z80BlockBuilder
    {
        private IEnumerable<Expression> TryDecodeNextEdPrefixOperation()
        {
            var opCode = (PrefixEdOpCode)mmuCache.NextByte();

            switch (opCode)
            {
                // ********* 8-bit load *********
                // LD A, I
                case PrefixEdOpCode.LD_A_I:
                    yield return Expression.Assign(Xpr.A, Xpr.I);
                    yield return Expression.Call(Xpr.Flags, Xpr.SetResultFlags, Xpr.A);

                    // Also reset H & N and copy IFF2 to P/V
                    yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.ParityOverflow, Xpr.IFF2);
                    timer.Add(2, 9);
                    break;

                // LD A, R
                case PrefixEdOpCode.LD_A_R:
                    yield return Expression.Assign(Xpr.A, Xpr.R);
                    yield return Expression.Call(Xpr.Flags, Xpr.SetResultFlags, Xpr.A);

                    // Also reset H & N and copy IFF2 to P/V
                    yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.ParityOverflow, Xpr.IFF2);
                    timer.Add(2, 9);
                    break;

                // LD I, A
                case PrefixEdOpCode.LD_I_A:
                    yield return Expression.Assign(Xpr.I, Xpr.A);
                    timer.Add(2, 9);
                    break;

                // LD R, A
                case PrefixEdOpCode.LD_R_A:
                    yield return Expression.Assign(Xpr.R, Xpr.A);
                    timer.Add(2, 9);
                    break;

                // ********* 16-bit load *********
                // LD dd, (nn)
                case PrefixEdOpCode.LD_BC_mnn:
                    yield return Expression.Assign(Xpr.BC, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_DE_mnn:
                    yield return Expression.Assign(Xpr.DE, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_HL_mnn:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_SP_mnn:
                    yield return Expression.Assign(Xpr.SP, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                    timer.Add(6, 20);
                    break;

                // LD (nn), dd
                case PrefixEdOpCode.LD_mnn_BC:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.BC);
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_DE:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.DE);
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_HL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.HL);
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_SP:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.SP);
                    timer.Add(6, 20);
                    break;


                // ********* Block Transfer *********
                // LDI
                case PrefixEdOpCode.LDI:
                    foreach (var expression in Xpr.GetLdExpressions())
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                // LDIR
                case PrefixEdOpCode.LDIR:
                    foreach (var expression in Xpr.GetLdrExpressions())
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                // LDD
                case PrefixEdOpCode.LDD:
                    foreach (var expression in Xpr.GetLdExpressions(true))
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                // LDDR
                case PrefixEdOpCode.LDDR:
                    foreach (var expression in Xpr.GetLdrExpressions(true))
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                // ********* Search *********
                case PrefixEdOpCode.CPI:
                    foreach (var expression in Xpr.GetCpExpressions())
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPIR:
                    yield return Xpr.GetCprExpression();
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPD:
                    foreach (var expression in Xpr.GetCpExpressions(true))
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPDR:
                    yield return Xpr.GetCprExpression(true);
                    timer.Add(4, 16);
                    break;

                // ********* 16-Bit Arithmetic *********
                // ADC HL, ss
                case PrefixEdOpCode.ADC_HL_BC:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, Xpr.HL, Xpr.BC));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.ADC_HL_DE:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, Xpr.HL, Xpr.DE));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.ADC_HL_HL:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, Xpr.HL, Xpr.HL));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.ADC_HL_SP:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, Xpr.HL, Xpr.SP));
                    timer.Add(4, 15);
                    break;

                // SBC HL, ss
                case PrefixEdOpCode.SBC_HL_BC:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluSubtract16WithCarry, Xpr.HL, Xpr.BC));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.SBC_HL_DE:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluSubtract16WithCarry, Xpr.HL, Xpr.DE));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.SBC_HL_HL:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluSubtract16WithCarry, Xpr.HL, Xpr.HL));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.SBC_HL_SP:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluSubtract16WithCarry, Xpr.HL, Xpr.SP));
                    timer.Add(4, 15);
                    break;

                // ********* General-Purpose Arithmetic *********
                // NEG
                case PrefixEdOpCode.NEG:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Expression.Constant((byte)0), Xpr.A));
                    timer.Add(2, 8);
                    break;

                // IM 0
                case PrefixEdOpCode.IM0:
                    yield return Expression.Assign(Xpr.IM, Expression.Constant(InterruptMode.InterruptMode0));
                    timer.Add(2, 8);
                    break;

                // IM 1
                case PrefixEdOpCode.IM1:
                    yield return Expression.Assign(Xpr.IM, Expression.Constant(InterruptMode.InterruptMode1));
                    timer.Add(2, 8);
                    break;

                // IM 2
                case PrefixEdOpCode.IM2:
                    yield return Expression.Assign(Xpr.IM, Expression.Constant(InterruptMode.InterruptMode2));
                    timer.Add(2, 8);
                    break;

                default:
                    throw new NotImplementedException(opCode.ToString());
            }
        }
    }
}
