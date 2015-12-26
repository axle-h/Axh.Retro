namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;

    internal partial class OpCodeDecoder
    {
        private OpCode DecodePrefixEd()
        {
            var opCode = (PrefixEdOpCode)prefetch.NextByte();

            timer.Nop();

            switch (opCode)
            {
                // ********* 8-bit load *********
                // LD A, I
                case PrefixEdOpCode.LD_A_I:
                    timer.Extend(1);
                    operand1 = Operand.A;
                    operand2 = Operand.I;
                    return OpCode.Load;

                // LD A, R
                case PrefixEdOpCode.LD_A_R:
                    timer.Extend(1);
                    operand1 = Operand.A;
                    operand2 = Operand.R;
                    return OpCode.Load;

                // LD I, A
                case PrefixEdOpCode.LD_I_A:
                    timer.Extend(1);
                    operand1 = Operand.I;
                    operand2 = Operand.A;
                    return OpCode.Load;

                // LD R, A
                case PrefixEdOpCode.LD_R_A:
                    timer.Extend(1);
                    operand1 = Operand.R;
                    operand2 = Operand.A;
                    return OpCode.Load;

                // ********* 16-bit load *********
                // LD dd, (nn)
                case PrefixEdOpCode.LD_BC_mnn:
                    timer.IndexAndMmuWord();
                    operand1 = Operand.BC;
                    operand2 = Operand.mnn;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;
                case PrefixEdOpCode.LD_DE_mnn:
                    timer.IndexAndMmuWord();
                    operand1 = Operand.DE;
                    operand2 = Operand.mnn;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;
                case PrefixEdOpCode.LD_HL_mnn:
                    timer.IndexAndMmuWord();
                    operand1 = Operand.HL;
                    operand2 = Operand.mnn;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;
                case PrefixEdOpCode.LD_SP_mnn:
                    timer.IndexAndMmuWord();
                    operand1 = Operand.SP;
                    operand2 = Operand.mnn;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;

                // LD (nn), dd
                case PrefixEdOpCode.LD_mnn_BC:
                    timer.IndexAndMmuWord();
                    operand1 = Operand.mnn;
                    operand2 = Operand.BC;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;
                case PrefixEdOpCode.LD_mnn_DE:
                    timer.IndexAndMmuWord();
                    operand1 = Operand.mnn;
                    operand2 = Operand.DE;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;
                case PrefixEdOpCode.LD_mnn_HL:
                    timer.IndexAndMmuWord();
                    operand1 = Operand.mnn;
                    operand2 = Operand.HL;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;
                case PrefixEdOpCode.LD_mnn_SP:
                    timer.IndexAndMmuWord();
                    operand1 = Operand.mnn;
                    operand2 = Operand.SP;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;


                // ********* Block Transfer *********
                // LDI
                case PrefixEdOpCode.LDI:
                    timer.MmuWord().Extend(2);
                    return OpCode.TransferIncrement;

                // LDIR
                case PrefixEdOpCode.LDIR:
                    timer.MmuWord().Extend(2);
                    return OpCode.TransferIncrementRepeat;

                // LDD
                case PrefixEdOpCode.LDD:
                    timer.MmuWord().Extend(2);
                    return OpCode.TransferDecrement;

                // LDDR
                case PrefixEdOpCode.LDDR:
                    timer.MmuWord().Extend(2);
                    return OpCode.TransferDecrementRepeat;

                // ********* Search *********
                case PrefixEdOpCode.CPI:
                    timer.MmuWord().Extend(2);
                    return OpCode.SearchIncrement;

                case PrefixEdOpCode.CPIR:
                    timer.MmuWord().Extend(2);
                    return OpCode.SearchIncrementRepeat;

                case PrefixEdOpCode.CPD:
                    timer.MmuWord().Extend(2);
                    return OpCode.SearchDecrement;

                case PrefixEdOpCode.CPDR:
                    timer.MmuWord().Extend(2);
                    return OpCode.SearchDecrementRepeat;

                // ********* 16-Bit Arithmetic *********
                // ADC HL, ss
                case PrefixEdOpCode.ADC_HL_BC:
                    timer.Arithmetic16();
                    operand1 = Operand.HL;
                    operand2 = Operand.BC;
                    return OpCode.AddCarry16;
                case PrefixEdOpCode.ADC_HL_DE:
                    timer.Arithmetic16();
                    operand1 = Operand.HL;
                    operand2 = Operand.DE;
                    return OpCode.AddCarry16;
                case PrefixEdOpCode.ADC_HL_HL:
                    timer.Arithmetic16();
                    operand1 = operand2 = Operand.HL;
                    return OpCode.AddCarry16;
                case PrefixEdOpCode.ADC_HL_SP:
                    timer.Arithmetic16();
                    operand1 = Operand.HL;
                    operand2 = Operand.SP;
                    return OpCode.AddCarry16;

                // SBC HL, ss
                case PrefixEdOpCode.SBC_HL_BC:
                    timer.Arithmetic16();
                    operand1 = Operand.HL;
                    operand2 = Operand.BC;
                    return OpCode.SubtractCarry16;
                case PrefixEdOpCode.SBC_HL_DE:
                    timer.Arithmetic16();
                    operand1 = Operand.HL;
                    operand2 = Operand.DE;
                    return OpCode.SubtractCarry16;
                case PrefixEdOpCode.SBC_HL_HL:
                    timer.Arithmetic16();
                    operand1 = operand2 =  Operand.HL;
                    return OpCode.SubtractCarry16;
                case PrefixEdOpCode.SBC_HL_SP:
                    timer.Arithmetic16();
                    operand1 = Operand.HL;
                    operand2 = Operand.SP;
                    return OpCode.SubtractCarry16;

                // ********* General-Purpose Arithmetic *********
                // NEG
                case PrefixEdOpCode.NEG:
                    return OpCode.NegateTwosComplement;

                // IM 0
                case PrefixEdOpCode.IM0:
                    return OpCode.InterruptMode0;

                // IM 1
                case PrefixEdOpCode.IM1:
                    return OpCode.InterruptMode1;

                // IM 2
                case PrefixEdOpCode.IM2:
                    return OpCode.InterruptMode2;

                // ********* Rotate *********
                // RLD
                case PrefixEdOpCode.RLD:
                    timer.MmuWord().MmuByte().Extend(1);
                    return OpCode.RotateLeftDigit;

                // RRD
                case PrefixEdOpCode.RRD:
                    timer.MmuWord().MmuByte().Extend(1);
                    return OpCode.RotateRightDigit;

                // ********* Return *********
                case PrefixEdOpCode.RETI:
                    timer.MmuWord();
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.ReturnFromInterrupt;

                case PrefixEdOpCode.RETN:
                    timer.MmuWord();
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.ReturnFromNonmaskableInterrupt;

                // ********* IO *********
                // IN r, (C)
                case PrefixEdOpCode.IN_A_C:
                    timer.Io();
                    operand1 = Operand.A;
                    operand2 = Operand.C;
                    return OpCode.Input;

                case PrefixEdOpCode.IN_B_C:
                    timer.Io();
                    operand1 = Operand.B;
                    operand2 = Operand.C;
                    return OpCode.Input;

                case PrefixEdOpCode.IN_C_C:
                    timer.Io();
                    operand1 = Operand.C;
                    operand2 = Operand.C;
                    return OpCode.Input;

                case PrefixEdOpCode.IN_D_C:
                    timer.Io();
                    operand1 = Operand.D;
                    operand2 = Operand.C;
                    return OpCode.Input;

                case PrefixEdOpCode.IN_E_C:
                    timer.Io();
                    operand1 = Operand.E;
                    operand2 = Operand.C;
                    return OpCode.Input;

                case PrefixEdOpCode.IN_F_C:
                    timer.Io();
                    operand1 = Operand.F;
                    operand2 = Operand.C;
                    return OpCode.Input;

                case PrefixEdOpCode.IN_H_C:
                    timer.Io();
                    operand1 = Operand.H;
                    operand2 = Operand.C;
                    return OpCode.Input;

                case PrefixEdOpCode.IN_L_C:
                    timer.Io();
                    operand1 = Operand.L;
                    operand2 = Operand.C;
                    return OpCode.Input;

                // INI
                case PrefixEdOpCode.INI:
                    timer.MmuWord().Extend(2);
                    return OpCode.InputTransferIncrement;

                // INIR
                case PrefixEdOpCode.INIR:
                    timer.MmuWord().Extend(2);
                    return OpCode.InputTransferIncrementRepeat;

                // IND
                case PrefixEdOpCode.IND:
                    timer.MmuWord().Extend(2);
                    return OpCode.InputTransferDecrement;

                // INDR
                case PrefixEdOpCode.INDR:
                    timer.MmuWord().Extend(2);
                    return OpCode.InputTransferDecrementRepeat;

                // OUT r, (C)
                case PrefixEdOpCode.OUT_A_C:
                    timer.Io();
                    operand1 = Operand.A;
                    operand2 = Operand.C;
                    return OpCode.Output;

                case PrefixEdOpCode.OUT_B_C:
                    timer.Io();
                    operand1 = Operand.B;
                    operand2 = Operand.C;
                    return OpCode.Output;

                case PrefixEdOpCode.OUT_C_C:
                    timer.Io();
                    operand1 = Operand.C;
                    operand2 = Operand.C;
                    return OpCode.Output;

                case PrefixEdOpCode.OUT_D_C:
                    timer.Io();
                    operand1 = Operand.D;
                    operand2 = Operand.C;
                    return OpCode.Output;

                case PrefixEdOpCode.OUT_E_C:
                    timer.Io();
                    operand1 = Operand.E;
                    operand2 = Operand.C;
                    return OpCode.Output;

                case PrefixEdOpCode.OUT_F_C:
                    timer.Io();
                    operand1 = Operand.F;
                    operand2 = Operand.C;
                    return OpCode.Output;

                case PrefixEdOpCode.OUT_H_C:
                    timer.Io();
                    operand1 = Operand.H;
                    operand2 = Operand.C;
                    return OpCode.Output;

                case PrefixEdOpCode.OUT_L_C:
                    timer.Io();
                    operand1 = Operand.L;
                    operand2 = Operand.C;
                    return OpCode.Output;

                // OUTI
                case PrefixEdOpCode.OUTI:
                    timer.MmuWord().Extend(2);
                    return OpCode.OutputTransferIncrement;

                // OUTIR
                case PrefixEdOpCode.OUTIR:
                    timer.MmuWord().Extend(2);
                    return OpCode.OutputTransferIncrementRepeat;

                // OUTD
                case PrefixEdOpCode.OUTD:
                    timer.MmuWord().Extend(2);
                    return OpCode.OutputTransferDecrement;

                // OUTDR
                case PrefixEdOpCode.OUTDR:
                    timer.MmuWord().Extend(2);
                    return OpCode.OutputTransferDecrementRepeat;

                default:
                    // The Prefix ED opcode set is not saturated
                    return undefinedInstruction;
            }
        }
    }
}
