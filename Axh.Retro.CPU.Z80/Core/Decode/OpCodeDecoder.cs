namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;

    internal class OpCodeDecoder
    {
        public static DecodeResult? GetDecodeResult(IPrefetchQueue prefetchQueue)
        {
            var code = (PrimaryOpCode)prefetchQueue.NextByte();
            switch (code)
            {
                case PrimaryOpCode.NOP:
                    return new DecodeResult(Opcode.NOP);
                case PrimaryOpCode.HALT:
                    return new DecodeResult(Opcode.HALT);
                case PrimaryOpCode.Prefix_CB:
                case PrimaryOpCode.Prefix_DD:
                case PrimaryOpCode.Prefix_ED:
                case PrimaryOpCode.Prefix_FD:
                    return null;
                case PrimaryOpCode.LD_A_A:
                    return new DecodeResult(Opcode.LD, Operand.A, Operand.A);
                case PrimaryOpCode.LD_B_A:
                    return new DecodeResult(Opcode.LD, Operand.B, Operand.A);
                case PrimaryOpCode.LD_C_A:
                    return new DecodeResult(Opcode.LD, Operand.C, Operand.A);
                case PrimaryOpCode.LD_D_A:
                    return new DecodeResult(Opcode.LD, Operand.D, Operand.A);
                case PrimaryOpCode.LD_E_A:
                    return new DecodeResult(Opcode.LD, Operand.E, Operand.A);
                case PrimaryOpCode.LD_H_A:
                    return new DecodeResult(Opcode.LD, Operand.H, Operand.A);
                case PrimaryOpCode.LD_L_A:
                    return new DecodeResult(Opcode.LD, Operand.L, Operand.A);
                case PrimaryOpCode.LD_A_B:
                    return new DecodeResult(Opcode.LD, Operand.A, Operand.B);
                case PrimaryOpCode.LD_B_B:
                    return new DecodeResult(Opcode.LD, Operand.B, Operand.B);
                case PrimaryOpCode.LD_C_B:
                    return new DecodeResult(Opcode.LD, Operand.C, Operand.B);
                case PrimaryOpCode.LD_D_B:
                    return new DecodeResult(Opcode.LD, Operand.D, Operand.B);
                case PrimaryOpCode.LD_E_B:
                    return new DecodeResult(Opcode.LD, Operand.E, Operand.B);
                case PrimaryOpCode.LD_H_B:
                    return new DecodeResult(Opcode.LD, Operand.H, Operand.B);
                case PrimaryOpCode.LD_L_B:
                    return new DecodeResult(Opcode.LD, Operand.L, Operand.B);
                case PrimaryOpCode.LD_A_C:
                    return new DecodeResult(Opcode.LD, Operand.A, Operand.C);
                case PrimaryOpCode.LD_B_C:
                    return new DecodeResult(Opcode.LD, Operand.B, Operand.C);
                case PrimaryOpCode.LD_C_C:
                    return new DecodeResult(Opcode.LD, Operand.C, Operand.C);
                case PrimaryOpCode.LD_D_C:
                    return new DecodeResult(Opcode.LD, Operand.D, Operand.C);
                case PrimaryOpCode.LD_E_C:
                    return new DecodeResult(Opcode.LD, Operand.E, Operand.C);
                case PrimaryOpCode.LD_H_C:
                    return new DecodeResult(Opcode.LD, Operand.H, Operand.C);
                case PrimaryOpCode.LD_L_C:
                    return new DecodeResult(Opcode.LD, Operand.L, Operand.C);
                case PrimaryOpCode.LD_A_D:
                    return new DecodeResult(Opcode.LD, Operand.A, Operand.D);
                case PrimaryOpCode.LD_B_D:
                    return new DecodeResult(Opcode.LD, Operand.B, Operand.D);
                case PrimaryOpCode.LD_C_D:
                    return new DecodeResult(Opcode.LD, Operand.C, Operand.D);
                case PrimaryOpCode.LD_D_D:
                    return new DecodeResult(Opcode.LD, Operand.D, Operand.D);
                case PrimaryOpCode.LD_E_D:
                    return new DecodeResult(Opcode.LD, Operand.E, Operand.D);
                case PrimaryOpCode.LD_H_D:
                    return new DecodeResult(Opcode.LD, Operand.H, Operand.D);
                case PrimaryOpCode.LD_L_D:
                    return new DecodeResult(Opcode.LD, Operand.L, Operand.D);
                case PrimaryOpCode.LD_A_E:
                    return new DecodeResult(Opcode.LD, Operand.A, Operand.E);
                case PrimaryOpCode.LD_B_E:
                    return new DecodeResult(Opcode.LD, Operand.B, Operand.E);
                case PrimaryOpCode.LD_C_E:
                    return new DecodeResult(Opcode.LD, Operand.C, Operand.E);
                case PrimaryOpCode.LD_D_E:
                    return new DecodeResult(Opcode.LD, Operand.D, Operand.E);
                case PrimaryOpCode.LD_E_E:
                    return new DecodeResult(Opcode.LD, Operand.E, Operand.E);
                case PrimaryOpCode.LD_H_E:
                    return new DecodeResult(Opcode.LD, Operand.H, Operand.E);
                case PrimaryOpCode.LD_L_E:
                    return new DecodeResult(Opcode.LD, Operand.L, Operand.E);
                case PrimaryOpCode.LD_A_H:
                    return new DecodeResult(Opcode.LD, Operand.A, Operand.H);
                case PrimaryOpCode.LD_B_H:
                    return new DecodeResult(Opcode.LD, Operand.B, Operand.H);
                case PrimaryOpCode.LD_C_H:
                    return new DecodeResult(Opcode.LD, Operand.C, Operand.H);
                case PrimaryOpCode.LD_D_H:
                    return new DecodeResult(Opcode.LD, Operand.D, Operand.H);
                case PrimaryOpCode.LD_E_H:
                    return new DecodeResult(Opcode.LD, Operand.E, Operand.H);
                case PrimaryOpCode.LD_H_H:
                    return new DecodeResult(Opcode.LD, Operand.H, Operand.H);
                case PrimaryOpCode.LD_L_H:
                    return new DecodeResult(Opcode.LD, Operand.L, Operand.H);
                case PrimaryOpCode.LD_A_L:
                    return new DecodeResult(Opcode.LD, Operand.A, Operand.L);
                case PrimaryOpCode.LD_B_L:
                    return new DecodeResult(Opcode.LD, Operand.B, Operand.L);
                case PrimaryOpCode.LD_C_L:
                    return new DecodeResult(Opcode.LD, Operand.C, Operand.L);
                case PrimaryOpCode.LD_D_L:
                    return new DecodeResult(Opcode.LD, Operand.D, Operand.L);
                case PrimaryOpCode.LD_E_L:
                    return new DecodeResult(Opcode.LD, Operand.E, Operand.L);
                case PrimaryOpCode.LD_H_L:
                    return new DecodeResult(Opcode.LD, Operand.H, Operand.L);
                case PrimaryOpCode.LD_L_L:
                    return new DecodeResult(Opcode.LD, Operand.L, Operand.L);
                case PrimaryOpCode.LD_A_n:
                    return new DecodeResult(Opcode.LD, Operand.A, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.LD_B_n:
                    return new DecodeResult(Opcode.LD, Operand.B, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.LD_C_n:
                    return new DecodeResult(Opcode.LD, Operand.C, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.LD_D_n:
                    return new DecodeResult(Opcode.LD, Operand.D, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.LD_E_n:
                    return new DecodeResult(Opcode.LD, Operand.E, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.LD_H_n:
                    return new DecodeResult(Opcode.LD, Operand.H, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.LD_L_n:
                    return new DecodeResult(Opcode.LD, Operand.L, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.LD_A_mHL:
                    return new DecodeResult(Opcode.LD, Operand.A, Operand.mHL);
                case PrimaryOpCode.LD_B_mHL:
                    return new DecodeResult(Opcode.LD, Operand.B, Operand.mHL);
                case PrimaryOpCode.LD_C_mHL:
                    return new DecodeResult(Opcode.LD, Operand.C, Operand.mHL);
                case PrimaryOpCode.LD_D_mHL:
                    return new DecodeResult(Opcode.LD, Operand.D, Operand.mHL);
                case PrimaryOpCode.LD_E_mHL:
                    return new DecodeResult(Opcode.LD, Operand.E, Operand.mHL);
                case PrimaryOpCode.LD_H_mHL:
                    return new DecodeResult(Opcode.LD, Operand.H, Operand.mHL);
                case PrimaryOpCode.LD_L_mHL:
                    return new DecodeResult(Opcode.LD, Operand.L, Operand.mHL);
                case PrimaryOpCode.LD_mHL_A:
                    return new DecodeResult(Opcode.LD, Operand.mHL, Operand.A);
                case PrimaryOpCode.LD_mHL_B:
                    return new DecodeResult(Opcode.LD, Operand.mHL, Operand.B);
                case PrimaryOpCode.LD_mHL_C:
                    return new DecodeResult(Opcode.LD, Operand.mHL, Operand.C);
                case PrimaryOpCode.LD_mHL_D:
                    return new DecodeResult(Opcode.LD, Operand.mHL, Operand.D);
                case PrimaryOpCode.LD_mHL_E:
                    return new DecodeResult(Opcode.LD, Operand.mHL, Operand.E);
                case PrimaryOpCode.LD_mHL_H:
                    return new DecodeResult(Opcode.LD, Operand.mHL, Operand.H);
                case PrimaryOpCode.LD_mHL_L:
                    return new DecodeResult(Opcode.LD, Operand.mHL, Operand.L);
                case PrimaryOpCode.LD_mHL_n:
                    return new DecodeResult(Opcode.LD, Operand.mHL, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.LD_A_mBC:
                    return new DecodeResult(Opcode.LD, Operand.A, Operand.mBC);
                case PrimaryOpCode.LD_A_mDE:
                    return new DecodeResult(Opcode.LD, Operand.A, Operand.mDE);
                case PrimaryOpCode.LD_A_mnn:
                    return new DecodeResult(Opcode.LD, Operand.A, Operand.mnn, prefetchQueue.NextWord());
                case PrimaryOpCode.LD_mBC_A:
                    return new DecodeResult(Opcode.LD, Operand.mBC, Operand.A);
                case PrimaryOpCode.LD_mDE_A:
                    return new DecodeResult(Opcode.LD, Operand.mDE, Operand.A);
                case PrimaryOpCode.LD_mnn_A:
                    return new DecodeResult(Opcode.LD, Operand.mnn, Operand.A, prefetchQueue.NextWord());

                case PrimaryOpCode.LD_BC_nn:
                    return new DecodeResult(Opcode.LD16, Operand.BC, Operand.nn, prefetchQueue.NextWord());
                case PrimaryOpCode.LD_DE_nn:
                    return new DecodeResult(Opcode.LD16, Operand.DE, Operand.nn, prefetchQueue.NextWord());
                case PrimaryOpCode.LD_HL_nn:
                    return new DecodeResult(Opcode.LD16, Operand.HL, Operand.nn, prefetchQueue.NextWord());
                case PrimaryOpCode.LD_SP_nn:
                    return new DecodeResult(Opcode.LD16, Operand.SP, Operand.nn, prefetchQueue.NextWord());
                case PrimaryOpCode.LD_HL_mnn:
                    return new DecodeResult(Opcode.LD16, Operand.HL, Operand.mnn, prefetchQueue.NextWord());
                case PrimaryOpCode.LD_mnn_HL:
                    return new DecodeResult(Opcode.LD16, Operand.mnn, Operand.HL, prefetchQueue.NextWord());
                case PrimaryOpCode.LD_SP_HL:
                    return new DecodeResult(Opcode.LD16, Operand.SP, Operand.HL);

                case PrimaryOpCode.PUSH_BC:
                    return new DecodeResult(Opcode.PUSH, Operand.BC);
                case PrimaryOpCode.PUSH_DE:
                    return new DecodeResult(Opcode.PUSH, Operand.DE);
                case PrimaryOpCode.PUSH_HL:
                    return new DecodeResult(Opcode.PUSH, Operand.HL);
                case PrimaryOpCode.PUSH_AF:
                    return new DecodeResult(Opcode.PUSH, Operand.AF);

                case PrimaryOpCode.POP_BC:
                    return new DecodeResult(Opcode.POP, Operand.BC);
                case PrimaryOpCode.POP_DE:
                    return new DecodeResult(Opcode.POP, Operand.DE);
                case PrimaryOpCode.POP_HL:
                    return new DecodeResult(Opcode.POP, Operand.HL);
                case PrimaryOpCode.POP_AF:
                    return new DecodeResult(Opcode.POP, Operand.AF);

                case PrimaryOpCode.ADD_A_A:
                    return new DecodeResult(Opcode.ADD, Operand.A, Operand.A);
                case PrimaryOpCode.ADD_A_B:
                    return new DecodeResult(Opcode.ADD, Operand.A, Operand.B);
                case PrimaryOpCode.ADD_A_C:
                    return new DecodeResult(Opcode.ADD, Operand.A, Operand.C);
                case PrimaryOpCode.ADD_A_D:
                    return new DecodeResult(Opcode.ADD, Operand.A, Operand.D);
                case PrimaryOpCode.ADD_A_E:
                    return new DecodeResult(Opcode.ADD, Operand.A, Operand.E);
                case PrimaryOpCode.ADD_A_H:
                    return new DecodeResult(Opcode.ADD, Operand.A, Operand.H);
                case PrimaryOpCode.ADD_A_L:
                    return new DecodeResult(Opcode.ADD, Operand.A, Operand.L);
                case PrimaryOpCode.ADD_A_n:
                    return new DecodeResult(Opcode.ADD, Operand.A, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.ADD_A_mHL:
                    return new DecodeResult(Opcode.ADD, Operand.A, Operand.mHL);

                case PrimaryOpCode.ADC_A_A:
                    return new DecodeResult(Opcode.ADC, Operand.A, Operand.A);
                case PrimaryOpCode.ADC_A_B:
                    return new DecodeResult(Opcode.ADC, Operand.A, Operand.B);
                case PrimaryOpCode.ADC_A_C:
                    return new DecodeResult(Opcode.ADC, Operand.A, Operand.C);
                case PrimaryOpCode.ADC_A_D:
                    return new DecodeResult(Opcode.ADC, Operand.A, Operand.D);
                case PrimaryOpCode.ADC_A_E:
                    return new DecodeResult(Opcode.ADC, Operand.A, Operand.E);
                case PrimaryOpCode.ADC_A_H:
                    return new DecodeResult(Opcode.ADC, Operand.A, Operand.H);
                case PrimaryOpCode.ADC_A_L:
                    return new DecodeResult(Opcode.ADC, Operand.A, Operand.L);
                case PrimaryOpCode.ADC_A_n:
                    return new DecodeResult(Opcode.ADC, Operand.A, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.ADC_A_mHL:
                    return new DecodeResult(Opcode.ADC, Operand.A, Operand.mHL);

                case PrimaryOpCode.SUB_A_A:
                    return new DecodeResult(Opcode.SUB, Operand.A, Operand.A);
                case PrimaryOpCode.SUB_A_B:
                    return new DecodeResult(Opcode.SUB, Operand.A, Operand.B);
                case PrimaryOpCode.SUB_A_C:
                    return new DecodeResult(Opcode.SUB, Operand.A, Operand.C);
                case PrimaryOpCode.SUB_A_D:
                    return new DecodeResult(Opcode.SUB, Operand.A, Operand.D);
                case PrimaryOpCode.SUB_A_E:
                    return new DecodeResult(Opcode.SUB, Operand.A, Operand.E);
                case PrimaryOpCode.SUB_A_H:
                    return new DecodeResult(Opcode.SUB, Operand.A, Operand.H);
                case PrimaryOpCode.SUB_A_L:
                    return new DecodeResult(Opcode.SUB, Operand.A, Operand.L);
                case PrimaryOpCode.SUB_A_n:
                    return new DecodeResult(Opcode.SUB, Operand.A, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.SUB_A_mHL:
                    return new DecodeResult(Opcode.SUB, Operand.A, Operand.mHL);

                case PrimaryOpCode.SBC_A_A:
                    return new DecodeResult(Opcode.SBC, Operand.A, Operand.A);
                case PrimaryOpCode.SBC_A_B:
                    return new DecodeResult(Opcode.SBC, Operand.A, Operand.B);
                case PrimaryOpCode.SBC_A_C:
                    return new DecodeResult(Opcode.SBC, Operand.A, Operand.C);
                case PrimaryOpCode.SBC_A_D:
                    return new DecodeResult(Opcode.SBC, Operand.A, Operand.D);
                case PrimaryOpCode.SBC_A_E:
                    return new DecodeResult(Opcode.SBC, Operand.A, Operand.E);
                case PrimaryOpCode.SBC_A_H:
                    return new DecodeResult(Opcode.SBC, Operand.A, Operand.H);
                case PrimaryOpCode.SBC_A_L:
                    return new DecodeResult(Opcode.SBC, Operand.A, Operand.L);
                case PrimaryOpCode.SBC_A_n:
                    return new DecodeResult(Opcode.SBC, Operand.A, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.SBC_A_mHL:
                    return new DecodeResult(Opcode.SBC, Operand.A, Operand.mHL);

                case PrimaryOpCode.AND_A:
                    return new DecodeResult(Opcode.AND, Operand.A, Operand.A);
                case PrimaryOpCode.AND_B:
                    return new DecodeResult(Opcode.AND, Operand.A, Operand.B);
                case PrimaryOpCode.AND_C:
                    return new DecodeResult(Opcode.AND, Operand.A, Operand.C);
                case PrimaryOpCode.AND_D:
                    return new DecodeResult(Opcode.AND, Operand.A, Operand.D);
                case PrimaryOpCode.AND_E:
                    return new DecodeResult(Opcode.AND, Operand.A, Operand.E);
                case PrimaryOpCode.AND_H:
                    return new DecodeResult(Opcode.AND, Operand.A, Operand.H);
                case PrimaryOpCode.AND_L:
                    return new DecodeResult(Opcode.AND, Operand.A, Operand.L);
                case PrimaryOpCode.AND_n:
                    return new DecodeResult(Opcode.AND, Operand.A, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.AND_mHL:
                    return new DecodeResult(Opcode.AND, Operand.A, Operand.mHL);

                case PrimaryOpCode.OR_A:
                    return new DecodeResult(Opcode.OR, Operand.A, Operand.A);
                case PrimaryOpCode.OR_B:
                    return new DecodeResult(Opcode.OR, Operand.A, Operand.B);
                case PrimaryOpCode.OR_C:
                    return new DecodeResult(Opcode.OR, Operand.A, Operand.C);
                case PrimaryOpCode.OR_D:
                    return new DecodeResult(Opcode.OR, Operand.A, Operand.D);
                case PrimaryOpCode.OR_E:
                    return new DecodeResult(Opcode.OR, Operand.A, Operand.E);
                case PrimaryOpCode.OR_H:
                    return new DecodeResult(Opcode.OR, Operand.A, Operand.H);
                case PrimaryOpCode.OR_L:
                    return new DecodeResult(Opcode.OR, Operand.A, Operand.L);
                case PrimaryOpCode.OR_n:
                    return new DecodeResult(Opcode.OR, Operand.A, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.OR_mHL:
                    return new DecodeResult(Opcode.OR, Operand.A, Operand.mHL);

                case PrimaryOpCode.XOR_A:
                    return new DecodeResult(Opcode.XOR, Operand.A, Operand.A);
                case PrimaryOpCode.XOR_B:
                    return new DecodeResult(Opcode.XOR, Operand.A, Operand.B);
                case PrimaryOpCode.XOR_C:
                    return new DecodeResult(Opcode.XOR, Operand.A, Operand.C);
                case PrimaryOpCode.XOR_D:
                    return new DecodeResult(Opcode.XOR, Operand.A, Operand.D);
                case PrimaryOpCode.XOR_E:
                    return new DecodeResult(Opcode.XOR, Operand.A, Operand.E);
                case PrimaryOpCode.XOR_H:
                    return new DecodeResult(Opcode.XOR, Operand.A, Operand.H);
                case PrimaryOpCode.XOR_L:
                    return new DecodeResult(Opcode.XOR, Operand.A, Operand.L);
                case PrimaryOpCode.XOR_n:
                    return new DecodeResult(Opcode.XOR, Operand.A, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.XOR_mHL:
                    return new DecodeResult(Opcode.XOR, Operand.A, Operand.mHL);

                case PrimaryOpCode.CP_A:
                    return new DecodeResult(Opcode.CP, Operand.A, Operand.A);
                case PrimaryOpCode.CP_B:
                    return new DecodeResult(Opcode.CP, Operand.A, Operand.B);
                case PrimaryOpCode.CP_C:
                    return new DecodeResult(Opcode.CP, Operand.A, Operand.C);
                case PrimaryOpCode.CP_D:
                    return new DecodeResult(Opcode.CP, Operand.A, Operand.D);
                case PrimaryOpCode.CP_E:
                    return new DecodeResult(Opcode.CP, Operand.A, Operand.E);
                case PrimaryOpCode.CP_H:
                    return new DecodeResult(Opcode.CP, Operand.A, Operand.H);
                case PrimaryOpCode.CP_L:
                    return new DecodeResult(Opcode.CP, Operand.A, Operand.L);
                case PrimaryOpCode.CP_n:
                    return new DecodeResult(Opcode.CP, Operand.A, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.CP_mHL:
                    return new DecodeResult(Opcode.CP, Operand.A, Operand.mHL);

                case PrimaryOpCode.INC_A:
                    return new DecodeResult(Opcode.INC, Operand.A);
                case PrimaryOpCode.INC_B:
                    return new DecodeResult(Opcode.INC, Operand.B);
                case PrimaryOpCode.INC_C:
                    return new DecodeResult(Opcode.INC, Operand.C);
                case PrimaryOpCode.INC_D:
                    return new DecodeResult(Opcode.INC, Operand.D);
                case PrimaryOpCode.INC_E:
                    return new DecodeResult(Opcode.INC, Operand.E);
                case PrimaryOpCode.INC_H:
                    return new DecodeResult(Opcode.INC, Operand.H);
                case PrimaryOpCode.INC_L:
                    return new DecodeResult(Opcode.INC, Operand.L);
                case PrimaryOpCode.INC_mHL:
                    return new DecodeResult(Opcode.INC, Operand.mHL);

                case PrimaryOpCode.DEC_A:
                    return new DecodeResult(Opcode.DEC, Operand.A);
                case PrimaryOpCode.DEC_B:
                    return new DecodeResult(Opcode.DEC, Operand.B);
                case PrimaryOpCode.DEC_C:
                    return new DecodeResult(Opcode.DEC, Operand.C);
                case PrimaryOpCode.DEC_D:
                    return new DecodeResult(Opcode.DEC, Operand.D);
                case PrimaryOpCode.DEC_E:
                    return new DecodeResult(Opcode.DEC, Operand.E);
                case PrimaryOpCode.DEC_H:
                    return new DecodeResult(Opcode.DEC, Operand.H);
                case PrimaryOpCode.DEC_L:
                    return new DecodeResult(Opcode.DEC, Operand.L);
                case PrimaryOpCode.DEC_mHL:
                    return new DecodeResult(Opcode.DEC, Operand.mHL);

                case PrimaryOpCode.ADD_HL_BC:
                    return new DecodeResult(Opcode.ADD16, Operand.HL, Operand.BC);
                case PrimaryOpCode.ADD_HL_DE:
                    return new DecodeResult(Opcode.ADD16, Operand.HL, Operand.DE);
                case PrimaryOpCode.ADD_HL_HL:
                    return new DecodeResult(Opcode.ADD16, Operand.HL, Operand.HL);
                case PrimaryOpCode.ADD_HL_SP:
                    return new DecodeResult(Opcode.ADD16, Operand.HL, Operand.SP);

                case PrimaryOpCode.INC_BC:
                    return new DecodeResult(Opcode.INC16, Operand.BC);
                case PrimaryOpCode.INC_DE:
                    return new DecodeResult(Opcode.INC16, Operand.DE);
                case PrimaryOpCode.INC_HL:
                    return new DecodeResult(Opcode.INC16, Operand.HL);
                case PrimaryOpCode.INC_SP:
                    return new DecodeResult(Opcode.INC16, Operand.SP);

                case PrimaryOpCode.DEC_BC:
                    return new DecodeResult(Opcode.DEC16, Operand.BC);
                case PrimaryOpCode.DEC_DE:
                    return new DecodeResult(Opcode.DEC16, Operand.DE);
                case PrimaryOpCode.DEC_HL:
                    return new DecodeResult(Opcode.DEC16, Operand.HL);
                case PrimaryOpCode.DEC_SP:
                    return new DecodeResult(Opcode.DEC16, Operand.SP);

                case PrimaryOpCode.EX_DE_HL:
                    return new DecodeResult(Opcode.EX, Operand.DE, Operand.HL);
                case PrimaryOpCode.EX_AF:
                    return new DecodeResult(Opcode.EX, Operand.AF);
                case PrimaryOpCode.EXX:
                    return new DecodeResult(Opcode.EXX);
                case PrimaryOpCode.EX_mSP_HL:
                    return new DecodeResult(Opcode.EX, Operand.mSP, Operand.HL);

                case PrimaryOpCode.DAA:
                    return new DecodeResult(Opcode.DAA);
                case PrimaryOpCode.CPL:
                    return new DecodeResult(Opcode.CPL);
                case PrimaryOpCode.CCF:
                    return new DecodeResult(Opcode.CCF);
                case PrimaryOpCode.SCF:
                    return new DecodeResult(Opcode.SCF);
                case PrimaryOpCode.DI:
                    return new DecodeResult(Opcode.DI);
                case PrimaryOpCode.EI:
                    return new DecodeResult(Opcode.EI);

                case PrimaryOpCode.RLCA:
                    return new DecodeResult(Opcode.RLCA);
                case PrimaryOpCode.RLA:
                    return new DecodeResult(Opcode.RLA);
                case PrimaryOpCode.RRCA:
                    return new DecodeResult(Opcode.RRCA);
                case PrimaryOpCode.RRA:
                    return new DecodeResult(Opcode.RRA);

                case PrimaryOpCode.JP:
                    return new DecodeResult(Opcode.JP, Operand.nn, prefetchQueue.NextWord());
                case PrimaryOpCode.JP_NZ:
                    return new DecodeResult(Opcode.JP, Operand.nn, FlagTest.NZ, prefetchQueue.NextWord());
                case PrimaryOpCode.JP_Z:
                    return new DecodeResult(Opcode.JP, Operand.nn, FlagTest.Z, prefetchQueue.NextWord());
                case PrimaryOpCode.JP_NC:
                    return new DecodeResult(Opcode.JP, Operand.nn, FlagTest.NC, prefetchQueue.NextWord());
                case PrimaryOpCode.JP_C:
                    return new DecodeResult(Opcode.JP, Operand.nn, FlagTest.C, prefetchQueue.NextWord());
                case PrimaryOpCode.JP_PO:
                    return new DecodeResult(Opcode.JP, Operand.nn, FlagTest.PO, prefetchQueue.NextWord());
                case PrimaryOpCode.JP_PE:
                    return new DecodeResult(Opcode.JP, Operand.nn, FlagTest.PE, prefetchQueue.NextWord());
                case PrimaryOpCode.JP_P:
                    return new DecodeResult(Opcode.JP, Operand.nn, FlagTest.P, prefetchQueue.NextWord());
                case PrimaryOpCode.JP_M:
                    return new DecodeResult(Opcode.JP, Operand.nn, FlagTest.M, prefetchQueue.NextWord());
                case PrimaryOpCode.JR:
                    return new DecodeResult(Opcode.JR, Operand.d, prefetchQueue.NextByte());
                case PrimaryOpCode.JR_C:
                    return new DecodeResult(Opcode.JR, Operand.d, FlagTest.C, prefetchQueue.NextByte());
                case PrimaryOpCode.JR_NC:
                    return new DecodeResult(Opcode.JR, Operand.d, FlagTest.NC, prefetchQueue.NextByte());
                case PrimaryOpCode.JR_Z:
                    return new DecodeResult(Opcode.JR, Operand.d, FlagTest.Z, prefetchQueue.NextByte());
                case PrimaryOpCode.JR_NZ:
                    return new DecodeResult(Opcode.JR, Operand.d, FlagTest.NZ, prefetchQueue.NextByte());
                case PrimaryOpCode.JP_mHL:
                    return new DecodeResult(Opcode.JP, Operand.HL);

                case PrimaryOpCode.DJNZ:
                    return new DecodeResult(Opcode.DJNZ, Operand.d, prefetchQueue.NextByte());

                case PrimaryOpCode.CALL:
                    return new DecodeResult(Opcode.CALL, Operand.nn, prefetchQueue.NextWord());
                case PrimaryOpCode.CALL_NZ:
                    return new DecodeResult(Opcode.CALL, Operand.nn, FlagTest.NZ, prefetchQueue.NextWord());
                case PrimaryOpCode.CALL_Z:
                    return new DecodeResult(Opcode.CALL, Operand.nn, FlagTest.Z, prefetchQueue.NextWord());
                case PrimaryOpCode.CALL_NC:
                    return new DecodeResult(Opcode.CALL, Operand.nn, FlagTest.NC, prefetchQueue.NextWord());
                case PrimaryOpCode.CALL_C:
                    return new DecodeResult(Opcode.CALL, Operand.nn, FlagTest.C, prefetchQueue.NextWord());
                case PrimaryOpCode.CALL_PO:
                    return new DecodeResult(Opcode.CALL, Operand.nn, FlagTest.PO, prefetchQueue.NextWord());
                case PrimaryOpCode.CALL_PE:
                    return new DecodeResult(Opcode.CALL, Operand.nn, FlagTest.PE, prefetchQueue.NextWord());
                case PrimaryOpCode.CALL_P:
                    return new DecodeResult(Opcode.CALL, Operand.nn, FlagTest.P, prefetchQueue.NextWord());
                case PrimaryOpCode.CALL_M:
                    return new DecodeResult(Opcode.CALL, Operand.nn, FlagTest.M, prefetchQueue.NextWord());

                case PrimaryOpCode.RET:
                    return new DecodeResult(Opcode.RET);
                case PrimaryOpCode.RET_NZ:
                    return new DecodeResult(Opcode.RET, FlagTest.NZ);
                case PrimaryOpCode.RET_Z:
                    return new DecodeResult(Opcode.RET, FlagTest.Z);
                case PrimaryOpCode.RET_NC:
                    return new DecodeResult(Opcode.RET, FlagTest.NC);
                case PrimaryOpCode.RET_C:
                    return new DecodeResult(Opcode.RET, FlagTest.C);
                case PrimaryOpCode.RET_PO:
                    return new DecodeResult(Opcode.RET, FlagTest.PO);
                case PrimaryOpCode.RET_PE:
                    return new DecodeResult(Opcode.RET, FlagTest.PE);
                case PrimaryOpCode.RET_P:
                    return new DecodeResult(Opcode.RET, FlagTest.P);
                case PrimaryOpCode.RET_M:
                    return new DecodeResult(Opcode.RET, FlagTest.M);

                case PrimaryOpCode.RST_00:
                    return new DecodeResult(Opcode.RET, Operand.nn, (ushort)0x0000);
                case PrimaryOpCode.RST_08:
                    return new DecodeResult(Opcode.RET, Operand.nn, 0x0008);
                case PrimaryOpCode.RST_10:
                    return new DecodeResult(Opcode.RET, Operand.nn, 0x0010);
                case PrimaryOpCode.RST_18:
                    return new DecodeResult(Opcode.RET, Operand.nn, 0x0018);
                case PrimaryOpCode.RST_20:
                    return new DecodeResult(Opcode.RET, Operand.nn, 0x0020);
                case PrimaryOpCode.RST_28:
                    return new DecodeResult(Opcode.RET, Operand.nn, 0x0028);
                case PrimaryOpCode.RST_30:
                    return new DecodeResult(Opcode.RET, Operand.nn, 0x0030);
                case PrimaryOpCode.RST_38:
                    return new DecodeResult(Opcode.RET, Operand.nn, 0x0038);

                case PrimaryOpCode.IN_A_n:
                    return new DecodeResult(Opcode.IN, Operand.A, Operand.n, prefetchQueue.NextByte());
                case PrimaryOpCode.OUT_A_n:
                    return new DecodeResult(Opcode.OUT, Operand.A, Operand.n, prefetchQueue.NextByte());
                default:
                    throw new ArgumentOutOfRangeException(nameof(code), code, null);
            }
        }
    }
}
