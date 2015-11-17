namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System;

    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;

    internal partial class OpCodeDecoder
    {
        private Operation DecodePrimary()
        {
            var code = (PrimaryOpCode)prefetch.NextByte();

            // Add a NOP on every frame. Reduce timings by a NOP elsewhere.
            timer.Nop();

            switch (code)
            {
                case PrimaryOpCode.NOP:
                    return new Operation(OpCode.NoOperation);
                case PrimaryOpCode.HALT:
                    return new Operation(OpCode.Halt).EndBlock();

                // ********* Prefixes *********
                case PrimaryOpCode.Prefix_CB:
                    if (cpuMode == CpuMode.Z80 || cpuMode == CpuMode.GameBoy)
                    {
                        // Only Z80 & GBCPU has prefix CB
                        if (!index.IsDisplaced)
                        {
                            return DecodePrefixCb();
                        }
                        var displacement = prefetch.NextByte();
                        return FixPrefixDdFdPrefixCbResult(DecodePrefixCb().AddDisplacement(displacement));
                    }
                    return new Operation(undefinedInstruction);

                case PrimaryOpCode.Prefix_DD:
                    if (cpuMode == CpuMode.Z80)
                    {
                        // Only Z80 has prefix DD
                        index = indexRegisterOperands[IndexRegister.IX];
                        return null;
                    }
                    return new Operation(undefinedInstruction);

                case PrimaryOpCode.Prefix_ED:
                    if (cpuMode == CpuMode.Z80)
                    {
                        // Only Z80 has prefix ED
                        return DecodePrefixEd();
                    }
                    return new Operation(undefinedInstruction);

                case PrimaryOpCode.Prefix_FD:
                    if (cpuMode == CpuMode.Z80)
                    {
                        // Only Z80 has prefix FD
                        index = indexRegisterOperands[IndexRegister.IY];
                        return null;
                    }
                    return new Operation(undefinedInstruction);

                // ********* 8-bit load *********
                // LD r, r'
                case PrimaryOpCode.LD_A_A:
                    return new Operation(OpCode.Load, Operand.A, Operand.A);
                case PrimaryOpCode.LD_B_A:
                    return new Operation(OpCode.Load, Operand.B, Operand.A);
                case PrimaryOpCode.LD_C_A:
                    return new Operation(OpCode.Load, Operand.C, Operand.A);
                case PrimaryOpCode.LD_D_A:
                    return new Operation(OpCode.Load, Operand.D, Operand.A);
                case PrimaryOpCode.LD_E_A:
                    return new Operation(OpCode.Load, Operand.E, Operand.A);
                case PrimaryOpCode.LD_H_A:
                    return new Operation(OpCode.Load, index.HighRegister, Operand.A);
                case PrimaryOpCode.LD_L_A:
                    return new Operation(OpCode.Load, index.LowRegister, Operand.A);
                case PrimaryOpCode.LD_A_B:
                    return new Operation(OpCode.Load, Operand.A, Operand.B);
                case PrimaryOpCode.LD_B_B:
                    return new Operation(OpCode.Load, Operand.B, Operand.B);
                case PrimaryOpCode.LD_C_B:
                    return new Operation(OpCode.Load, Operand.C, Operand.B);
                case PrimaryOpCode.LD_D_B:
                    return new Operation(OpCode.Load, Operand.D, Operand.B);
                case PrimaryOpCode.LD_E_B:
                    return new Operation(OpCode.Load, Operand.E, Operand.B);
                case PrimaryOpCode.LD_H_B:
                    return new Operation(OpCode.Load, index.HighRegister, Operand.B);
                case PrimaryOpCode.LD_L_B:
                    return new Operation(OpCode.Load, index.LowRegister, Operand.B);
                case PrimaryOpCode.LD_A_C:
                    return new Operation(OpCode.Load, Operand.A, Operand.C);
                case PrimaryOpCode.LD_B_C:
                    return new Operation(OpCode.Load, Operand.B, Operand.C);
                case PrimaryOpCode.LD_C_C:
                    return new Operation(OpCode.Load, Operand.C, Operand.C);
                case PrimaryOpCode.LD_D_C:
                    return new Operation(OpCode.Load, Operand.D, Operand.C);
                case PrimaryOpCode.LD_E_C:
                    return new Operation(OpCode.Load, Operand.E, Operand.C);
                case PrimaryOpCode.LD_H_C:
                    return new Operation(OpCode.Load, index.HighRegister, Operand.C);
                case PrimaryOpCode.LD_L_C:
                    return new Operation(OpCode.Load, index.LowRegister, Operand.C);
                case PrimaryOpCode.LD_A_D:
                    return new Operation(OpCode.Load, Operand.A, Operand.D);
                case PrimaryOpCode.LD_B_D:
                    return new Operation(OpCode.Load, Operand.B, Operand.D);
                case PrimaryOpCode.LD_C_D:
                    return new Operation(OpCode.Load, Operand.C, Operand.D);
                case PrimaryOpCode.LD_D_D:
                    return new Operation(OpCode.Load, Operand.D, Operand.D);
                case PrimaryOpCode.LD_E_D:
                    return new Operation(OpCode.Load, Operand.E, Operand.D);
                case PrimaryOpCode.LD_H_D:
                    return new Operation(OpCode.Load, index.HighRegister, Operand.D);
                case PrimaryOpCode.LD_L_D:
                    return new Operation(OpCode.Load, index.LowRegister, Operand.D);
                case PrimaryOpCode.LD_A_E:
                    return new Operation(OpCode.Load, Operand.A, Operand.E);
                case PrimaryOpCode.LD_B_E:
                    return new Operation(OpCode.Load, Operand.B, Operand.E);
                case PrimaryOpCode.LD_C_E:
                    return new Operation(OpCode.Load, Operand.C, Operand.E);
                case PrimaryOpCode.LD_D_E:
                    return new Operation(OpCode.Load, Operand.D, Operand.E);
                case PrimaryOpCode.LD_E_E:
                    return new Operation(OpCode.Load, Operand.E, Operand.E);
                case PrimaryOpCode.LD_H_E:
                    return new Operation(OpCode.Load, index.HighRegister, Operand.E);
                case PrimaryOpCode.LD_L_E:
                    return new Operation(OpCode.Load, index.LowRegister, Operand.E);
                case PrimaryOpCode.LD_A_H:
                    return new Operation(OpCode.Load, Operand.A, index.HighRegister);
                case PrimaryOpCode.LD_B_H:
                    return new Operation(OpCode.Load, Operand.B, index.HighRegister);
                case PrimaryOpCode.LD_C_H:
                    return new Operation(OpCode.Load, Operand.C, index.HighRegister);
                case PrimaryOpCode.LD_D_H:
                    return new Operation(OpCode.Load, Operand.D, index.HighRegister);
                case PrimaryOpCode.LD_E_H:
                    return new Operation(OpCode.Load, Operand.E, index.HighRegister);
                case PrimaryOpCode.LD_H_H:
                    return new Operation(OpCode.Load, index.HighRegister, index.HighRegister);
                case PrimaryOpCode.LD_L_H:
                    return new Operation(OpCode.Load, index.LowRegister, index.HighRegister);
                case PrimaryOpCode.LD_A_L:
                    return new Operation(OpCode.Load, Operand.A, index.LowRegister);
                case PrimaryOpCode.LD_B_L:
                    return new Operation(OpCode.Load, Operand.B, index.LowRegister);
                case PrimaryOpCode.LD_C_L:
                    return new Operation(OpCode.Load, Operand.C, index.LowRegister);
                case PrimaryOpCode.LD_D_L:
                    return new Operation(OpCode.Load, Operand.D, index.LowRegister);
                case PrimaryOpCode.LD_E_L:
                    return new Operation(OpCode.Load, Operand.E, index.LowRegister);
                case PrimaryOpCode.LD_H_L:
                    return new Operation(OpCode.Load, index.HighRegister, index.LowRegister);
                case PrimaryOpCode.LD_L_L:
                    return new Operation(OpCode.Load, index.LowRegister, index.LowRegister);

                // LD r,n
                case PrimaryOpCode.LD_A_n:
                    timer.MmuByte();
                    return new Operation(OpCode.Load, Operand.A, Operand.n).WithByteLiteral();
                case PrimaryOpCode.LD_B_n:
                    timer.MmuByte();
                    return new Operation(OpCode.Load, Operand.B, Operand.n).WithByteLiteral();
                case PrimaryOpCode.LD_C_n:
                    timer.MmuByte();
                    return new Operation(OpCode.Load, Operand.C, Operand.n).WithByteLiteral();
                case PrimaryOpCode.LD_D_n:
                    timer.MmuByte();
                    return new Operation(OpCode.Load, Operand.D, Operand.n).WithByteLiteral();
                case PrimaryOpCode.LD_E_n:
                    timer.MmuByte();
                    return new Operation(OpCode.Load, Operand.E, Operand.n).WithByteLiteral();
                case PrimaryOpCode.LD_H_n:
                    timer.MmuByte();
                    return new Operation(OpCode.Load, index.HighRegister, Operand.n).WithByteLiteral();
                case PrimaryOpCode.LD_L_n:
                    timer.MmuByte();
                    return new Operation(OpCode.Load, index.LowRegister, Operand.n).WithByteLiteral();

                // LD r, (HL)
                case PrimaryOpCode.LD_A_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Load, Operand.A, index.Index).WithDisplacement();
                case PrimaryOpCode.LD_B_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Load, Operand.B, index.Index).WithDisplacement();
                case PrimaryOpCode.LD_C_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Load, Operand.C, index.Index).WithDisplacement();
                case PrimaryOpCode.LD_D_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Load, Operand.D, index.Index).WithDisplacement();
                case PrimaryOpCode.LD_E_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Load, Operand.E, index.Index).WithDisplacement();
                case PrimaryOpCode.LD_H_mHL:
                    timer.Index(index.IsDisplaced);
                    // H register is always assigned here
                    return new Operation(OpCode.Load, Operand.H, index.Index).WithDisplacement();
                case PrimaryOpCode.LD_L_mHL:
                    timer.Index(index.IsDisplaced);
                    // L register is always assigned here
                    return new Operation(OpCode.Load, Operand.L, index.Index).WithDisplacement();

                // LD (HL), r
                case PrimaryOpCode.LD_mHL_A:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Load, index.Index, Operand.A).WithDisplacement();
                case PrimaryOpCode.LD_mHL_B:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Load, index.Index, Operand.B).WithDisplacement();
                case PrimaryOpCode.LD_mHL_C:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Load, index.Index, Operand.C).WithDisplacement();
                case PrimaryOpCode.LD_mHL_D:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Load, index.Index, Operand.D).WithDisplacement();
                case PrimaryOpCode.LD_mHL_E:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Load, index.Index, Operand.E).WithDisplacement();
                case PrimaryOpCode.LD_mHL_H:
                    timer.Index(index.IsDisplaced);
                    // Value of H register is always used here
                    return new Operation(OpCode.Load, index.Index, Operand.H).WithDisplacement();
                case PrimaryOpCode.LD_mHL_L:
                    timer.Index(index.IsDisplaced);
                    // Value of L register is always used here
                    return new Operation(OpCode.Load, index.Index, Operand.L).WithDisplacement();

                // LD (HL), n
                case PrimaryOpCode.LD_mHL_n:
                    timer.IndexAndMmuByte(index.IsDisplaced);
                    return new Operation(OpCode.Load, index.Index, Operand.n).WithDisplacement().WithByteLiteral();

                // LD A, (BC)
                case PrimaryOpCode.LD_A_mBC:
                    timer.Index(false);
                    return new Operation(OpCode.Load, Operand.A, Operand.mBC);

                // LD A, (BC)
                case PrimaryOpCode.LD_A_mDE:
                    timer.Index(false);
                    return new Operation(OpCode.Load, Operand.A, Operand.mDE);

                // LD A, (nn)
                case PrimaryOpCode.LD_A_mnn:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LDD  A, (HL) on GB
                        timer.MmuByte();
                        return new Operation(OpCode.LoadDecrement, Operand.A, index.Index);
                    }
                    timer.Index(false).MmuWord();
                    return new Operation(OpCode.Load, Operand.A, Operand.mnn).WithWordLiteral();

                // LD (BC), A
                case PrimaryOpCode.LD_mBC_A:
                    timer.Index(false);
                    return new Operation(OpCode.Load, Operand.mBC, Operand.A);

                // LD (DE), A
                case PrimaryOpCode.LD_mDE_A:
                    timer.Index(false);
                    return new Operation(OpCode.Load, Operand.mDE, Operand.A);

                // LD (nn), A
                case PrimaryOpCode.LD_mnn_A:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LDD  (HL), A on GB
                        timer.MmuByte();
                        return new Operation(OpCode.LoadDecrement, index.Index, Operand.A);
                    }
                    timer.Index(false).MmuWord();
                    return new Operation(OpCode.Load, Operand.mnn, Operand.A).WithWordLiteral();

                // ********* 16-bit load *********
                // LD dd, nn
                case PrimaryOpCode.LD_BC_nn:
                    timer.MmuWord();
                    return new Operation(OpCode.Load16, Operand.BC, Operand.nn).WithWordLiteral();
                case PrimaryOpCode.LD_DE_nn:
                    timer.MmuWord();
                    return new Operation(OpCode.Load16, Operand.DE, Operand.nn).WithWordLiteral();
                case PrimaryOpCode.LD_HL_nn:
                    timer.MmuWord();
                    return new Operation(OpCode.Load16, index.Register, Operand.nn).WithWordLiteral();
                case PrimaryOpCode.LD_SP_nn:
                    timer.MmuWord();
                    return new Operation(OpCode.Load16, Operand.SP, Operand.nn).WithWordLiteral();

                // LD HL, (nn)
                case PrimaryOpCode.LD_HL_mnn:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LDI  A, (HL) on GB
                        timer.MmuByte();
                        return new Operation(OpCode.LoadIncrement, Operand.A, index.Index);
                    }

                    timer.IndexAndMmuWord();
                    return new Operation(OpCode.Load16, index.Register, Operand.mnn).WithWordLiteral();

                // LD (nn), HL
                case PrimaryOpCode.LD_mnn_HL:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LDI  (HL), A on GB
                        timer.MmuByte();
                        return new Operation(OpCode.LoadIncrement, index.Index, Operand.A);
                    }

                    timer.IndexAndMmuWord();
                    return new Operation(OpCode.Load16, Operand.mnn, index.Register).WithWordLiteral();

                // LD SP, HL
                case PrimaryOpCode.LD_SP_HL:
                    timer.Extend(2);
                    return new Operation(OpCode.Load16, Operand.SP, index.Register);

                // PUSH qq
                case PrimaryOpCode.PUSH_BC:
                    timer.Extend(1).MmuWord();
                    return new Operation(OpCode.Push, Operand.BC);
                case PrimaryOpCode.PUSH_DE:
                    timer.Extend(1).MmuWord();
                    return new Operation(OpCode.Push, Operand.DE);
                case PrimaryOpCode.PUSH_HL:
                    timer.Extend(1).MmuWord();
                    return new Operation(OpCode.Push, index.Register);
                case PrimaryOpCode.PUSH_AF:
                    timer.Extend(1).MmuWord();
                    return new Operation(OpCode.Push, Operand.AF);

                // POP qq
                case PrimaryOpCode.POP_BC:
                    timer.MmuWord();
                    return new Operation(OpCode.Pop, Operand.BC);
                case PrimaryOpCode.POP_DE:
                    timer.MmuWord();
                    return new Operation(OpCode.Pop, Operand.DE);
                case PrimaryOpCode.POP_HL:
                    timer.MmuWord();
                    return new Operation(OpCode.Pop, index.Register);
                case PrimaryOpCode.POP_AF:
                    timer.MmuWord();
                    return new Operation(OpCode.Pop, Operand.AF);

                // ********* Exchange *********
                // EX DE, HL
                case PrimaryOpCode.EX_DE_HL:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return new Operation(undefinedInstruction);
                    }

                    // This affects HL register directly, always ignoring index register prefixes
                    return new Operation(OpCode.Exchange, Operand.DE, Operand.HL);

                // EX AF, AF′
                case PrimaryOpCode.EX_AF:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD (nn),SP on GB
                        timer.IndexAndMmuWord();
                        return new Operation(OpCode.Load16, Operand.mnn, Operand.SP).WithWordLiteral();
                    }

                    return new Operation(OpCode.ExchangeAccumulatorAndFlags);

                // EXX
                case PrimaryOpCode.EXX:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as RETI on GB, retains NOP timing
                        timer.MmuWord().Nop();
                        return new Operation(OpCode.ReturnFromInterrupt).EndBlock();
                    }

                    return new Operation(OpCode.ExchangeGeneralPurpose);

                // EX (SP), HL
                case PrimaryOpCode.EX_mSP_HL:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return new Operation(undefinedInstruction);
                    }

                    timer.Arithmetic16().MmuWord().Extend(2);
                    return new Operation(OpCode.Exchange, Operand.mSP, index.Register);

                // ********* 8-Bit Arithmetic *********
                // ADD A, r
                case PrimaryOpCode.ADD_A_A:
                    return new Operation(OpCode.Add, Operand.A, Operand.A);
                case PrimaryOpCode.ADD_A_B:
                    return new Operation(OpCode.Add, Operand.A, Operand.B);
                case PrimaryOpCode.ADD_A_C:
                    return new Operation(OpCode.Add, Operand.A, Operand.C);
                case PrimaryOpCode.ADD_A_D:
                    return new Operation(OpCode.Add, Operand.A, Operand.D);
                case PrimaryOpCode.ADD_A_E:
                    return new Operation(OpCode.Add, Operand.A, Operand.E);
                case PrimaryOpCode.ADD_A_H:
                    return new Operation(OpCode.Add, Operand.A, index.HighRegister);
                case PrimaryOpCode.ADD_A_L:
                    return new Operation(OpCode.Add, Operand.A, index.LowRegister);
                case PrimaryOpCode.ADD_A_n:
                    timer.MmuByte();
                    return new Operation(OpCode.Add, Operand.A, Operand.n).WithByteLiteral();
                case PrimaryOpCode.ADD_A_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Add, Operand.A, index.Index).WithDisplacement();

                // ADC A, r
                case PrimaryOpCode.ADC_A_A:
                    return new Operation(OpCode.AddCarry, Operand.A, Operand.A);
                case PrimaryOpCode.ADC_A_B:
                    return new Operation(OpCode.AddCarry, Operand.A, Operand.B);
                case PrimaryOpCode.ADC_A_C:
                    return new Operation(OpCode.AddCarry, Operand.A, Operand.C);
                case PrimaryOpCode.ADC_A_D:
                    return new Operation(OpCode.AddCarry, Operand.A, Operand.D);
                case PrimaryOpCode.ADC_A_E:
                    return new Operation(OpCode.AddCarry, Operand.A, Operand.E);
                case PrimaryOpCode.ADC_A_H:
                    return new Operation(OpCode.AddCarry, Operand.A, index.HighRegister);
                case PrimaryOpCode.ADC_A_L:
                    return new Operation(OpCode.AddCarry, Operand.A, index.LowRegister);
                case PrimaryOpCode.ADC_A_n:
                    timer.MmuByte();
                    return new Operation(OpCode.AddCarry, Operand.A, Operand.n).WithByteLiteral();
                case PrimaryOpCode.ADC_A_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.AddCarry, Operand.A, index.Index).WithDisplacement();

                // SUB A, r
                case PrimaryOpCode.SUB_A_A:
                    return new Operation(OpCode.Subtract, Operand.A, Operand.A);
                case PrimaryOpCode.SUB_A_B:
                    return new Operation(OpCode.Subtract, Operand.A, Operand.B);
                case PrimaryOpCode.SUB_A_C:
                    return new Operation(OpCode.Subtract, Operand.A, Operand.C);
                case PrimaryOpCode.SUB_A_D:
                    return new Operation(OpCode.Subtract, Operand.A, Operand.D);
                case PrimaryOpCode.SUB_A_E:
                    return new Operation(OpCode.Subtract, Operand.A, Operand.E);
                case PrimaryOpCode.SUB_A_H:
                    return new Operation(OpCode.Subtract, Operand.A, index.HighRegister);
                case PrimaryOpCode.SUB_A_L:
                    return new Operation(OpCode.Subtract, Operand.A, index.LowRegister);
                case PrimaryOpCode.SUB_A_n:
                    timer.MmuByte();
                    return new Operation(OpCode.Subtract, Operand.A, Operand.n).WithByteLiteral();
                case PrimaryOpCode.SUB_A_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Subtract, Operand.A, index.Index).WithDisplacement();

                // SBC A, r
                case PrimaryOpCode.SBC_A_A:
                    return new Operation(OpCode.SubtractCarry, Operand.A, Operand.A);
                case PrimaryOpCode.SBC_A_B:
                    return new Operation(OpCode.SubtractCarry, Operand.A, Operand.B);
                case PrimaryOpCode.SBC_A_C:
                    return new Operation(OpCode.SubtractCarry, Operand.A, Operand.C);
                case PrimaryOpCode.SBC_A_D:
                    return new Operation(OpCode.SubtractCarry, Operand.A, Operand.D);
                case PrimaryOpCode.SBC_A_E:
                    return new Operation(OpCode.SubtractCarry, Operand.A, Operand.E);
                case PrimaryOpCode.SBC_A_H:
                    return new Operation(OpCode.SubtractCarry, Operand.A, index.HighRegister);
                case PrimaryOpCode.SBC_A_L:
                    return new Operation(OpCode.SubtractCarry, Operand.A, index.LowRegister);
                case PrimaryOpCode.SBC_A_n:
                    timer.MmuByte();
                    return new Operation(OpCode.SubtractCarry, Operand.A, Operand.n).WithByteLiteral();
                case PrimaryOpCode.SBC_A_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.SubtractCarry, Operand.A, index.Index).WithDisplacement();

                // AND A, r
                case PrimaryOpCode.AND_A:
                    return new Operation(OpCode.And, Operand.A, Operand.A);
                case PrimaryOpCode.AND_B:
                    return new Operation(OpCode.And, Operand.A, Operand.B);
                case PrimaryOpCode.AND_C:
                    return new Operation(OpCode.And, Operand.A, Operand.C);
                case PrimaryOpCode.AND_D:
                    return new Operation(OpCode.And, Operand.A, Operand.D);
                case PrimaryOpCode.AND_E:
                    return new Operation(OpCode.And, Operand.A, Operand.E);
                case PrimaryOpCode.AND_H:
                    return new Operation(OpCode.And, Operand.A, index.HighRegister);
                case PrimaryOpCode.AND_L:
                    return new Operation(OpCode.And, Operand.A, index.LowRegister);
                case PrimaryOpCode.AND_n:
                    timer.MmuByte();
                    return new Operation(OpCode.And, Operand.A, Operand.n).WithByteLiteral();
                case PrimaryOpCode.AND_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.And, Operand.A, index.Index).WithDisplacement();

                // OR A, r
                case PrimaryOpCode.OR_A:
                    return new Operation(OpCode.Or, Operand.A, Operand.A);
                case PrimaryOpCode.OR_B:
                    return new Operation(OpCode.Or, Operand.A, Operand.B);
                case PrimaryOpCode.OR_C:
                    return new Operation(OpCode.Or, Operand.A, Operand.C);
                case PrimaryOpCode.OR_D:
                    return new Operation(OpCode.Or, Operand.A, Operand.D);
                case PrimaryOpCode.OR_E:
                    return new Operation(OpCode.Or, Operand.A, Operand.E);
                case PrimaryOpCode.OR_H:
                    return new Operation(OpCode.Or, Operand.A, index.HighRegister);
                case PrimaryOpCode.OR_L:
                    return new Operation(OpCode.Or, Operand.A, index.LowRegister);
                case PrimaryOpCode.OR_n:
                    timer.MmuByte();
                    return new Operation(OpCode.Or, Operand.A, Operand.n).WithByteLiteral();
                case PrimaryOpCode.OR_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Or, Operand.A, index.Index).WithDisplacement();

                // XOR A, r
                case PrimaryOpCode.XOR_A:
                    return new Operation(OpCode.Xor, Operand.A, Operand.A);
                case PrimaryOpCode.XOR_B:
                    return new Operation(OpCode.Xor, Operand.A, Operand.B);
                case PrimaryOpCode.XOR_C:
                    return new Operation(OpCode.Xor, Operand.A, Operand.C);
                case PrimaryOpCode.XOR_D:
                    return new Operation(OpCode.Xor, Operand.A, Operand.D);
                case PrimaryOpCode.XOR_E:
                    return new Operation(OpCode.Xor, Operand.A, Operand.E);
                case PrimaryOpCode.XOR_H:
                    return new Operation(OpCode.Xor, Operand.A, index.HighRegister);
                case PrimaryOpCode.XOR_L:
                    return new Operation(OpCode.Xor, Operand.A, index.LowRegister);
                case PrimaryOpCode.XOR_n:
                    timer.MmuByte();
                    return new Operation(OpCode.Xor, Operand.A, Operand.n).WithByteLiteral();
                case PrimaryOpCode.XOR_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Xor, Operand.A, index.Index).WithDisplacement();

                // CP A, r
                case PrimaryOpCode.CP_A:
                    return new Operation(OpCode.Compare, Operand.A, Operand.A);
                case PrimaryOpCode.CP_B:
                    return new Operation(OpCode.Compare, Operand.A, Operand.B);
                case PrimaryOpCode.CP_C:
                    return new Operation(OpCode.Compare, Operand.A, Operand.C);
                case PrimaryOpCode.CP_D:
                    return new Operation(OpCode.Compare, Operand.A, Operand.D);
                case PrimaryOpCode.CP_E:
                    return new Operation(OpCode.Compare, Operand.A, Operand.E);
                case PrimaryOpCode.CP_H:
                    return new Operation(OpCode.Compare, Operand.A, index.HighRegister);
                case PrimaryOpCode.CP_L:
                    return new Operation(OpCode.Compare, Operand.A, index.LowRegister);
                case PrimaryOpCode.CP_n:
                    timer.MmuByte();
                    return new Operation(OpCode.Compare, Operand.A, Operand.n).WithByteLiteral();
                case PrimaryOpCode.CP_mHL:
                    timer.Index(index.IsDisplaced);
                    return new Operation(OpCode.Compare, Operand.A, index.Index).WithDisplacement();

                // INC r
                case PrimaryOpCode.INC_A:
                    return new Operation(OpCode.Increment, Operand.A);
                case PrimaryOpCode.INC_B:
                    return new Operation(OpCode.Increment, Operand.B);
                case PrimaryOpCode.INC_C:
                    return new Operation(OpCode.Increment, Operand.C);
                case PrimaryOpCode.INC_D:
                    return new Operation(OpCode.Increment, Operand.D);
                case PrimaryOpCode.INC_E:
                    return new Operation(OpCode.Increment, Operand.E);
                case PrimaryOpCode.INC_H:
                    return new Operation(OpCode.Increment, index.HighRegister);
                case PrimaryOpCode.INC_L:
                    return new Operation(OpCode.Increment, index.LowRegister);
                case PrimaryOpCode.INC_mHL:
                    timer.Index(index.IsDisplaced).MmuByte().Extend(1);
                    return new Operation(OpCode.Increment, index.Index).WithDisplacement();

                // DEC r
                case PrimaryOpCode.DEC_A:
                    return new Operation(OpCode.Decrement, Operand.A);
                case PrimaryOpCode.DEC_B:
                    return new Operation(OpCode.Decrement, Operand.B);
                case PrimaryOpCode.DEC_C:
                    return new Operation(OpCode.Decrement, Operand.C);
                case PrimaryOpCode.DEC_D:
                    return new Operation(OpCode.Decrement, Operand.D);
                case PrimaryOpCode.DEC_E:
                    return new Operation(OpCode.Decrement, Operand.E);
                case PrimaryOpCode.DEC_H:
                    return new Operation(OpCode.Decrement, index.HighRegister);
                case PrimaryOpCode.DEC_L:
                    return new Operation(OpCode.Decrement, index.LowRegister);
                case PrimaryOpCode.DEC_mHL:
                    timer.Index(index.IsDisplaced).MmuByte().Extend(1);
                    return new Operation(OpCode.Decrement, index.Index).WithDisplacement();

                // ********* 16-Bit Arithmetic *********
                // ADD HL, ss
                case PrimaryOpCode.ADD_HL_BC:
                    timer.Arithmetic16();
                    return new Operation(OpCode.Add16, index.Register, Operand.BC);
                case PrimaryOpCode.ADD_HL_DE:
                    timer.Arithmetic16();
                    return new Operation(OpCode.Add16, index.Register, Operand.DE);
                case PrimaryOpCode.ADD_HL_HL:
                    timer.Arithmetic16();
                    return new Operation(OpCode.Add16, index.Register, index.Register);
                case PrimaryOpCode.ADD_HL_SP:
                    timer.Arithmetic16();
                    return new Operation(OpCode.Add16, index.Register, Operand.SP);

                // INC ss
                case PrimaryOpCode.INC_BC:
                    timer.Extend(2);
                    return new Operation(OpCode.Increment16, Operand.BC);
                case PrimaryOpCode.INC_DE:
                    timer.Extend(2);
                    return new Operation(OpCode.Increment16, Operand.DE);
                case PrimaryOpCode.INC_HL:
                    timer.Extend(2);
                    return new Operation(OpCode.Increment16, index.Register);
                case PrimaryOpCode.INC_SP:
                    timer.Extend(2);
                    return new Operation(OpCode.Increment16, Operand.SP);

                // DEC ss
                case PrimaryOpCode.DEC_BC:
                    timer.Extend(2);
                    return new Operation(OpCode.Decrement16, Operand.BC);
                case PrimaryOpCode.DEC_DE:
                    timer.Extend(2);
                    return new Operation(OpCode.Decrement16, Operand.DE);
                case PrimaryOpCode.DEC_HL:
                    timer.Extend(2);
                    return new Operation(OpCode.Decrement16, index.Register);
                case PrimaryOpCode.DEC_SP:
                    timer.Extend(2);
                    return new Operation(OpCode.Decrement16, Operand.SP);

                // ********* General-Purpose Arithmetic *********
                // DAA
                case PrimaryOpCode.DAA:
                    return new Operation(OpCode.DecimalArithmeticAdjust);

                // CPL
                case PrimaryOpCode.CPL:
                    return new Operation(OpCode.NegateOnesComplement);

                // CCF
                case PrimaryOpCode.CCF:
                    return new Operation(OpCode.InvertCarryFlag);

                // SCF
                case PrimaryOpCode.SCF:
                    return new Operation(OpCode.SetCarryFlag);

                // DI
                case PrimaryOpCode.DI:
                    return new Operation(OpCode.DisableInterrupts);

                // EI
                case PrimaryOpCode.EI:
                    return new Operation(OpCode.EnableInterrupts);

                // ********* Rotate *********
                // RLCA
                case PrimaryOpCode.RLCA:
                    return new Operation(OpCode.RotateLeftWithCarry, Operand.A);

                // RLA
                case PrimaryOpCode.RLA:
                    return new Operation(OpCode.RotateLeft, Operand.A);

                // RRCA
                case PrimaryOpCode.RRCA:
                    return new Operation(OpCode.RotateRightWithCarry, Operand.A);

                // RRA
                case PrimaryOpCode.RRA:
                    return new Operation(OpCode.RotateRight, Operand.A);

                // ********* Jump *********
                case PrimaryOpCode.JP:
                    timer.MmuWord();
                    return new Operation(OpCode.Jump, Operand.nn).WithWordLiteral().EndBlock();
                case PrimaryOpCode.JP_NZ:
                    timer.MmuWord();
                    return new Operation(OpCode.Jump, Operand.nn).WithFlag(FlagTest.NotZero).WithWordLiteral().EndBlock();
                case PrimaryOpCode.JP_Z:
                    timer.MmuWord();
                    return new Operation(OpCode.Jump, Operand.nn).WithFlag(FlagTest.Zero).WithWordLiteral().EndBlock();
                case PrimaryOpCode.JP_NC:
                    timer.MmuWord();
                    return new Operation(OpCode.Jump, Operand.nn).WithFlag(FlagTest.NotCarry).WithWordLiteral().EndBlock();
                case PrimaryOpCode.JP_C:
                    timer.MmuWord();
                    return new Operation(OpCode.Jump, Operand.nn).WithFlag(FlagTest.Carry).WithWordLiteral().EndBlock();
                case PrimaryOpCode.JP_PO:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD (FF00+C), A on GB
                        timer.MmuByte();
                        return new Operation(OpCode.Load, Operand.mCl, Operand.A);
                    }
                    timer.MmuWord();
                    return new Operation(OpCode.Jump, Operand.nn).WithFlag(FlagTest.ParityOdd).WithWordLiteral().EndBlock();
                case PrimaryOpCode.JP_PE:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD (nn), A on GB
                        timer.MmuWord().MmuByte();
                        return new Operation(OpCode.Load, Operand.mnn, Operand.A).WithWordLiteral();
                    }
                    timer.MmuWord();
                    return new Operation(OpCode.Jump, Operand.nn).WithFlag(FlagTest.ParityEven).WithWordLiteral().EndBlock();
                case PrimaryOpCode.JP_P:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD A, (FF00+C) on GB
                        timer.MmuByte();
                        return new Operation(OpCode.Load, Operand.A, Operand.mCl);
                    }
                    timer.MmuWord();
                    return new Operation(OpCode.Jump, Operand.nn).WithFlag(FlagTest.Possitive).WithWordLiteral().EndBlock();
                case PrimaryOpCode.JP_M:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD A, (nn) on GB
                        timer.MmuWord().MmuByte();
                        return new Operation(OpCode.Load, Operand.A, Operand.mnn).WithWordLiteral();
                    }
                    timer.MmuWord();
                    return new Operation(OpCode.Jump, Operand.nn).WithFlag(FlagTest.Negative).WithWordLiteral().EndBlock();
                case PrimaryOpCode.JR:
                    timer.MmuByte().ApplyDisplacement();
                    return new Operation(OpCode.JumpRelative, Operand.d).WithByteLiteral().EndBlock();
                case PrimaryOpCode.JR_C:
                    timer.MmuByte();
                    return new Operation(OpCode.JumpRelative, Operand.d).WithFlag(FlagTest.Carry).WithByteLiteral().EndBlock();
                case PrimaryOpCode.JR_NC:
                    timer.MmuByte();
                    return new Operation(OpCode.JumpRelative, Operand.d).WithFlag(FlagTest.NotCarry).WithByteLiteral().EndBlock();
                case PrimaryOpCode.JR_Z:
                    timer.MmuByte();
                    return new Operation(OpCode.JumpRelative, Operand.d).WithFlag(FlagTest.Zero).WithByteLiteral().EndBlock();
                case PrimaryOpCode.JR_NZ:
                    timer.MmuByte();
                    return new Operation(OpCode.JumpRelative, Operand.d).WithFlag(FlagTest.NotZero).WithByteLiteral().EndBlock();
                case PrimaryOpCode.JP_mHL:
                    return new Operation(OpCode.Jump, index.Register).EndBlock();
                case PrimaryOpCode.DJNZ:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as STOP on GB
                        return new Operation(OpCode.Stop).EndBlock();
                    }

                    timer.Extend(1).MmuByte();
                    return new Operation(OpCode.DecrementJumpRelativeIfNonZero, Operand.d).WithByteLiteral().EndBlock();

                // ********* Call *********
                case PrimaryOpCode.CALL:
                    timer.MmuWord().Extend(1).MmuWord();
                    return new Operation(OpCode.Call, Operand.nn).WithWordLiteral().EndBlock();
                case PrimaryOpCode.CALL_NZ:
                    timer.MmuWord();
                    return new Operation(OpCode.Call, Operand.nn).WithFlag(FlagTest.NotZero).WithWordLiteral().EndBlock();
                case PrimaryOpCode.CALL_Z:
                    timer.MmuWord();
                    return new Operation(OpCode.Call, Operand.nn).WithFlag(FlagTest.Zero).WithWordLiteral().EndBlock();
                case PrimaryOpCode.CALL_NC:
                    timer.MmuWord();
                    return new Operation(OpCode.Call, Operand.nn).WithFlag(FlagTest.NotCarry).WithWordLiteral().EndBlock();
                case PrimaryOpCode.CALL_C:
                    timer.MmuWord();
                    return new Operation(OpCode.Call, Operand.nn).WithFlag(FlagTest.Carry).WithWordLiteral().EndBlock();
                case PrimaryOpCode.CALL_PO:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return new Operation(undefinedInstruction);
                    }
                    timer.MmuWord();
                    return new Operation(OpCode.Call, Operand.nn).WithFlag(FlagTest.ParityOdd).WithWordLiteral().EndBlock();
                case PrimaryOpCode.CALL_PE:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return new Operation(undefinedInstruction);
                    }
                    timer.MmuWord();
                    return new Operation(OpCode.Call, Operand.nn).WithFlag(FlagTest.ParityEven).WithWordLiteral().EndBlock();
                case PrimaryOpCode.CALL_P:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return new Operation(undefinedInstruction);
                    }
                    timer.MmuWord();
                    return new Operation(OpCode.Call, Operand.nn).WithFlag(FlagTest.Possitive).WithWordLiteral().EndBlock();
                case PrimaryOpCode.CALL_M:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return new Operation(undefinedInstruction);
                    }
                    timer.MmuWord();
                    return new Operation(OpCode.Call, Operand.nn).WithFlag(FlagTest.Negative).WithWordLiteral().EndBlock();

                // ********* Return *********
                case PrimaryOpCode.RET:
                    timer.MmuWord();
                    return new Operation(OpCode.Return).EndBlock();
                case PrimaryOpCode.RET_NZ:
                    timer.Extend(1);
                    return new Operation(OpCode.Return).WithFlag(FlagTest.NotZero).EndBlock();
                case PrimaryOpCode.RET_Z:
                    timer.Extend(1);
                    return new Operation(OpCode.Return).WithFlag(FlagTest.Zero).EndBlock();
                case PrimaryOpCode.RET_NC:
                    timer.Extend(1);
                    return new Operation(OpCode.Return).WithFlag(FlagTest.NotCarry).EndBlock();
                case PrimaryOpCode.RET_C:
                    timer.Extend(1);
                    return new Operation(OpCode.Return).WithFlag(FlagTest.Carry).EndBlock();
                case PrimaryOpCode.RET_PO:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD (FF00+n), A on GB
                        timer.MmuWord();
                        return new Operation(OpCode.Load, Operand.mnl, Operand.A).WithByteLiteral();
                    }

                    timer.Extend(1);
                    return new Operation(OpCode.Return).WithFlag(FlagTest.ParityOdd).EndBlock();
                case PrimaryOpCode.RET_PE:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD SP, SP+d on GB
                        timer.MmuByte().Arithmetic16();
                        return new Operation(OpCode.Load16, Operand.SP, Operand.SPd).WithByteLiteral();
                    }

                    timer.Extend(1);
                    return new Operation(OpCode.Return).WithFlag(FlagTest.ParityEven).EndBlock();
                case PrimaryOpCode.RET_P:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD A, (FF00+n) on GB
                        timer.MmuWord();
                        return new Operation(OpCode.Load, Operand.A, Operand.mnl).WithByteLiteral();
                    }

                    timer.Extend(1);
                    return new Operation(OpCode.Return).WithFlag(FlagTest.Possitive).EndBlock();
                case PrimaryOpCode.RET_M:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD HL, SP+dd on GB
                        timer.Arithmetic16();
                        return new Operation(OpCode.Load, Operand.HL, Operand.SPd).WithByteLiteral();
                    }

                    timer.Extend(1);
                    return new Operation(OpCode.Return).WithFlag(FlagTest.Negative).EndBlock();

                // ********* Reset *********
                case PrimaryOpCode.RST_00:
                    timer.Extend(1).MmuWord();
                    return new Operation(OpCode.Reset, Operand.nn).AddLiteral((ushort)0x0000).EndBlock();
                case PrimaryOpCode.RST_08:
                    timer.Extend(1).MmuWord();
                    return new Operation(OpCode.Reset, Operand.nn).AddLiteral((ushort)0x0008).EndBlock();
                case PrimaryOpCode.RST_10:
                    timer.Extend(1).MmuWord();
                    return new Operation(OpCode.Reset, Operand.nn).AddLiteral((ushort)0x0010).EndBlock();
                case PrimaryOpCode.RST_18:
                    timer.Extend(1).MmuWord();
                    return new Operation(OpCode.Reset, Operand.nn).AddLiteral((ushort)0x0018).EndBlock();
                case PrimaryOpCode.RST_20:
                    timer.Extend(1).MmuWord();
                    return new Operation(OpCode.Reset, Operand.nn).AddLiteral((ushort)0x0020).EndBlock();
                case PrimaryOpCode.RST_28:
                    timer.Extend(1).MmuWord();
                    return new Operation(OpCode.Reset, Operand.nn).AddLiteral((ushort)0x0028).EndBlock();
                case PrimaryOpCode.RST_30:
                    timer.Extend(1).MmuWord();
                    return new Operation(OpCode.Reset, Operand.nn).AddLiteral((ushort)0x0030).EndBlock();
                case PrimaryOpCode.RST_38:
                    timer.Extend(1).MmuWord();
                    return new Operation(OpCode.Reset, Operand.nn).AddLiteral((ushort)0x0038).EndBlock();

                // ********* IO *********
                case PrimaryOpCode.IN_A_n:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return new Operation(undefinedInstruction);
                    }
                    timer.MmuByte().Io();
                    return new Operation(OpCode.Input, Operand.A, Operand.n).WithByteLiteral();
                case PrimaryOpCode.OUT_A_n:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return new Operation(undefinedInstruction);
                    }
                    timer.MmuByte().Io();
                    return new Operation(OpCode.Output, Operand.A, Operand.n).WithByteLiteral();
                default:
                    throw new ArgumentOutOfRangeException(nameof(code), code, null);
            }
        }
    }
}
