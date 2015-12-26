namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System;

    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;

    internal partial class OpCodeDecoder
    {
        private OpCode? DecodePrimary()
        {
            var code = (PrimaryOpCode)prefetch.NextByte();

            // Add a NOP on every frame. Reduce timings by a NOP elsewhere.
            timer.Nop();

            switch (code)
            {
                case PrimaryOpCode.NOP:
                    return OpCode.NoOperation;
                case PrimaryOpCode.HALT:
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.Halt;

                // ********* Prefixes *********
                case PrimaryOpCode.Prefix_CB:
                    if (cpuMode == CpuMode.Z80 || cpuMode == CpuMode.GameBoy)
                    {
                        // Only Z80 & GBCPU has prefix CB
                        if (!index.IsDisplaced)
                        {
                            return DecodePrefixCb();
                        }
                        displacement = prefetch.NextByte();
                        return FixPrefixDdFdPrefixCbResult(DecodePrefixCb());
                    }
                    return undefinedInstruction;

                case PrimaryOpCode.Prefix_DD:
                    if (cpuMode == CpuMode.Z80)
                    {
                        // Only Z80 has prefix DD
                        index = indexRegisterOperands[IndexRegister.IX];
                        return null;
                    }
                    return undefinedInstruction;

                case PrimaryOpCode.Prefix_ED:
                    if (cpuMode == CpuMode.Z80)
                    {
                        // Only Z80 has prefix ED
                        return DecodePrefixEd();
                    }
                    return undefinedInstruction;

                case PrimaryOpCode.Prefix_FD:
                    if (cpuMode == CpuMode.Z80)
                    {
                        // Only Z80 has prefix FD
                        index = indexRegisterOperands[IndexRegister.IY];
                        return null;
                    }
                    return undefinedInstruction;

                // ********* 8-bit load *********
                // LD r, r'
                case PrimaryOpCode.LD_A_A:
                    return OpCode.NoOperation;
                case PrimaryOpCode.LD_B_A:
                    operand1 = Operand.B;
                    operand2 = Operand.A;
                    return OpCode.Load;
                case PrimaryOpCode.LD_C_A:
                    operand1 = Operand.C;
                    operand2 = Operand.A;
                    return OpCode.Load;
                case PrimaryOpCode.LD_D_A:
                    operand1 = Operand.D;
                    operand2 = Operand.A;
                    return OpCode.Load;
                case PrimaryOpCode.LD_E_A:
                    operand1 = Operand.E;
                    operand2 = Operand.A;
                    return OpCode.Load;
                case PrimaryOpCode.LD_H_A:
                    operand1 = index.HighRegister;
                    operand2 = Operand.A;
                    return OpCode.Load;
                case PrimaryOpCode.LD_L_A:
                    operand1 = index.LowRegister;
                    operand2 = Operand.A;
                    return OpCode.Load;
                case PrimaryOpCode.LD_A_B:
                    operand1 = Operand.A;
                    operand2 = Operand.B;
                    return OpCode.Load;
                case PrimaryOpCode.LD_B_B:
                    return OpCode.NoOperation;
                case PrimaryOpCode.LD_C_B:
                    operand1 = Operand.C;
                    operand2 = Operand.B;
                    return OpCode.Load;
                case PrimaryOpCode.LD_D_B:
                    operand1 = Operand.D;
                    operand2 = Operand.B;
                    return OpCode.Load;
                case PrimaryOpCode.LD_E_B:
                    operand1 = Operand.E;
                    operand2 = Operand.B;
                    return OpCode.Load;
                case PrimaryOpCode.LD_H_B:
                    operand1 = index.HighRegister;
                    operand2 = Operand.B;
                    return OpCode.Load;
                case PrimaryOpCode.LD_L_B:
                    operand1 = index.LowRegister;
                    operand2 = Operand.B;
                    return OpCode.Load;
                case PrimaryOpCode.LD_A_C:
                    operand1 = Operand.A;
                    operand2 = Operand.C;
                    return OpCode.Load;
                case PrimaryOpCode.LD_B_C:
                    operand1 = Operand.B;
                    operand2 = Operand.C;
                    return OpCode.Load;
                case PrimaryOpCode.LD_C_C:
                    return OpCode.NoOperation;
                case PrimaryOpCode.LD_D_C:
                    operand1 = Operand.D;
                    operand2 = Operand.C;
                    return OpCode.Load;
                case PrimaryOpCode.LD_E_C:
                    operand1 = Operand.E;
                    operand2 = Operand.C;
                    return OpCode.Load;
                case PrimaryOpCode.LD_H_C:
                    operand1 = index.HighRegister;
                    operand2 = Operand.C;
                    return OpCode.Load;
                case PrimaryOpCode.LD_L_C:
                    operand1 = index.LowRegister;
                    operand2 = Operand.C;
                    return OpCode.Load;
                case PrimaryOpCode.LD_A_D:
                    operand1 = Operand.A;
                    operand2 = Operand.D;
                    return OpCode.Load;
                case PrimaryOpCode.LD_B_D:
                    operand1 = Operand.B;
                    operand2 = Operand.D;
                    return OpCode.Load;
                case PrimaryOpCode.LD_C_D:
                    operand1 = Operand.C;
                    operand2 = Operand.D;
                    return OpCode.Load;
                case PrimaryOpCode.LD_D_D:
                    return OpCode.NoOperation;
                case PrimaryOpCode.LD_E_D:
                    operand1 = Operand.E;
                    operand2 = Operand.D;
                    return OpCode.Load;
                case PrimaryOpCode.LD_H_D:
                    operand1 = index.HighRegister;
                    operand2 = Operand.D;
                    return OpCode.Load;
                case PrimaryOpCode.LD_L_D:
                    operand1 = index.LowRegister;
                    operand2 = Operand.D;
                    return OpCode.Load;
                case PrimaryOpCode.LD_A_E:
                    operand1 = Operand.A;
                    operand2 = Operand.E;
                    return OpCode.Load;
                case PrimaryOpCode.LD_B_E:
                    operand1 = Operand.B;
                    operand2 = Operand.E;
                    return OpCode.Load;
                case PrimaryOpCode.LD_C_E:
                    operand1 = Operand.C;
                    operand2 = Operand.E;
                    return OpCode.Load;
                case PrimaryOpCode.LD_D_E:
                    operand1 = Operand.D;
                    operand2 = Operand.E;
                    return OpCode.Load;
                case PrimaryOpCode.LD_E_E:
                    return OpCode.NoOperation;
                case PrimaryOpCode.LD_H_E:
                    operand1 = index.HighRegister;
                    operand2 = Operand.E;
                    return OpCode.Load;
                case PrimaryOpCode.LD_L_E:
                    operand1 = index.LowRegister;
                    operand2 = Operand.E;
                    return OpCode.Load;
                case PrimaryOpCode.LD_A_H:
                    operand1 = Operand.A;
                    operand2 = index.HighRegister;
                    return OpCode.Load;
                case PrimaryOpCode.LD_B_H:
                    operand1 = Operand.B;
                    operand2 = index.HighRegister;
                    return OpCode.Load;
                case PrimaryOpCode.LD_C_H:
                    operand1 = Operand.C;
                    operand2 = index.HighRegister;
                    return OpCode.Load;
                case PrimaryOpCode.LD_D_H:
                    operand1 = Operand.D;
                    operand2 = index.HighRegister;
                    return OpCode.Load;
                case PrimaryOpCode.LD_E_H:
                    operand1 = Operand.E;
                    operand2 = index.HighRegister;
                    return OpCode.Load;
                case PrimaryOpCode.LD_H_H:
                    return OpCode.NoOperation;
                case PrimaryOpCode.LD_L_H:
                    operand1 = index.LowRegister;
                    operand2 = index.HighRegister;
                    return OpCode.Load;
                case PrimaryOpCode.LD_A_L:
                    operand1 = Operand.A;
                    operand2 = index.LowRegister;
                    return OpCode.Load;
                case PrimaryOpCode.LD_B_L:
                    operand1 = Operand.B;
                    operand2 = index.LowRegister;
                    return OpCode.Load;
                case PrimaryOpCode.LD_C_L:
                    operand1 = Operand.C;
                    operand2 = index.LowRegister;
                    return OpCode.Load;
                case PrimaryOpCode.LD_D_L:
                    operand1 = Operand.D;
                    operand2 = index.LowRegister;
                    return OpCode.Load;
                case PrimaryOpCode.LD_E_L:
                    operand1 = Operand.E;
                    operand2 = index.LowRegister;
                    return OpCode.Load;
                case PrimaryOpCode.LD_H_L:
                    operand1 = index.HighRegister;
                    operand2 = index.LowRegister;
                    return OpCode.Load;
                case PrimaryOpCode.LD_L_L:
                    return OpCode.NoOperation;

                // LD r,n
                case PrimaryOpCode.LD_A_n:
                    timer.MmuByte();
                    operand1 = Operand.A;
                    operand2 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Load;
                case PrimaryOpCode.LD_B_n:
                    timer.MmuByte();
                    operand1 = Operand.B;
                    operand2 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Load;
                case PrimaryOpCode.LD_C_n:
                    timer.MmuByte();
                    operand1 = Operand.C;
                    operand2 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Load;
                case PrimaryOpCode.LD_D_n:
                    timer.MmuByte();
                    operand1 = Operand.D;
                    operand2 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Load;
                case PrimaryOpCode.LD_E_n:
                    timer.MmuByte();
                    operand1 = Operand.E;
                    operand2 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Load;
                case PrimaryOpCode.LD_H_n:
                    timer.MmuByte();
                    operand1 = index.HighRegister;
                    operand2 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Load;
                case PrimaryOpCode.LD_L_n:
                    timer.MmuByte();
                    operand1 = index.LowRegister;
                    operand2 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Load;

                // LD r, (HL)
                case PrimaryOpCode.LD_A_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = Operand.A;
                    operand2 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;
                case PrimaryOpCode.LD_B_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = Operand.B;
                    operand2 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;
                case PrimaryOpCode.LD_C_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = Operand.C;
                    operand2 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;
                case PrimaryOpCode.LD_D_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = Operand.D;
                    operand2 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;
                case PrimaryOpCode.LD_E_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = Operand.E;
                    operand2 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;
                case PrimaryOpCode.LD_H_mHL:
                    timer.Index(index.IsDisplaced);
                    // H register is always assigned here
                    operand1 = Operand.H;
                    operand2 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;
                case PrimaryOpCode.LD_L_mHL:
                    timer.Index(index.IsDisplaced);
                    // L register is always assigned here
                    operand1 = Operand.L;
                    operand2 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;

                // LD (HL), r
                case PrimaryOpCode.LD_mHL_A:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    operand2 = Operand.A;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;
                case PrimaryOpCode.LD_mHL_B:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    operand2 = Operand.B;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;
                case PrimaryOpCode.LD_mHL_C:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    operand2 = Operand.C;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;
                case PrimaryOpCode.LD_mHL_D:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    operand2 = Operand.D;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;
                case PrimaryOpCode.LD_mHL_E:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    operand2 = Operand.E;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;
                case PrimaryOpCode.LD_mHL_H:
                    timer.Index(index.IsDisplaced);
                    // Value of H register is always used here
                    operand1 = index.Index;
                    operand2 = Operand.H;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;
                case PrimaryOpCode.LD_mHL_L:
                    timer.Index(index.IsDisplaced);
                    // Value of L register is always used here
                    operand1 = index.Index;
                    operand2 = Operand.L;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Load;

                // LD (HL), n
                case PrimaryOpCode.LD_mHL_n:
                    timer.IndexAndMmuByte(index.IsDisplaced);
                    operand1 = index.Index;
                    operand2 = Operand.n;
                    opCodeMeta = OpCodeMeta.Displacement | OpCodeMeta.ByteLiteral;
                    return OpCode.Load;

                // LD A, (BC)
                case PrimaryOpCode.LD_A_mBC:
                    timer.Index(false);
                    operand1 = Operand.A;
                    operand2 = Operand.mBC;
                    return OpCode.Load;

                // LD A, (BC)
                case PrimaryOpCode.LD_A_mDE:
                    timer.Index(false);
                    operand1 = Operand.A;
                    operand2 = Operand.mDE;
                    return OpCode.Load;

                // LD A, (nn)
                case PrimaryOpCode.LD_A_mnn:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LDD  A, (HL) on GB
                        timer.MmuByte();
                        operand1 = Operand.A;
                        operand2 = index.Index;
                        return OpCode.LoadDecrement;
                    }
                    timer.Index(false).MmuWord();
                    operand1 = Operand.A;
                    operand2 = Operand.mnn;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load;

                // LD (BC), A
                case PrimaryOpCode.LD_mBC_A:
                    timer.Index(false);
                    operand1 = Operand.mBC;
                    operand2 = Operand.A;
                    return OpCode.Load;

                // LD (DE), A
                case PrimaryOpCode.LD_mDE_A:
                    timer.Index(false);
                    operand1 = Operand.mDE;
                    operand2 = Operand.A;
                    return OpCode.Load;

                // LD (nn), A
                case PrimaryOpCode.LD_mnn_A:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LDD  (HL), A on GB
                        timer.MmuByte();
                        operand1 = index.Index;
                        operand2 = Operand.A;
                        return OpCode.LoadDecrement;
                    }
                    timer.Index(false).MmuWord();
                    operand1 = Operand.mnn;
                    operand2 = Operand.A;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load;

                // ********* 16-bit load *********
                // LD dd, nn
                case PrimaryOpCode.LD_BC_nn:
                    timer.MmuWord();
                    operand1 = Operand.BC;
                    operand2 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;
                case PrimaryOpCode.LD_DE_nn:
                    timer.MmuWord();
                    operand1 = Operand.DE;
                    operand2 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;
                case PrimaryOpCode.LD_HL_nn:
                    timer.MmuWord();
                    operand1 = index.Register;
                    operand2 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;
                case PrimaryOpCode.LD_SP_nn:
                    timer.MmuWord();
                    operand1 = Operand.SP;
                    operand2 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;

                // LD HL, (nn)
                case PrimaryOpCode.LD_HL_mnn:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LDI  A, (HL) on GB
                        timer.MmuByte();
                        operand1 = Operand.A;
                        operand2 = index.Index;
                        return OpCode.LoadIncrement;
                    }

                    timer.IndexAndMmuWord();
                    operand1 = index.Register;
                    operand2 = Operand.mnn;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;

                // LD (nn), HL
                case PrimaryOpCode.LD_mnn_HL:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LDI  (HL), A on GB
                        timer.MmuByte();
                        operand1 = index.Index;
                        operand2 = Operand.A;
                        return OpCode.LoadIncrement;
                    }

                    timer.IndexAndMmuWord();
                    operand1 = Operand.mnn;
                    operand2 = index.Register;
                    opCodeMeta = OpCodeMeta.WordLiteral;
                    return OpCode.Load16;

                // LD SP, HL
                case PrimaryOpCode.LD_SP_HL:
                    timer.Extend(2);
                    operand1 = Operand.SP;
                    operand2 = index.Register;
                    return OpCode.Load16;

                // PUSH qq
                case PrimaryOpCode.PUSH_BC:
                    timer.Extend(1).MmuWord();
                    operand1 = Operand.BC;
                    return OpCode.Push;
                case PrimaryOpCode.PUSH_DE:
                    timer.Extend(1).MmuWord();
                    operand1 = Operand.DE;
                    return OpCode.Push;
                case PrimaryOpCode.PUSH_HL:
                    timer.Extend(1).MmuWord();
                    operand1 = index.Register;
                    return OpCode.Push;
                case PrimaryOpCode.PUSH_AF:
                    timer.Extend(1).MmuWord();
                    operand1 = Operand.AF;
                    return OpCode.Push;

                // POP qq
                case PrimaryOpCode.POP_BC:
                    timer.MmuWord();
                    operand1 = Operand.BC;
                    return OpCode.Pop;
                case PrimaryOpCode.POP_DE:
                    timer.MmuWord();
                    operand1 = Operand.DE;
                    return OpCode.Pop;
                case PrimaryOpCode.POP_HL:
                    timer.MmuWord();
                    operand1 = index.Register;
                    return OpCode.Pop;
                case PrimaryOpCode.POP_AF:
                    timer.MmuWord();
                    operand1 = Operand.AF;
                    return OpCode.Pop;

                // ********* Exchange *********
                // EX DE, HL
                case PrimaryOpCode.EX_DE_HL:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return undefinedInstruction;
                    }

                    // This affects HL register directly, always ignoring index register prefixes
                    operand1 = Operand.DE;
                    operand2 = Operand.HL;
                    return OpCode.Exchange;

                // EX AF, AF′
                case PrimaryOpCode.EX_AF:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD (nn),SP on GB
                        timer.IndexAndMmuWord();
                        operand1 = Operand.mnn;
                        operand2 = Operand.SP;
                        opCodeMeta = OpCodeMeta.WordLiteral;
                        return OpCode.Load16;
                    }

                    return OpCode.ExchangeAccumulatorAndFlags;

                // EXX
                case PrimaryOpCode.EXX:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as RETI on GB, retains NOP timing
                        timer.MmuWord().Nop();
                        opCodeMeta = OpCodeMeta.EndBlock;
                        return OpCode.ReturnFromInterrupt;
                    }

                    return OpCode.ExchangeGeneralPurpose;

                // EX (SP), HL
                case PrimaryOpCode.EX_mSP_HL:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return undefinedInstruction;
                    }

                    timer.Arithmetic16().MmuWord().Extend(2);
                    operand1 = Operand.mSP;
                    operand2 = index.Register;
                    return OpCode.Exchange;

                // ********* 8-Bit Arithmetic *********
                // ADD A, r
                case PrimaryOpCode.ADD_A_A:
                    operand1 = Operand.A;
                    return OpCode.Add;
                case PrimaryOpCode.ADD_A_B:
                    operand1 = Operand.B;
                    return OpCode.Add;
                case PrimaryOpCode.ADD_A_C:
                    operand1 = Operand.C;
                    return OpCode.Add;
                case PrimaryOpCode.ADD_A_D:
                    operand1 = Operand.D;
                    return OpCode.Add;
                case PrimaryOpCode.ADD_A_E:
                    operand1 = Operand.E;
                    return OpCode.Add;
                case PrimaryOpCode.ADD_A_H:
                    operand1 = index.HighRegister;
                    return OpCode.Add;
                case PrimaryOpCode.ADD_A_L:
                    operand1 = index.LowRegister;
                    return OpCode.Add;
                case PrimaryOpCode.ADD_A_n:
                    timer.MmuByte();
                    operand1 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Add;
                case PrimaryOpCode.ADD_A_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Add;

                // ADC A, r
                case PrimaryOpCode.ADC_A_A:
                    operand1 = Operand.A;
                    return OpCode.AddCarry;
                case PrimaryOpCode.ADC_A_B:
                    operand1 = Operand.B;
                    return OpCode.AddCarry;
                case PrimaryOpCode.ADC_A_C:
                    operand1 = Operand.C;
                    return OpCode.AddCarry;
                case PrimaryOpCode.ADC_A_D:
                    operand1 = Operand.D;
                    return OpCode.AddCarry;
                case PrimaryOpCode.ADC_A_E:
                    operand1 = Operand.E;
                    return OpCode.AddCarry;
                case PrimaryOpCode.ADC_A_H:
                    operand1 = index.HighRegister;
                    return OpCode.AddCarry;
                case PrimaryOpCode.ADC_A_L:
                    operand1 = index.LowRegister;
                    return OpCode.AddCarry;
                case PrimaryOpCode.ADC_A_n:
                    timer.MmuByte();
                    operand1 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.AddCarry;
                case PrimaryOpCode.ADC_A_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.AddCarry;

                // SUB A, r
                case PrimaryOpCode.SUB_A_A:
                    operand1 = Operand.A;
                    return OpCode.Subtract;
                case PrimaryOpCode.SUB_A_B:
                    operand1 = Operand.B;
                    return OpCode.Subtract;
                case PrimaryOpCode.SUB_A_C:
                    operand1 = Operand.C;
                    return OpCode.Subtract;
                case PrimaryOpCode.SUB_A_D:
                    operand1 = Operand.D;
                    return OpCode.Subtract;
                case PrimaryOpCode.SUB_A_E:
                    operand1 = Operand.E;
                    return OpCode.Subtract;
                case PrimaryOpCode.SUB_A_H:
                    operand1 = index.HighRegister;
                    return OpCode.Subtract;
                case PrimaryOpCode.SUB_A_L:
                    operand1 = index.LowRegister;
                    return OpCode.Subtract;
                case PrimaryOpCode.SUB_A_n:
                    timer.MmuByte();
                    operand1 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Subtract;
                case PrimaryOpCode.SUB_A_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Subtract;

                // SBC A, r
                case PrimaryOpCode.SBC_A_A:
                    operand1 = Operand.A;
                    return OpCode.SubtractCarry;
                case PrimaryOpCode.SBC_A_B:
                    operand1 = Operand.B;
                    return OpCode.SubtractCarry;
                case PrimaryOpCode.SBC_A_C:
                    operand1 = Operand.C;
                    return OpCode.SubtractCarry;
                case PrimaryOpCode.SBC_A_D:
                    operand1 = Operand.D;
                    return OpCode.SubtractCarry;
                case PrimaryOpCode.SBC_A_E:
                    operand1 = Operand.E;
                    return OpCode.SubtractCarry;
                case PrimaryOpCode.SBC_A_H:
                    operand1 = index.HighRegister;
                    return OpCode.SubtractCarry;
                case PrimaryOpCode.SBC_A_L:
                    operand1 = index.LowRegister;
                    return OpCode.SubtractCarry;
                case PrimaryOpCode.SBC_A_n:
                    timer.MmuByte();
                    operand1 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.SubtractCarry;
                case PrimaryOpCode.SBC_A_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.SubtractCarry;

                // AND A, r
                case PrimaryOpCode.AND_A:
                    operand1 = Operand.A;
                    return OpCode.And;
                case PrimaryOpCode.AND_B:
                    operand1 = Operand.B;
                    return OpCode.And;
                case PrimaryOpCode.AND_C:
                    operand1 = Operand.C;
                    return OpCode.And;
                case PrimaryOpCode.AND_D:
                    operand1 = Operand.D;
                    return OpCode.And;
                case PrimaryOpCode.AND_E:
                    operand1 = Operand.E;
                    return OpCode.And;
                case PrimaryOpCode.AND_H:
                    operand1 = index.HighRegister;
                    return OpCode.And;
                case PrimaryOpCode.AND_L:
                    operand1 = index.LowRegister;
                    return OpCode.And;
                case PrimaryOpCode.AND_n:
                    timer.MmuByte();
                    operand1 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.And;
                case PrimaryOpCode.AND_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.And;

                // OR A, r
                case PrimaryOpCode.OR_A:
                    operand1 = Operand.A;
                    return OpCode.Or;
                case PrimaryOpCode.OR_B:
                    operand1 = Operand.B;
                    return OpCode.Or;
                case PrimaryOpCode.OR_C:
                    operand1 = Operand.C;
                    return OpCode.Or;
                case PrimaryOpCode.OR_D:
                    operand1 = Operand.D;
                    return OpCode.Or;
                case PrimaryOpCode.OR_E:
                    operand1 = Operand.E;
                    return OpCode.Or;
                case PrimaryOpCode.OR_H:
                    operand1 = index.HighRegister;
                    return OpCode.Or;
                case PrimaryOpCode.OR_L:
                    operand1 = index.LowRegister;
                    return OpCode.Or;
                case PrimaryOpCode.OR_n:
                    timer.MmuByte();
                    operand1 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Or;
                case PrimaryOpCode.OR_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Or;

                // XOR A, r
                case PrimaryOpCode.XOR_A:
                    operand1 = Operand.A;
                    return OpCode.Xor;
                case PrimaryOpCode.XOR_B:
                    operand1 = Operand.B;
                    return OpCode.Xor;
                case PrimaryOpCode.XOR_C:
                    operand1 = Operand.C;
                    return OpCode.Xor;
                case PrimaryOpCode.XOR_D:
                    operand1 = Operand.D;
                    return OpCode.Xor;
                case PrimaryOpCode.XOR_E:
                    operand1 = Operand.E;
                    return OpCode.Xor;
                case PrimaryOpCode.XOR_H:
                    operand1 = index.HighRegister;
                    return OpCode.Xor;
                case PrimaryOpCode.XOR_L:
                    operand1 = index.LowRegister;
                    return OpCode.Xor;
                case PrimaryOpCode.XOR_n:
                    timer.MmuByte();
                    operand1 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Xor;
                case PrimaryOpCode.XOR_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Xor;

                // CP A, r
                case PrimaryOpCode.CP_A:
                    operand1 = Operand.A;
                    return OpCode.Compare;
                case PrimaryOpCode.CP_B:
                    operand1 = Operand.B;
                    return OpCode.Compare;
                case PrimaryOpCode.CP_C:
                    operand1 = Operand.C;
                    return OpCode.Compare;
                case PrimaryOpCode.CP_D:
                    operand1 = Operand.D;
                    return OpCode.Compare;
                case PrimaryOpCode.CP_E:
                    operand1 = Operand.E;
                    return OpCode.Compare;
                case PrimaryOpCode.CP_H:
                    operand1 = index.HighRegister;
                    return OpCode.Compare;
                case PrimaryOpCode.CP_L:
                    operand1 = index.LowRegister;
                    return OpCode.Compare;
                case PrimaryOpCode.CP_n:
                    timer.MmuByte();
                    operand1 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Compare;
                case PrimaryOpCode.CP_mHL:
                    timer.Index(index.IsDisplaced);
                    operand1 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Compare;

                // INC r
                case PrimaryOpCode.INC_A:
                    operand1 = Operand.A;
                    return OpCode.Increment;
                case PrimaryOpCode.INC_B:
                    operand1 = Operand.B;
                    return OpCode.Increment;
                case PrimaryOpCode.INC_C:
                    operand1 = Operand.C;
                    return OpCode.Increment;
                case PrimaryOpCode.INC_D:
                    operand1 = Operand.D;
                    return OpCode.Increment;
                case PrimaryOpCode.INC_E:
                    operand1 = Operand.E;
                    return OpCode.Increment;
                case PrimaryOpCode.INC_H:
                    operand1 = index.HighRegister;
                    return OpCode.Increment;
                case PrimaryOpCode.INC_L:
                    operand1 = index.LowRegister;
                    return OpCode.Increment;
                case PrimaryOpCode.INC_mHL:
                    timer.Index(index.IsDisplaced).MmuByte().Extend(1);
                    operand1 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Increment;

                // DEC r
                case PrimaryOpCode.DEC_A:
                    operand1 = Operand.A;
                    return OpCode.Decrement;
                case PrimaryOpCode.DEC_B:
                    operand1 = Operand.B;
                    return OpCode.Decrement;
                case PrimaryOpCode.DEC_C:
                    operand1 = Operand.C;
                    return OpCode.Decrement;
                case PrimaryOpCode.DEC_D:
                    operand1 = Operand.D;
                    return OpCode.Decrement;
                case PrimaryOpCode.DEC_E:
                    operand1 = Operand.E;
                    return OpCode.Decrement;
                case PrimaryOpCode.DEC_H:
                    operand1 = index.HighRegister;
                    return OpCode.Decrement;
                case PrimaryOpCode.DEC_L:
                    operand1 = index.LowRegister;
                    return OpCode.Decrement;
                case PrimaryOpCode.DEC_mHL:
                    timer.Index(index.IsDisplaced).MmuByte().Extend(1);
                    operand1 = index.Index;
                    opCodeMeta = OpCodeMeta.Displacement;
                    return OpCode.Decrement;

                // ********* 16-Bit Arithmetic *********
                // ADD HL, ss
                case PrimaryOpCode.ADD_HL_BC:
                    timer.Arithmetic16();
                    operand1 = index.Register;
                    operand2 = Operand.BC;
                    return OpCode.Add16;
                case PrimaryOpCode.ADD_HL_DE:
                    timer.Arithmetic16();
                    operand1 = index.Register;
                    operand2 = Operand.DE;
                    return OpCode.Add16;
                case PrimaryOpCode.ADD_HL_HL:
                    timer.Arithmetic16();
                    operand1 = operand2 = index.Register;
                    return OpCode.Add16;
                case PrimaryOpCode.ADD_HL_SP:
                    timer.Arithmetic16();
                    operand1 = index.Register;
                    operand2 = Operand.SP;
                    return OpCode.Add16;

                // INC ss
                case PrimaryOpCode.INC_BC:
                    timer.Extend(2);
                    operand1 = Operand.BC;
                    return OpCode.Increment16;
                case PrimaryOpCode.INC_DE:
                    timer.Extend(2);
                    operand1 = Operand.DE;
                    return OpCode.Increment16;
                case PrimaryOpCode.INC_HL:
                    timer.Extend(2);
                    operand1 = index.Register;
                    return OpCode.Increment16;
                case PrimaryOpCode.INC_SP:
                    timer.Extend(2);
                    operand1 = Operand.SP;
                    return OpCode.Increment16;

                // DEC ss
                case PrimaryOpCode.DEC_BC:
                    timer.Extend(2);
                    operand1 = Operand.BC;
                    return OpCode.Decrement16;
                case PrimaryOpCode.DEC_DE:
                    timer.Extend(2);
                    operand1 = Operand.DE;
                    return OpCode.Decrement16;
                case PrimaryOpCode.DEC_HL:
                    timer.Extend(2);
                    operand1 = index.Register;
                    return OpCode.Decrement16;
                case PrimaryOpCode.DEC_SP:
                    timer.Extend(2);
                    operand1 = Operand.SP;
                    return OpCode.Decrement16;

                // ********* General-Purpose Arithmetic *********
                // DAA
                case PrimaryOpCode.DAA:
                    return OpCode.DecimalArithmeticAdjust;

                // CPL
                case PrimaryOpCode.CPL:
                    return OpCode.NegateOnesComplement;

                // CCF
                case PrimaryOpCode.CCF:
                    return OpCode.InvertCarryFlag;

                // SCF
                case PrimaryOpCode.SCF:
                    return OpCode.SetCarryFlag;

                // DI
                case PrimaryOpCode.DI:
                    return OpCode.DisableInterrupts;

                // EI
                case PrimaryOpCode.EI:
                    return OpCode.EnableInterrupts;

                // ********* Rotate *********
                // RLCA
                case PrimaryOpCode.RLCA:
                    operand1 = Operand.A;
                    opCodeMeta = OpCodeMeta.UseAlternativeFlagAffection;
                    return OpCode.RotateLeftWithCarry;

                // RLA
                case PrimaryOpCode.RLA:
                    operand1 = Operand.A;
                    opCodeMeta = OpCodeMeta.UseAlternativeFlagAffection;
                    return OpCode.RotateLeft;

                // RRCA
                case PrimaryOpCode.RRCA:
                    operand1 = Operand.A;
                    opCodeMeta = OpCodeMeta.UseAlternativeFlagAffection;
                    return OpCode.RotateRightWithCarry;

                // RRA
                case PrimaryOpCode.RRA:
                    operand1 = Operand.A;
                    opCodeMeta = OpCodeMeta.UseAlternativeFlagAffection;
                    return OpCode.RotateRight;

                // ********* Jump *********
                case PrimaryOpCode.JP:
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    return OpCode.Jump;
                case PrimaryOpCode.JP_NZ:
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.NotZero;
                    return OpCode.Jump;
                case PrimaryOpCode.JP_Z:
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Zero;
                    return OpCode.Jump;
                case PrimaryOpCode.JP_NC:
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.NotCarry;
                    return OpCode.Jump;
                case PrimaryOpCode.JP_C:
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Carry;
                    return OpCode.Jump;
                case PrimaryOpCode.JP_PO:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD (FF00+C), A on GB
                        timer.MmuByte();
                        operand1 = Operand.mCl;
                        operand2 = Operand.A;
                        return OpCode.Load;
                    }
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.ParityOdd;
                    return OpCode.Jump;
                case PrimaryOpCode.JP_PE:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD (nn), A on GB
                        timer.MmuWord().MmuByte();
                        operand1 = Operand.mnn;
                        operand2 = Operand.A;
                        opCodeMeta = OpCodeMeta.WordLiteral;
                        return OpCode.Load;
                    }
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.ParityEven;
                    return OpCode.Jump;
                case PrimaryOpCode.JP_P:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD A, (FF00+C) on GB
                        timer.MmuByte();
                        operand1 = Operand.A;
                        operand2 = Operand.mCl;
                        return OpCode.Load;
                    }
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Possitive;
                    return OpCode.Jump;
                case PrimaryOpCode.JP_M:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD A, (nn) on GB
                        timer.MmuWord().MmuByte();
                        operand1 = Operand.A;
                        operand2 = Operand.mnn;
                        opCodeMeta = OpCodeMeta.WordLiteral;
                        return OpCode.Load;
                    }
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Negative;
                    return OpCode.Jump;
                    
                case PrimaryOpCode.JR:
                    timer.MmuByte().ApplyDisplacement();
                    operand1 = Operand.d;
                    opCodeMeta = OpCodeMeta.ByteLiteral | OpCodeMeta.EndBlock;
                    return OpCode.JumpRelative;
                case PrimaryOpCode.JR_C:
                    timer.MmuByte();
                    operand1 = Operand.d;
                    opCodeMeta = OpCodeMeta.ByteLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Carry;
                    return OpCode.JumpRelative;
                case PrimaryOpCode.JR_NC:
                    timer.MmuByte();
                    operand1 = Operand.d;
                    opCodeMeta = OpCodeMeta.ByteLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.NotCarry;
                    return OpCode.JumpRelative;
                case PrimaryOpCode.JR_Z:
                    timer.MmuByte();
                    operand1 = Operand.d;
                    opCodeMeta = OpCodeMeta.ByteLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Zero;
                    return OpCode.JumpRelative;
                case PrimaryOpCode.JR_NZ:
                    timer.MmuByte();
                    operand1 = Operand.d;
                    opCodeMeta = OpCodeMeta.ByteLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.NotZero;
                    return OpCode.JumpRelative;

                case PrimaryOpCode.JP_mHL:
                    operand1 = index.Register;
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.Jump;

                case PrimaryOpCode.DJNZ:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as STOP on GB
                        opCodeMeta = OpCodeMeta.EndBlock;
                        return OpCode.Stop;
                    }

                    timer.Extend(1).MmuByte();
                    operand1 = Operand.d;
                    opCodeMeta = OpCodeMeta.ByteLiteral | OpCodeMeta.EndBlock;
                    return OpCode.DecrementJumpRelativeIfNonZero;

                // ********* Call *********
                case PrimaryOpCode.CALL:
                    timer.MmuWord().Extend(1).MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    return OpCode.Call;
                case PrimaryOpCode.CALL_NZ:
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.NotZero;
                    return OpCode.Call;
                case PrimaryOpCode.CALL_Z:
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Zero;
                    return OpCode.Call;
                case PrimaryOpCode.CALL_NC:
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.NotCarry;
                    return OpCode.Call;
                case PrimaryOpCode.CALL_C:
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Carry;
                    return OpCode.Call;
                case PrimaryOpCode.CALL_PO:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return undefinedInstruction;
                    }
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.ParityOdd;
                    return OpCode.Call;
                case PrimaryOpCode.CALL_PE:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return undefinedInstruction;
                    }
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.ParityEven;
                    return OpCode.Call;
                case PrimaryOpCode.CALL_P:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return undefinedInstruction;
                    }
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Possitive;
                    return OpCode.Call;
                case PrimaryOpCode.CALL_M:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return undefinedInstruction;
                    }
                    timer.MmuWord();
                    operand1 = Operand.nn;
                    opCodeMeta = OpCodeMeta.WordLiteral | OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Negative;
                    return OpCode.Call;

                // ********* Return *********
                case PrimaryOpCode.RET:
                    timer.MmuWord();
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.Return;
                case PrimaryOpCode.RET_NZ:
                    timer.Extend(1);
                    opCodeMeta = OpCodeMeta.EndBlock;
                    flagTest = FlagTest.NotZero;
                    return OpCode.Return;
                case PrimaryOpCode.RET_Z:
                    timer.Extend(1);
                    opCodeMeta = OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Zero;
                    return OpCode.Return;
                case PrimaryOpCode.RET_NC:
                    timer.Extend(1);
                    opCodeMeta = OpCodeMeta.EndBlock;
                    flagTest = FlagTest.NotCarry;
                    return OpCode.Return;
                case PrimaryOpCode.RET_C:
                    timer.Extend(1);
                    opCodeMeta = OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Carry;
                    return OpCode.Return;
                case PrimaryOpCode.RET_PO:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD (FF00+n), A on GB
                        timer.MmuWord();
                        operand1 = Operand.mnl;
                        operand2 = Operand.A;
                        opCodeMeta = OpCodeMeta.ByteLiteral;
                        return OpCode.Load;
                    }

                    timer.Extend(1);
                    opCodeMeta = OpCodeMeta.EndBlock;
                    flagTest = FlagTest.ParityOdd;
                    return OpCode.Return;
                case PrimaryOpCode.RET_PE:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD SP, SP+d on GB
                        timer.MmuByte().Arithmetic16();
                        operand1 = Operand.SP;
                        operand2 = Operand.SPd;
                        opCodeMeta = OpCodeMeta.ByteLiteral;
                        return OpCode.Load16;
                    }

                    timer.Extend(1);
                    opCodeMeta = OpCodeMeta.EndBlock;
                    flagTest = FlagTest.ParityEven;
                    return OpCode.Return;
                case PrimaryOpCode.RET_P:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD A, (FF00+n) on GB
                        timer.MmuWord();
                        operand1 = Operand.A;
                        operand2 = Operand.mnl;
                        opCodeMeta = OpCodeMeta.ByteLiteral;
                        return OpCode.Load;
                    }

                    timer.Extend(1);
                    opCodeMeta = OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Possitive;
                    return OpCode.Return;
                case PrimaryOpCode.RET_M:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Runs as LD HL, SP+dd on GB
                        timer.Arithmetic16();
                        operand1 = Operand.HL;
                        operand2 = Operand.SPd;
                        opCodeMeta = OpCodeMeta.ByteLiteral;
                        return OpCode.Load16;
                    }

                    timer.Extend(1);
                    opCodeMeta = OpCodeMeta.EndBlock;
                    flagTest = FlagTest.Negative;
                    return OpCode.Return;

                // ********* Reset *********
                case PrimaryOpCode.RST_00:
                    timer.Extend(1).MmuWord();
                    operand1 = Operand.nn;
                    wordLiteral = 0x0000;
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.Reset;
                case PrimaryOpCode.RST_08:
                    timer.Extend(1).MmuWord();
                    operand1 = Operand.nn;
                    wordLiteral = 0x0008;
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.Reset;
                case PrimaryOpCode.RST_10:
                    timer.Extend(1).MmuWord();
                    operand1 = Operand.nn;
                    wordLiteral = 0x0010;
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.Reset;
                case PrimaryOpCode.RST_18:
                    timer.Extend(1).MmuWord();
                    operand1 = Operand.nn;
                    wordLiteral = 0x0018;
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.Reset;
                case PrimaryOpCode.RST_20:
                    timer.Extend(1).MmuWord();
                    operand1 = Operand.nn;
                    wordLiteral = 0x0020;
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.Reset;
                case PrimaryOpCode.RST_28:
                    timer.Extend(1).MmuWord();
                    operand1 = Operand.nn;
                    wordLiteral = 0x0028;
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.Reset;
                case PrimaryOpCode.RST_30:
                    timer.Extend(1).MmuWord();
                    operand1 = Operand.nn;
                    wordLiteral = 0x0030;
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.Reset;
                case PrimaryOpCode.RST_38:
                    timer.Extend(1).MmuWord();
                    operand1 = Operand.nn;
                    wordLiteral = 0x0038;
                    opCodeMeta = OpCodeMeta.EndBlock;
                    return OpCode.Reset;

                // ********* IO *********
                case PrimaryOpCode.IN_A_n:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return undefinedInstruction;
                    }
                    timer.MmuByte().Io();
                    operand1 = Operand.A;
                    operand2 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Input;
                case PrimaryOpCode.OUT_A_n:
                    if (this.cpuMode == CpuMode.GameBoy)
                    {
                        // Instruction not on GBCPU
                        return undefinedInstruction;
                    }
                    timer.MmuByte().Io();
                    operand1 = Operand.A;
                    operand2 = Operand.n;
                    opCodeMeta = OpCodeMeta.ByteLiteral;
                    return OpCode.Output;
                default:
                    throw new ArgumentOutOfRangeException(nameof(code), code, null);
            }
        }
    }
}
