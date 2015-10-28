namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    internal partial class DynaRecBlockBuilder<TRegisters> where TRegisters : IRegisters
    {

        /// <summary>
        /// Run CB prefixed opcodes without an index
        /// </summary>
        /// <returns></returns>
        private Expression TryDecodeNextCbPrefixOperation()
        {
            var opCode = (PrefixCbOpCode)mmuCache.NextByte();

            switch (opCode)
            {
                // ********* Rotate *********
                // RLC r
                case PrefixCbOpCode.RLC_A:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeftWithCarry, DynaRecExpressions.A);
                case PrefixCbOpCode.RLC_B:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeftWithCarry, DynaRecExpressions.B);
                case PrefixCbOpCode.RLC_C:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeftWithCarry, DynaRecExpressions.C);
                case PrefixCbOpCode.RLC_D:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeftWithCarry, DynaRecExpressions.D);
                case PrefixCbOpCode.RLC_E:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeftWithCarry, DynaRecExpressions.E);
                case PrefixCbOpCode.RLC_H:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeftWithCarry, DynaRecExpressions.H);
                case PrefixCbOpCode.RLC_L:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeftWithCarry, DynaRecExpressions.L);
                case PrefixCbOpCode.RLC_mHL:
                    return GetAluCallWrite(4, 15, DynaRecExpressions.AluRotateLeftWithCarry);
                    
                // RL r
                case PrefixCbOpCode.RL_A:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeft, DynaRecExpressions.A);
                case PrefixCbOpCode.RL_B:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeft, DynaRecExpressions.B);
                case PrefixCbOpCode.RL_C:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeft, DynaRecExpressions.C);
                case PrefixCbOpCode.RL_D:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeft, DynaRecExpressions.D);
                case PrefixCbOpCode.RL_E:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeft, DynaRecExpressions.E);
                case PrefixCbOpCode.RL_H:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeft, DynaRecExpressions.H);
                case PrefixCbOpCode.RL_L:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateLeft, DynaRecExpressions.L);
                case PrefixCbOpCode.RL_mHL:
                    return GetAluCallWrite(4, 15, DynaRecExpressions.AluRotateLeft);
                    
                // RRC r
                case PrefixCbOpCode.RRC_A:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRightWithCarry, DynaRecExpressions.A);
                case PrefixCbOpCode.RRC_B:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRightWithCarry, DynaRecExpressions.B);
                case PrefixCbOpCode.RRC_C:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRightWithCarry, DynaRecExpressions.C);
                case PrefixCbOpCode.RRC_D:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRightWithCarry, DynaRecExpressions.D);
                case PrefixCbOpCode.RRC_E:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRightWithCarry, DynaRecExpressions.E);
                case PrefixCbOpCode.RRC_H:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRightWithCarry, DynaRecExpressions.H);
                case PrefixCbOpCode.RRC_L:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRightWithCarry, DynaRecExpressions.L);
                case PrefixCbOpCode.RRC_mHL:
                    return GetAluCallWrite(4, 15, DynaRecExpressions.AluRotateRightWithCarry);
                    
                // RR r
                case PrefixCbOpCode.RR_A:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRight, DynaRecExpressions.A);
                case PrefixCbOpCode.RR_B:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRight, DynaRecExpressions.B);
                case PrefixCbOpCode.RR_C:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRight, DynaRecExpressions.C);
                case PrefixCbOpCode.RR_D:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRight, DynaRecExpressions.D);
                case PrefixCbOpCode.RR_E:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRight, DynaRecExpressions.E);
                case PrefixCbOpCode.RR_H:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRight, DynaRecExpressions.H);
                case PrefixCbOpCode.RR_L:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluRotateRight, DynaRecExpressions.L);
                case PrefixCbOpCode.RR_mHL:
                    return GetAluCallWrite(4, 15, DynaRecExpressions.AluRotateRight);

                // ********* Shift *********
                // SLA r
                case PrefixCbOpCode.SLA_A:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeft, DynaRecExpressions.A);
                case PrefixCbOpCode.SLA_B:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeft, DynaRecExpressions.B);
                case PrefixCbOpCode.SLA_C:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeft, DynaRecExpressions.C);
                case PrefixCbOpCode.SLA_D:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeft, DynaRecExpressions.D);
                case PrefixCbOpCode.SLA_E:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeft, DynaRecExpressions.E);
                case PrefixCbOpCode.SLA_H:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeft, DynaRecExpressions.H);
                case PrefixCbOpCode.SLA_L:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeft, DynaRecExpressions.L);
                case PrefixCbOpCode.SLA_mHL:
                    return GetAluCallWrite(4, 15, DynaRecExpressions.AluShiftLeft);

                // SLS r (undocumented)
                case PrefixCbOpCode.SLS_A:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeftSet, DynaRecExpressions.A);
                case PrefixCbOpCode.SLS_B:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeftSet, DynaRecExpressions.B);
                case PrefixCbOpCode.SLS_C:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeftSet, DynaRecExpressions.C);
                case PrefixCbOpCode.SLS_D:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeftSet, DynaRecExpressions.D);
                case PrefixCbOpCode.SLS_E:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeftSet, DynaRecExpressions.E);
                case PrefixCbOpCode.SLS_H:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeftSet, DynaRecExpressions.H);
                case PrefixCbOpCode.SLS_L:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftLeftSet, DynaRecExpressions.L);
                case PrefixCbOpCode.SLS_mHL:
                    return GetAluCallWrite(4, 15, DynaRecExpressions.AluShiftLeftSet);

                // SRA r
                case PrefixCbOpCode.SRA_A:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRight, DynaRecExpressions.A);
                case PrefixCbOpCode.SRA_B:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRight, DynaRecExpressions.B);
                case PrefixCbOpCode.SRA_C:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRight, DynaRecExpressions.C);
                case PrefixCbOpCode.SRA_D:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRight, DynaRecExpressions.D);
                case PrefixCbOpCode.SRA_E:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRight, DynaRecExpressions.E);
                case PrefixCbOpCode.SRA_H:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRight, DynaRecExpressions.H);
                case PrefixCbOpCode.SRA_L:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRight, DynaRecExpressions.L);
                case PrefixCbOpCode.SRA_mHL:
                    return GetAluCallWrite(4, 15, DynaRecExpressions.AluShiftRight);
                    
                // SRL r
                case PrefixCbOpCode.SRL_A:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRightLogical, DynaRecExpressions.A);
                case PrefixCbOpCode.SRL_B:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRightLogical, DynaRecExpressions.B);
                case PrefixCbOpCode.SRL_C:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRightLogical, DynaRecExpressions.C);
                case PrefixCbOpCode.SRL_D:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRightLogical, DynaRecExpressions.D);
                case PrefixCbOpCode.SRL_E:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRightLogical, DynaRecExpressions.E);
                case PrefixCbOpCode.SRL_H:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRightLogical, DynaRecExpressions.H);
                case PrefixCbOpCode.SRL_L:
                    return GetAluCallAssign(2, 8, DynaRecExpressions.AluShiftRightLogical, DynaRecExpressions.L);
                case PrefixCbOpCode.SRL_mHL:
                    return GetAluCallWrite(4, 15, DynaRecExpressions.AluShiftRightLogical);

                // ********* Bit Test, Set & Reset *********
                // BIT r
                case PrefixCbOpCode.BIT_0_B:
                    return BitTest(DynaRecExpressions.B, 0); 
                case PrefixCbOpCode.BIT_1_B:
                    return BitTest(DynaRecExpressions.B, 1); 
                case PrefixCbOpCode.BIT_2_B:
                    return BitTest(DynaRecExpressions.B, 2); 
                case PrefixCbOpCode.BIT_3_B:
                    return BitTest(DynaRecExpressions.B, 3); 
                case PrefixCbOpCode.BIT_4_B:
                    return BitTest(DynaRecExpressions.B, 4); 
                case PrefixCbOpCode.BIT_5_B:
                    return BitTest(DynaRecExpressions.B, 5); 
                case PrefixCbOpCode.BIT_6_B:
                    return BitTest(DynaRecExpressions.B, 6); 
                case PrefixCbOpCode.BIT_7_B:
                    return BitTest(DynaRecExpressions.B, 7); 
                case PrefixCbOpCode.BIT_0_C:
                    return BitTest(DynaRecExpressions.C, 0); 
                case PrefixCbOpCode.BIT_1_C:
                    return BitTest(DynaRecExpressions.C, 1); 
                case PrefixCbOpCode.BIT_2_C:
                    return BitTest(DynaRecExpressions.C, 2); 
                case PrefixCbOpCode.BIT_3_C:
                    return BitTest(DynaRecExpressions.C, 3); 
                case PrefixCbOpCode.BIT_4_C:
                    return BitTest(DynaRecExpressions.C, 4); 
                case PrefixCbOpCode.BIT_5_C:
                    return BitTest(DynaRecExpressions.C, 5); 
                case PrefixCbOpCode.BIT_6_C:
                    return BitTest(DynaRecExpressions.C, 6); 
                case PrefixCbOpCode.BIT_7_C:
                    return BitTest(DynaRecExpressions.C, 7); 
                case PrefixCbOpCode.BIT_0_D:
                    return BitTest(DynaRecExpressions.D, 0); 
                case PrefixCbOpCode.BIT_1_D:
                    return BitTest(DynaRecExpressions.D, 1); 
                case PrefixCbOpCode.BIT_2_D:
                    return BitTest(DynaRecExpressions.D, 2); 
                case PrefixCbOpCode.BIT_3_D:
                    return BitTest(DynaRecExpressions.D, 3); 
                case PrefixCbOpCode.BIT_4_D:
                    return BitTest(DynaRecExpressions.D, 4); 
                case PrefixCbOpCode.BIT_5_D:
                    return BitTest(DynaRecExpressions.D, 5); 
                case PrefixCbOpCode.BIT_6_D:
                    return BitTest(DynaRecExpressions.D, 6); 
                case PrefixCbOpCode.BIT_7_D:
                    return BitTest(DynaRecExpressions.D, 7); 
                case PrefixCbOpCode.BIT_0_E:
                    return BitTest(DynaRecExpressions.E, 0); 
                case PrefixCbOpCode.BIT_1_E:
                    return BitTest(DynaRecExpressions.E, 1); 
                case PrefixCbOpCode.BIT_2_E:
                    return BitTest(DynaRecExpressions.E, 2); 
                case PrefixCbOpCode.BIT_3_E:
                    return BitTest(DynaRecExpressions.E, 3); 
                case PrefixCbOpCode.BIT_4_E:
                    return BitTest(DynaRecExpressions.E, 4); 
                case PrefixCbOpCode.BIT_5_E:
                    return BitTest(DynaRecExpressions.E, 5); 
                case PrefixCbOpCode.BIT_6_E:
                    return BitTest(DynaRecExpressions.E, 6); 
                case PrefixCbOpCode.BIT_7_E:
                    return BitTest(DynaRecExpressions.E, 7); 
                case PrefixCbOpCode.BIT_0_H:
                    return BitTest(DynaRecExpressions.H, 0); 
                case PrefixCbOpCode.BIT_1_H:
                    return BitTest(DynaRecExpressions.H, 1); 
                case PrefixCbOpCode.BIT_2_H:
                    return BitTest(DynaRecExpressions.H, 2); 
                case PrefixCbOpCode.BIT_3_H:
                    return BitTest(DynaRecExpressions.H, 3); 
                case PrefixCbOpCode.BIT_4_H:
                    return BitTest(DynaRecExpressions.H, 4); 
                case PrefixCbOpCode.BIT_5_H:
                    return BitTest(DynaRecExpressions.H, 5); 
                case PrefixCbOpCode.BIT_6_H:
                    return BitTest(DynaRecExpressions.H, 6); 
                case PrefixCbOpCode.BIT_7_H:
                    return BitTest(DynaRecExpressions.H, 7); 
                case PrefixCbOpCode.BIT_0_L:
                    return BitTest(DynaRecExpressions.L, 0); 
                case PrefixCbOpCode.BIT_1_L:
                    return BitTest(DynaRecExpressions.L, 1); 
                case PrefixCbOpCode.BIT_2_L:
                    return BitTest(DynaRecExpressions.L, 2); 
                case PrefixCbOpCode.BIT_3_L:
                    return BitTest(DynaRecExpressions.L, 3); 
                case PrefixCbOpCode.BIT_4_L:
                    return BitTest(DynaRecExpressions.L, 4); 
                case PrefixCbOpCode.BIT_5_L:
                    return BitTest(DynaRecExpressions.L, 5); 
                case PrefixCbOpCode.BIT_6_L:
                    return BitTest(DynaRecExpressions.L, 6); 
                case PrefixCbOpCode.BIT_7_L:
                    return BitTest(DynaRecExpressions.L, 7); 
                case PrefixCbOpCode.BIT_0_mHL:
                    return BitTestFromIndex(0); 
                case PrefixCbOpCode.BIT_1_mHL:
                    return BitTestFromIndex(1); 
                case PrefixCbOpCode.BIT_2_mHL:
                    return BitTestFromIndex(2); 
                case PrefixCbOpCode.BIT_3_mHL:
                    return BitTestFromIndex(3); 
                case PrefixCbOpCode.BIT_4_mHL:
                    return BitTestFromIndex(4); 
                case PrefixCbOpCode.BIT_5_mHL:
                    return BitTestFromIndex(5); 
                case PrefixCbOpCode.BIT_6_mHL:
                    return BitTestFromIndex(6); 
                case PrefixCbOpCode.BIT_7_mHL:
                    return BitTestFromIndex(7); 
                case PrefixCbOpCode.BIT_0_A:
                    return BitTest(DynaRecExpressions.A, 0); 
                case PrefixCbOpCode.BIT_1_A:
                    return BitTest(DynaRecExpressions.A, 1); 
                case PrefixCbOpCode.BIT_2_A:
                    return BitTest(DynaRecExpressions.A, 2); 
                case PrefixCbOpCode.BIT_3_A:
                    return BitTest(DynaRecExpressions.A, 3); 
                case PrefixCbOpCode.BIT_4_A:
                    return BitTest(DynaRecExpressions.A, 4); 
                case PrefixCbOpCode.BIT_5_A:
                    return BitTest(DynaRecExpressions.A, 5); 
                case PrefixCbOpCode.BIT_6_A:
                    return BitTest(DynaRecExpressions.A, 6); 
                case PrefixCbOpCode.BIT_7_A:
                    return BitTest(DynaRecExpressions.A, 7);

                // SET r
                case PrefixCbOpCode.SET_0_B:
                    return BitSet(DynaRecExpressions.B, 0); 
                case PrefixCbOpCode.SET_1_B:
                    return BitSet(DynaRecExpressions.B, 1); 
                case PrefixCbOpCode.SET_2_B:
                    return BitSet(DynaRecExpressions.B, 2); 
                case PrefixCbOpCode.SET_3_B:
                    return BitSet(DynaRecExpressions.B, 3); 
                case PrefixCbOpCode.SET_4_B:
                    return BitSet(DynaRecExpressions.B, 4); 
                case PrefixCbOpCode.SET_5_B:
                    return BitSet(DynaRecExpressions.B, 5); 
                case PrefixCbOpCode.SET_6_B:
                    return BitSet(DynaRecExpressions.B, 6); 
                case PrefixCbOpCode.SET_7_B:
                    return BitSet(DynaRecExpressions.B, 7); 
                case PrefixCbOpCode.SET_0_C:
                    return BitSet(DynaRecExpressions.C, 0); 
                case PrefixCbOpCode.SET_1_C:
                    return BitSet(DynaRecExpressions.C, 1); 
                case PrefixCbOpCode.SET_2_C:
                    return BitSet(DynaRecExpressions.C, 2); 
                case PrefixCbOpCode.SET_3_C:
                    return BitSet(DynaRecExpressions.C, 3); 
                case PrefixCbOpCode.SET_4_C:
                    return BitSet(DynaRecExpressions.C, 4); 
                case PrefixCbOpCode.SET_5_C:
                    return BitSet(DynaRecExpressions.C, 5); 
                case PrefixCbOpCode.SET_6_C:
                    return BitSet(DynaRecExpressions.C, 6); 
                case PrefixCbOpCode.SET_7_C:
                    return BitSet(DynaRecExpressions.C, 7); 
                case PrefixCbOpCode.SET_0_D:
                    return BitSet(DynaRecExpressions.D, 0); 
                case PrefixCbOpCode.SET_1_D:
                    return BitSet(DynaRecExpressions.D, 1); 
                case PrefixCbOpCode.SET_2_D:
                    return BitSet(DynaRecExpressions.D, 2); 
                case PrefixCbOpCode.SET_3_D:
                    return BitSet(DynaRecExpressions.D, 3); 
                case PrefixCbOpCode.SET_4_D:
                    return BitSet(DynaRecExpressions.D, 4); 
                case PrefixCbOpCode.SET_5_D:
                    return BitSet(DynaRecExpressions.D, 5); 
                case PrefixCbOpCode.SET_6_D:
                    return BitSet(DynaRecExpressions.D, 6); 
                case PrefixCbOpCode.SET_7_D:
                    return BitSet(DynaRecExpressions.D, 7); 
                case PrefixCbOpCode.SET_0_E:
                    return BitSet(DynaRecExpressions.E, 0); 
                case PrefixCbOpCode.SET_1_E:
                    return BitSet(DynaRecExpressions.E, 1); 
                case PrefixCbOpCode.SET_2_E:
                    return BitSet(DynaRecExpressions.E, 2); 
                case PrefixCbOpCode.SET_3_E:
                    return BitSet(DynaRecExpressions.E, 3); 
                case PrefixCbOpCode.SET_4_E:
                    return BitSet(DynaRecExpressions.E, 4); 
                case PrefixCbOpCode.SET_5_E:
                    return BitSet(DynaRecExpressions.E, 5); 
                case PrefixCbOpCode.SET_6_E:
                    return BitSet(DynaRecExpressions.E, 6); 
                case PrefixCbOpCode.SET_7_E:
                    return BitSet(DynaRecExpressions.E, 7); 
                case PrefixCbOpCode.SET_0_H:
                    return BitSet(DynaRecExpressions.H, 0); 
                case PrefixCbOpCode.SET_1_H:
                    return BitSet(DynaRecExpressions.H, 1); 
                case PrefixCbOpCode.SET_2_H:
                    return BitSet(DynaRecExpressions.H, 2); 
                case PrefixCbOpCode.SET_3_H:
                    return BitSet(DynaRecExpressions.H, 3); 
                case PrefixCbOpCode.SET_4_H:
                    return BitSet(DynaRecExpressions.H, 4); 
                case PrefixCbOpCode.SET_5_H:
                    return BitSet(DynaRecExpressions.H, 5); 
                case PrefixCbOpCode.SET_6_H:
                    return BitSet(DynaRecExpressions.H, 6); 
                case PrefixCbOpCode.SET_7_H:
                    return BitSet(DynaRecExpressions.H, 7); 
                case PrefixCbOpCode.SET_0_L:
                    return BitSet(DynaRecExpressions.L, 0); 
                case PrefixCbOpCode.SET_1_L:
                    return BitSet(DynaRecExpressions.L, 1); 
                case PrefixCbOpCode.SET_2_L:
                    return BitSet(DynaRecExpressions.L, 2); 
                case PrefixCbOpCode.SET_3_L:
                    return BitSet(DynaRecExpressions.L, 3); 
                case PrefixCbOpCode.SET_4_L:
                    return BitSet(DynaRecExpressions.L, 4); 
                case PrefixCbOpCode.SET_5_L:
                    return BitSet(DynaRecExpressions.L, 5); 
                case PrefixCbOpCode.SET_6_L:
                    return BitSet(DynaRecExpressions.L, 6); 
                case PrefixCbOpCode.SET_7_L:
                    return BitSet(DynaRecExpressions.L, 7); 
                case PrefixCbOpCode.SET_0_mHL:
                    return BitSetFromIndex(0); 
                case PrefixCbOpCode.SET_1_mHL:
                    return BitSetFromIndex(1); 
                case PrefixCbOpCode.SET_2_mHL:
                    return BitSetFromIndex(2); 
                case PrefixCbOpCode.SET_3_mHL:
                    return BitSetFromIndex(3); 
                case PrefixCbOpCode.SET_4_mHL:
                    return BitSetFromIndex(4); 
                case PrefixCbOpCode.SET_5_mHL:
                    return BitSetFromIndex(5); 
                case PrefixCbOpCode.SET_6_mHL:
                    return BitSetFromIndex(6); 
                case PrefixCbOpCode.SET_7_mHL:
                    return BitSetFromIndex(7); 
                case PrefixCbOpCode.SET_0_A:
                    return BitSet(DynaRecExpressions.A, 0); 
                case PrefixCbOpCode.SET_1_A:
                    return BitSet(DynaRecExpressions.A, 1); 
                case PrefixCbOpCode.SET_2_A:
                    return BitSet(DynaRecExpressions.A, 2); 
                case PrefixCbOpCode.SET_3_A:
                    return BitSet(DynaRecExpressions.A, 3); 
                case PrefixCbOpCode.SET_4_A:
                    return BitSet(DynaRecExpressions.A, 4); 
                case PrefixCbOpCode.SET_5_A:
                    return BitSet(DynaRecExpressions.A, 5); 
                case PrefixCbOpCode.SET_6_A:
                    return BitSet(DynaRecExpressions.A, 6); 
                case PrefixCbOpCode.SET_7_A:
                    return BitSet(DynaRecExpressions.A, 7);

                // RES r
                case PrefixCbOpCode.RES_0_B:
                    return BitReset(DynaRecExpressions.B, 0); 
                case PrefixCbOpCode.RES_1_B:
                    return BitReset(DynaRecExpressions.B, 1); 
                case PrefixCbOpCode.RES_2_B:
                    return BitReset(DynaRecExpressions.B, 2); 
                case PrefixCbOpCode.RES_3_B:
                    return BitReset(DynaRecExpressions.B, 3); 
                case PrefixCbOpCode.RES_4_B:
                    return BitReset(DynaRecExpressions.B, 4); 
                case PrefixCbOpCode.RES_5_B:
                    return BitReset(DynaRecExpressions.B, 5); 
                case PrefixCbOpCode.RES_6_B:
                    return BitReset(DynaRecExpressions.B, 6); 
                case PrefixCbOpCode.RES_7_B:
                    return BitReset(DynaRecExpressions.B, 7); 
                case PrefixCbOpCode.RES_0_C:
                    return BitReset(DynaRecExpressions.C, 0); 
                case PrefixCbOpCode.RES_1_C:
                    return BitReset(DynaRecExpressions.C, 1); 
                case PrefixCbOpCode.RES_2_C:
                    return BitReset(DynaRecExpressions.C, 2); 
                case PrefixCbOpCode.RES_3_C:
                    return BitReset(DynaRecExpressions.C, 3); 
                case PrefixCbOpCode.RES_4_C:
                    return BitReset(DynaRecExpressions.C, 4); 
                case PrefixCbOpCode.RES_5_C:
                    return BitReset(DynaRecExpressions.C, 5); 
                case PrefixCbOpCode.RES_6_C:
                    return BitReset(DynaRecExpressions.C, 6); 
                case PrefixCbOpCode.RES_7_C:
                    return BitReset(DynaRecExpressions.C, 7); 
                case PrefixCbOpCode.RES_0_D:
                    return BitReset(DynaRecExpressions.D, 0); 
                case PrefixCbOpCode.RES_1_D:
                    return BitReset(DynaRecExpressions.D, 1); 
                case PrefixCbOpCode.RES_2_D:
                    return BitReset(DynaRecExpressions.D, 2); 
                case PrefixCbOpCode.RES_3_D:
                    return BitReset(DynaRecExpressions.D, 3); 
                case PrefixCbOpCode.RES_4_D:
                    return BitReset(DynaRecExpressions.D, 4); 
                case PrefixCbOpCode.RES_5_D:
                    return BitReset(DynaRecExpressions.D, 5); 
                case PrefixCbOpCode.RES_6_D:
                    return BitReset(DynaRecExpressions.D, 6); 
                case PrefixCbOpCode.RES_7_D:
                    return BitReset(DynaRecExpressions.D, 7); 
                case PrefixCbOpCode.RES_0_E:
                    return BitReset(DynaRecExpressions.E, 0); 
                case PrefixCbOpCode.RES_1_E:
                    return BitReset(DynaRecExpressions.E, 1); 
                case PrefixCbOpCode.RES_2_E:
                    return BitReset(DynaRecExpressions.E, 2); 
                case PrefixCbOpCode.RES_3_E:
                    return BitReset(DynaRecExpressions.E, 3); 
                case PrefixCbOpCode.RES_4_E:
                    return BitReset(DynaRecExpressions.E, 4); 
                case PrefixCbOpCode.RES_5_E:
                    return BitReset(DynaRecExpressions.E, 5); 
                case PrefixCbOpCode.RES_6_E:
                    return BitReset(DynaRecExpressions.E, 6); 
                case PrefixCbOpCode.RES_7_E:
                    return BitReset(DynaRecExpressions.E, 7); 
                case PrefixCbOpCode.RES_0_H:
                    return BitReset(DynaRecExpressions.H, 0); 
                case PrefixCbOpCode.RES_1_H:
                    return BitReset(DynaRecExpressions.H, 1); 
                case PrefixCbOpCode.RES_2_H:
                    return BitReset(DynaRecExpressions.H, 2); 
                case PrefixCbOpCode.RES_3_H:
                    return BitReset(DynaRecExpressions.H, 3); 
                case PrefixCbOpCode.RES_4_H:
                    return BitReset(DynaRecExpressions.H, 4); 
                case PrefixCbOpCode.RES_5_H:
                    return BitReset(DynaRecExpressions.H, 5); 
                case PrefixCbOpCode.RES_6_H:
                    return BitReset(DynaRecExpressions.H, 6); 
                case PrefixCbOpCode.RES_7_H:
                    return BitReset(DynaRecExpressions.H, 7); 
                case PrefixCbOpCode.RES_0_L:
                    return BitReset(DynaRecExpressions.L, 0); 
                case PrefixCbOpCode.RES_1_L:
                    return BitReset(DynaRecExpressions.L, 1); 
                case PrefixCbOpCode.RES_2_L:
                    return BitReset(DynaRecExpressions.L, 2); 
                case PrefixCbOpCode.RES_3_L:
                    return BitReset(DynaRecExpressions.L, 3); 
                case PrefixCbOpCode.RES_4_L:
                    return BitReset(DynaRecExpressions.L, 4); 
                case PrefixCbOpCode.RES_5_L:
                    return BitReset(DynaRecExpressions.L, 5); 
                case PrefixCbOpCode.RES_6_L:
                    return BitReset(DynaRecExpressions.L, 6); 
                case PrefixCbOpCode.RES_7_L:
                    return BitReset(DynaRecExpressions.L, 7); 
                case PrefixCbOpCode.RES_0_mHL:
                    return BitResetFromIndex(0); 
                case PrefixCbOpCode.RES_1_mHL:
                    return BitResetFromIndex(1); 
                case PrefixCbOpCode.RES_2_mHL:
                    return BitResetFromIndex(2); 
                case PrefixCbOpCode.RES_3_mHL:
                    return BitResetFromIndex(3); 
                case PrefixCbOpCode.RES_4_mHL:
                    return BitResetFromIndex(4); 
                case PrefixCbOpCode.RES_5_mHL:
                    return BitResetFromIndex(5); 
                case PrefixCbOpCode.RES_6_mHL:
                    return BitResetFromIndex(6); 
                case PrefixCbOpCode.RES_7_mHL:
                    return BitResetFromIndex(7); 
                case PrefixCbOpCode.RES_0_A:
                    return BitReset(DynaRecExpressions.A, 0); 
                case PrefixCbOpCode.RES_1_A:
                    return BitReset(DynaRecExpressions.A, 1); 
                case PrefixCbOpCode.RES_2_A:
                    return BitReset(DynaRecExpressions.A, 2); 
                case PrefixCbOpCode.RES_3_A:
                    return BitReset(DynaRecExpressions.A, 3); 
                case PrefixCbOpCode.RES_4_A:
                    return BitReset(DynaRecExpressions.A, 4); 
                case PrefixCbOpCode.RES_5_A:
                    return BitReset(DynaRecExpressions.A, 5); 
                case PrefixCbOpCode.RES_6_A:
                    return BitReset(DynaRecExpressions.A, 6); 
                case PrefixCbOpCode.RES_7_A:
                    return BitReset(DynaRecExpressions.A, 7); 
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}
