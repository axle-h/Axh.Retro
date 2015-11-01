namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    using Xpr = DynaRecExpressions;

    internal partial class DynaRecBlockBuilder<TRegisters> where TRegisters : IRegisters
    {
        /// <summary>
        /// Run CB prefixed opcodes with an index
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Expression> TryDecodeNextDdFdCbPrefixOperation()
        {
            // Read the displacement as the next byte
            yield return Expression.Assign(Xpr.LocalByte, NextByte);

            var opCode = (PrefixCbOpCode)mmuCache.NextByte();
            
            switch (opCode)
            {
                // ********* Rotate *********
                // RLC r (undocumented)
                // Actually RLC (IX+d) & LD r,(IX+d) or RLC (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.RLC_A:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RLC_B:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RLC_C:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RLC_D:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RLC_E:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RLC_H:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RLC_L:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeftWithCarry);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // RLC (IX+d)
                // RLC (IY+d)
                case PrefixCbOpCode.RLC_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluRotateLeftWithCarry);
                    break;

                // RL r (undocumented)
                // Actually RL (IX+d) & LD r,(IX+d) or RL (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.RL_A:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RL_B:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RL_C:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RL_D:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RL_E:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RL_H:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RL_L:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateLeft);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // RL (IX+d)
                // RL (IY+d)
                case PrefixCbOpCode.RL_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluRotateLeft);
                    break;

                // RRC r (undocumented)
                // Actually RRC (IX+d) & LD r,(IX+d) or RRC (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.RRC_A:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RRC_B:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RRC_C:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RRC_D:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RRC_E:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RRC_H:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RRC_L:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRightWithCarry);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // RRC (IX+d)
                // RRC (IY+d)
                case PrefixCbOpCode.RRC_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluRotateRightWithCarry);
                    break;

                // RR r (undocumented)
                // Actually RR (IX+d) & LD r,(IX+d) or RR (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.RR_A:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RR_B:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RR_C:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RR_D:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RR_E:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RR_H:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.RR_L:
                    yield return GetAluCallWrite(7, 27, Xpr.AluRotateRight);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // RR (IX+d)
                // RR (IY+d)
                case PrefixCbOpCode.RR_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluRotateRight);
                    break;

                // ********* Shift *********
                // SLA r (undocumented)
                // Actually SLA (IX+d) & LD r,(IX+d) or SLA (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.SLA_A:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLA_B:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLA_C:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLA_D:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLA_E:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLA_H:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLA_L:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeft);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // SLA (IX+d)
                // SLA (IY+d)
                case PrefixCbOpCode.SLA_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluShiftLeft);
                    break;

                // SLS r (undocumented)
                // Actually SLS (IX+d) & LD r,(IX+d) or SLS (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.SLS_A:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLS_B:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLS_C:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLS_D:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLS_E:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLS_H:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SLS_L:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftLeftSet);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // SLS (IX+d) (undocumented)
                // SLS (IY+d) (undocumented)
                case PrefixCbOpCode.SLS_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluShiftLeftSet);
                    break;

                // SRA r (undocumented)
                // Actually SRA (IX+d) & LD r,(IX+d) or SRA (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.SRA_A:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRA_B:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRA_C:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRA_D:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRA_E:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRA_H:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRA_L:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRight);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // SRA (IX+d)
                // SRA (IY+d)
                case PrefixCbOpCode.SRA_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluShiftRight);
                    break;

                // SRL r (undocumented)
                // Actually SRL (IX+d) & LD r,(IX+d) or SRL (IY+d) & LD r,(IY+d)
                case PrefixCbOpCode.SRL_A:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRL_B:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRL_C:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRL_D:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRL_E:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRL_H:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    break;
                case PrefixCbOpCode.SRL_L:
                    yield return GetAluCallWrite(7, 27, Xpr.AluShiftRightLogical);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    break;

                // SRL (IX+d)
                // SRL (IY+d)
                case PrefixCbOpCode.SRL_mHL:
                    yield return GetAluCallWrite(5, 19, Xpr.AluShiftRightLogical);
                    break;

                // ********* Bit Test, Set & Reset *********
                // BIT r
                case PrefixCbOpCode.BIT_0_A:
                case PrefixCbOpCode.BIT_0_B:
                case PrefixCbOpCode.BIT_0_C:
                case PrefixCbOpCode.BIT_0_D:
                case PrefixCbOpCode.BIT_0_E:
                case PrefixCbOpCode.BIT_0_H:
                case PrefixCbOpCode.BIT_0_L:
                case PrefixCbOpCode.BIT_0_mHL:
                    yield return BitTestFromIndex(0);
                    break;

                case PrefixCbOpCode.BIT_1_A:
                case PrefixCbOpCode.BIT_1_B:
                case PrefixCbOpCode.BIT_1_C:
                case PrefixCbOpCode.BIT_1_D:
                case PrefixCbOpCode.BIT_1_E:
                case PrefixCbOpCode.BIT_1_H:
                case PrefixCbOpCode.BIT_1_L:
                case PrefixCbOpCode.BIT_1_mHL:
                    yield return BitTestFromIndex(1);
                    break;
                    
                case PrefixCbOpCode.BIT_2_A:
                case PrefixCbOpCode.BIT_2_B:
                case PrefixCbOpCode.BIT_2_C:
                case PrefixCbOpCode.BIT_2_D:
                case PrefixCbOpCode.BIT_2_E:
                case PrefixCbOpCode.BIT_2_H:
                case PrefixCbOpCode.BIT_2_L:
                case PrefixCbOpCode.BIT_2_mHL:
                    yield return BitTestFromIndex(2);
                    break;
                    
                case PrefixCbOpCode.BIT_3_A:
                case PrefixCbOpCode.BIT_3_B:
                case PrefixCbOpCode.BIT_3_C:
                case PrefixCbOpCode.BIT_3_D:
                case PrefixCbOpCode.BIT_3_E:
                case PrefixCbOpCode.BIT_3_H:
                case PrefixCbOpCode.BIT_3_L:
                case PrefixCbOpCode.BIT_3_mHL:
                    yield return BitTestFromIndex(3);
                    break;

                case PrefixCbOpCode.BIT_4_A:
                case PrefixCbOpCode.BIT_4_B:
                case PrefixCbOpCode.BIT_4_C:
                case PrefixCbOpCode.BIT_4_D:
                case PrefixCbOpCode.BIT_4_E:
                case PrefixCbOpCode.BIT_4_H:
                case PrefixCbOpCode.BIT_4_L:
                case PrefixCbOpCode.BIT_4_mHL:
                    yield return BitTestFromIndex(4);
                    break;
                    
                case PrefixCbOpCode.BIT_5_A:
                case PrefixCbOpCode.BIT_5_B:
                case PrefixCbOpCode.BIT_5_C:
                case PrefixCbOpCode.BIT_5_D:
                case PrefixCbOpCode.BIT_5_E:
                case PrefixCbOpCode.BIT_5_H:
                case PrefixCbOpCode.BIT_5_L:
                case PrefixCbOpCode.BIT_5_mHL:
                    yield return BitTestFromIndex(5);
                    break;
                    
                case PrefixCbOpCode.BIT_6_A:
                case PrefixCbOpCode.BIT_6_B:
                case PrefixCbOpCode.BIT_6_C:
                case PrefixCbOpCode.BIT_6_D:
                case PrefixCbOpCode.BIT_6_E:
                case PrefixCbOpCode.BIT_6_H:
                case PrefixCbOpCode.BIT_6_L:
                case PrefixCbOpCode.BIT_6_mHL:
                    yield return BitTestFromIndex(6);
                    break;
                    
                case PrefixCbOpCode.BIT_7_A:
                case PrefixCbOpCode.BIT_7_B:
                case PrefixCbOpCode.BIT_7_C:
                case PrefixCbOpCode.BIT_7_D:
                case PrefixCbOpCode.BIT_7_E:
                case PrefixCbOpCode.BIT_7_H:
                case PrefixCbOpCode.BIT_7_L:
                case PrefixCbOpCode.BIT_7_mHL:
                    yield return BitTestFromIndex(7);
                    break;

                // SET r
                case PrefixCbOpCode.SET_0_B:
                    yield return BitSetFromIndex(0);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_1_B:
                    yield return BitSetFromIndex(1);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_2_B:
                    yield return BitSetFromIndex(2);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_3_B:
                    yield return BitSetFromIndex(3);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_4_B:
                    yield return BitSetFromIndex(4);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_5_B:
                    yield return BitSetFromIndex(5);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_6_B:
                    yield return BitSetFromIndex(6);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_7_B:
                    yield return BitSetFromIndex(7);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_0_C:
                    yield return BitSetFromIndex(0);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_1_C:
                    yield return BitSetFromIndex(1);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_2_C:
                    yield return BitSetFromIndex(2);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_3_C:
                    yield return BitSetFromIndex(3);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_4_C:
                    yield return BitSetFromIndex(4);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_5_C:
                    yield return BitSetFromIndex(5);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_6_C:
                    yield return BitSetFromIndex(6);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_7_C:
                    yield return BitSetFromIndex(7);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_0_D:
                    yield return BitSetFromIndex(0);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_1_D:
                    yield return BitSetFromIndex(1);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_2_D:
                    yield return BitSetFromIndex(2);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_3_D:
                    yield return BitSetFromIndex(3);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_4_D:
                    yield return BitSetFromIndex(4);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_5_D:
                    yield return BitSetFromIndex(5);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_6_D:
                    yield return BitSetFromIndex(6);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_7_D:
                    yield return BitSetFromIndex(7);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_0_E:
                    yield return BitSetFromIndex(0);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_1_E:
                    yield return BitSetFromIndex(1);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_2_E:
                    yield return BitSetFromIndex(2);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_3_E:
                    yield return BitSetFromIndex(3);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_4_E:
                    yield return BitSetFromIndex(4);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_5_E:
                    yield return BitSetFromIndex(5);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_6_E:
                    yield return BitSetFromIndex(6);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_7_E:
                    yield return BitSetFromIndex(7);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_0_H:
                    yield return BitSetFromIndex(0);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_1_H:
                    yield return BitSetFromIndex(1);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_2_H:
                    yield return BitSetFromIndex(2);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_3_H:
                    yield return BitSetFromIndex(3);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_4_H:
                    yield return BitSetFromIndex(4);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_5_H:
                    yield return BitSetFromIndex(5);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_6_H:
                    yield return BitSetFromIndex(6);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_7_H:
                    yield return BitSetFromIndex(7);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_0_L:
                    yield return BitSetFromIndex(0);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_1_L:
                    yield return BitSetFromIndex(1);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_2_L:
                    yield return BitSetFromIndex(2);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_3_L:
                    yield return BitSetFromIndex(3);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_4_L:
                    yield return BitSetFromIndex(4);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_5_L:
                    yield return BitSetFromIndex(5);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_6_L:
                    yield return BitSetFromIndex(6);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_7_L:
                    yield return BitSetFromIndex(7);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_0_mHL:
                    yield return BitSetFromIndex(0);
                    break; 
                case PrefixCbOpCode.SET_1_mHL:
                    yield return BitSetFromIndex(1);
                    break; 
                case PrefixCbOpCode.SET_2_mHL:
                    yield return BitSetFromIndex(2);
                    break; 
                case PrefixCbOpCode.SET_3_mHL:
                    yield return BitSetFromIndex(3);
                    break; 
                case PrefixCbOpCode.SET_4_mHL:
                    yield return BitSetFromIndex(4);
                    break; 
                case PrefixCbOpCode.SET_5_mHL:
                    yield return BitSetFromIndex(5);
                    break; 
                case PrefixCbOpCode.SET_6_mHL:
                    yield return BitSetFromIndex(6);
                    break; 
                case PrefixCbOpCode.SET_7_mHL:
                    yield return BitSetFromIndex(7);
                    break; 
                case PrefixCbOpCode.SET_0_A:
                    yield return BitSetFromIndex(0);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_1_A:
                    yield return BitSetFromIndex(1);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_2_A:
                    yield return BitSetFromIndex(2);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_3_A:
                    yield return BitSetFromIndex(3);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_4_A:
                    yield return BitSetFromIndex(4);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_5_A:
                    yield return BitSetFromIndex(5);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_6_A:
                    yield return BitSetFromIndex(6);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.SET_7_A:
                    yield return BitSetFromIndex(7);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break;

                // RES r
                case PrefixCbOpCode.RES_0_B:
                    yield return BitResetFromIndex(0);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_1_B:
                    yield return BitResetFromIndex(1);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_2_B:
                    yield return BitResetFromIndex(2);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_3_B:
                    yield return BitResetFromIndex(3);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_4_B:
                    yield return BitResetFromIndex(4);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_5_B:
                    yield return BitResetFromIndex(5);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_6_B:
                    yield return BitResetFromIndex(6);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_7_B:
                    yield return BitResetFromIndex(7);
                    yield return Expression.Assign(Xpr.B, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_0_C:
                    yield return BitResetFromIndex(0);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_1_C:
                    yield return BitResetFromIndex(1);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_2_C:
                    yield return BitResetFromIndex(2);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_3_C:
                    yield return BitResetFromIndex(3);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_4_C:
                    yield return BitResetFromIndex(4);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_5_C:
                    yield return BitResetFromIndex(5);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_6_C:
                    yield return BitResetFromIndex(6);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_7_C:
                    yield return BitResetFromIndex(7);
                    yield return Expression.Assign(Xpr.C, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_0_D:
                    yield return BitResetFromIndex(0);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_1_D:
                    yield return BitResetFromIndex(1);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_2_D:
                    yield return BitResetFromIndex(2);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_3_D:
                    yield return BitResetFromIndex(3);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_4_D:
                    yield return BitResetFromIndex(4);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_5_D:
                    yield return BitResetFromIndex(5);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_6_D:
                    yield return BitResetFromIndex(6);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_7_D:
                    yield return BitResetFromIndex(7);
                    yield return Expression.Assign(Xpr.D, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_0_E:
                    yield return BitResetFromIndex(0);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_1_E:
                    yield return BitResetFromIndex(1);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_2_E:
                    yield return BitResetFromIndex(2);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_3_E:
                    yield return BitResetFromIndex(3);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_4_E:
                    yield return BitResetFromIndex(4);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_5_E:
                    yield return BitResetFromIndex(5);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_6_E:
                    yield return BitResetFromIndex(6);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_7_E:
                    yield return BitResetFromIndex(7);
                    yield return Expression.Assign(Xpr.E, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_0_H:
                    yield return BitResetFromIndex(0);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_1_H:
                    yield return BitResetFromIndex(1);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_2_H:
                    yield return BitResetFromIndex(2);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_3_H:
                    yield return BitResetFromIndex(3);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_4_H:
                    yield return BitResetFromIndex(4);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_5_H:
                    yield return BitResetFromIndex(5);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_6_H:
                    yield return BitResetFromIndex(6);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_7_H:
                    yield return BitResetFromIndex(7);
                    yield return Expression.Assign(Xpr.H, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_0_L:
                    yield return BitResetFromIndex(0);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_1_L:
                    yield return BitResetFromIndex(1);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_2_L:
                    yield return BitResetFromIndex(2);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_3_L:
                    yield return BitResetFromIndex(3);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_4_L:
                    yield return BitResetFromIndex(4);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_5_L:
                    yield return BitResetFromIndex(5);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_6_L:
                    yield return BitResetFromIndex(6);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_7_L:
                    yield return BitResetFromIndex(7);
                    yield return Expression.Assign(Xpr.L, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_0_mHL:
                    yield return BitResetFromIndex(0);
                    break; 
                case PrefixCbOpCode.RES_1_mHL:
                    yield return BitResetFromIndex(1);
                    break; 
                case PrefixCbOpCode.RES_2_mHL:
                    yield return BitResetFromIndex(2);
                    break; 
                case PrefixCbOpCode.RES_3_mHL:
                    yield return BitResetFromIndex(3);
                    break; 
                case PrefixCbOpCode.RES_4_mHL:
                    yield return BitResetFromIndex(4);
                    break; 
                case PrefixCbOpCode.RES_5_mHL:
                    yield return BitResetFromIndex(5);
                    break; 
                case PrefixCbOpCode.RES_6_mHL:
                    yield return BitResetFromIndex(6);
                    break; 
                case PrefixCbOpCode.RES_7_mHL:
                    yield return BitResetFromIndex(7);
                    break; 
                case PrefixCbOpCode.RES_0_A:
                    yield return BitResetFromIndex(0);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_1_A:
                    yield return BitResetFromIndex(1);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_2_A:
                    yield return BitResetFromIndex(2);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_3_A:
                    yield return BitResetFromIndex(3);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_4_A:
                    yield return BitResetFromIndex(4);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_5_A:
                    yield return BitResetFromIndex(5);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_6_A:
                    yield return BitResetFromIndex(6);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                case PrefixCbOpCode.RES_7_A:
                    yield return BitResetFromIndex(7);
                    yield return Expression.Assign(Xpr.A, index.ReadIndexedValue);
                    timingsBuilder.Add(4, 15);
                    break; 
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
