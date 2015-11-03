namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    using Xpr = DynaRecExpressions;

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
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.A);
                case PrefixCbOpCode.RLC_B:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.B);
                case PrefixCbOpCode.RLC_C:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.C);
                case PrefixCbOpCode.RLC_D:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.D);
                case PrefixCbOpCode.RLC_E:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.E);
                case PrefixCbOpCode.RLC_H:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.H);
                case PrefixCbOpCode.RLC_L:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeftWithCarry, Xpr.L);
                case PrefixCbOpCode.RLC_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluRotateLeftWithCarry);
                    
                // RL r
                case PrefixCbOpCode.RL_A:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.A);
                case PrefixCbOpCode.RL_B:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.B);
                case PrefixCbOpCode.RL_C:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.C);
                case PrefixCbOpCode.RL_D:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.D);
                case PrefixCbOpCode.RL_E:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.E);
                case PrefixCbOpCode.RL_H:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.H);
                case PrefixCbOpCode.RL_L:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateLeft, Xpr.L);
                case PrefixCbOpCode.RL_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluRotateLeft);
                    
                // RRC r
                case PrefixCbOpCode.RRC_A:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.A);
                case PrefixCbOpCode.RRC_B:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.B);
                case PrefixCbOpCode.RRC_C:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.C);
                case PrefixCbOpCode.RRC_D:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.D);
                case PrefixCbOpCode.RRC_E:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.E);
                case PrefixCbOpCode.RRC_H:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.H);
                case PrefixCbOpCode.RRC_L:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRightWithCarry, Xpr.L);
                case PrefixCbOpCode.RRC_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluRotateRightWithCarry);
                    
                // RR r
                case PrefixCbOpCode.RR_A:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.A);
                case PrefixCbOpCode.RR_B:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.B);
                case PrefixCbOpCode.RR_C:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.C);
                case PrefixCbOpCode.RR_D:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.D);
                case PrefixCbOpCode.RR_E:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.E);
                case PrefixCbOpCode.RR_H:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.H);
                case PrefixCbOpCode.RR_L:
                    return GetAluCallAssign(2, 8, Xpr.AluRotateRight, Xpr.L);
                case PrefixCbOpCode.RR_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluRotateRight);

                // ********* Shift *********
                // SLA r
                case PrefixCbOpCode.SLA_A:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.A);
                case PrefixCbOpCode.SLA_B:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.B);
                case PrefixCbOpCode.SLA_C:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.C);
                case PrefixCbOpCode.SLA_D:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.D);
                case PrefixCbOpCode.SLA_E:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.E);
                case PrefixCbOpCode.SLA_H:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.H);
                case PrefixCbOpCode.SLA_L:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftLeft, Xpr.L);
                case PrefixCbOpCode.SLA_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluShiftLeft);

                // SLS r (undocumented)
                case PrefixCbOpCode.SLS_A:
                    return GetAluCallAssign(2, 8, this.cpuMode == CpuMode.GameBoy ? Xpr.AluSwap : Xpr.AluShiftLeftSet, Xpr.A);
                case PrefixCbOpCode.SLS_B:
                    return GetAluCallAssign(2, 8, this.cpuMode == CpuMode.GameBoy ? Xpr.AluSwap : Xpr.AluShiftLeftSet, Xpr.B);
                case PrefixCbOpCode.SLS_C:
                    return GetAluCallAssign(2, 8, this.cpuMode == CpuMode.GameBoy ? Xpr.AluSwap : Xpr.AluShiftLeftSet, Xpr.C);
                case PrefixCbOpCode.SLS_D:
                    return GetAluCallAssign(2, 8, this.cpuMode == CpuMode.GameBoy ? Xpr.AluSwap : Xpr.AluShiftLeftSet, Xpr.D);
                case PrefixCbOpCode.SLS_E:
                    return GetAluCallAssign(2, 8, this.cpuMode == CpuMode.GameBoy ? Xpr.AluSwap : Xpr.AluShiftLeftSet, Xpr.E);
                case PrefixCbOpCode.SLS_H:
                    return GetAluCallAssign(2, 8, this.cpuMode == CpuMode.GameBoy ? Xpr.AluSwap : Xpr.AluShiftLeftSet, Xpr.H);
                case PrefixCbOpCode.SLS_L:
                    return GetAluCallAssign(2, 8, this.cpuMode == CpuMode.GameBoy ? Xpr.AluSwap : Xpr.AluShiftLeftSet, Xpr.L);
                case PrefixCbOpCode.SLS_mHL:
                    return GetAluCallWrite(4, 15, this.cpuMode == CpuMode.GameBoy ? Xpr.AluSwap : Xpr.AluShiftLeftSet);

                // SRA r
                case PrefixCbOpCode.SRA_A:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.A);
                case PrefixCbOpCode.SRA_B:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.B);
                case PrefixCbOpCode.SRA_C:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.C);
                case PrefixCbOpCode.SRA_D:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.D);
                case PrefixCbOpCode.SRA_E:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.E);
                case PrefixCbOpCode.SRA_H:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.H);
                case PrefixCbOpCode.SRA_L:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRight, Xpr.L);
                case PrefixCbOpCode.SRA_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluShiftRight);
                    
                // SRL r
                case PrefixCbOpCode.SRL_A:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.A);
                case PrefixCbOpCode.SRL_B:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.B);
                case PrefixCbOpCode.SRL_C:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.C);
                case PrefixCbOpCode.SRL_D:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.D);
                case PrefixCbOpCode.SRL_E:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.E);
                case PrefixCbOpCode.SRL_H:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.H);
                case PrefixCbOpCode.SRL_L:
                    return GetAluCallAssign(2, 8, Xpr.AluShiftRightLogical, Xpr.L);
                case PrefixCbOpCode.SRL_mHL:
                    return GetAluCallWrite(4, 15, Xpr.AluShiftRightLogical);

                // ********* Bit Test, Set & Reset *********
                // BIT r
                case PrefixCbOpCode.BIT_0_B:
                    return BitTest(Xpr.B, 0); 
                case PrefixCbOpCode.BIT_1_B:
                    return BitTest(Xpr.B, 1); 
                case PrefixCbOpCode.BIT_2_B:
                    return BitTest(Xpr.B, 2); 
                case PrefixCbOpCode.BIT_3_B:
                    return BitTest(Xpr.B, 3); 
                case PrefixCbOpCode.BIT_4_B:
                    return BitTest(Xpr.B, 4); 
                case PrefixCbOpCode.BIT_5_B:
                    return BitTest(Xpr.B, 5); 
                case PrefixCbOpCode.BIT_6_B:
                    return BitTest(Xpr.B, 6); 
                case PrefixCbOpCode.BIT_7_B:
                    return BitTest(Xpr.B, 7); 
                case PrefixCbOpCode.BIT_0_C:
                    return BitTest(Xpr.C, 0); 
                case PrefixCbOpCode.BIT_1_C:
                    return BitTest(Xpr.C, 1); 
                case PrefixCbOpCode.BIT_2_C:
                    return BitTest(Xpr.C, 2); 
                case PrefixCbOpCode.BIT_3_C:
                    return BitTest(Xpr.C, 3); 
                case PrefixCbOpCode.BIT_4_C:
                    return BitTest(Xpr.C, 4); 
                case PrefixCbOpCode.BIT_5_C:
                    return BitTest(Xpr.C, 5); 
                case PrefixCbOpCode.BIT_6_C:
                    return BitTest(Xpr.C, 6); 
                case PrefixCbOpCode.BIT_7_C:
                    return BitTest(Xpr.C, 7); 
                case PrefixCbOpCode.BIT_0_D:
                    return BitTest(Xpr.D, 0); 
                case PrefixCbOpCode.BIT_1_D:
                    return BitTest(Xpr.D, 1); 
                case PrefixCbOpCode.BIT_2_D:
                    return BitTest(Xpr.D, 2); 
                case PrefixCbOpCode.BIT_3_D:
                    return BitTest(Xpr.D, 3); 
                case PrefixCbOpCode.BIT_4_D:
                    return BitTest(Xpr.D, 4); 
                case PrefixCbOpCode.BIT_5_D:
                    return BitTest(Xpr.D, 5); 
                case PrefixCbOpCode.BIT_6_D:
                    return BitTest(Xpr.D, 6); 
                case PrefixCbOpCode.BIT_7_D:
                    return BitTest(Xpr.D, 7); 
                case PrefixCbOpCode.BIT_0_E:
                    return BitTest(Xpr.E, 0); 
                case PrefixCbOpCode.BIT_1_E:
                    return BitTest(Xpr.E, 1); 
                case PrefixCbOpCode.BIT_2_E:
                    return BitTest(Xpr.E, 2); 
                case PrefixCbOpCode.BIT_3_E:
                    return BitTest(Xpr.E, 3); 
                case PrefixCbOpCode.BIT_4_E:
                    return BitTest(Xpr.E, 4); 
                case PrefixCbOpCode.BIT_5_E:
                    return BitTest(Xpr.E, 5); 
                case PrefixCbOpCode.BIT_6_E:
                    return BitTest(Xpr.E, 6); 
                case PrefixCbOpCode.BIT_7_E:
                    return BitTest(Xpr.E, 7); 
                case PrefixCbOpCode.BIT_0_H:
                    return BitTest(Xpr.H, 0); 
                case PrefixCbOpCode.BIT_1_H:
                    return BitTest(Xpr.H, 1); 
                case PrefixCbOpCode.BIT_2_H:
                    return BitTest(Xpr.H, 2); 
                case PrefixCbOpCode.BIT_3_H:
                    return BitTest(Xpr.H, 3); 
                case PrefixCbOpCode.BIT_4_H:
                    return BitTest(Xpr.H, 4); 
                case PrefixCbOpCode.BIT_5_H:
                    return BitTest(Xpr.H, 5); 
                case PrefixCbOpCode.BIT_6_H:
                    return BitTest(Xpr.H, 6); 
                case PrefixCbOpCode.BIT_7_H:
                    return BitTest(Xpr.H, 7); 
                case PrefixCbOpCode.BIT_0_L:
                    return BitTest(Xpr.L, 0); 
                case PrefixCbOpCode.BIT_1_L:
                    return BitTest(Xpr.L, 1); 
                case PrefixCbOpCode.BIT_2_L:
                    return BitTest(Xpr.L, 2); 
                case PrefixCbOpCode.BIT_3_L:
                    return BitTest(Xpr.L, 3); 
                case PrefixCbOpCode.BIT_4_L:
                    return BitTest(Xpr.L, 4); 
                case PrefixCbOpCode.BIT_5_L:
                    return BitTest(Xpr.L, 5); 
                case PrefixCbOpCode.BIT_6_L:
                    return BitTest(Xpr.L, 6); 
                case PrefixCbOpCode.BIT_7_L:
                    return BitTest(Xpr.L, 7); 
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
                    return BitTest(Xpr.A, 0); 
                case PrefixCbOpCode.BIT_1_A:
                    return BitTest(Xpr.A, 1); 
                case PrefixCbOpCode.BIT_2_A:
                    return BitTest(Xpr.A, 2); 
                case PrefixCbOpCode.BIT_3_A:
                    return BitTest(Xpr.A, 3); 
                case PrefixCbOpCode.BIT_4_A:
                    return BitTest(Xpr.A, 4); 
                case PrefixCbOpCode.BIT_5_A:
                    return BitTest(Xpr.A, 5); 
                case PrefixCbOpCode.BIT_6_A:
                    return BitTest(Xpr.A, 6); 
                case PrefixCbOpCode.BIT_7_A:
                    return BitTest(Xpr.A, 7);

                // SET r
                case PrefixCbOpCode.SET_0_B:
                    return BitSet(Xpr.B, 0); 
                case PrefixCbOpCode.SET_1_B:
                    return BitSet(Xpr.B, 1); 
                case PrefixCbOpCode.SET_2_B:
                    return BitSet(Xpr.B, 2); 
                case PrefixCbOpCode.SET_3_B:
                    return BitSet(Xpr.B, 3); 
                case PrefixCbOpCode.SET_4_B:
                    return BitSet(Xpr.B, 4); 
                case PrefixCbOpCode.SET_5_B:
                    return BitSet(Xpr.B, 5); 
                case PrefixCbOpCode.SET_6_B:
                    return BitSet(Xpr.B, 6); 
                case PrefixCbOpCode.SET_7_B:
                    return BitSet(Xpr.B, 7); 
                case PrefixCbOpCode.SET_0_C:
                    return BitSet(Xpr.C, 0); 
                case PrefixCbOpCode.SET_1_C:
                    return BitSet(Xpr.C, 1); 
                case PrefixCbOpCode.SET_2_C:
                    return BitSet(Xpr.C, 2); 
                case PrefixCbOpCode.SET_3_C:
                    return BitSet(Xpr.C, 3); 
                case PrefixCbOpCode.SET_4_C:
                    return BitSet(Xpr.C, 4); 
                case PrefixCbOpCode.SET_5_C:
                    return BitSet(Xpr.C, 5); 
                case PrefixCbOpCode.SET_6_C:
                    return BitSet(Xpr.C, 6); 
                case PrefixCbOpCode.SET_7_C:
                    return BitSet(Xpr.C, 7); 
                case PrefixCbOpCode.SET_0_D:
                    return BitSet(Xpr.D, 0); 
                case PrefixCbOpCode.SET_1_D:
                    return BitSet(Xpr.D, 1); 
                case PrefixCbOpCode.SET_2_D:
                    return BitSet(Xpr.D, 2); 
                case PrefixCbOpCode.SET_3_D:
                    return BitSet(Xpr.D, 3); 
                case PrefixCbOpCode.SET_4_D:
                    return BitSet(Xpr.D, 4); 
                case PrefixCbOpCode.SET_5_D:
                    return BitSet(Xpr.D, 5); 
                case PrefixCbOpCode.SET_6_D:
                    return BitSet(Xpr.D, 6); 
                case PrefixCbOpCode.SET_7_D:
                    return BitSet(Xpr.D, 7); 
                case PrefixCbOpCode.SET_0_E:
                    return BitSet(Xpr.E, 0); 
                case PrefixCbOpCode.SET_1_E:
                    return BitSet(Xpr.E, 1); 
                case PrefixCbOpCode.SET_2_E:
                    return BitSet(Xpr.E, 2); 
                case PrefixCbOpCode.SET_3_E:
                    return BitSet(Xpr.E, 3); 
                case PrefixCbOpCode.SET_4_E:
                    return BitSet(Xpr.E, 4); 
                case PrefixCbOpCode.SET_5_E:
                    return BitSet(Xpr.E, 5); 
                case PrefixCbOpCode.SET_6_E:
                    return BitSet(Xpr.E, 6); 
                case PrefixCbOpCode.SET_7_E:
                    return BitSet(Xpr.E, 7); 
                case PrefixCbOpCode.SET_0_H:
                    return BitSet(Xpr.H, 0); 
                case PrefixCbOpCode.SET_1_H:
                    return BitSet(Xpr.H, 1); 
                case PrefixCbOpCode.SET_2_H:
                    return BitSet(Xpr.H, 2); 
                case PrefixCbOpCode.SET_3_H:
                    return BitSet(Xpr.H, 3); 
                case PrefixCbOpCode.SET_4_H:
                    return BitSet(Xpr.H, 4); 
                case PrefixCbOpCode.SET_5_H:
                    return BitSet(Xpr.H, 5); 
                case PrefixCbOpCode.SET_6_H:
                    return BitSet(Xpr.H, 6); 
                case PrefixCbOpCode.SET_7_H:
                    return BitSet(Xpr.H, 7); 
                case PrefixCbOpCode.SET_0_L:
                    return BitSet(Xpr.L, 0); 
                case PrefixCbOpCode.SET_1_L:
                    return BitSet(Xpr.L, 1); 
                case PrefixCbOpCode.SET_2_L:
                    return BitSet(Xpr.L, 2); 
                case PrefixCbOpCode.SET_3_L:
                    return BitSet(Xpr.L, 3); 
                case PrefixCbOpCode.SET_4_L:
                    return BitSet(Xpr.L, 4); 
                case PrefixCbOpCode.SET_5_L:
                    return BitSet(Xpr.L, 5); 
                case PrefixCbOpCode.SET_6_L:
                    return BitSet(Xpr.L, 6); 
                case PrefixCbOpCode.SET_7_L:
                    return BitSet(Xpr.L, 7); 
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
                    return BitSet(Xpr.A, 0); 
                case PrefixCbOpCode.SET_1_A:
                    return BitSet(Xpr.A, 1); 
                case PrefixCbOpCode.SET_2_A:
                    return BitSet(Xpr.A, 2); 
                case PrefixCbOpCode.SET_3_A:
                    return BitSet(Xpr.A, 3); 
                case PrefixCbOpCode.SET_4_A:
                    return BitSet(Xpr.A, 4); 
                case PrefixCbOpCode.SET_5_A:
                    return BitSet(Xpr.A, 5); 
                case PrefixCbOpCode.SET_6_A:
                    return BitSet(Xpr.A, 6); 
                case PrefixCbOpCode.SET_7_A:
                    return BitSet(Xpr.A, 7);

                // RES r
                case PrefixCbOpCode.RES_0_B:
                    return BitReset(Xpr.B, 0); 
                case PrefixCbOpCode.RES_1_B:
                    return BitReset(Xpr.B, 1); 
                case PrefixCbOpCode.RES_2_B:
                    return BitReset(Xpr.B, 2); 
                case PrefixCbOpCode.RES_3_B:
                    return BitReset(Xpr.B, 3); 
                case PrefixCbOpCode.RES_4_B:
                    return BitReset(Xpr.B, 4); 
                case PrefixCbOpCode.RES_5_B:
                    return BitReset(Xpr.B, 5); 
                case PrefixCbOpCode.RES_6_B:
                    return BitReset(Xpr.B, 6); 
                case PrefixCbOpCode.RES_7_B:
                    return BitReset(Xpr.B, 7); 
                case PrefixCbOpCode.RES_0_C:
                    return BitReset(Xpr.C, 0); 
                case PrefixCbOpCode.RES_1_C:
                    return BitReset(Xpr.C, 1); 
                case PrefixCbOpCode.RES_2_C:
                    return BitReset(Xpr.C, 2); 
                case PrefixCbOpCode.RES_3_C:
                    return BitReset(Xpr.C, 3); 
                case PrefixCbOpCode.RES_4_C:
                    return BitReset(Xpr.C, 4); 
                case PrefixCbOpCode.RES_5_C:
                    return BitReset(Xpr.C, 5); 
                case PrefixCbOpCode.RES_6_C:
                    return BitReset(Xpr.C, 6); 
                case PrefixCbOpCode.RES_7_C:
                    return BitReset(Xpr.C, 7); 
                case PrefixCbOpCode.RES_0_D:
                    return BitReset(Xpr.D, 0); 
                case PrefixCbOpCode.RES_1_D:
                    return BitReset(Xpr.D, 1); 
                case PrefixCbOpCode.RES_2_D:
                    return BitReset(Xpr.D, 2); 
                case PrefixCbOpCode.RES_3_D:
                    return BitReset(Xpr.D, 3); 
                case PrefixCbOpCode.RES_4_D:
                    return BitReset(Xpr.D, 4); 
                case PrefixCbOpCode.RES_5_D:
                    return BitReset(Xpr.D, 5); 
                case PrefixCbOpCode.RES_6_D:
                    return BitReset(Xpr.D, 6); 
                case PrefixCbOpCode.RES_7_D:
                    return BitReset(Xpr.D, 7); 
                case PrefixCbOpCode.RES_0_E:
                    return BitReset(Xpr.E, 0); 
                case PrefixCbOpCode.RES_1_E:
                    return BitReset(Xpr.E, 1); 
                case PrefixCbOpCode.RES_2_E:
                    return BitReset(Xpr.E, 2); 
                case PrefixCbOpCode.RES_3_E:
                    return BitReset(Xpr.E, 3); 
                case PrefixCbOpCode.RES_4_E:
                    return BitReset(Xpr.E, 4); 
                case PrefixCbOpCode.RES_5_E:
                    return BitReset(Xpr.E, 5); 
                case PrefixCbOpCode.RES_6_E:
                    return BitReset(Xpr.E, 6); 
                case PrefixCbOpCode.RES_7_E:
                    return BitReset(Xpr.E, 7); 
                case PrefixCbOpCode.RES_0_H:
                    return BitReset(Xpr.H, 0); 
                case PrefixCbOpCode.RES_1_H:
                    return BitReset(Xpr.H, 1); 
                case PrefixCbOpCode.RES_2_H:
                    return BitReset(Xpr.H, 2); 
                case PrefixCbOpCode.RES_3_H:
                    return BitReset(Xpr.H, 3); 
                case PrefixCbOpCode.RES_4_H:
                    return BitReset(Xpr.H, 4); 
                case PrefixCbOpCode.RES_5_H:
                    return BitReset(Xpr.H, 5); 
                case PrefixCbOpCode.RES_6_H:
                    return BitReset(Xpr.H, 6); 
                case PrefixCbOpCode.RES_7_H:
                    return BitReset(Xpr.H, 7); 
                case PrefixCbOpCode.RES_0_L:
                    return BitReset(Xpr.L, 0); 
                case PrefixCbOpCode.RES_1_L:
                    return BitReset(Xpr.L, 1); 
                case PrefixCbOpCode.RES_2_L:
                    return BitReset(Xpr.L, 2); 
                case PrefixCbOpCode.RES_3_L:
                    return BitReset(Xpr.L, 3); 
                case PrefixCbOpCode.RES_4_L:
                    return BitReset(Xpr.L, 4); 
                case PrefixCbOpCode.RES_5_L:
                    return BitReset(Xpr.L, 5); 
                case PrefixCbOpCode.RES_6_L:
                    return BitReset(Xpr.L, 6); 
                case PrefixCbOpCode.RES_7_L:
                    return BitReset(Xpr.L, 7); 
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
                    return BitReset(Xpr.A, 0); 
                case PrefixCbOpCode.RES_1_A:
                    return BitReset(Xpr.A, 1); 
                case PrefixCbOpCode.RES_2_A:
                    return BitReset(Xpr.A, 2); 
                case PrefixCbOpCode.RES_3_A:
                    return BitReset(Xpr.A, 3); 
                case PrefixCbOpCode.RES_4_A:
                    return BitReset(Xpr.A, 4); 
                case PrefixCbOpCode.RES_5_A:
                    return BitReset(Xpr.A, 5); 
                case PrefixCbOpCode.RES_6_A:
                    return BitReset(Xpr.A, 6); 
                case PrefixCbOpCode.RES_7_A:
                    return BitReset(Xpr.A, 7); 
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}
