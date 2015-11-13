namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System;
    using System.Collections.Generic;

    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;

    internal partial class OpCodeDecoder
    {
        public IEnumerable<DecodeResult> GetDecodeResult()
        {
            index = indexRegisterOperands[IndexRegister.HL];

            while (true)
            {
                var code = (PrimaryOpCode)prefetch.NextByte();
                DecodeResult result = null;

                // Add a NOP on every frame. Reduce timings by a NOP elsewhere.
                timer.Nop();

                switch (code)
                {
                    case PrimaryOpCode.NOP:
                        result = new DecodeResult(Opcode.NOP);
                        break;
                    case PrimaryOpCode.HALT:
                        result = new DecodeResult(Opcode.HALT).EndBlock();
                        break;
                    case PrimaryOpCode.Prefix_CB:
                        // Prefix CB
                        break;
                    case PrimaryOpCode.Prefix_DD:
                        if (cpuMode == CpuMode.Z80)
                        {
                            index = indexRegisterOperands[IndexRegister.IX];
                        }
                        continue;
                    case PrimaryOpCode.Prefix_ED:
                        // Prefix ED
                        break;
                    case PrimaryOpCode.Prefix_FD:
                        if (cpuMode == CpuMode.Z80)
                        {
                            index = indexRegisterOperands[IndexRegister.IY];
                        }
                        continue;
                    case PrimaryOpCode.LD_A_A:
                        result = new DecodeResult(Opcode.LD, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.LD_B_A:
                        result = new DecodeResult(Opcode.LD, Operand.B, Operand.A);
                        break;
                    case PrimaryOpCode.LD_C_A:
                        result = new DecodeResult(Opcode.LD, Operand.C, Operand.A);
                        break;
                    case PrimaryOpCode.LD_D_A:
                        result = new DecodeResult(Opcode.LD, Operand.D, Operand.A);
                        break;
                    case PrimaryOpCode.LD_E_A:
                        result = new DecodeResult(Opcode.LD, Operand.E, Operand.A);
                        break;
                    case PrimaryOpCode.LD_H_A:
                        result = new DecodeResult(Opcode.LD, index.HighRegister, Operand.A);
                        break;
                    case PrimaryOpCode.LD_L_A:
                        result = new DecodeResult(Opcode.LD, index.LowRegister, Operand.A);
                        break;
                    case PrimaryOpCode.LD_A_B:
                        result = new DecodeResult(Opcode.LD, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.LD_B_B:
                        result = new DecodeResult(Opcode.LD, Operand.B, Operand.B);
                        break;
                    case PrimaryOpCode.LD_C_B:
                        result = new DecodeResult(Opcode.LD, Operand.C, Operand.B);
                        break;
                    case PrimaryOpCode.LD_D_B:
                        result = new DecodeResult(Opcode.LD, Operand.D, Operand.B);
                        break;
                    case PrimaryOpCode.LD_E_B:
                        result = new DecodeResult(Opcode.LD, Operand.E, Operand.B);
                        break;
                    case PrimaryOpCode.LD_H_B:
                        result = new DecodeResult(Opcode.LD, index.HighRegister, Operand.B);
                        break;
                    case PrimaryOpCode.LD_L_B:
                        result = new DecodeResult(Opcode.LD, index.LowRegister, Operand.B);
                        break;
                    case PrimaryOpCode.LD_A_C:
                        result = new DecodeResult(Opcode.LD, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.LD_B_C:
                        result = new DecodeResult(Opcode.LD, Operand.B, Operand.C);
                        break;
                    case PrimaryOpCode.LD_C_C:
                        result = new DecodeResult(Opcode.LD, Operand.C, Operand.C);
                        break;
                    case PrimaryOpCode.LD_D_C:
                        result = new DecodeResult(Opcode.LD, Operand.D, Operand.C);
                        break;
                    case PrimaryOpCode.LD_E_C:
                        result = new DecodeResult(Opcode.LD, Operand.E, Operand.C);
                        break;
                    case PrimaryOpCode.LD_H_C:
                        result = new DecodeResult(Opcode.LD, index.HighRegister, Operand.C);
                        break;
                    case PrimaryOpCode.LD_L_C:
                        result = new DecodeResult(Opcode.LD, index.LowRegister, Operand.C);
                        break;
                    case PrimaryOpCode.LD_A_D:
                        result = new DecodeResult(Opcode.LD, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.LD_B_D:
                        result = new DecodeResult(Opcode.LD, Operand.B, Operand.D);
                        break;
                    case PrimaryOpCode.LD_C_D:
                        result = new DecodeResult(Opcode.LD, Operand.C, Operand.D);
                        break;
                    case PrimaryOpCode.LD_D_D:
                        result = new DecodeResult(Opcode.LD, Operand.D, Operand.D);
                        break;
                    case PrimaryOpCode.LD_E_D:
                        result = new DecodeResult(Opcode.LD, Operand.E, Operand.D);
                        break;
                    case PrimaryOpCode.LD_H_D:
                        result = new DecodeResult(Opcode.LD, index.HighRegister, Operand.D);
                        break;
                    case PrimaryOpCode.LD_L_D:
                        result = new DecodeResult(Opcode.LD, index.LowRegister, Operand.D);
                        break;
                    case PrimaryOpCode.LD_A_E:
                        result = new DecodeResult(Opcode.LD, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.LD_B_E:
                        result = new DecodeResult(Opcode.LD, Operand.B, Operand.E);
                        break;
                    case PrimaryOpCode.LD_C_E:
                        result = new DecodeResult(Opcode.LD, Operand.C, Operand.E);
                        break;
                    case PrimaryOpCode.LD_D_E:
                        result = new DecodeResult(Opcode.LD, Operand.D, Operand.E);
                        break;
                    case PrimaryOpCode.LD_E_E:
                        result = new DecodeResult(Opcode.LD, Operand.E, Operand.E);
                        break;
                    case PrimaryOpCode.LD_H_E:
                        result = new DecodeResult(Opcode.LD, index.HighRegister, Operand.E);
                        break;
                    case PrimaryOpCode.LD_L_E:
                        result = new DecodeResult(Opcode.LD, index.LowRegister, Operand.E);
                        break;
                    case PrimaryOpCode.LD_A_H:
                        result = new DecodeResult(Opcode.LD, Operand.A, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_B_H:
                        result = new DecodeResult(Opcode.LD, Operand.B, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_C_H:
                        result = new DecodeResult(Opcode.LD, Operand.C, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_D_H:
                        result = new DecodeResult(Opcode.LD, Operand.D, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_E_H:
                        result = new DecodeResult(Opcode.LD, Operand.E, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_H_H:
                        result = new DecodeResult(Opcode.LD, index.HighRegister, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_L_H:
                        result = new DecodeResult(Opcode.LD, index.LowRegister, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_A_L:
                        result = new DecodeResult(Opcode.LD, Operand.A, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_B_L:
                        result = new DecodeResult(Opcode.LD, Operand.B, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_C_L:
                        result = new DecodeResult(Opcode.LD, Operand.C, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_D_L:
                        result = new DecodeResult(Opcode.LD, Operand.D, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_E_L:
                        result = new DecodeResult(Opcode.LD, Operand.E, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_H_L:
                        result = new DecodeResult(Opcode.LD, index.HighRegister, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_L_L:
                        result = new DecodeResult(Opcode.LD, index.LowRegister, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_A_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.LD, Operand.A, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.LD_B_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.LD, Operand.B, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.LD_C_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.LD, Operand.C, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.LD_D_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.LD, Operand.D, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.LD_E_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.LD, Operand.E, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.LD_H_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.LD, index.HighRegister, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.LD_L_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.LD, index.LowRegister, Operand.n).WithByteLiteral();
                        break;

                    // LD r, (HL)
                    case PrimaryOpCode.LD_A_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.LD, Operand.A, index.Index).WithDisplacement();
                        break;
                    case PrimaryOpCode.LD_B_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.LD, Operand.B, index.Index).WithDisplacement();
                        break;
                    case PrimaryOpCode.LD_C_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.LD, Operand.C, index.Index).WithDisplacement();
                        break;
                    case PrimaryOpCode.LD_D_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.LD, Operand.D, index.Index).WithDisplacement();
                        break;
                    case PrimaryOpCode.LD_E_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.LD, Operand.E, index.Index).WithDisplacement();
                        break;
                    case PrimaryOpCode.LD_H_mHL:
                        timer.Index(index.IsDisplaced);
                        // H register is always assigned here
                        result = new DecodeResult(Opcode.LD, Operand.H, index.Index).WithDisplacement();
                        break;
                    case PrimaryOpCode.LD_L_mHL:
                        timer.Index(index.IsDisplaced);
                        // L register is always assigned here
                        result = new DecodeResult(Opcode.LD, Operand.L, index.Index).WithDisplacement();
                        break;

                    // LD (HL), r
                    case PrimaryOpCode.LD_mHL_A:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.LD, index.Index, Operand.A).WithDisplacement();
                        break;
                    case PrimaryOpCode.LD_mHL_B:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.LD, index.Index, Operand.B).WithDisplacement();
                        break;
                    case PrimaryOpCode.LD_mHL_C:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.LD, index.Index, Operand.C).WithDisplacement();
                        break;
                    case PrimaryOpCode.LD_mHL_D:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.LD, index.Index, Operand.D).WithDisplacement();
                        break;
                    case PrimaryOpCode.LD_mHL_E:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.LD, index.Index, Operand.E).WithDisplacement();
                        break;
                    case PrimaryOpCode.LD_mHL_H:
                        timer.Index(index.IsDisplaced);
                        // Value of H register is always used here
                        result = new DecodeResult(Opcode.LD, index.Index, Operand.H).WithDisplacement();
                        break;
                    case PrimaryOpCode.LD_mHL_L:
                        timer.Index(index.IsDisplaced);
                        // Value of L register is always used here
                        result = new DecodeResult(Opcode.LD, index.Index, Operand.L).WithDisplacement();
                        break;

                    // LD (HL), n
                    case PrimaryOpCode.LD_mHL_n:
                        timer.IndexAndPrefetch(index.IsDisplaced);
                        result = new DecodeResult(Opcode.LD, index.Index, Operand.n).WithDisplacement().WithByteLiteral();
                        break;

                    // LD A, (BC)
                    case PrimaryOpCode.LD_A_mBC:
                        timer.Index(false);
                        result = new DecodeResult(Opcode.LD, Operand.A, Operand.mBC);
                        break;

                    // LD A, (BC)
                    case PrimaryOpCode.LD_A_mDE:
                        timer.Index(false);
                        result = new DecodeResult(Opcode.LD, Operand.A, Operand.mDE);
                        break;

                    // LD A, (nn)
                    case PrimaryOpCode.LD_A_mnn:
                        timer.Index(false).MmuWord();
                        result = new DecodeResult(Opcode.LD, Operand.A, Operand.mnn).WithWordLiteral();
                        break;

                    // LD (BC), A
                    case PrimaryOpCode.LD_mBC_A:
                        timer.Index(false);
                        result = new DecodeResult(Opcode.LD, Operand.mBC, Operand.A);
                        break;

                    // LD (DE), A
                    case PrimaryOpCode.LD_mDE_A:
                        timer.Index(false);
                        result = new DecodeResult(Opcode.LD, Operand.mDE, Operand.A);
                        break;

                    // LD (nn), A
                    case PrimaryOpCode.LD_mnn_A:
                        timer.Index(false).MmuWord();
                        result = new DecodeResult(Opcode.LD, Operand.mnn, Operand.A).WithWordLiteral();
                        break;

                    // LD dd, nn
                    case PrimaryOpCode.LD_BC_nn:
                        timer.MmuWord();
                        result = new DecodeResult(Opcode.LD16, Operand.BC, Operand.nn).WithWordLiteral();
                        break;
                    case PrimaryOpCode.LD_DE_nn:
                        timer.MmuWord();
                        result = new DecodeResult(Opcode.LD16, Operand.DE, Operand.nn).WithWordLiteral();
                        break;
                    case PrimaryOpCode.LD_HL_nn:
                        timer.MmuWord();
                        result = new DecodeResult(Opcode.LD16, index.Register, Operand.nn).WithWordLiteral();
                        break;
                    case PrimaryOpCode.LD_SP_nn:
                        timer.MmuWord();
                        result = new DecodeResult(Opcode.LD16, Operand.SP, Operand.nn).WithWordLiteral();
                        break;

                    // LD HL, (nn)
                    case PrimaryOpCode.LD_HL_mnn:
                        timer.IndexAndPrefetchWord();
                        result = new DecodeResult(Opcode.LD16, index.Register, Operand.mnn).WithWordLiteral();
                        break;

                    // LD (nn), HL
                    case PrimaryOpCode.LD_mnn_HL:
                        timer.IndexAndPrefetchWord();
                        result = new DecodeResult(Opcode.LD16, Operand.mnn, index.Register).WithWordLiteral();
                        break;

                    // LD SP, HL
                    case PrimaryOpCode.LD_SP_HL:
                        timer.Extend(2);
                        result = new DecodeResult(Opcode.LD16, Operand.SP, index.Register);
                        break;

                    case PrimaryOpCode.PUSH_BC:
                        timer.Extend(1).MmuWord();
                        result = new DecodeResult(Opcode.PUSH, Operand.BC);
                        break;
                    case PrimaryOpCode.PUSH_DE:
                        timer.Extend(1).MmuWord();
                        result = new DecodeResult(Opcode.PUSH, Operand.DE);
                        break;
                    case PrimaryOpCode.PUSH_HL:
                        timer.Extend(1).MmuWord();
                        result = new DecodeResult(Opcode.PUSH, index.Register);
                        break;
                    case PrimaryOpCode.PUSH_AF:
                        timer.Extend(1).MmuWord();
                        result = new DecodeResult(Opcode.PUSH, Operand.AF);
                        break;

                    case PrimaryOpCode.POP_BC:
                        timer.MmuWord();
                        result = new DecodeResult(Opcode.POP, Operand.BC);
                        break;
                    case PrimaryOpCode.POP_DE:
                        timer.MmuWord();
                        result = new DecodeResult(Opcode.POP, Operand.DE);
                        break;
                    case PrimaryOpCode.POP_HL:
                        timer.MmuWord();
                        result = new DecodeResult(Opcode.POP, index.Register);
                        break;
                    case PrimaryOpCode.POP_AF:
                        timer.MmuWord();
                        result = new DecodeResult(Opcode.POP, Operand.AF);
                        break;

                    case PrimaryOpCode.ADD_A_A:
                        result = new DecodeResult(Opcode.ADD, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.ADD_A_B:
                        result = new DecodeResult(Opcode.ADD, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.ADD_A_C:
                        result = new DecodeResult(Opcode.ADD, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.ADD_A_D:
                        result = new DecodeResult(Opcode.ADD, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.ADD_A_E:
                        result = new DecodeResult(Opcode.ADD, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.ADD_A_H:
                        result = new DecodeResult(Opcode.ADD, Operand.A, index.HighRegister);
                        break;
                    case PrimaryOpCode.ADD_A_L:
                        result = new DecodeResult(Opcode.ADD, Operand.A, index.LowRegister);
                        break;
                    case PrimaryOpCode.ADD_A_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.ADD, Operand.A, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.ADD_A_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.ADD, Operand.A, index.Index).WithDisplacement();
                        break;

                    case PrimaryOpCode.ADC_A_A:
                        result = new DecodeResult(Opcode.ADC, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.ADC_A_B:
                        result = new DecodeResult(Opcode.ADC, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.ADC_A_C:
                        result = new DecodeResult(Opcode.ADC, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.ADC_A_D:
                        result = new DecodeResult(Opcode.ADC, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.ADC_A_E:
                        result = new DecodeResult(Opcode.ADC, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.ADC_A_H:
                        result = new DecodeResult(Opcode.ADC, Operand.A, index.HighRegister);
                        break;
                    case PrimaryOpCode.ADC_A_L:
                        result = new DecodeResult(Opcode.ADC, Operand.A, index.LowRegister);
                        break;
                    case PrimaryOpCode.ADC_A_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.ADC, Operand.A, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.ADC_A_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.ADC, Operand.A, index.Index).WithDisplacement();
                        break;

                    case PrimaryOpCode.SUB_A_A:
                        result = new DecodeResult(Opcode.SUB, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.SUB_A_B:
                        result = new DecodeResult(Opcode.SUB, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.SUB_A_C:
                        result = new DecodeResult(Opcode.SUB, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.SUB_A_D:
                        result = new DecodeResult(Opcode.SUB, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.SUB_A_E:
                        result = new DecodeResult(Opcode.SUB, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.SUB_A_H:
                        result = new DecodeResult(Opcode.SUB, Operand.A, index.HighRegister);
                        break;
                    case PrimaryOpCode.SUB_A_L:
                        result = new DecodeResult(Opcode.SUB, Operand.A, index.LowRegister);
                        break;
                    case PrimaryOpCode.SUB_A_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.SUB, Operand.A, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.SUB_A_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.SUB, Operand.A, index.Index).WithDisplacement();
                        break;

                    case PrimaryOpCode.SBC_A_A:
                        result = new DecodeResult(Opcode.SBC, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.SBC_A_B:
                        result = new DecodeResult(Opcode.SBC, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.SBC_A_C:
                        result = new DecodeResult(Opcode.SBC, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.SBC_A_D:
                        result = new DecodeResult(Opcode.SBC, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.SBC_A_E:
                        result = new DecodeResult(Opcode.SBC, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.SBC_A_H:
                        result = new DecodeResult(Opcode.SBC, Operand.A, index.HighRegister);
                        break;
                    case PrimaryOpCode.SBC_A_L:
                        result = new DecodeResult(Opcode.SBC, Operand.A, index.LowRegister);
                        break;
                    case PrimaryOpCode.SBC_A_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.SBC, Operand.A, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.SBC_A_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.SBC, Operand.A, index.Index).WithDisplacement();
                        break;

                    case PrimaryOpCode.AND_A:
                        result = new DecodeResult(Opcode.AND, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.AND_B:
                        result = new DecodeResult(Opcode.AND, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.AND_C:
                        result = new DecodeResult(Opcode.AND, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.AND_D:
                        result = new DecodeResult(Opcode.AND, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.AND_E:
                        result = new DecodeResult(Opcode.AND, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.AND_H:
                        result = new DecodeResult(Opcode.AND, Operand.A, index.HighRegister);
                        break;
                    case PrimaryOpCode.AND_L:
                        result = new DecodeResult(Opcode.AND, Operand.A, index.LowRegister);
                        break;
                    case PrimaryOpCode.AND_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.AND, Operand.A, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.AND_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.AND, Operand.A, index.Index).WithDisplacement();
                        break;

                    case PrimaryOpCode.OR_A:
                        result = new DecodeResult(Opcode.OR, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.OR_B:
                        result = new DecodeResult(Opcode.OR, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.OR_C:
                        result = new DecodeResult(Opcode.OR, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.OR_D:
                        result = new DecodeResult(Opcode.OR, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.OR_E:
                        result = new DecodeResult(Opcode.OR, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.OR_H:
                        result = new DecodeResult(Opcode.OR, Operand.A, index.HighRegister);
                        break;
                    case PrimaryOpCode.OR_L:
                        result = new DecodeResult(Opcode.OR, Operand.A, index.LowRegister);
                        break;
                    case PrimaryOpCode.OR_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.OR, Operand.A, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.OR_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.OR, Operand.A, index.Index).WithDisplacement();
                        break;

                    case PrimaryOpCode.XOR_A:
                        result = new DecodeResult(Opcode.XOR, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.XOR_B:
                        result = new DecodeResult(Opcode.XOR, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.XOR_C:
                        result = new DecodeResult(Opcode.XOR, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.XOR_D:
                        result = new DecodeResult(Opcode.XOR, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.XOR_E:
                        result = new DecodeResult(Opcode.XOR, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.XOR_H:
                        result = new DecodeResult(Opcode.XOR, Operand.A, index.HighRegister);
                        break;
                    case PrimaryOpCode.XOR_L:
                        result = new DecodeResult(Opcode.XOR, Operand.A, index.LowRegister);
                        break;
                    case PrimaryOpCode.XOR_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.XOR, Operand.A, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.XOR_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.XOR, Operand.A, index.Index).WithDisplacement();
                        break;

                    case PrimaryOpCode.CP_A:
                        result = new DecodeResult(Opcode.CP, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.CP_B:
                        result = new DecodeResult(Opcode.CP, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.CP_C:
                        result = new DecodeResult(Opcode.CP, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.CP_D:
                        result = new DecodeResult(Opcode.CP, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.CP_E:
                        result = new DecodeResult(Opcode.CP, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.CP_H:
                        result = new DecodeResult(Opcode.CP, Operand.A, index.HighRegister);
                        break;
                    case PrimaryOpCode.CP_L:
                        result = new DecodeResult(Opcode.CP, Operand.A, index.LowRegister);
                        break;
                    case PrimaryOpCode.CP_n:
                        timer.MmuByte();
                        result = new DecodeResult(Opcode.CP, Operand.A, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.CP_mHL:
                        result = new DecodeResult(Opcode.CP, Operand.A, index.Index).WithDisplacement();
                        break;

                    case PrimaryOpCode.INC_A:
                        result = new DecodeResult(Opcode.INC, Operand.A);
                        break;
                    case PrimaryOpCode.INC_B:
                        result = new DecodeResult(Opcode.INC, Operand.B);
                        break;
                    case PrimaryOpCode.INC_C:
                        result = new DecodeResult(Opcode.INC, Operand.C);
                        break;
                    case PrimaryOpCode.INC_D:
                        result = new DecodeResult(Opcode.INC, Operand.D);
                        break;
                    case PrimaryOpCode.INC_E:
                        result = new DecodeResult(Opcode.INC, Operand.E);
                        break;
                    case PrimaryOpCode.INC_H:
                        result = new DecodeResult(Opcode.INC, index.HighRegister);
                        break;
                    case PrimaryOpCode.INC_L:
                        result = new DecodeResult(Opcode.INC, index.LowRegister);
                        break;
                    case PrimaryOpCode.INC_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.INC, index.Index).WithDisplacement();
                        break;

                    case PrimaryOpCode.DEC_A:
                        result = new DecodeResult(Opcode.DEC, Operand.A);
                        break;
                    case PrimaryOpCode.DEC_B:
                        result = new DecodeResult(Opcode.DEC, Operand.B);
                        break;
                    case PrimaryOpCode.DEC_C:
                        result = new DecodeResult(Opcode.DEC, Operand.C);
                        break;
                    case PrimaryOpCode.DEC_D:
                        result = new DecodeResult(Opcode.DEC, Operand.D);
                        break;
                    case PrimaryOpCode.DEC_E:
                        result = new DecodeResult(Opcode.DEC, Operand.E);
                        break;
                    case PrimaryOpCode.DEC_H:
                        result = new DecodeResult(Opcode.DEC, index.HighRegister);
                        break;
                    case PrimaryOpCode.DEC_L:
                        result = new DecodeResult(Opcode.DEC, index.LowRegister);
                        break;
                    case PrimaryOpCode.DEC_mHL:
                        timer.Index(index.IsDisplaced);
                        result = new DecodeResult(Opcode.DEC, index.Index).WithDisplacement();
                        break;

                    case PrimaryOpCode.ADD_HL_BC:
                        timer.Arithmetic16();
                        result = new DecodeResult(Opcode.ADD16, index.Register, Operand.BC);
                        break;
                    case PrimaryOpCode.ADD_HL_DE:
                        timer.Arithmetic16();
                        result = new DecodeResult(Opcode.ADD16, index.Register, Operand.DE);
                        break;
                    case PrimaryOpCode.ADD_HL_HL:
                        timer.Arithmetic16();
                        result = new DecodeResult(Opcode.ADD16, index.Register, index.Register);
                        break;
                    case PrimaryOpCode.ADD_HL_SP:
                        timer.Arithmetic16();
                        result = new DecodeResult(Opcode.ADD16, index.Register, Operand.SP);
                        break;

                    case PrimaryOpCode.INC_BC:
                        timer.Extend(2);
                        result = new DecodeResult(Opcode.INC16, Operand.BC);
                        break;
                    case PrimaryOpCode.INC_DE:
                        timer.Extend(2);
                        result = new DecodeResult(Opcode.INC16, Operand.DE);
                        break;
                    case PrimaryOpCode.INC_HL:
                        timer.Extend(2);
                        result = new DecodeResult(Opcode.INC16, index.Register);
                        break;
                    case PrimaryOpCode.INC_SP:
                        timer.Extend(2);
                        result = new DecodeResult(Opcode.INC16, Operand.SP);
                        break;

                    case PrimaryOpCode.DEC_BC:
                        timer.Extend(2);
                        result = new DecodeResult(Opcode.DEC16, Operand.BC);
                        break;
                    case PrimaryOpCode.DEC_DE:
                        timer.Extend(2);
                        result = new DecodeResult(Opcode.DEC16, Operand.DE);
                        break;
                    case PrimaryOpCode.DEC_HL:
                        timer.Extend(2);
                        result = new DecodeResult(Opcode.DEC16, index.Register);
                        break;
                    case PrimaryOpCode.DEC_SP:
                        timer.Extend(2);
                        result = new DecodeResult(Opcode.DEC16, Operand.SP);
                        break;

                    case PrimaryOpCode.EX_DE_HL:
                        // This affects HL register directly, always ignoring index register prefixes
                        result = new DecodeResult(Opcode.EX, Operand.DE, Operand.HL);
                        break;
                    case PrimaryOpCode.EX_AF:
                        result = new DecodeResult(Opcode.EX, Operand.AF);
                        break;
                    case PrimaryOpCode.EXX:
                        result = new DecodeResult(Opcode.EXX);
                        break;
                    case PrimaryOpCode.EX_mSP_HL:
                        timer.Arithmetic16().MmuWord().Extend(2);
                        result = new DecodeResult(Opcode.EX, Operand.mSP, index.Register);
                        break;

                    case PrimaryOpCode.DAA:
                        result = new DecodeResult(Opcode.DAA);
                        break;
                    case PrimaryOpCode.CPL:
                        result = new DecodeResult(Opcode.CPL);
                        break;
                    case PrimaryOpCode.CCF:
                        result = new DecodeResult(Opcode.CCF);
                        break;
                    case PrimaryOpCode.SCF:
                        result = new DecodeResult(Opcode.SCF);
                        break;
                    case PrimaryOpCode.DI:
                        result = new DecodeResult(Opcode.DI);
                        break;
                    case PrimaryOpCode.EI:
                        result = new DecodeResult(Opcode.EI);
                        break;

                    case PrimaryOpCode.RLCA:
                        result = new DecodeResult(Opcode.RLCA);
                        break;
                    case PrimaryOpCode.RLA:
                        result = new DecodeResult(Opcode.RLA);
                        break;
                    case PrimaryOpCode.RRCA:
                        result = new DecodeResult(Opcode.RRCA);
                        break;
                    case PrimaryOpCode.RRA:
                        result = new DecodeResult(Opcode.RRA);
                        break;

                    case PrimaryOpCode.JP:
                        result = new DecodeResult(Opcode.JP, Operand.nn).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JP_NZ:
                        result = new DecodeResult(Opcode.JP, Operand.nn).WithFlag(FlagTest.NZ).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JP_Z:
                        result = new DecodeResult(Opcode.JP, Operand.nn).WithFlag(FlagTest.Z).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JP_NC:
                        result = new DecodeResult(Opcode.JP, Operand.nn).WithFlag(FlagTest.NC).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JP_C:
                        result = new DecodeResult(Opcode.JP, Operand.nn).WithFlag(FlagTest.C).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JP_PO:
                        result = new DecodeResult(Opcode.JP, Operand.nn).WithFlag(FlagTest.PO).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JP_PE:
                        result = new DecodeResult(Opcode.JP, Operand.nn).WithFlag(FlagTest.PE).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JP_P:
                        result = new DecodeResult(Opcode.JP, Operand.nn).WithFlag(FlagTest.P).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JP_M:
                        result = new DecodeResult(Opcode.JP, Operand.nn).WithFlag(FlagTest.M).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JR:
                        result = new DecodeResult(Opcode.JR, Operand.d).WithByteLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JR_C:
                        result = new DecodeResult(Opcode.JR, Operand.d).WithFlag(FlagTest.C).WithByteLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JR_NC:
                        result = new DecodeResult(Opcode.JR, Operand.d).WithFlag(FlagTest.NC).WithByteLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JR_Z:
                        result = new DecodeResult(Opcode.JR, Operand.d).WithFlag(FlagTest.Z).WithByteLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JR_NZ:
                        result = new DecodeResult(Opcode.JR, Operand.d).WithFlag(FlagTest.NZ).WithByteLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.JP_mHL:
                        result = new DecodeResult(Opcode.JP, Operand.HL).EndBlock();
                        break;

                    case PrimaryOpCode.DJNZ:
                        result = new DecodeResult(Opcode.DJNZ, Operand.d).WithByteLiteral().EndBlock();
                        break;

                    case PrimaryOpCode.CALL:
                        result = new DecodeResult(Opcode.CALL, Operand.nn).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.CALL_NZ:
                        result = new DecodeResult(Opcode.CALL, Operand.nn).WithFlag(FlagTest.NZ).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.CALL_Z:
                        result = new DecodeResult(Opcode.CALL, Operand.nn).WithFlag(FlagTest.Z).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.CALL_NC:
                        result = new DecodeResult(Opcode.CALL, Operand.nn).WithFlag(FlagTest.NC).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.CALL_C:
                        result = new DecodeResult(Opcode.CALL, Operand.nn).WithFlag(FlagTest.C).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.CALL_PO:
                        result = new DecodeResult(Opcode.CALL, Operand.nn).WithFlag(FlagTest.PO).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.CALL_PE:
                        result = new DecodeResult(Opcode.CALL, Operand.nn).WithFlag(FlagTest.PE).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.CALL_P:
                        result = new DecodeResult(Opcode.CALL, Operand.nn).WithFlag(FlagTest.P).WithWordLiteral().EndBlock();
                        break;
                    case PrimaryOpCode.CALL_M:
                        result = new DecodeResult(Opcode.CALL, Operand.nn).WithFlag(FlagTest.M).WithWordLiteral().EndBlock();
                        break;

                    case PrimaryOpCode.RET:
                        result = new DecodeResult(Opcode.RET).EndBlock();
                        break;
                    case PrimaryOpCode.RET_NZ:
                        result = new DecodeResult(Opcode.RET).WithFlag(FlagTest.NZ).EndBlock();
                        break;
                    case PrimaryOpCode.RET_Z:
                        result = new DecodeResult(Opcode.RET).WithFlag(FlagTest.Z).EndBlock();
                        break;
                    case PrimaryOpCode.RET_NC:
                        result = new DecodeResult(Opcode.RET).WithFlag(FlagTest.NC).EndBlock();
                        break;
                    case PrimaryOpCode.RET_C:
                        result = new DecodeResult(Opcode.RET).WithFlag(FlagTest.C).EndBlock();
                        break;
                    case PrimaryOpCode.RET_PO:
                        result = new DecodeResult(Opcode.RET).WithFlag(FlagTest.PO).EndBlock();
                        break;
                    case PrimaryOpCode.RET_PE:
                        result = new DecodeResult(Opcode.RET).WithFlag(FlagTest.PE).EndBlock();
                        break;
                    case PrimaryOpCode.RET_P:
                        result = new DecodeResult(Opcode.RET).WithFlag(FlagTest.P).EndBlock();
                        break;
                    case PrimaryOpCode.RET_M:
                        result = new DecodeResult(Opcode.RET).WithFlag(FlagTest.M).EndBlock();
                        break;

                    case PrimaryOpCode.RST_00:
                        result = new DecodeResult(Opcode.RET, Operand.nn).AddLiteral((ushort)0x0000).EndBlock();
                        break;
                    case PrimaryOpCode.RST_08:
                        result = new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0008).EndBlock();
                        break;
                    case PrimaryOpCode.RST_10:
                        result = new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0010).EndBlock();
                        break;
                    case PrimaryOpCode.RST_18:
                        result = new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0018).EndBlock();
                        break;
                    case PrimaryOpCode.RST_20:
                        result = new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0020).EndBlock();
                        break;
                    case PrimaryOpCode.RST_28:
                        result = new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0028).EndBlock();
                        break;
                    case PrimaryOpCode.RST_30:
                        result = new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0030).EndBlock();
                        break;
                    case PrimaryOpCode.RST_38:
                        result = new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0038).EndBlock();
                        break;

                    case PrimaryOpCode.IN_A_n:
                        result = new DecodeResult(Opcode.IN, Operand.A, Operand.n).WithByteLiteral();
                        break;
                    case PrimaryOpCode.OUT_A_n:
                        result = new DecodeResult(Opcode.OUT, Operand.A, Operand.n).WithByteLiteral();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(code), code, null);
                }

                if (result.OpCodeMeta.HasFlag(OpCodeMeta.Displacement) && index.IsDisplaced)
                {
                    result.AddDisplacement(prefetch.NextByte());
                }

                if (result.OpCodeMeta.HasFlag(OpCodeMeta.ByteLiteral))
                {
                    result.AddLiteral(prefetch.NextByte());
                }
                
                if (result.OpCodeMeta.HasFlag(OpCodeMeta.WordLiteral))
                {
                    result.AddLiteral(prefetch.NextWord());
                }

                yield return result;

                if (result.OpCodeMeta.HasFlag(OpCodeMeta.EndBlock))
                {
                    yield break;
                }

                index = this.indexRegisterOperands[IndexRegister.HL];
            }

        }
    }
}
