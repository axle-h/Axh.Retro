namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;

    internal partial class OpCodeDecoder
    {
        private Operation DecodePrefixEd()
        {
            var opCode = (PrefixEdOpCode)prefetch.NextByte();

            timer.Nop();

            switch (opCode)
            {
                // ********* 8-bit load *********
                // LD A, I
                case PrefixEdOpCode.LD_A_I:
                    timer.Extend(1);
                    return new Operation(OpCode.Load, Operand.A, Operand.I);

                // LD A, R
                case PrefixEdOpCode.LD_A_R:
                    timer.Extend(1);
                    return new Operation(OpCode.Load, Operand.A, Operand.R);

                // LD I, A
                case PrefixEdOpCode.LD_I_A:
                    timer.Extend(1);
                    return new Operation(OpCode.Load, Operand.I, Operand.A);

                // LD R, A
                case PrefixEdOpCode.LD_R_A:
                    timer.Extend(1);
                    return new Operation(OpCode.Load, Operand.R, Operand.A);

                // ********* 16-bit load *********
                // LD dd, (nn)
                case PrefixEdOpCode.LD_BC_mnn:
                    timer.IndexAndMmuWord();
                    return new Operation(OpCode.Load16, Operand.BC, Operand.mnn).WithWordLiteral();
                case PrefixEdOpCode.LD_DE_mnn:
                    timer.IndexAndMmuWord();
                    return new Operation(OpCode.Load16, Operand.DE, Operand.mnn).WithWordLiteral();
                case PrefixEdOpCode.LD_HL_mnn:
                    timer.IndexAndMmuWord();
                    return new Operation(OpCode.Load16, Operand.HL, Operand.mnn).WithWordLiteral();
                case PrefixEdOpCode.LD_SP_mnn:
                    timer.IndexAndMmuWord();
                    return new Operation(OpCode.Load16, Operand.SP, Operand.mnn).WithWordLiteral();

                // LD (nn), dd
                case PrefixEdOpCode.LD_mnn_BC:
                    timer.IndexAndMmuWord();
                    return new Operation(OpCode.Load16, Operand.mnn, Operand.BC).WithWordLiteral();
                case PrefixEdOpCode.LD_mnn_DE:
                    timer.IndexAndMmuWord();
                    return new Operation(OpCode.Load16, Operand.mnn, Operand.DE).WithWordLiteral();
                case PrefixEdOpCode.LD_mnn_HL:
                    timer.IndexAndMmuWord();
                    return new Operation(OpCode.Load16, Operand.mnn, Operand.HL).WithWordLiteral();
                case PrefixEdOpCode.LD_mnn_SP:
                    timer.IndexAndMmuWord();
                    return new Operation(OpCode.Load16, Operand.mnn, Operand.SP).WithWordLiteral();


                // ********* Block Transfer *********
                // LDI
                case PrefixEdOpCode.LDI:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.TransferIncrement);

                // LDIR
                case PrefixEdOpCode.LDIR:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.TransferIncrementRepeat);

                // LDD
                case PrefixEdOpCode.LDD:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.TransferDecrement);

                // LDDR
                case PrefixEdOpCode.LDDR:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.TransferDecrementRepeat);

                // ********* Search *********
                case PrefixEdOpCode.CPI:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.SearchIncrement);

                case PrefixEdOpCode.CPIR:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.SearchIncrementRepeat);

                case PrefixEdOpCode.CPD:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.SearchDecrement);

                case PrefixEdOpCode.CPDR:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.SearchDecrementRepeat);

                // ********* 16-Bit Arithmetic *********
                // ADC HL, ss
                case PrefixEdOpCode.ADC_HL_BC:
                    timer.Arithmetic16();
                    return new Operation(OpCode.AddCarry16, Operand.HL, Operand.BC);
                case PrefixEdOpCode.ADC_HL_DE:
                    timer.Arithmetic16();
                    return new Operation(OpCode.AddCarry16, Operand.HL, Operand.DE);
                case PrefixEdOpCode.ADC_HL_HL:
                    timer.Arithmetic16();
                    return new Operation(OpCode.AddCarry16, Operand.HL, Operand.HL);
                case PrefixEdOpCode.ADC_HL_SP:
                    timer.Arithmetic16();
                    return new Operation(OpCode.AddCarry16, Operand.HL, Operand.SP);

                // SBC HL, ss
                case PrefixEdOpCode.SBC_HL_BC:
                    timer.Arithmetic16();
                    return new Operation(OpCode.SubtractCarry16, Operand.HL, Operand.BC);
                case PrefixEdOpCode.SBC_HL_DE:
                    timer.Arithmetic16();
                    return new Operation(OpCode.SubtractCarry16, Operand.HL, Operand.DE);
                case PrefixEdOpCode.SBC_HL_HL:
                    timer.Arithmetic16();
                    return new Operation(OpCode.SubtractCarry16, Operand.HL, Operand.HL);
                case PrefixEdOpCode.SBC_HL_SP:
                    timer.Arithmetic16();
                    return new Operation(OpCode.SubtractCarry16, Operand.HL, Operand.SP);

                // ********* General-Purpose Arithmetic *********
                // NEG
                case PrefixEdOpCode.NEG:
                    return new Operation(OpCode.NegateTwosComplement);

                // IM 0
                case PrefixEdOpCode.IM0:
                    return new Operation(OpCode.InterruptMode0);

                // IM 1
                case PrefixEdOpCode.IM1:
                    return new Operation(OpCode.InterruptMode1);

                // IM 2
                case PrefixEdOpCode.IM2:
                    return new Operation(OpCode.InterruptMode2);

                // ********* Rotate *********
                // RLD
                case PrefixEdOpCode.RLD:
                    timer.MmuWord().MmuByte().Extend(1);
                    return new Operation(OpCode.RotateLeftDigit);

                // RRD
                case PrefixEdOpCode.RRD:
                    timer.MmuWord().MmuByte().Extend(1);
                    return new Operation(OpCode.RotateRightDigit);

                // ********* Return *********
                case PrefixEdOpCode.RETI:
                    timer.MmuWord();
                    return new Operation(OpCode.ReturnFromInterrupt).EndBlock();

                case PrefixEdOpCode.RETN:
                    timer.MmuWord();
                    return new Operation(OpCode.ReturnFromNonmaskableInterrupt).EndBlock();

                // ********* IO *********
                // IN r, (C)
                case PrefixEdOpCode.IN_A_C:
                    timer.Io();
                    return new Operation(OpCode.Input, Operand.A, Operand.C);

                case PrefixEdOpCode.IN_B_C:
                    timer.Io();
                    return new Operation(OpCode.Input, Operand.B, Operand.C);

                case PrefixEdOpCode.IN_C_C:
                    timer.Io();
                    return new Operation(OpCode.Input, Operand.C, Operand.C);

                case PrefixEdOpCode.IN_D_C:
                    timer.Io();
                    return new Operation(OpCode.Input, Operand.D, Operand.C);

                case PrefixEdOpCode.IN_E_C:
                    timer.Io();
                    return new Operation(OpCode.Input, Operand.E, Operand.C);

                case PrefixEdOpCode.IN_F_C:
                    timer.Io();
                    return new Operation(OpCode.Input, Operand.F, Operand.C);

                case PrefixEdOpCode.IN_H_C:
                    timer.Io();
                    return new Operation(OpCode.Input, Operand.H, Operand.C);

                case PrefixEdOpCode.IN_L_C:
                    timer.Io();
                    return new Operation(OpCode.Input, Operand.L, Operand.C);

                // INI
                case PrefixEdOpCode.INI:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.InputTransferIncrement);

                // INIR
                case PrefixEdOpCode.INIR:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.InputTransferIncrementRepeat);

                // IND
                case PrefixEdOpCode.IND:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.InputTransferDecrement);

                // INDR
                case PrefixEdOpCode.INDR:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.InputTransferDecrementRepeat);

                // OUT r, (C)
                case PrefixEdOpCode.OUT_A_C:
                    timer.Io();
                    return new Operation(OpCode.Output, Operand.A, Operand.C);

                case PrefixEdOpCode.OUT_B_C:
                    timer.Io();
                    return new Operation(OpCode.Output, Operand.B, Operand.C);

                case PrefixEdOpCode.OUT_C_C:
                    timer.Io();
                    return new Operation(OpCode.Output, Operand.C, Operand.C);

                case PrefixEdOpCode.OUT_D_C:
                    timer.Io();
                    return new Operation(OpCode.Output, Operand.D, Operand.C);

                case PrefixEdOpCode.OUT_E_C:
                    timer.Io();
                    return new Operation(OpCode.Output, Operand.E, Operand.C);

                case PrefixEdOpCode.OUT_F_C:
                    timer.Io();
                    return new Operation(OpCode.Output, Operand.F, Operand.C);

                case PrefixEdOpCode.OUT_H_C:
                    timer.Io();
                    return new Operation(OpCode.Output, Operand.H, Operand.C);

                case PrefixEdOpCode.OUT_L_C:
                    timer.Io();
                    return new Operation(OpCode.Output, Operand.L, Operand.C);

                // OUTI
                case PrefixEdOpCode.OUTI:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.OutputTransferIncrement);

                // OUTIR
                case PrefixEdOpCode.OUTIR:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.OutputTransferIncrementRepeat);

                // OUTD
                case PrefixEdOpCode.OUTD:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.OutputTransferDecrement);

                // OUTDR
                case PrefixEdOpCode.OUTDR:
                    timer.MmuWord().Extend(2);
                    return new Operation(OpCode.OutputTransferDecrementRepeat);

                default:
                    // The Prefix ED opcode set is not saturated
                    return new Operation(undefinedInstruction);
            }
        }
    }
}
