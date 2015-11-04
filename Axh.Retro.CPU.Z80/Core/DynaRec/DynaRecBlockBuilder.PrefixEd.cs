namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

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
                    yield return Expression.Assign(Xpr.A, Xpr.I);
                    yield return Expression.Call(Xpr.Flags, Xpr.SetResultFlags, Xpr.A);

                    // Also reset H & N and copy IFF2 to P/V
                    yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.ParityOverflow, Xpr.IFF2);
                    timingsBuilder.Add(2, 9);
                    break;

                // LD A, R
                case PrefixEdOpCode.LD_A_R:
                    yield return Expression.Assign(Xpr.A, Xpr.R);
                    yield return Expression.Call(Xpr.Flags, Xpr.SetResultFlags, Xpr.A);

                    // Also reset H & N and copy IFF2 to P/V
                    yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.ParityOverflow, Xpr.IFF2);
                    timingsBuilder.Add(2, 9);
                    break;

                // LD I, A
                case PrefixEdOpCode.LD_I_A:
                    yield return Expression.Assign(Xpr.I, Xpr.A);
                    timingsBuilder.Add(2, 9);
                    break;

                // LD R, A
                case PrefixEdOpCode.LD_R_A:
                    yield return Expression.Assign(Xpr.R, Xpr.A);
                    timingsBuilder.Add(2, 9);
                    break;

                // ********* 16-bit load *********
                // LD dd, (nn)
                case PrefixEdOpCode.LD_BC_mnn:
                    yield return Expression.Assign(Xpr.BC, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                    timingsBuilder.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_DE_mnn:
                    yield return Expression.Assign(Xpr.DE, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                    timingsBuilder.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_HL_mnn:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                    timingsBuilder.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_SP_mnn:
                    yield return Expression.Assign(Xpr.SP, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                    timingsBuilder.Add(6, 20);
                    break;

                // LD (nn), dd
                case PrefixEdOpCode.LD_mnn_BC:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.BC);
                    timingsBuilder.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_DE:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.DE);
                    timingsBuilder.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_HL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.HL);
                    timingsBuilder.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_SP:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.SP);
                    timingsBuilder.Add(6, 20);
                    break;


                // ********* Block Transfer *********
                // LDI
                case PrefixEdOpCode.LDI:
                    foreach (var expression in Xpr.GetLdExpressions())
                    {
                        yield return expression;
                    }
                    timingsBuilder.Add(4, 16);
                    break;

                // LDIR
                case PrefixEdOpCode.LDIR:
                    foreach (var expression in Xpr.GetLdrExpressions())
                    {
                        yield return expression;
                    }
                    timingsBuilder.Add(4, 16);
                    break;

                // LDD
                case PrefixEdOpCode.LDD:
                    foreach (var expression in Xpr.GetLdExpressions(true))
                    {
                        yield return expression;
                    }
                    timingsBuilder.Add(4, 16);
                    break;

                // LDDR
                case PrefixEdOpCode.LDDR:
                    foreach (var expression in Xpr.GetLdrExpressions(true))
                    {
                        yield return expression;
                    }
                    timingsBuilder.Add(4, 16);
                    break;

                // ********* Search *********
                case PrefixEdOpCode.CPI:
                    foreach (var expression in Xpr.GetCpExpressions())
                    {
                        yield return expression;
                    }
                    timingsBuilder.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPIR:
                    yield return Xpr.GetCprExpression();
                    timingsBuilder.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPD:
                    foreach (var expression in Xpr.GetCpExpressions(true))
                    {
                        yield return expression;
                    }
                    timingsBuilder.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPDR:
                    yield return Xpr.GetCprExpression(true);
                    timingsBuilder.Add(4, 16);
                    break;

                // ********* 16-Bit Arithmetic *********
                // ADC HL, ss
                case PrefixEdOpCode.ADC_HL_BC:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, Xpr.HL, Xpr.BC));
                    timingsBuilder.Add(4, 15);
                    break;
                case PrefixEdOpCode.ADC_HL_DE:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, Xpr.HL, Xpr.DE));
                    timingsBuilder.Add(4, 15);
                    break;
                case PrefixEdOpCode.ADC_HL_HL:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, Xpr.HL, Xpr.HL));
                    timingsBuilder.Add(4, 15);
                    break;
                case PrefixEdOpCode.ADC_HL_SP:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, Xpr.HL, Xpr.SP));
                    timingsBuilder.Add(4, 15);
                    break;

                // SBC HL, ss
                case PrefixEdOpCode.SBC_HL_BC:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluSubtract16WithCarry, Xpr.HL, Xpr.BC));
                    timingsBuilder.Add(4, 15);
                    break;
                case PrefixEdOpCode.SBC_HL_DE:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluSubtract16WithCarry, Xpr.HL, Xpr.DE));
                    timingsBuilder.Add(4, 15);
                    break;
                case PrefixEdOpCode.SBC_HL_HL:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluSubtract16WithCarry, Xpr.HL, Xpr.HL));
                    timingsBuilder.Add(4, 15);
                    break;
                case PrefixEdOpCode.SBC_HL_SP:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluSubtract16WithCarry, Xpr.HL, Xpr.SP));
                    timingsBuilder.Add(4, 15);
                    break;

                // ********* General-Purpose Arithmetic *********
                // NEG
                case PrefixEdOpCode.NEG:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Expression.Constant((byte)0), Xpr.A));
                    timingsBuilder.Add(2, 8);
                    break;

                // IM 0
                case PrefixEdOpCode.IM0:
                    yield return Expression.Assign(Xpr.IM, Expression.Constant(InterruptMode.InterruptMode0));
                    timingsBuilder.Add(2, 8);
                    break;

                // IM 1
                case PrefixEdOpCode.IM1:
                    yield return Expression.Assign(Xpr.IM, Expression.Constant(InterruptMode.InterruptMode1));
                    timingsBuilder.Add(2, 8);
                    break;

                // IM 2
                case PrefixEdOpCode.IM2:
                    yield return Expression.Assign(Xpr.IM, Expression.Constant(InterruptMode.InterruptMode2));
                    timingsBuilder.Add(2, 8);
                    break;

                // ********* Rotate *********
                // RLD
                case PrefixEdOpCode.RLD:
                    yield return Expression.Assign(Xpr.AccumulatorAndResult, Expression.Call(Xpr.Alu, Xpr.AluRotateLeftDigit, Xpr.A, Xpr.ReadByteAtHL));
                    yield return Expression.Assign(Xpr.A, Xpr.AccumulatorAndResult_Accumulator);
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Xpr.AccumulatorAndResult_Result);
                    timingsBuilder.Add(5, 18);
                    break;

                // RRD
                case PrefixEdOpCode.RRD:
                    yield return Expression.Assign(Xpr.AccumulatorAndResult, Expression.Call(Xpr.Alu, Xpr.AluRotateRightDigit, Xpr.A, Xpr.ReadByteAtHL));
                    yield return Expression.Assign(Xpr.A, Xpr.AccumulatorAndResult_Accumulator);
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.HL, Xpr.AccumulatorAndResult_Result);
                    timingsBuilder.Add(5, 18);
                    break;

                // ********* Return *********
                case PrefixEdOpCode.RETI:
                    yield return Xpr.ReadPCFromStack;
                    yield return Xpr.PopPopSP;
                    timingsBuilder.Add(4, 14);
                    LastDecodeResult = DecodeResult.Finalize;
                    yield break;

                case PrefixEdOpCode.RETN:
                    yield return Xpr.ReadPCFromStack;
                    yield return Xpr.PopPopSP;
                    yield return Expression.Assign(Xpr.IFF1, Xpr.IFF2);
                    timingsBuilder.Add(4, 14);
                    LastDecodeResult = DecodeResult.Finalize;
                    yield break;
                    
                // ********* IO *********
                // IN r, (C)
                case PrefixEdOpCode.IN_A_C:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.IO, Xpr.IoReadByte, Xpr.C, Xpr.B));
                    timingsBuilder.Add(3, 12);
                    break;

                case PrefixEdOpCode.IN_B_C:
                    yield return Expression.Assign(Xpr.B, Expression.Call(Xpr.IO, Xpr.IoReadByte, Xpr.C, Xpr.B));
                    timingsBuilder.Add(3, 12);
                    break;

                case PrefixEdOpCode.IN_C_C:
                    yield return Expression.Assign(Xpr.C, Expression.Call(Xpr.IO, Xpr.IoReadByte, Xpr.C, Xpr.B));
                    timingsBuilder.Add(3, 12);
                    break;

                case PrefixEdOpCode.IN_D_C:
                    yield return Expression.Assign(Xpr.D, Expression.Call(Xpr.IO, Xpr.IoReadByte, Xpr.C, Xpr.B));
                    timingsBuilder.Add(3, 12);
                    break;

                case PrefixEdOpCode.IN_E_C:
                    yield return Expression.Assign(Xpr.E, Expression.Call(Xpr.IO, Xpr.IoReadByte, Xpr.C, Xpr.B));
                    timingsBuilder.Add(3, 12);
                    break;

                case PrefixEdOpCode.IN_F_C:
                    yield return Expression.Assign(Xpr.F, Expression.Call(Xpr.IO, Xpr.IoReadByte, Xpr.C, Xpr.B));
                    timingsBuilder.Add(3, 12);
                    break;

                case PrefixEdOpCode.IN_H_C:
                    yield return Expression.Assign(Xpr.H, Expression.Call(Xpr.IO, Xpr.IoReadByte, Xpr.C, Xpr.B));
                    timingsBuilder.Add(3, 12);
                    break;

                case PrefixEdOpCode.IN_L_C:
                    yield return Expression.Assign(Xpr.L, Expression.Call(Xpr.IO, Xpr.IoReadByte, Xpr.C, Xpr.B));
                    timingsBuilder.Add(3, 12);
                    break;

                // INI
                case PrefixEdOpCode.INI:
                    foreach (var expression in Xpr.GetInExpressions())
                    {
                        yield return expression;
                    }
                    timingsBuilder.Add(4, 16);
                    break;

                // INIR
                case PrefixEdOpCode.INIR:
                    yield return Xpr.GetInrExpression();
                    timingsBuilder.Add(4, 16);
                    break;

                // IND
                case PrefixEdOpCode.IND:
                    foreach (var expression in Xpr.GetInExpressions(true))
                    {
                        yield return expression;
                    }
                    timingsBuilder.Add(4, 16);
                    break;

                // INDR
                case PrefixEdOpCode.INDR:
                    yield return Xpr.GetInrExpression(true);
                    timingsBuilder.Add(4, 16);
                    break;

                // OUT r, (C)
                case PrefixEdOpCode.OUT_A_C:
                    yield return Expression.Call(Xpr.IO, Xpr.IoWriteByte, Xpr.C, Xpr.B, Xpr.A);
                    timingsBuilder.Add(3, 12);
                    break;

                case PrefixEdOpCode.OUT_B_C:
                    yield return Expression.Call(Xpr.IO, Xpr.IoWriteByte, Xpr.C, Xpr.B, Xpr.B);
                    timingsBuilder.Add(3, 12);
                    break;

                case PrefixEdOpCode.OUT_C_C:
                    yield return Expression.Call(Xpr.IO, Xpr.IoWriteByte, Xpr.C, Xpr.B, Xpr.C);
                    timingsBuilder.Add(3, 12);
                    break;
                    
                case PrefixEdOpCode.OUT_D_C:
                    yield return Expression.Call(Xpr.IO, Xpr.IoWriteByte, Xpr.C, Xpr.B, Xpr.D);
                    timingsBuilder.Add(3, 12);
                    break;

                case PrefixEdOpCode.OUT_E_C:
                    yield return Expression.Call(Xpr.IO, Xpr.IoWriteByte, Xpr.C, Xpr.B, Xpr.E);
                    timingsBuilder.Add(3, 12);
                    break;

                case PrefixEdOpCode.OUT_F_C:
                    yield return Expression.Call(Xpr.IO, Xpr.IoWriteByte, Xpr.C, Xpr.B, Xpr.F);
                    timingsBuilder.Add(3, 12);
                    break;

                case PrefixEdOpCode.OUT_H_C:
                    yield return Expression.Call(Xpr.IO, Xpr.IoWriteByte, Xpr.C, Xpr.B, Xpr.H);
                    timingsBuilder.Add(3, 12);
                    break;
                    
                case PrefixEdOpCode.OUT_L_C:
                    yield return Expression.Call(Xpr.IO, Xpr.IoWriteByte, Xpr.C, Xpr.B, Xpr.L);
                    timingsBuilder.Add(3, 12);
                    break;

                // OUTI
                case PrefixEdOpCode.OUTI:
                    foreach (var expression in Xpr.GetOutExpressions())
                    {
                        yield return expression;
                    }
                    timingsBuilder.Add(4, 16);
                    break;

                // OUTIR
                case PrefixEdOpCode.OUTIR:
                    yield return Xpr.GetOutrExpression();
                    timingsBuilder.Add(4, 16);
                    break;

                // OUTD
                case PrefixEdOpCode.OUTD:
                    foreach (var expression in Xpr.GetOutExpressions(true))
                    {
                        yield return expression;
                    }
                    timingsBuilder.Add(4, 16);
                    break;

                // OUTDR
                case PrefixEdOpCode.OUTDR:
                    yield return Xpr.GetOutrExpression(true);
                    timingsBuilder.Add(4, 16);
                    break;

                default:
                    // The Prefix ED opcode set is not saturated
                    // Opcodes not defined are just run as NOP's
                    timingsBuilder.Add(1, 4);
                    break;
            }
        }
    }
}
