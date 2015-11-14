namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;

    internal partial class OpCodeDecoder
    {
        private DecodeResult DecodePrefixEd()
        {
            var opCode = (PrefixEdOpCode)prefetch.NextByte();

            timer.Nop();

            switch (opCode)
            {
                // ********* 8-bit load *********
                // LD A, I
                case PrefixEdOpCode.LD_A_I:
                    timer.Extend(1);
                    break;

                // LD A, R
                case PrefixEdOpCode.LD_A_R:
                    timer.Extend(1);
                    break;

                // LD I, A
                case PrefixEdOpCode.LD_I_A:
                    timer.Extend(1);
                    break;

                // LD R, A
                case PrefixEdOpCode.LD_R_A:
                    timer.Extend(1);
                    break;

                // ********* 16-bit load *********
                // LD dd, (nn)
                case PrefixEdOpCode.LD_BC_mnn:
                    timer.IndexAndMmuWord();
                    break;
                case PrefixEdOpCode.LD_DE_mnn:
                    timer.IndexAndMmuWord();
                    break;
                case PrefixEdOpCode.LD_HL_mnn:
                    timer.IndexAndMmuWord();
                    break;
                case PrefixEdOpCode.LD_SP_mnn:
                    timer.IndexAndMmuWord();
                    break;

                // LD (nn), dd
                case PrefixEdOpCode.LD_mnn_BC:
                    timer.IndexAndMmuWord();
                    break;
                case PrefixEdOpCode.LD_mnn_DE:
                    timer.IndexAndMmuWord();
                    break;
                case PrefixEdOpCode.LD_mnn_HL:
                    timer.IndexAndMmuWord();
                    break;
                case PrefixEdOpCode.LD_mnn_SP:
                    timer.IndexAndMmuWord();
                    break;


                // ********* Block Transfer *********
                // LDI
                case PrefixEdOpCode.LDI:
                    timer.MmuWord().Extend(2);
                    break;

                // LDIR
                case PrefixEdOpCode.LDIR:
                    timer.MmuWord().Extend(2);
                    break;

                // LDD
                case PrefixEdOpCode.LDD:
                    timer.MmuWord().Extend(2);
                    break;

                // LDDR
                case PrefixEdOpCode.LDDR:
                    timer.MmuWord().Extend(2);
                    break;

                // ********* Search *********
                case PrefixEdOpCode.CPI:
                    timer.MmuWord().Extend(2);
                    break;

                case PrefixEdOpCode.CPIR:
                    timer.MmuWord().Extend(2);
                    break;

                case PrefixEdOpCode.CPD:
                    timer.MmuWord().Extend(2);
                    break;

                case PrefixEdOpCode.CPDR:
                    timer.MmuWord().Extend(2);
                    break;

                // ********* 16-Bit Arithmetic *********
                // ADC HL, ss
                case PrefixEdOpCode.ADC_HL_BC:
                    timer.Arithmetic16();
                    break;
                case PrefixEdOpCode.ADC_HL_DE:
                    timer.Arithmetic16();
                    break;
                case PrefixEdOpCode.ADC_HL_HL:
                    timer.Arithmetic16();
                    break;
                case PrefixEdOpCode.ADC_HL_SP:
                    timer.Arithmetic16();
                    break;

                // SBC HL, ss
                case PrefixEdOpCode.SBC_HL_BC:
                    timer.Arithmetic16();
                    break;
                case PrefixEdOpCode.SBC_HL_DE:
                    timer.Arithmetic16();
                    break;
                case PrefixEdOpCode.SBC_HL_HL:
                    timer.Arithmetic16();
                    break;
                case PrefixEdOpCode.SBC_HL_SP:
                    timer.Arithmetic16();
                    break;

                // ********* General-Purpose Arithmetic *********
                // NEG
                case PrefixEdOpCode.NEG:
                    
                    break;

                // IM 0
                case PrefixEdOpCode.IM0:
                    
                    break;

                // IM 1
                case PrefixEdOpCode.IM1:
                    
                    break;

                // IM 2
                case PrefixEdOpCode.IM2:
                    
                    break;

                // ********* Rotate *********
                // RLD
                case PrefixEdOpCode.RLD:
                    timer.MmuWord().MmuByte().Extend(1);
                    break;

                // RRD
                case PrefixEdOpCode.RRD:
                    timer.MmuWord().MmuByte().Extend(1);
                    break;

                // ********* Return *********
                case PrefixEdOpCode.RETI:
                    timer.MmuWord();
                    break;

                case PrefixEdOpCode.RETN:
                    timer.MmuWord();
                    break;

                // ********* IO *********
                // IN r, (C)
                case PrefixEdOpCode.IN_A_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.IN_B_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.IN_C_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.IN_D_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.IN_E_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.IN_F_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.IN_H_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.IN_L_C:
                    timer.Io();
                    break;

                // INI
                case PrefixEdOpCode.INI:
                    timer.MmuWord().Extend(2);
                    break;

                // INIR
                case PrefixEdOpCode.INIR:
                    timer.MmuWord().Extend(2);
                    break;

                // IND
                case PrefixEdOpCode.IND:
                    timer.MmuWord().Extend(2);
                    break;

                // INDR
                case PrefixEdOpCode.INDR:
                    timer.MmuWord().Extend(2);
                    break;

                // OUT r, (C)
                case PrefixEdOpCode.OUT_A_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.OUT_B_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.OUT_C_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.OUT_D_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.OUT_E_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.OUT_F_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.OUT_H_C:
                    timer.Io();
                    break;

                case PrefixEdOpCode.OUT_L_C:
                    timer.Io();
                    break;

                // OUTI
                case PrefixEdOpCode.OUTI:
                    timer.MmuWord().Extend(2);
                    break;

                // OUTIR
                case PrefixEdOpCode.OUTIR:
                    timer.MmuWord().Extend(2);
                    break;

                // OUTD
                case PrefixEdOpCode.OUTD:
                    timer.MmuWord().Extend(2);
                    break;

                // OUTDR
                case PrefixEdOpCode.OUTDR:
                    timer.MmuWord().Extend(2);
                    break;

                default:
                    // The Prefix ED opcode set is not saturated
                    // Opcodes not defined are just run as NOP's
                    return new DecodeResult(Opcode.NOP);
            }
        }
    }
}
