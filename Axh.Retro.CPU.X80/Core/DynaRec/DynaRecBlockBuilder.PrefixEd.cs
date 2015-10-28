namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    internal partial class DynaRecBlockBuilder<TRegisters> where TRegisters : IRegisters
    {
        private IEnumerable<Expression> TryDecodeNextEdPrefixOperation()
        {
            var opCode = (PrefixEdOpCode)mmuCache.NextByte();

            switch (opCode)
            {
                // ********* 8-bit load *********
                // LD A, I
                case PrefixEdOpCode.LD_A_I:
                    yield return Expression.Assign(DynaRecExpressions.A, DynaRecExpressions.I);
                    yield return Expression.Call(DynaRecExpressions.Flags, DynaRecExpressions.SetResultFlags, DynaRecExpressions.A);

                    // Also reset H & N and copy IFF2 to P/V
                    yield return Expression.Assign(DynaRecExpressions.HalfCarry, Expression.Constant(false));
                    yield return Expression.Assign(DynaRecExpressions.Subtract, Expression.Constant(false));
                    yield return Expression.Assign(DynaRecExpressions.ParityOverflow, DynaRecExpressions.IFF2);
                    timer.Add(2, 9);
                    break;

                // LD A, R
                case PrefixEdOpCode.LD_A_R:
                    yield return Expression.Assign(DynaRecExpressions.A, DynaRecExpressions.R);
                    yield return Expression.Call(DynaRecExpressions.Flags, DynaRecExpressions.SetResultFlags, DynaRecExpressions.A);

                    // Also reset H & N and copy IFF2 to P/V
                    yield return Expression.Assign(DynaRecExpressions.HalfCarry, Expression.Constant(false));
                    yield return Expression.Assign(DynaRecExpressions.Subtract, Expression.Constant(false));
                    yield return Expression.Assign(DynaRecExpressions.ParityOverflow, DynaRecExpressions.IFF2);
                    timer.Add(2, 9);
                    break;

                // LD I, A
                case PrefixEdOpCode.LD_I_A:
                    yield return Expression.Assign(DynaRecExpressions.I, DynaRecExpressions.A);
                    timer.Add(2, 9);
                    break;

                // LD R, A
                case PrefixEdOpCode.LD_R_A:
                    yield return Expression.Assign(DynaRecExpressions.R, DynaRecExpressions.A);
                    timer.Add(2, 9);
                    break;

                // ********* 16-bit load *********
                // LD dd, (nn)
                case PrefixEdOpCode.LD_BC_mnn:
                    yield return Expression.Assign(DynaRecExpressions.BC, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadWord, NextWord));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_DE_mnn:
                    yield return Expression.Assign(DynaRecExpressions.DE, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadWord, NextWord));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_HL_mnn:
                    yield return Expression.Assign(DynaRecExpressions.HL, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadWord, NextWord));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_SP_mnn:
                    yield return Expression.Assign(DynaRecExpressions.SP, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadWord, NextWord));
                    timer.Add(6, 20);
                    break;

                // LD (nn), dd
                case PrefixEdOpCode.LD_mnn_BC:
                    yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteWord, NextWord, DynaRecExpressions.BC);
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_DE:
                    yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteWord, NextWord, DynaRecExpressions.DE);
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_HL:
                    yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteWord, NextWord, DynaRecExpressions.HL);
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_SP:
                    yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteWord, NextWord, DynaRecExpressions.SP);
                    timer.Add(6, 20);
                    break;


                // ********* Block Transfer *********
                // LDI
                case PrefixEdOpCode.LDI:
                    foreach (var expression in DynaRecExpressions.GetLdExpressions())
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                // LDIR
                case PrefixEdOpCode.LDIR:
                    foreach (var expression in DynaRecExpressions.GetLdrExpressions())
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                // LDD
                case PrefixEdOpCode.LDD:
                    foreach (var expression in DynaRecExpressions.GetLdExpressions(true))
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                // LDDR
                case PrefixEdOpCode.LDDR:
                    foreach (var expression in DynaRecExpressions.GetLdrExpressions(true))
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                // ********* Search *********
                case PrefixEdOpCode.CPI:
                    foreach (var expression in DynaRecExpressions.GetCpExpressions())
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPIR:
                    yield return DynaRecExpressions.GetCprExpression();
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPD:
                    foreach (var expression in DynaRecExpressions.GetCpExpressions(true))
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPDR:
                    yield return DynaRecExpressions.GetCprExpression(true);
                    timer.Add(4, 16);
                    break;

                // ********* 16-Bit Arithmetic *********
                // ADC HL, ss
                case PrefixEdOpCode.ADC_HL_BC:
                    yield return Expression.Assign(DynaRecExpressions.HL, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd16WithCarry, DynaRecExpressions.HL, DynaRecExpressions.BC));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.ADC_HL_DE:
                    yield return Expression.Assign(DynaRecExpressions.HL, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd16WithCarry, DynaRecExpressions.HL, DynaRecExpressions.DE));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.ADC_HL_HL:
                    yield return Expression.Assign(DynaRecExpressions.HL, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd16WithCarry, DynaRecExpressions.HL, DynaRecExpressions.HL));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.ADC_HL_SP:
                    yield return Expression.Assign(DynaRecExpressions.HL, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd16WithCarry, DynaRecExpressions.HL, DynaRecExpressions.SP));
                    timer.Add(4, 15);
                    break;

                // SBC HL, ss
                case PrefixEdOpCode.SBC_HL_BC:
                    yield return Expression.Assign(DynaRecExpressions.HL, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract16WithCarry, DynaRecExpressions.HL, DynaRecExpressions.BC));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.SBC_HL_DE:
                    yield return Expression.Assign(DynaRecExpressions.HL, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract16WithCarry, DynaRecExpressions.HL, DynaRecExpressions.DE));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.SBC_HL_HL:
                    yield return Expression.Assign(DynaRecExpressions.HL, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract16WithCarry, DynaRecExpressions.HL, DynaRecExpressions.HL));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.SBC_HL_SP:
                    yield return Expression.Assign(DynaRecExpressions.HL, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract16WithCarry, DynaRecExpressions.HL, DynaRecExpressions.SP));
                    timer.Add(4, 15);
                    break;

                // ********* General-Purpose Arithmetic *********
                // NEG
                case PrefixEdOpCode.NEG:
                    yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract, Expression.Constant((byte)0), DynaRecExpressions.A));
                    timer.Add(2, 8);
                    break;

                // IM 0
                case PrefixEdOpCode.IM0:
                    yield return Expression.Assign(DynaRecExpressions.IM, Expression.Constant(InterruptMode.InterruptMode0));
                    timer.Add(2, 8);
                    break;

                // IM 1
                case PrefixEdOpCode.IM1:
                    yield return Expression.Assign(DynaRecExpressions.IM, Expression.Constant(InterruptMode.InterruptMode1));
                    timer.Add(2, 8);
                    break;

                // IM 2
                case PrefixEdOpCode.IM2:
                    yield return Expression.Assign(DynaRecExpressions.IM, Expression.Constant(InterruptMode.InterruptMode2));
                    timer.Add(2, 8);
                    break;

                // ********* Rotate *********
                // RLD
                case PrefixEdOpCode.RLD:
                    yield return Expression.Assign(DynaRecExpressions.AccumulatorAndResult, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluRotateLeftDigit, DynaRecExpressions.A, DynaRecExpressions.ReadByteAtHL));
                    yield return Expression.Assign(DynaRecExpressions.A, DynaRecExpressions.AccumulatorAndResult_Accumulator);
                    yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.HL, DynaRecExpressions.AccumulatorAndResult_Result);
                    timer.Add(5, 18);
                    break;

                // RRD
                case PrefixEdOpCode.RRD:
                    yield return Expression.Assign(DynaRecExpressions.AccumulatorAndResult, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluRotateRightDigit, DynaRecExpressions.A, DynaRecExpressions.ReadByteAtHL));
                    yield return Expression.Assign(DynaRecExpressions.A, DynaRecExpressions.AccumulatorAndResult_Accumulator);
                    yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.HL, DynaRecExpressions.AccumulatorAndResult_Result);
                    timer.Add(5, 18);
                    break;

                default:
                    throw new NotImplementedException(opCode.ToString());
            }
        }
    }
}
