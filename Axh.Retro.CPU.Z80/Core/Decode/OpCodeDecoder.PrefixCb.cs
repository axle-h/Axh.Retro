namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System;

    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;

    internal partial class OpCodeDecoder
    {
        private Operation FixPrefixDdFdPrefixCbResult(Operation result)
        {
            if (result.Operand1 == index.Index)
            {
                // Documented operation, no autocopy
                return result;
            }

            // Add index operand
            result.Operand2 = index.Index;

            if (result.Opcode != Opcode.BitTest)
            {
                // Only BIT has no autocopy
                // Add autocopy timings
                result.AutoCopy();
                timer.AutoCopy();
            }
            
            return result;
        }

        private Operation DecodePrefixCb()
        {
            var code = (PrefixCbOpCode)prefetch.NextByte();

            timer.Nop();

            switch (code)
            {
                // ********* Rotate *********
                // RLC r
                case PrefixCbOpCode.RLC_A:
                    return new Operation(Opcode.RotateLeftWithCarry, Operand.A);
                case PrefixCbOpCode.RLC_B:
                    return new Operation(Opcode.RotateLeftWithCarry, Operand.B);
                case PrefixCbOpCode.RLC_C:
                    return new Operation(Opcode.RotateLeftWithCarry, Operand.C);
                case PrefixCbOpCode.RLC_D:
                    return new Operation(Opcode.RotateLeftWithCarry, Operand.D);
                case PrefixCbOpCode.RLC_E:
                    return new Operation(Opcode.RotateLeftWithCarry, Operand.E);
                case PrefixCbOpCode.RLC_H:
                    return new Operation(Opcode.RotateLeftWithCarry, Operand.H);
                case PrefixCbOpCode.RLC_L:
                    return new Operation(Opcode.RotateLeftWithCarry, Operand.L);
                case PrefixCbOpCode.RLC_mHL:
                    timer.IndexAndMmuByte(false).Extend(1);
                    return new Operation(Opcode.RotateLeftWithCarry, index.Index);

                // RL r
                case PrefixCbOpCode.RL_A:
                    return new Operation(Opcode.RotateLeft, Operand.A);
                case PrefixCbOpCode.RL_B:
                    return new Operation(Opcode.RotateLeft, Operand.B);
                case PrefixCbOpCode.RL_C:
                    return new Operation(Opcode.RotateLeft, Operand.C);
                case PrefixCbOpCode.RL_D:
                    return new Operation(Opcode.RotateLeft, Operand.D);
                case PrefixCbOpCode.RL_E:
                    return new Operation(Opcode.RotateLeft, Operand.E);
                case PrefixCbOpCode.RL_H:
                    return new Operation(Opcode.RotateLeft, Operand.H);
                case PrefixCbOpCode.RL_L:
                    return new Operation(Opcode.RotateLeft, Operand.L);
                case PrefixCbOpCode.RL_mHL:
                    timer.IndexAndMmuByte(false).Extend(1);
                    return new Operation(Opcode.RotateLeft, index.Index);

                // RRC r
                case PrefixCbOpCode.RRC_A:
                    return new Operation(Opcode.RotateRightWithCarry, Operand.A);
                case PrefixCbOpCode.RRC_B:
                    return new Operation(Opcode.RotateRightWithCarry, Operand.B);
                case PrefixCbOpCode.RRC_C:
                    return new Operation(Opcode.RotateRightWithCarry, Operand.C);
                case PrefixCbOpCode.RRC_D:
                    return new Operation(Opcode.RotateRightWithCarry, Operand.D);
                case PrefixCbOpCode.RRC_E:
                    return new Operation(Opcode.RotateRightWithCarry, Operand.E);
                case PrefixCbOpCode.RRC_H:
                    return new Operation(Opcode.RotateRightWithCarry, Operand.H);
                case PrefixCbOpCode.RRC_L:
                    return new Operation(Opcode.RotateRightWithCarry, Operand.L);
                case PrefixCbOpCode.RRC_mHL:
                    timer.IndexAndMmuByte(false).Extend(1);
                    return new Operation(Opcode.RotateRightWithCarry, index.Index);

                // RR r
                case PrefixCbOpCode.RR_A:
                    return new Operation(Opcode.RotateRight, Operand.A);
                case PrefixCbOpCode.RR_B:
                    return new Operation(Opcode.RotateRight, Operand.B);
                case PrefixCbOpCode.RR_C:
                    return new Operation(Opcode.RotateRight, Operand.C);
                case PrefixCbOpCode.RR_D:
                    return new Operation(Opcode.RotateRight, Operand.D);
                case PrefixCbOpCode.RR_E:
                    return new Operation(Opcode.RotateRight, Operand.E);
                case PrefixCbOpCode.RR_H:
                    return new Operation(Opcode.RotateRight, Operand.H);
                case PrefixCbOpCode.RR_L:
                    return new Operation(Opcode.RotateRight, Operand.L);
                case PrefixCbOpCode.RR_mHL:
                    timer.IndexAndMmuByte(false).Extend(1);
                    return new Operation(Opcode.RotateRight, index.Index);

                // ********* Shift *********
                // SLA r
                case PrefixCbOpCode.SLA_A:
                    return new Operation(Opcode.ShiftLeft, Operand.A);
                case PrefixCbOpCode.SLA_B:
                    return new Operation(Opcode.ShiftLeft, Operand.B);
                case PrefixCbOpCode.SLA_C:
                    return new Operation(Opcode.ShiftLeft, Operand.C);
                case PrefixCbOpCode.SLA_D:
                    return new Operation(Opcode.ShiftLeft, Operand.D);
                case PrefixCbOpCode.SLA_E:
                    return new Operation(Opcode.ShiftLeft, Operand.E);
                case PrefixCbOpCode.SLA_H:
                    return new Operation(Opcode.ShiftLeft, Operand.H);
                case PrefixCbOpCode.SLA_L:
                    return new Operation(Opcode.ShiftLeft, Operand.L);
                case PrefixCbOpCode.SLA_mHL:
                    timer.IndexAndMmuByte(false).Extend(1);
                    return new Operation(Opcode.ShiftLeft, index.Index);

                // SLS r (undocumented)
                case PrefixCbOpCode.SLS_A:
                    return new Operation(this.cpuMode == CpuMode.GameBoy ? Opcode.Swap : Opcode.ShiftLeftSet, Operand.A);
                case PrefixCbOpCode.SLS_B:
                    return new Operation(this.cpuMode == CpuMode.GameBoy ? Opcode.Swap : Opcode.ShiftLeftSet, Operand.B);
                case PrefixCbOpCode.SLS_C:
                    return new Operation(this.cpuMode == CpuMode.GameBoy ? Opcode.Swap : Opcode.ShiftLeftSet, Operand.C);
                case PrefixCbOpCode.SLS_D:
                    return new Operation(this.cpuMode == CpuMode.GameBoy ? Opcode.Swap : Opcode.ShiftLeftSet, Operand.D);
                case PrefixCbOpCode.SLS_E:
                    return new Operation(this.cpuMode == CpuMode.GameBoy ? Opcode.Swap : Opcode.ShiftLeftSet, Operand.E);
                case PrefixCbOpCode.SLS_H:
                    return new Operation(this.cpuMode == CpuMode.GameBoy ? Opcode.Swap : Opcode.ShiftLeftSet, Operand.H);
                case PrefixCbOpCode.SLS_L:
                    return new Operation(this.cpuMode == CpuMode.GameBoy ? Opcode.Swap : Opcode.ShiftLeftSet, Operand.L);
                case PrefixCbOpCode.SLS_mHL:
                    timer.IndexAndMmuByte(false).Extend(1);
                    return new Operation(this.cpuMode == CpuMode.GameBoy ? Opcode.Swap : Opcode.ShiftLeftSet, index.Index);

                // SRA r
                case PrefixCbOpCode.SRA_A:
                    return new Operation(Opcode.ShiftRight, Operand.A);
                case PrefixCbOpCode.SRA_B:
                    return new Operation(Opcode.ShiftRight, Operand.B);
                case PrefixCbOpCode.SRA_C:
                    return new Operation(Opcode.ShiftRight, Operand.C);
                case PrefixCbOpCode.SRA_D:
                    return new Operation(Opcode.ShiftRight, Operand.D);
                case PrefixCbOpCode.SRA_E:
                    return new Operation(Opcode.ShiftRight, Operand.E);
                case PrefixCbOpCode.SRA_H:
                    return new Operation(Opcode.ShiftRight, Operand.H);
                case PrefixCbOpCode.SRA_L:
                    return new Operation(Opcode.ShiftRight, Operand.L);
                case PrefixCbOpCode.SRA_mHL:
                    timer.IndexAndMmuByte(false).Extend(1);
                    return new Operation(Opcode.ShiftRight, index.Index);

                // SRL r
                case PrefixCbOpCode.SRL_A:
                    return new Operation(Opcode.ShiftRightLogical, Operand.A);
                case PrefixCbOpCode.SRL_B:
                    return new Operation(Opcode.ShiftRightLogical, Operand.B);
                case PrefixCbOpCode.SRL_C:
                    return new Operation(Opcode.ShiftRightLogical, Operand.C);
                case PrefixCbOpCode.SRL_D:
                    return new Operation(Opcode.ShiftRightLogical, Operand.D);
                case PrefixCbOpCode.SRL_E:
                    return new Operation(Opcode.ShiftRightLogical, Operand.E);
                case PrefixCbOpCode.SRL_H:
                    return new Operation(Opcode.ShiftRightLogical, Operand.H);
                case PrefixCbOpCode.SRL_L:
                    return new Operation(Opcode.ShiftRightLogical, Operand.L);
                case PrefixCbOpCode.SRL_mHL:
                    timer.IndexAndMmuByte(false).Extend(1);
                    return new Operation(Opcode.ShiftRightLogical, index.Index);

                // ********* Bit Test, Set & Reset *********
                // BIT r
                case PrefixCbOpCode.BIT_0_B:
                    return BitTest(Operand.B, 0);
                case PrefixCbOpCode.BIT_1_B:
                    return BitTest(Operand.B, 1);
                case PrefixCbOpCode.BIT_2_B:
                    return BitTest(Operand.B, 2);
                case PrefixCbOpCode.BIT_3_B:
                    return BitTest(Operand.B, 3);
                case PrefixCbOpCode.BIT_4_B:
                    return BitTest(Operand.B, 4);
                case PrefixCbOpCode.BIT_5_B:
                    return BitTest(Operand.B, 5);
                case PrefixCbOpCode.BIT_6_B:
                    return BitTest(Operand.B, 6);
                case PrefixCbOpCode.BIT_7_B:
                    return BitTest(Operand.B, 7);
                case PrefixCbOpCode.BIT_0_C:
                    return BitTest(Operand.C, 0);
                case PrefixCbOpCode.BIT_1_C:
                    return BitTest(Operand.C, 1);
                case PrefixCbOpCode.BIT_2_C:
                    return BitTest(Operand.C, 2);
                case PrefixCbOpCode.BIT_3_C:
                    return BitTest(Operand.C, 3);
                case PrefixCbOpCode.BIT_4_C:
                    return BitTest(Operand.C, 4);
                case PrefixCbOpCode.BIT_5_C:
                    return BitTest(Operand.C, 5);
                case PrefixCbOpCode.BIT_6_C:
                    return BitTest(Operand.C, 6);
                case PrefixCbOpCode.BIT_7_C:
                    return BitTest(Operand.C, 7);
                case PrefixCbOpCode.BIT_0_D:
                    return BitTest(Operand.D, 0);
                case PrefixCbOpCode.BIT_1_D:
                    return BitTest(Operand.D, 1);
                case PrefixCbOpCode.BIT_2_D:
                    return BitTest(Operand.D, 2);
                case PrefixCbOpCode.BIT_3_D:
                    return BitTest(Operand.D, 3);
                case PrefixCbOpCode.BIT_4_D:
                    return BitTest(Operand.D, 4);
                case PrefixCbOpCode.BIT_5_D:
                    return BitTest(Operand.D, 5);
                case PrefixCbOpCode.BIT_6_D:
                    return BitTest(Operand.D, 6);
                case PrefixCbOpCode.BIT_7_D:
                    return BitTest(Operand.D, 7);
                case PrefixCbOpCode.BIT_0_E:
                    return BitTest(Operand.E, 0);
                case PrefixCbOpCode.BIT_1_E:
                    return BitTest(Operand.E, 1);
                case PrefixCbOpCode.BIT_2_E:
                    return BitTest(Operand.E, 2);
                case PrefixCbOpCode.BIT_3_E:
                    return BitTest(Operand.E, 3);
                case PrefixCbOpCode.BIT_4_E:
                    return BitTest(Operand.E, 4);
                case PrefixCbOpCode.BIT_5_E:
                    return BitTest(Operand.E, 5);
                case PrefixCbOpCode.BIT_6_E:
                    return BitTest(Operand.E, 6);
                case PrefixCbOpCode.BIT_7_E:
                    return BitTest(Operand.E, 7);
                case PrefixCbOpCode.BIT_0_H:
                    return BitTest(Operand.H, 0);
                case PrefixCbOpCode.BIT_1_H:
                    return BitTest(Operand.H, 1);
                case PrefixCbOpCode.BIT_2_H:
                    return BitTest(Operand.H, 2);
                case PrefixCbOpCode.BIT_3_H:
                    return BitTest(Operand.H, 3);
                case PrefixCbOpCode.BIT_4_H:
                    return BitTest(Operand.H, 4);
                case PrefixCbOpCode.BIT_5_H:
                    return BitTest(Operand.H, 5);
                case PrefixCbOpCode.BIT_6_H:
                    return BitTest(Operand.H, 6);
                case PrefixCbOpCode.BIT_7_H:
                    return BitTest(Operand.H, 7);
                case PrefixCbOpCode.BIT_0_L:
                    return BitTest(Operand.L, 0);
                case PrefixCbOpCode.BIT_1_L:
                    return BitTest(Operand.L, 1);
                case PrefixCbOpCode.BIT_2_L:
                    return BitTest(Operand.L, 2);
                case PrefixCbOpCode.BIT_3_L:
                    return BitTest(Operand.L, 3);
                case PrefixCbOpCode.BIT_4_L:
                    return BitTest(Operand.L, 4);
                case PrefixCbOpCode.BIT_5_L:
                    return BitTest(Operand.L, 5);
                case PrefixCbOpCode.BIT_6_L:
                    return BitTest(Operand.L, 6);
                case PrefixCbOpCode.BIT_7_L:
                    return BitTest(Operand.L, 7);
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
                    return BitTest(Operand.A, 0);
                case PrefixCbOpCode.BIT_1_A:
                    return BitTest(Operand.A, 1);
                case PrefixCbOpCode.BIT_2_A:
                    return BitTest(Operand.A, 2);
                case PrefixCbOpCode.BIT_3_A:
                    return BitTest(Operand.A, 3);
                case PrefixCbOpCode.BIT_4_A:
                    return BitTest(Operand.A, 4);
                case PrefixCbOpCode.BIT_5_A:
                    return BitTest(Operand.A, 5);
                case PrefixCbOpCode.BIT_6_A:
                    return BitTest(Operand.A, 6);
                case PrefixCbOpCode.BIT_7_A:
                    return BitTest(Operand.A, 7);

                // SET r
                case PrefixCbOpCode.SET_0_B:
                    return BitSet(Operand.B, 0);
                case PrefixCbOpCode.SET_1_B:
                    return BitSet(Operand.B, 1);
                case PrefixCbOpCode.SET_2_B:
                    return BitSet(Operand.B, 2);
                case PrefixCbOpCode.SET_3_B:
                    return BitSet(Operand.B, 3);
                case PrefixCbOpCode.SET_4_B:
                    return BitSet(Operand.B, 4);
                case PrefixCbOpCode.SET_5_B:
                    return BitSet(Operand.B, 5);
                case PrefixCbOpCode.SET_6_B:
                    return BitSet(Operand.B, 6);
                case PrefixCbOpCode.SET_7_B:
                    return BitSet(Operand.B, 7);
                case PrefixCbOpCode.SET_0_C:
                    return BitSet(Operand.C, 0);
                case PrefixCbOpCode.SET_1_C:
                    return BitSet(Operand.C, 1);
                case PrefixCbOpCode.SET_2_C:
                    return BitSet(Operand.C, 2);
                case PrefixCbOpCode.SET_3_C:
                    return BitSet(Operand.C, 3);
                case PrefixCbOpCode.SET_4_C:
                    return BitSet(Operand.C, 4);
                case PrefixCbOpCode.SET_5_C:
                    return BitSet(Operand.C, 5);
                case PrefixCbOpCode.SET_6_C:
                    return BitSet(Operand.C, 6);
                case PrefixCbOpCode.SET_7_C:
                    return BitSet(Operand.C, 7);
                case PrefixCbOpCode.SET_0_D:
                    return BitSet(Operand.D, 0);
                case PrefixCbOpCode.SET_1_D:
                    return BitSet(Operand.D, 1);
                case PrefixCbOpCode.SET_2_D:
                    return BitSet(Operand.D, 2);
                case PrefixCbOpCode.SET_3_D:
                    return BitSet(Operand.D, 3);
                case PrefixCbOpCode.SET_4_D:
                    return BitSet(Operand.D, 4);
                case PrefixCbOpCode.SET_5_D:
                    return BitSet(Operand.D, 5);
                case PrefixCbOpCode.SET_6_D:
                    return BitSet(Operand.D, 6);
                case PrefixCbOpCode.SET_7_D:
                    return BitSet(Operand.D, 7);
                case PrefixCbOpCode.SET_0_E:
                    return BitSet(Operand.E, 0);
                case PrefixCbOpCode.SET_1_E:
                    return BitSet(Operand.E, 1);
                case PrefixCbOpCode.SET_2_E:
                    return BitSet(Operand.E, 2);
                case PrefixCbOpCode.SET_3_E:
                    return BitSet(Operand.E, 3);
                case PrefixCbOpCode.SET_4_E:
                    return BitSet(Operand.E, 4);
                case PrefixCbOpCode.SET_5_E:
                    return BitSet(Operand.E, 5);
                case PrefixCbOpCode.SET_6_E:
                    return BitSet(Operand.E, 6);
                case PrefixCbOpCode.SET_7_E:
                    return BitSet(Operand.E, 7);
                case PrefixCbOpCode.SET_0_H:
                    return BitSet(Operand.H, 0);
                case PrefixCbOpCode.SET_1_H:
                    return BitSet(Operand.H, 1);
                case PrefixCbOpCode.SET_2_H:
                    return BitSet(Operand.H, 2);
                case PrefixCbOpCode.SET_3_H:
                    return BitSet(Operand.H, 3);
                case PrefixCbOpCode.SET_4_H:
                    return BitSet(Operand.H, 4);
                case PrefixCbOpCode.SET_5_H:
                    return BitSet(Operand.H, 5);
                case PrefixCbOpCode.SET_6_H:
                    return BitSet(Operand.H, 6);
                case PrefixCbOpCode.SET_7_H:
                    return BitSet(Operand.H, 7);
                case PrefixCbOpCode.SET_0_L:
                    return BitSet(Operand.L, 0);
                case PrefixCbOpCode.SET_1_L:
                    return BitSet(Operand.L, 1);
                case PrefixCbOpCode.SET_2_L:
                    return BitSet(Operand.L, 2);
                case PrefixCbOpCode.SET_3_L:
                    return BitSet(Operand.L, 3);
                case PrefixCbOpCode.SET_4_L:
                    return BitSet(Operand.L, 4);
                case PrefixCbOpCode.SET_5_L:
                    return BitSet(Operand.L, 5);
                case PrefixCbOpCode.SET_6_L:
                    return BitSet(Operand.L, 6);
                case PrefixCbOpCode.SET_7_L:
                    return BitSet(Operand.L, 7);
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
                    return BitSet(Operand.A, 0);
                case PrefixCbOpCode.SET_1_A:
                    return BitSet(Operand.A, 1);
                case PrefixCbOpCode.SET_2_A:
                    return BitSet(Operand.A, 2);
                case PrefixCbOpCode.SET_3_A:
                    return BitSet(Operand.A, 3);
                case PrefixCbOpCode.SET_4_A:
                    return BitSet(Operand.A, 4);
                case PrefixCbOpCode.SET_5_A:
                    return BitSet(Operand.A, 5);
                case PrefixCbOpCode.SET_6_A:
                    return BitSet(Operand.A, 6);
                case PrefixCbOpCode.SET_7_A:
                    return BitSet(Operand.A, 7);

                // RES r
                case PrefixCbOpCode.RES_0_B:
                    return BitReset(Operand.B, 0);
                case PrefixCbOpCode.RES_1_B:
                    return BitReset(Operand.B, 1);
                case PrefixCbOpCode.RES_2_B:
                    return BitReset(Operand.B, 2);
                case PrefixCbOpCode.RES_3_B:
                    return BitReset(Operand.B, 3);
                case PrefixCbOpCode.RES_4_B:
                    return BitReset(Operand.B, 4);
                case PrefixCbOpCode.RES_5_B:
                    return BitReset(Operand.B, 5);
                case PrefixCbOpCode.RES_6_B:
                    return BitReset(Operand.B, 6);
                case PrefixCbOpCode.RES_7_B:
                    return BitReset(Operand.B, 7);
                case PrefixCbOpCode.RES_0_C:
                    return BitReset(Operand.C, 0);
                case PrefixCbOpCode.RES_1_C:
                    return BitReset(Operand.C, 1);
                case PrefixCbOpCode.RES_2_C:
                    return BitReset(Operand.C, 2);
                case PrefixCbOpCode.RES_3_C:
                    return BitReset(Operand.C, 3);
                case PrefixCbOpCode.RES_4_C:
                    return BitReset(Operand.C, 4);
                case PrefixCbOpCode.RES_5_C:
                    return BitReset(Operand.C, 5);
                case PrefixCbOpCode.RES_6_C:
                    return BitReset(Operand.C, 6);
                case PrefixCbOpCode.RES_7_C:
                    return BitReset(Operand.C, 7);
                case PrefixCbOpCode.RES_0_D:
                    return BitReset(Operand.D, 0);
                case PrefixCbOpCode.RES_1_D:
                    return BitReset(Operand.D, 1);
                case PrefixCbOpCode.RES_2_D:
                    return BitReset(Operand.D, 2);
                case PrefixCbOpCode.RES_3_D:
                    return BitReset(Operand.D, 3);
                case PrefixCbOpCode.RES_4_D:
                    return BitReset(Operand.D, 4);
                case PrefixCbOpCode.RES_5_D:
                    return BitReset(Operand.D, 5);
                case PrefixCbOpCode.RES_6_D:
                    return BitReset(Operand.D, 6);
                case PrefixCbOpCode.RES_7_D:
                    return BitReset(Operand.D, 7);
                case PrefixCbOpCode.RES_0_E:
                    return BitReset(Operand.E, 0);
                case PrefixCbOpCode.RES_1_E:
                    return BitReset(Operand.E, 1);
                case PrefixCbOpCode.RES_2_E:
                    return BitReset(Operand.E, 2);
                case PrefixCbOpCode.RES_3_E:
                    return BitReset(Operand.E, 3);
                case PrefixCbOpCode.RES_4_E:
                    return BitReset(Operand.E, 4);
                case PrefixCbOpCode.RES_5_E:
                    return BitReset(Operand.E, 5);
                case PrefixCbOpCode.RES_6_E:
                    return BitReset(Operand.E, 6);
                case PrefixCbOpCode.RES_7_E:
                    return BitReset(Operand.E, 7);
                case PrefixCbOpCode.RES_0_H:
                    return BitReset(Operand.H, 0);
                case PrefixCbOpCode.RES_1_H:
                    return BitReset(Operand.H, 1);
                case PrefixCbOpCode.RES_2_H:
                    return BitReset(Operand.H, 2);
                case PrefixCbOpCode.RES_3_H:
                    return BitReset(Operand.H, 3);
                case PrefixCbOpCode.RES_4_H:
                    return BitReset(Operand.H, 4);
                case PrefixCbOpCode.RES_5_H:
                    return BitReset(Operand.H, 5);
                case PrefixCbOpCode.RES_6_H:
                    return BitReset(Operand.H, 6);
                case PrefixCbOpCode.RES_7_H:
                    return BitReset(Operand.H, 7);
                case PrefixCbOpCode.RES_0_L:
                    return BitReset(Operand.L, 0);
                case PrefixCbOpCode.RES_1_L:
                    return BitReset(Operand.L, 1);
                case PrefixCbOpCode.RES_2_L:
                    return BitReset(Operand.L, 2);
                case PrefixCbOpCode.RES_3_L:
                    return BitReset(Operand.L, 3);
                case PrefixCbOpCode.RES_4_L:
                    return BitReset(Operand.L, 4);
                case PrefixCbOpCode.RES_5_L:
                    return BitReset(Operand.L, 5);
                case PrefixCbOpCode.RES_6_L:
                    return BitReset(Operand.L, 6);
                case PrefixCbOpCode.RES_7_L:
                    return BitReset(Operand.L, 7);
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
                    return BitReset(Operand.A, 0);
                case PrefixCbOpCode.RES_1_A:
                    return BitReset(Operand.A, 1);
                case PrefixCbOpCode.RES_2_A:
                    return BitReset(Operand.A, 2);
                case PrefixCbOpCode.RES_3_A:
                    return BitReset(Operand.A, 3);
                case PrefixCbOpCode.RES_4_A:
                    return BitReset(Operand.A, 4);
                case PrefixCbOpCode.RES_5_A:
                    return BitReset(Operand.A, 5);
                case PrefixCbOpCode.RES_6_A:
                    return BitReset(Operand.A, 6);
                case PrefixCbOpCode.RES_7_A:
                    return BitReset(Operand.A, 7);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Operation BitResetFromIndex(int bit)
        {
            timer.Index(index.IsDisplaced).Extend(1);
            return new Operation(Opcode.BitReset, index.Index).AddLiteral((byte)bit);
        }

        private static Operation BitReset(Operand register, int bit)
        {
            return new Operation(Opcode.BitReset, register).AddLiteral((byte)bit);
        }

        private Operation BitSetFromIndex(int bit)
        {
            timer.Index(index.IsDisplaced).Extend(1);
            return new Operation(Opcode.BitSet, index.Index).AddLiteral((byte)bit);
        }

        private static Operation BitSet(Operand register, int bit)
        {
            return new Operation(Opcode.BitSet, register).AddLiteral((byte)bit);
        }

        private Operation BitTestFromIndex(int bit)
        {
            timer.Index(index.IsDisplaced).Extend(1);
            return new Operation(Opcode.BitTest, index.Index).AddLiteral((byte)bit);
        }

        private static Operation BitTest(Operand register, int bit)
        {
            return new Operation(Opcode.BitTest, register).AddLiteral((byte)bit);
        }
    }
}
