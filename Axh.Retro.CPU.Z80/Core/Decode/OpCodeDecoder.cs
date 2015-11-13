namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System;
    using System.Collections.Generic;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;

    internal class OpCodeDecoder
    {
        private readonly IDictionary<IndexRegister, IndexRegisterOperands> indexRegisterOperands;

        private readonly CpuMode cpuMode;

        public OpCodeDecoder(CpuMode cpuMode)
        {
            this.cpuMode = cpuMode;
            this.indexRegisterOperands = new Dictionary<IndexRegister, IndexRegisterOperands> { { IndexRegister.HL, new IndexRegisterOperands(Operand.HL, Operand.mHL, Operand.L, Operand.H, false) } };

            if (cpuMode == CpuMode.Z80)
            {
                indexRegisterOperands.Add(IndexRegister.IX, new IndexRegisterOperands(Operand.IX, Operand.mIXd, Operand.IXl, Operand.IXh, true));
                indexRegisterOperands.Add(IndexRegister.IY, new IndexRegisterOperands(Operand.IY, Operand.mIYd, Operand.IYl, Operand.IYh, true));
            }
        }

        private struct IndexRegisterOperands
        {
            public IndexRegisterOperands(Operand register, Operand index, Operand lowRegister, Operand highRegister, bool isDisplaced) : this()
            {
                Register = register;
                Index = index;
                LowRegister = lowRegister;
                HighRegister = highRegister;
                IsDisplaced = isDisplaced;
            }

            public Operand Register { get; }

            public Operand Index { get; }

            public Operand LowRegister { get; }

            public Operand HighRegister { get; }

            public bool IsDisplaced { get; }
        }


        public IEnumerable<DecodeResult> GetDecodeResult(IInstructionTimingsBuilder timer, IPrefetchQueue prefetchQueue)
        {
            var index = indexRegisterOperands[IndexRegister.HL];

            while (true)
            {
                var code = (PrimaryOpCode)prefetchQueue.NextByte();
                
                // Add a NOP on every frame. Reduce timings by a NOP elsewhere.
                timer.Nop();

                switch (code)
                {
                    case PrimaryOpCode.NOP:
                        yield return new DecodeResult(Opcode.NOP);
                        break;
                    case PrimaryOpCode.HALT:
                        yield return new DecodeResult(Opcode.HALT);
                        yield break;
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
                        yield return new DecodeResult(Opcode.LD, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.LD_B_A:
                        yield return new DecodeResult(Opcode.LD, Operand.B, Operand.A);
                        break;
                    case PrimaryOpCode.LD_C_A:
                        yield return new DecodeResult(Opcode.LD, Operand.C, Operand.A);
                        break;
                    case PrimaryOpCode.LD_D_A:
                        yield return new DecodeResult(Opcode.LD, Operand.D, Operand.A);
                        break;
                    case PrimaryOpCode.LD_E_A:
                        yield return new DecodeResult(Opcode.LD, Operand.E, Operand.A);
                        break;
                    case PrimaryOpCode.LD_H_A:
                        yield return new DecodeResult(Opcode.LD, index.HighRegister, Operand.A);
                        break;
                    case PrimaryOpCode.LD_L_A:
                        yield return new DecodeResult(Opcode.LD, index.LowRegister, Operand.A);
                        break;
                    case PrimaryOpCode.LD_A_B:
                        yield return new DecodeResult(Opcode.LD, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.LD_B_B:
                        yield return new DecodeResult(Opcode.LD, Operand.B, Operand.B);
                        break;
                    case PrimaryOpCode.LD_C_B:
                        yield return new DecodeResult(Opcode.LD, Operand.C, Operand.B);
                        break;
                    case PrimaryOpCode.LD_D_B:
                        yield return new DecodeResult(Opcode.LD, Operand.D, Operand.B);
                        break;
                    case PrimaryOpCode.LD_E_B:
                        yield return new DecodeResult(Opcode.LD, Operand.E, Operand.B);
                        break;
                    case PrimaryOpCode.LD_H_B:
                        yield return new DecodeResult(Opcode.LD, index.HighRegister, Operand.B);
                        break;
                    case PrimaryOpCode.LD_L_B:
                        yield return new DecodeResult(Opcode.LD, index.LowRegister, Operand.B);
                        break;
                    case PrimaryOpCode.LD_A_C:
                        yield return new DecodeResult(Opcode.LD, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.LD_B_C:
                        yield return new DecodeResult(Opcode.LD, Operand.B, Operand.C);
                        break;
                    case PrimaryOpCode.LD_C_C:
                        yield return new DecodeResult(Opcode.LD, Operand.C, Operand.C);
                        break;
                    case PrimaryOpCode.LD_D_C:
                        yield return new DecodeResult(Opcode.LD, Operand.D, Operand.C);
                        break;
                    case PrimaryOpCode.LD_E_C:
                        yield return new DecodeResult(Opcode.LD, Operand.E, Operand.C);
                        break;
                    case PrimaryOpCode.LD_H_C:
                        yield return new DecodeResult(Opcode.LD, index.HighRegister, Operand.C);
                        break;
                    case PrimaryOpCode.LD_L_C:
                        yield return new DecodeResult(Opcode.LD, index.LowRegister, Operand.C);
                        break;
                    case PrimaryOpCode.LD_A_D:
                        yield return new DecodeResult(Opcode.LD, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.LD_B_D:
                        yield return new DecodeResult(Opcode.LD, Operand.B, Operand.D);
                        break;
                    case PrimaryOpCode.LD_C_D:
                        yield return new DecodeResult(Opcode.LD, Operand.C, Operand.D);
                        break;
                    case PrimaryOpCode.LD_D_D:
                        yield return new DecodeResult(Opcode.LD, Operand.D, Operand.D);
                        break;
                    case PrimaryOpCode.LD_E_D:
                        yield return new DecodeResult(Opcode.LD, Operand.E, Operand.D);
                        break;
                    case PrimaryOpCode.LD_H_D:
                        yield return new DecodeResult(Opcode.LD, index.HighRegister, Operand.D);
                        break;
                    case PrimaryOpCode.LD_L_D:
                        yield return new DecodeResult(Opcode.LD, index.LowRegister, Operand.D);
                        break;
                    case PrimaryOpCode.LD_A_E:
                        yield return new DecodeResult(Opcode.LD, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.LD_B_E:
                        yield return new DecodeResult(Opcode.LD, Operand.B, Operand.E);
                        break;
                    case PrimaryOpCode.LD_C_E:
                        yield return new DecodeResult(Opcode.LD, Operand.C, Operand.E);
                        break;
                    case PrimaryOpCode.LD_D_E:
                        yield return new DecodeResult(Opcode.LD, Operand.D, Operand.E);
                        break;
                    case PrimaryOpCode.LD_E_E:
                        yield return new DecodeResult(Opcode.LD, Operand.E, Operand.E);
                        break;
                    case PrimaryOpCode.LD_H_E:
                        yield return new DecodeResult(Opcode.LD, index.HighRegister, Operand.E);
                        break;
                    case PrimaryOpCode.LD_L_E:
                        yield return new DecodeResult(Opcode.LD, index.LowRegister, Operand.E);
                        break;
                    case PrimaryOpCode.LD_A_H:
                        yield return new DecodeResult(Opcode.LD, Operand.A, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_B_H:
                        yield return new DecodeResult(Opcode.LD, Operand.B, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_C_H:
                        yield return new DecodeResult(Opcode.LD, Operand.C, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_D_H:
                        yield return new DecodeResult(Opcode.LD, Operand.D, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_E_H:
                        yield return new DecodeResult(Opcode.LD, Operand.E, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_H_H:
                        yield return new DecodeResult(Opcode.LD, index.HighRegister, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_L_H:
                        yield return new DecodeResult(Opcode.LD, index.LowRegister, index.HighRegister);
                        break;
                    case PrimaryOpCode.LD_A_L:
                        yield return new DecodeResult(Opcode.LD, Operand.A, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_B_L:
                        yield return new DecodeResult(Opcode.LD, Operand.B, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_C_L:
                        yield return new DecodeResult(Opcode.LD, Operand.C, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_D_L:
                        yield return new DecodeResult(Opcode.LD, Operand.D, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_E_L:
                        yield return new DecodeResult(Opcode.LD, Operand.E, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_H_L:
                        yield return new DecodeResult(Opcode.LD, index.HighRegister, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_L_L:
                        yield return new DecodeResult(Opcode.LD, index.LowRegister, index.LowRegister);
                        break;
                    case PrimaryOpCode.LD_A_n:
                        timer.Prefetch();
                        yield return new DecodeResult(Opcode.LD, Operand.A, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_B_n:
                        timer.Prefetch();
                        yield return new DecodeResult(Opcode.LD, Operand.B, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_C_n:
                        timer.Prefetch();
                        yield return new DecodeResult(Opcode.LD, Operand.C, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_D_n:
                        timer.Prefetch();
                        yield return new DecodeResult(Opcode.LD, Operand.D, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_E_n:
                        timer.Prefetch();
                        yield return new DecodeResult(Opcode.LD, Operand.E, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_H_n:
                        timer.Prefetch();
                        yield return new DecodeResult(Opcode.LD, index.HighRegister, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_L_n:
                        timer.Prefetch();
                        yield return new DecodeResult(Opcode.LD, index.LowRegister, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;

                    // LD r, (HL)
                    case PrimaryOpCode.LD_A_mHL:
                        timer.Index(index.IsDisplaced);
                        yield return new DecodeResult(Opcode.LD, Operand.A, index.Index).AddDisplacement(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_B_mHL:
                        timer.Index(index.IsDisplaced);
                        yield return new DecodeResult(Opcode.LD, Operand.B, index.Index).AddDisplacement(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_C_mHL:
                        timer.Index(index.IsDisplaced);
                        yield return new DecodeResult(Opcode.LD, Operand.C, index.Index).AddDisplacement(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_D_mHL:
                        timer.Index(index.IsDisplaced);
                        yield return new DecodeResult(Opcode.LD, Operand.D, index.Index).AddDisplacement(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_E_mHL:
                        timer.Index(index.IsDisplaced);
                        yield return new DecodeResult(Opcode.LD, Operand.E, index.Index).AddDisplacement(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_H_mHL:
                        timer.Index(index.IsDisplaced);
                        // H register is always assigned here
                        yield return new DecodeResult(Opcode.LD, Operand.H, index.Index).AddDisplacement(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_L_mHL:
                        timer.Index(index.IsDisplaced);
                        // L register is always assigned here
                        yield return new DecodeResult(Opcode.LD, Operand.L, index.Index).AddDisplacement(prefetchQueue.NextByte());
                        break;

                    // LD (HL), r
                    case PrimaryOpCode.LD_mHL_A:
                        timer.Index(index.IsDisplaced);
                        yield return new DecodeResult(Opcode.LD, index.Index, Operand.A).AddDisplacement(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_mHL_B:
                        timer.Index(index.IsDisplaced);
                        yield return new DecodeResult(Opcode.LD, index.Index, Operand.B).AddDisplacement(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_mHL_C:
                        timer.Index(index.IsDisplaced);
                        yield return new DecodeResult(Opcode.LD, index.Index, Operand.C).AddDisplacement(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_mHL_D:
                        timer.Index(index.IsDisplaced);
                        yield return new DecodeResult(Opcode.LD, index.Index, Operand.D).AddDisplacement(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_mHL_E:
                        timer.Index(index.IsDisplaced);
                        yield return new DecodeResult(Opcode.LD, index.Index, Operand.E).AddDisplacement(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_mHL_H:
                        timer.Index(index.IsDisplaced);
                        // Value of H register is always used here
                        yield return new DecodeResult(Opcode.LD, index.Index, Operand.H).AddDisplacement(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.LD_mHL_L:
                        timer.Index(index.IsDisplaced);
                        // Value of L register is always used here
                        yield return new DecodeResult(Opcode.LD, index.Index, Operand.L).AddDisplacement(prefetchQueue.NextByte());
                        break;
                        
                    // LD (HL), n
                    case PrimaryOpCode.LD_mHL_n:
                        timer.IndexAndPrefetch(index.IsDisplaced);
                        yield return new DecodeResult(Opcode.LD, index.Index, Operand.n).AddDisplacement(prefetchQueue.NextByte()).AddLiteral(prefetchQueue.NextByte());
                        break;
                        
                    // LD A, (BC)
                    case PrimaryOpCode.LD_A_mBC:
                        timer.Index(false);
                        yield return new DecodeResult(Opcode.LD, Operand.A, Operand.mBC);
                        break;

                    // LD A, (BC)
                    case PrimaryOpCode.LD_A_mDE:
                        timer.Index(false);
                        yield return new DecodeResult(Opcode.LD, Operand.A, Operand.mDE);
                        break;

                    // LD A, (nn)
                    case PrimaryOpCode.LD_A_mnn:
                        timer.Index(false).Prefetch().Prefetch();
                        yield return new DecodeResult(Opcode.LD, Operand.A, Operand.mnn).AddLiteral(prefetchQueue.NextWord());
                        break;


                    case PrimaryOpCode.LD_mBC_A:
                        yield return new DecodeResult(Opcode.LD, Operand.mBC, Operand.A);
                        break;
                    case PrimaryOpCode.LD_mDE_A:
                        yield return new DecodeResult(Opcode.LD, Operand.mDE, Operand.A);
                        break;
                    case PrimaryOpCode.LD_mnn_A:
                        yield return new DecodeResult(Opcode.LD, Operand.mnn, Operand.A).AddLiteral(prefetchQueue.NextWord());
                        break;

                    case PrimaryOpCode.LD_BC_nn:
                        yield return new DecodeResult(Opcode.LD16, Operand.BC, Operand.nn).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.LD_DE_nn:
                        yield return new DecodeResult(Opcode.LD16, Operand.DE, Operand.nn).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.LD_HL_nn:
                        yield return new DecodeResult(Opcode.LD16, Operand.HL, Operand.nn).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.LD_SP_nn:
                        yield return new DecodeResult(Opcode.LD16, Operand.SP, Operand.nn).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.LD_HL_mnn:
                        yield return new DecodeResult(Opcode.LD16, Operand.HL, Operand.mnn).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.LD_mnn_HL:
                        yield return new DecodeResult(Opcode.LD16, Operand.mnn, Operand.HL).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.LD_SP_HL:
                        yield return new DecodeResult(Opcode.LD16, Operand.SP, Operand.HL);
                        break;

                    case PrimaryOpCode.PUSH_BC:
                        yield return new DecodeResult(Opcode.PUSH, Operand.BC);
                        break;
                    case PrimaryOpCode.PUSH_DE:
                        yield return new DecodeResult(Opcode.PUSH, Operand.DE);
                        break;
                    case PrimaryOpCode.PUSH_HL:
                        yield return new DecodeResult(Opcode.PUSH, Operand.HL);
                        break;
                    case PrimaryOpCode.PUSH_AF:
                        yield return new DecodeResult(Opcode.PUSH, Operand.AF);
                        break;

                    case PrimaryOpCode.POP_BC:
                        yield return new DecodeResult(Opcode.POP, Operand.BC);
                        break;
                    case PrimaryOpCode.POP_DE:
                        yield return new DecodeResult(Opcode.POP, Operand.DE);
                        break;
                    case PrimaryOpCode.POP_HL:
                        yield return new DecodeResult(Opcode.POP, Operand.HL);
                        break;
                    case PrimaryOpCode.POP_AF:
                        yield return new DecodeResult(Opcode.POP, Operand.AF);
                        break;

                    case PrimaryOpCode.ADD_A_A:
                        yield return new DecodeResult(Opcode.ADD, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.ADD_A_B:
                        yield return new DecodeResult(Opcode.ADD, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.ADD_A_C:
                        yield return new DecodeResult(Opcode.ADD, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.ADD_A_D:
                        yield return new DecodeResult(Opcode.ADD, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.ADD_A_E:
                        yield return new DecodeResult(Opcode.ADD, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.ADD_A_H:
                        yield return new DecodeResult(Opcode.ADD, Operand.A, Operand.H);
                        break;
                    case PrimaryOpCode.ADD_A_L:
                        yield return new DecodeResult(Opcode.ADD, Operand.A, Operand.L);
                        break;
                    case PrimaryOpCode.ADD_A_n:
                        yield return new DecodeResult(Opcode.ADD, Operand.A, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.ADD_A_mHL:
                        yield return new DecodeResult(Opcode.ADD, Operand.A, Operand.mHL);
                        break;

                    case PrimaryOpCode.ADC_A_A:
                        yield return new DecodeResult(Opcode.ADC, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.ADC_A_B:
                        yield return new DecodeResult(Opcode.ADC, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.ADC_A_C:
                        yield return new DecodeResult(Opcode.ADC, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.ADC_A_D:
                        yield return new DecodeResult(Opcode.ADC, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.ADC_A_E:
                        yield return new DecodeResult(Opcode.ADC, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.ADC_A_H:
                        yield return new DecodeResult(Opcode.ADC, Operand.A, Operand.H);
                        break;
                    case PrimaryOpCode.ADC_A_L:
                        yield return new DecodeResult(Opcode.ADC, Operand.A, Operand.L);
                        break;
                    case PrimaryOpCode.ADC_A_n:
                        yield return new DecodeResult(Opcode.ADC, Operand.A, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.ADC_A_mHL:
                        yield return new DecodeResult(Opcode.ADC, Operand.A, Operand.mHL);
                        break;

                    case PrimaryOpCode.SUB_A_A:
                        yield return new DecodeResult(Opcode.SUB, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.SUB_A_B:
                        yield return new DecodeResult(Opcode.SUB, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.SUB_A_C:
                        yield return new DecodeResult(Opcode.SUB, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.SUB_A_D:
                        yield return new DecodeResult(Opcode.SUB, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.SUB_A_E:
                        yield return new DecodeResult(Opcode.SUB, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.SUB_A_H:
                        yield return new DecodeResult(Opcode.SUB, Operand.A, Operand.H);
                        break;
                    case PrimaryOpCode.SUB_A_L:
                        yield return new DecodeResult(Opcode.SUB, Operand.A, Operand.L);
                        break;
                    case PrimaryOpCode.SUB_A_n:
                        yield return new DecodeResult(Opcode.SUB, Operand.A, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.SUB_A_mHL:
                        yield return new DecodeResult(Opcode.SUB, Operand.A, Operand.mHL);
                        break;

                    case PrimaryOpCode.SBC_A_A:
                        yield return new DecodeResult(Opcode.SBC, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.SBC_A_B:
                        yield return new DecodeResult(Opcode.SBC, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.SBC_A_C:
                        yield return new DecodeResult(Opcode.SBC, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.SBC_A_D:
                        yield return new DecodeResult(Opcode.SBC, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.SBC_A_E:
                        yield return new DecodeResult(Opcode.SBC, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.SBC_A_H:
                        yield return new DecodeResult(Opcode.SBC, Operand.A, Operand.H);
                        break;
                    case PrimaryOpCode.SBC_A_L:
                        yield return new DecodeResult(Opcode.SBC, Operand.A, Operand.L);
                        break;
                    case PrimaryOpCode.SBC_A_n:
                        yield return new DecodeResult(Opcode.SBC, Operand.A, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.SBC_A_mHL:
                        yield return new DecodeResult(Opcode.SBC, Operand.A, Operand.mHL);
                        break;

                    case PrimaryOpCode.AND_A:
                        yield return new DecodeResult(Opcode.AND, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.AND_B:
                        yield return new DecodeResult(Opcode.AND, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.AND_C:
                        yield return new DecodeResult(Opcode.AND, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.AND_D:
                        yield return new DecodeResult(Opcode.AND, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.AND_E:
                        yield return new DecodeResult(Opcode.AND, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.AND_H:
                        yield return new DecodeResult(Opcode.AND, Operand.A, Operand.H);
                        break;
                    case PrimaryOpCode.AND_L:
                        yield return new DecodeResult(Opcode.AND, Operand.A, Operand.L);
                        break;
                    case PrimaryOpCode.AND_n:
                        yield return new DecodeResult(Opcode.AND, Operand.A, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.AND_mHL:
                        yield return new DecodeResult(Opcode.AND, Operand.A, Operand.mHL);
                        break;

                    case PrimaryOpCode.OR_A:
                        yield return new DecodeResult(Opcode.OR, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.OR_B:
                        yield return new DecodeResult(Opcode.OR, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.OR_C:
                        yield return new DecodeResult(Opcode.OR, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.OR_D:
                        yield return new DecodeResult(Opcode.OR, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.OR_E:
                        yield return new DecodeResult(Opcode.OR, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.OR_H:
                        yield return new DecodeResult(Opcode.OR, Operand.A, Operand.H);
                        break;
                    case PrimaryOpCode.OR_L:
                        yield return new DecodeResult(Opcode.OR, Operand.A, Operand.L);
                        break;
                    case PrimaryOpCode.OR_n:
                        yield return new DecodeResult(Opcode.OR, Operand.A, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.OR_mHL:
                        yield return new DecodeResult(Opcode.OR, Operand.A, Operand.mHL);
                        break;

                    case PrimaryOpCode.XOR_A:
                        yield return new DecodeResult(Opcode.XOR, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.XOR_B:
                        yield return new DecodeResult(Opcode.XOR, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.XOR_C:
                        yield return new DecodeResult(Opcode.XOR, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.XOR_D:
                        yield return new DecodeResult(Opcode.XOR, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.XOR_E:
                        yield return new DecodeResult(Opcode.XOR, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.XOR_H:
                        yield return new DecodeResult(Opcode.XOR, Operand.A, Operand.H);
                        break;
                    case PrimaryOpCode.XOR_L:
                        yield return new DecodeResult(Opcode.XOR, Operand.A, Operand.L);
                        break;
                    case PrimaryOpCode.XOR_n:
                        yield return new DecodeResult(Opcode.XOR, Operand.A, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.XOR_mHL:
                        yield return new DecodeResult(Opcode.XOR, Operand.A, Operand.mHL);
                        break;

                    case PrimaryOpCode.CP_A:
                        yield return new DecodeResult(Opcode.CP, Operand.A, Operand.A);
                        break;
                    case PrimaryOpCode.CP_B:
                        yield return new DecodeResult(Opcode.CP, Operand.A, Operand.B);
                        break;
                    case PrimaryOpCode.CP_C:
                        yield return new DecodeResult(Opcode.CP, Operand.A, Operand.C);
                        break;
                    case PrimaryOpCode.CP_D:
                        yield return new DecodeResult(Opcode.CP, Operand.A, Operand.D);
                        break;
                    case PrimaryOpCode.CP_E:
                        yield return new DecodeResult(Opcode.CP, Operand.A, Operand.E);
                        break;
                    case PrimaryOpCode.CP_H:
                        yield return new DecodeResult(Opcode.CP, Operand.A, Operand.H);
                        break;
                    case PrimaryOpCode.CP_L:
                        yield return new DecodeResult(Opcode.CP, Operand.A, Operand.L);
                        break;
                    case PrimaryOpCode.CP_n:
                        yield return new DecodeResult(Opcode.CP, Operand.A, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.CP_mHL:
                        yield return new DecodeResult(Opcode.CP, Operand.A, Operand.mHL);
                        break;

                    case PrimaryOpCode.INC_A:
                        yield return new DecodeResult(Opcode.INC, Operand.A);
                        break;
                    case PrimaryOpCode.INC_B:
                        yield return new DecodeResult(Opcode.INC, Operand.B);
                        break;
                    case PrimaryOpCode.INC_C:
                        yield return new DecodeResult(Opcode.INC, Operand.C);
                        break;
                    case PrimaryOpCode.INC_D:
                        yield return new DecodeResult(Opcode.INC, Operand.D);
                        break;
                    case PrimaryOpCode.INC_E:
                        yield return new DecodeResult(Opcode.INC, Operand.E);
                        break;
                    case PrimaryOpCode.INC_H:
                        yield return new DecodeResult(Opcode.INC, Operand.H);
                        break;
                    case PrimaryOpCode.INC_L:
                        yield return new DecodeResult(Opcode.INC, Operand.L);
                        break;
                    case PrimaryOpCode.INC_mHL:
                        yield return new DecodeResult(Opcode.INC, Operand.mHL);
                        break;

                    case PrimaryOpCode.DEC_A:
                        yield return new DecodeResult(Opcode.DEC, Operand.A);
                        break;
                    case PrimaryOpCode.DEC_B:
                        yield return new DecodeResult(Opcode.DEC, Operand.B);
                        break;
                    case PrimaryOpCode.DEC_C:
                        yield return new DecodeResult(Opcode.DEC, Operand.C);
                        break;
                    case PrimaryOpCode.DEC_D:
                        yield return new DecodeResult(Opcode.DEC, Operand.D);
                        break;
                    case PrimaryOpCode.DEC_E:
                        yield return new DecodeResult(Opcode.DEC, Operand.E);
                        break;
                    case PrimaryOpCode.DEC_H:
                        yield return new DecodeResult(Opcode.DEC, Operand.H);
                        break;
                    case PrimaryOpCode.DEC_L:
                        yield return new DecodeResult(Opcode.DEC, Operand.L);
                        break;
                    case PrimaryOpCode.DEC_mHL:
                        yield return new DecodeResult(Opcode.DEC, Operand.mHL);
                        break;

                    case PrimaryOpCode.ADD_HL_BC:
                        yield return new DecodeResult(Opcode.ADD16, Operand.HL, Operand.BC);
                        break;
                    case PrimaryOpCode.ADD_HL_DE:
                        yield return new DecodeResult(Opcode.ADD16, Operand.HL, Operand.DE);
                        break;
                    case PrimaryOpCode.ADD_HL_HL:
                        yield return new DecodeResult(Opcode.ADD16, Operand.HL, Operand.HL);
                        break;
                    case PrimaryOpCode.ADD_HL_SP:
                        yield return new DecodeResult(Opcode.ADD16, Operand.HL, Operand.SP);
                        break;

                    case PrimaryOpCode.INC_BC:
                        yield return new DecodeResult(Opcode.INC16, Operand.BC);
                        break;
                    case PrimaryOpCode.INC_DE:
                        yield return new DecodeResult(Opcode.INC16, Operand.DE);
                        break;
                    case PrimaryOpCode.INC_HL:
                        yield return new DecodeResult(Opcode.INC16, Operand.HL);
                        break;
                    case PrimaryOpCode.INC_SP:
                        yield return new DecodeResult(Opcode.INC16, Operand.SP);
                        break;

                    case PrimaryOpCode.DEC_BC:
                        yield return new DecodeResult(Opcode.DEC16, Operand.BC);
                        break;
                    case PrimaryOpCode.DEC_DE:
                        yield return new DecodeResult(Opcode.DEC16, Operand.DE);
                        break;
                    case PrimaryOpCode.DEC_HL:
                        yield return new DecodeResult(Opcode.DEC16, Operand.HL);
                        break;
                    case PrimaryOpCode.DEC_SP:
                        yield return new DecodeResult(Opcode.DEC16, Operand.SP);
                        break;

                    case PrimaryOpCode.EX_DE_HL:
                        yield return new DecodeResult(Opcode.EX, Operand.DE, Operand.HL);
                        break;
                    case PrimaryOpCode.EX_AF:
                        yield return new DecodeResult(Opcode.EX, Operand.AF);
                        break;
                    case PrimaryOpCode.EXX:
                        yield return new DecodeResult(Opcode.EXX);
                        break;
                    case PrimaryOpCode.EX_mSP_HL:
                        yield return new DecodeResult(Opcode.EX, Operand.mSP, Operand.HL);
                        break;

                    case PrimaryOpCode.DAA:
                        yield return new DecodeResult(Opcode.DAA);
                        break;
                    case PrimaryOpCode.CPL:
                        yield return new DecodeResult(Opcode.CPL);
                        break;
                    case PrimaryOpCode.CCF:
                        yield return new DecodeResult(Opcode.CCF);
                        break;
                    case PrimaryOpCode.SCF:
                        yield return new DecodeResult(Opcode.SCF);
                        break;
                    case PrimaryOpCode.DI:
                        yield return new DecodeResult(Opcode.DI);
                        break;
                    case PrimaryOpCode.EI:
                        yield return new DecodeResult(Opcode.EI);
                        break;

                    case PrimaryOpCode.RLCA:
                        yield return new DecodeResult(Opcode.RLCA);
                        break;
                    case PrimaryOpCode.RLA:
                        yield return new DecodeResult(Opcode.RLA);
                        break;
                    case PrimaryOpCode.RRCA:
                        yield return new DecodeResult(Opcode.RRCA);
                        break;
                    case PrimaryOpCode.RRA:
                        yield return new DecodeResult(Opcode.RRA);
                        break;

                    case PrimaryOpCode.JP:
                        yield return new DecodeResult(Opcode.JP, Operand.nn).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.JP_NZ:
                        yield return new DecodeResult(Opcode.JP, Operand.nn).AddFlagTest(FlagTest.NZ).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.JP_Z:
                        yield return new DecodeResult(Opcode.JP, Operand.nn).AddFlagTest(FlagTest.Z).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.JP_NC:
                        yield return new DecodeResult(Opcode.JP, Operand.nn).AddFlagTest(FlagTest.NC).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.JP_C:
                        yield return new DecodeResult(Opcode.JP, Operand.nn).AddFlagTest(FlagTest.C).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.JP_PO:
                        yield return new DecodeResult(Opcode.JP, Operand.nn).AddFlagTest(FlagTest.PO).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.JP_PE:
                        yield return new DecodeResult(Opcode.JP, Operand.nn).AddFlagTest(FlagTest.PE).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.JP_P:
                        yield return new DecodeResult(Opcode.JP, Operand.nn).AddFlagTest(FlagTest.P).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.JP_M:
                        yield return new DecodeResult(Opcode.JP, Operand.nn).AddFlagTest(FlagTest.M).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.JR:
                        yield return new DecodeResult(Opcode.JR, Operand.d).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.JR_C:
                        yield return new DecodeResult(Opcode.JR, Operand.d).AddFlagTest(FlagTest.C).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.JR_NC:
                        yield return new DecodeResult(Opcode.JR, Operand.d).AddFlagTest(FlagTest.NC).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.JR_Z:
                        yield return new DecodeResult(Opcode.JR, Operand.d).AddFlagTest(FlagTest.Z).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.JR_NZ:
                        yield return new DecodeResult(Opcode.JR, Operand.d).AddFlagTest(FlagTest.NZ).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.JP_mHL:
                        yield return new DecodeResult(Opcode.JP, Operand.HL);
                        break;

                    case PrimaryOpCode.DJNZ:
                        yield return new DecodeResult(Opcode.DJNZ, Operand.d).AddLiteral(prefetchQueue.NextByte());
                        break;

                    case PrimaryOpCode.CALL:
                        yield return new DecodeResult(Opcode.CALL, Operand.nn).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.CALL_NZ:
                        yield return new DecodeResult(Opcode.CALL, Operand.nn).AddFlagTest(FlagTest.NZ).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.CALL_Z:
                        yield return new DecodeResult(Opcode.CALL, Operand.nn).AddFlagTest(FlagTest.Z).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.CALL_NC:
                        yield return new DecodeResult(Opcode.CALL, Operand.nn).AddFlagTest(FlagTest.NC).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.CALL_C:
                        yield return new DecodeResult(Opcode.CALL, Operand.nn).AddFlagTest(FlagTest.C).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.CALL_PO:
                        yield return new DecodeResult(Opcode.CALL, Operand.nn).AddFlagTest(FlagTest.PO).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.CALL_PE:
                        yield return new DecodeResult(Opcode.CALL, Operand.nn).AddFlagTest(FlagTest.PE).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.CALL_P:
                        yield return new DecodeResult(Opcode.CALL, Operand.nn).AddFlagTest(FlagTest.P).AddLiteral(prefetchQueue.NextWord());
                        break;
                    case PrimaryOpCode.CALL_M:
                        yield return new DecodeResult(Opcode.CALL, Operand.nn).AddFlagTest(FlagTest.M).AddLiteral(prefetchQueue.NextWord());
                        break;

                    case PrimaryOpCode.RET:
                        yield return new DecodeResult(Opcode.RET);
                        break;
                    case PrimaryOpCode.RET_NZ:
                        yield return new DecodeResult(Opcode.RET).AddFlagTest(FlagTest.NZ);
                        break;
                    case PrimaryOpCode.RET_Z:
                        yield return new DecodeResult(Opcode.RET).AddFlagTest(FlagTest.Z);
                        break;
                    case PrimaryOpCode.RET_NC:
                        yield return new DecodeResult(Opcode.RET).AddFlagTest(FlagTest.NC);
                        break;
                    case PrimaryOpCode.RET_C:
                        yield return new DecodeResult(Opcode.RET).AddFlagTest(FlagTest.C);
                        break;
                    case PrimaryOpCode.RET_PO:
                        yield return new DecodeResult(Opcode.RET).AddFlagTest(FlagTest.PO);
                        break;
                    case PrimaryOpCode.RET_PE:
                        yield return new DecodeResult(Opcode.RET).AddFlagTest(FlagTest.PE);
                        break;
                    case PrimaryOpCode.RET_P:
                        yield return new DecodeResult(Opcode.RET).AddFlagTest(FlagTest.P);
                        break;
                    case PrimaryOpCode.RET_M:
                        yield return new DecodeResult(Opcode.RET).AddFlagTest(FlagTest.M);
                        break;

                    case PrimaryOpCode.RST_00:
                        yield return new DecodeResult(Opcode.RET, Operand.nn).AddLiteral((ushort)0x0000);
                        break;
                    case PrimaryOpCode.RST_08:
                        yield return new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0008);
                        break;
                    case PrimaryOpCode.RST_10:
                        yield return new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0010);
                        break;
                    case PrimaryOpCode.RST_18:
                        yield return new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0018);
                        break;
                    case PrimaryOpCode.RST_20:
                        yield return new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0020);
                        break;
                    case PrimaryOpCode.RST_28:
                        yield return new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0028);
                        break;
                    case PrimaryOpCode.RST_30:
                        yield return new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0030);
                        break;
                    case PrimaryOpCode.RST_38:
                        yield return new DecodeResult(Opcode.RET, Operand.nn).AddLiteral(0x0038);
                        break;

                    case PrimaryOpCode.IN_A_n:
                        yield return new DecodeResult(Opcode.IN, Operand.A, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    case PrimaryOpCode.OUT_A_n:
                        yield return new DecodeResult(Opcode.OUT, Operand.A, Operand.n).AddLiteral(prefetchQueue.NextByte());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(code), code, null);
                }

                index = this.indexRegisterOperands[IndexRegister.HL];
            }
            
        }
    }
}
