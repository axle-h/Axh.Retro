namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Util;

    internal partial class DynaRecBlockBuilder<TRegisters> where TRegisters : IRegisters
    {
        private IEnumerable<Expression> GetBlockExpressions()
        {
            lastDecodeResult = DecodeResult.Continue;
            index = DynaRecExpressions.IndexRegisterExpressions[IndexRegister.HL];

            while (true)
            {
                if (mmuCache.TotalBytesRead == ushort.MaxValue)
                {
                    lastDecodeResult = DecodeResult.FinalizeAndSync;
                    yield break;
                }

                var opCode = (PrimaryOpCode)this.mmuCache.NextByte();

                if (index.UsesDisplacedIndexTimings && OpCodeUsesDisplacedIndex(opCode))
                {
                    // Read the displacement as the next byte
                    yield return Expression.Assign(DynaRecExpressions.LocalByte, NextByte);
                }

                switch (opCode)
                {
                    case PrimaryOpCode.NOP:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.HALT:
                        timer.Add(1, 4);
                        lastDecodeResult = DecodeResult.FinalizeAndSync;
                        yield break;

                    // ********* Prefixes *********
                    case PrimaryOpCode.Prefix_DD:
                        // Add a NOP timing for now and take a NOP off known indexed timings later.
                        timer.Add(1, 4);
                        if (cpuMode == CpuMode.Z80)
                        {
                            // Only Z80 has prefix DD
                            this.index = DynaRecExpressions.IndexRegisterExpressions[IndexRegister.IX];
                        }
                        continue;

                    case PrimaryOpCode.Prefix_FD:
                        // Add a NOP timing for now and take a NOP off known indexed timings later.
                        timer.Add(1, 4);
                        if (cpuMode == CpuMode.Z80)
                        {
                            // Only Z80 has prefix FD
                            this.index = DynaRecExpressions.IndexRegisterExpressions[IndexRegister.IY];
                        }
                        continue;

                    case PrimaryOpCode.Prefix_ED:
                        if (cpuMode == CpuMode.Z80)
                        {
                            // Only Z80 has prefix ED
                            foreach (var expression in TryDecodeNextEdPrefixOperation())
                            {
                                yield return expression;
                            }
                        }
                        else
                        {
                            // CPU's without ED prefix treat this as a NOP.
                            timer.Add(1, 4);
                        }
                        break;

                    case PrimaryOpCode.Prefix_CB:
                        if (cpuMode == CpuMode.Z80 || cpuMode == CpuMode.GameBoy)
                        {
                            // Only Gameboy & Z80 have prefix CB opcodes.
                            if (index.UsesDisplacedIndexTimings)
                            {
                                foreach (var expression in TryDecodeNextDdFdCbPrefixOperation())
                                {
                                    yield return expression;
                                }
                            }
                            else
                            {
                                yield return TryDecodeNextCbPrefixOperation();
                            }
                        }
                        else
                        {
                            // CPU's without CB prefix treat this as a NOP.
                            timer.Add(1, 4);
                        }
                        break;

                    // ********* 8-bit load *********
                    // LD r, r'
                    case PrimaryOpCode.LD_A_A:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_A:
                        yield return Expression.Assign(DynaRecExpressions.B, DynaRecExpressions.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_A:
                        yield return Expression.Assign(DynaRecExpressions.C, DynaRecExpressions.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_A:
                        yield return Expression.Assign(DynaRecExpressions.D, DynaRecExpressions.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_A:
                        yield return Expression.Assign(DynaRecExpressions.E, DynaRecExpressions.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_A:
                        yield return Expression.Assign(index.RegisterHighOrder, DynaRecExpressions.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_A:
                        yield return Expression.Assign(index.RegisterLowOrder, DynaRecExpressions.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_B:
                        yield return Expression.Assign(DynaRecExpressions.A, DynaRecExpressions.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_B:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_B:
                        yield return Expression.Assign(DynaRecExpressions.C, DynaRecExpressions.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_B:
                        yield return Expression.Assign(DynaRecExpressions.D, DynaRecExpressions.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_B:
                        yield return Expression.Assign(DynaRecExpressions.E, DynaRecExpressions.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_B:
                        yield return Expression.Assign(index.RegisterHighOrder, DynaRecExpressions.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_B:
                        yield return Expression.Assign(index.RegisterLowOrder, DynaRecExpressions.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_C:
                        yield return Expression.Assign(DynaRecExpressions.A, DynaRecExpressions.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_C:
                        yield return Expression.Assign(DynaRecExpressions.B, DynaRecExpressions.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_C:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_C:
                        yield return Expression.Assign(DynaRecExpressions.D, DynaRecExpressions.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_C:
                        yield return Expression.Assign(DynaRecExpressions.E, DynaRecExpressions.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_C:
                        yield return Expression.Assign(index.RegisterHighOrder, DynaRecExpressions.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_C:
                        yield return Expression.Assign(index.RegisterLowOrder, DynaRecExpressions.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_D:
                        yield return Expression.Assign(DynaRecExpressions.A, DynaRecExpressions.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_D:
                        yield return Expression.Assign(DynaRecExpressions.B, DynaRecExpressions.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_D:
                        yield return Expression.Assign(DynaRecExpressions.C, DynaRecExpressions.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_D:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_D:
                        yield return Expression.Assign(DynaRecExpressions.E, DynaRecExpressions.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_D:
                        yield return Expression.Assign(index.RegisterHighOrder, DynaRecExpressions.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_D:
                        yield return Expression.Assign(index.RegisterLowOrder, DynaRecExpressions.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_E:
                        yield return Expression.Assign(DynaRecExpressions.A, DynaRecExpressions.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_E:
                        yield return Expression.Assign(DynaRecExpressions.B, DynaRecExpressions.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_E:
                        yield return Expression.Assign(DynaRecExpressions.C, DynaRecExpressions.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_E:
                        yield return Expression.Assign(DynaRecExpressions.D, DynaRecExpressions.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_E:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_E:
                        yield return Expression.Assign(index.RegisterHighOrder, DynaRecExpressions.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_E:
                        yield return Expression.Assign(index.RegisterLowOrder, DynaRecExpressions.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_H:
                        yield return Expression.Assign(DynaRecExpressions.A, index.RegisterHighOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_H:
                        yield return Expression.Assign(DynaRecExpressions.B, index.RegisterHighOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_H:
                        yield return Expression.Assign(DynaRecExpressions.C, index.RegisterHighOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_H:
                        yield return Expression.Assign(DynaRecExpressions.D, index.RegisterHighOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_H:
                        yield return Expression.Assign(DynaRecExpressions.E, index.RegisterHighOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_H:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_H:
                        yield return Expression.Assign(index.RegisterLowOrder, index.RegisterHighOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_L:
                        yield return Expression.Assign(DynaRecExpressions.A, index.RegisterLowOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_L:
                        yield return Expression.Assign(DynaRecExpressions.B, index.RegisterLowOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_L:
                        yield return Expression.Assign(DynaRecExpressions.C, index.RegisterLowOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_L:
                        yield return Expression.Assign(DynaRecExpressions.D, index.RegisterLowOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_L:
                        yield return Expression.Assign(DynaRecExpressions.E, index.RegisterLowOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_L:
                        yield return Expression.Assign(index.RegisterHighOrder, index.RegisterLowOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_L:
                        timer.Add(1, 4);
                        break;

                    // LD r,n
                    case PrimaryOpCode.LD_A_n:
                        yield return Expression.Assign(DynaRecExpressions.A, NextByte);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_B_n:
                        yield return Expression.Assign(DynaRecExpressions.B, NextByte);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_C_n:
                        yield return Expression.Assign(DynaRecExpressions.C, NextByte);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_D_n:
                        yield return Expression.Assign(DynaRecExpressions.D, NextByte);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_E_n:
                        yield return Expression.Assign(DynaRecExpressions.E, NextByte);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_H_n:
                        yield return Expression.Assign(index.RegisterHighOrder, NextByte);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_L_n:
                        yield return Expression.Assign(index.RegisterLowOrder, NextByte);
                        timer.Add(2, 7);
                        break;

                    // LD r, (HL)
                    case PrimaryOpCode.LD_A_mHL:
                        yield return Expression.Assign(DynaRecExpressions.A, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_B_mHL:
                        yield return Expression.Assign(DynaRecExpressions.B, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_C_mHL:
                        yield return Expression.Assign(DynaRecExpressions.C, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_D_mHL:
                        yield return Expression.Assign(DynaRecExpressions.D, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_E_mHL:
                        yield return Expression.Assign(DynaRecExpressions.E, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_H_mHL:
                        // H register is always assigned here
                        yield return Expression.Assign(DynaRecExpressions.H, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_L_mHL:
                        // L register is always assigned here
                        yield return Expression.Assign(DynaRecExpressions.L, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // LD (HL), r
                    case PrimaryOpCode.LD_mHL_A:
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, index.IndexedAddress, DynaRecExpressions.A);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_B:
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, index.IndexedAddress, DynaRecExpressions.B);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_C:
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, index.IndexedAddress, DynaRecExpressions.C);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_D:
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, index.IndexedAddress, DynaRecExpressions.D);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_E:
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, index.IndexedAddress, DynaRecExpressions.E);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_H:
                        // Value of H register is always used here
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, index.IndexedAddress, DynaRecExpressions.H);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_L:
                        // Value of L register is always used here
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, index.IndexedAddress, DynaRecExpressions.L);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // LD (HL), n
                    case PrimaryOpCode.LD_mHL_n:
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, index.IndexedAddress, NextByte);
                        if (index.UsesDisplacedIndexTimings) timer.Add(1, 5);
                        timer.Add(3, 10);
                        break;

                    // LD A, (BC)
                    case PrimaryOpCode.LD_A_mBC:
                        yield return Expression.Assign(DynaRecExpressions.A, DynaRecExpressions.ReadByteAtBC);
                        timer.Add(2, 7);
                        break;

                    // LD A, (DE)
                    case PrimaryOpCode.LD_A_mDE:
                        yield return Expression.Assign(DynaRecExpressions.A, DynaRecExpressions.ReadByteAtDE);
                        timer.Add(2, 7);
                        break;

                    // LD A, (nn)
                    case PrimaryOpCode.LD_A_mnn:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadByte, NextWord));
                        timer.Add(4, 13);
                        break;

                    // LD (BC), A
                    case PrimaryOpCode.LD_mBC_A:
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.BC, DynaRecExpressions.A);
                        timer.Add(2, 7);
                        break;

                    // LD (DE), A
                    case PrimaryOpCode.LD_mDE_A:
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.DE, DynaRecExpressions.A);
                        timer.Add(2, 7);
                        break;

                    // LD (nn), A
                    case PrimaryOpCode.LD_mnn_A:
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, NextWord, DynaRecExpressions.A);
                        timer.Add(2, 7);
                        break;

                    // ********* 16-bit load *********
                    // LD dd, nn
                    case PrimaryOpCode.LD_BC_nn:
                        yield return Expression.Assign(DynaRecExpressions.BC, NextWord);
                        timer.Add(2, 10);
                        break;
                    case PrimaryOpCode.LD_DE_nn:
                        yield return Expression.Assign(DynaRecExpressions.DE, NextWord);
                        timer.Add(2, 10);
                        break;
                    case PrimaryOpCode.LD_HL_nn:
                        yield return Expression.Assign(index.Register, NextWord);
                        timer.Add(index.UsesDisplacedIndexTimings ? 3 : 2, 10);
                        break;
                    case PrimaryOpCode.LD_SP_nn:
                        yield return Expression.Assign(DynaRecExpressions.SP, NextWord);
                        timer.Add(2, 10);
                        break;

                    // LD HL, (nn)
                    case PrimaryOpCode.LD_HL_mnn:
                        yield return Expression.Assign(index.Register, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadWord, NextWord));
                        timer.Add(5, 16);
                        break;

                    // LD (nn), HL
                    case PrimaryOpCode.LD_mnn_HL:
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteWord, NextWord, index.Register);
                        timer.Add(5, 16);
                        break;

                    // LD SP, HL
                    case PrimaryOpCode.LD_SP_HL:
                        yield return Expression.Assign(DynaRecExpressions.SP, index.Register);
                        timer.Add(1, 6);
                        break;

                    // PUSH qq
                    case PrimaryOpCode.PUSH_BC:
                        yield return DynaRecExpressions.PushSP;
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.SP, DynaRecExpressions.B);
                        yield return DynaRecExpressions.PushSP;
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.SP, DynaRecExpressions.C);
                        timer.Add(3, 11);
                        break;
                    case PrimaryOpCode.PUSH_DE:
                        yield return DynaRecExpressions.PushSP;
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.SP, DynaRecExpressions.D);
                        yield return DynaRecExpressions.PushSP;
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.SP, DynaRecExpressions.E);
                        timer.Add(3, 11);
                        break;
                    case PrimaryOpCode.PUSH_HL:
                        yield return DynaRecExpressions.PushSP;
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.SP, index.RegisterHighOrder);
                        yield return DynaRecExpressions.PushSP;
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.SP, index.RegisterLowOrder);
                        timer.Add(3, 11);
                        break;
                    case PrimaryOpCode.PUSH_AF:
                        yield return DynaRecExpressions.PushSP;
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.SP, DynaRecExpressions.A);
                        yield return DynaRecExpressions.PushSP;
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.SP, DynaRecExpressions.F);
                        timer.Add(3, 11);
                        break;

                    // POP qq
                    case PrimaryOpCode.POP_BC:
                        yield return Expression.Assign(DynaRecExpressions.C, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadByte, DynaRecExpressions.SP));
                        yield return DynaRecExpressions.PopSP;
                        yield return Expression.Assign(DynaRecExpressions.B, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadByte, DynaRecExpressions.SP));
                        yield return DynaRecExpressions.PopSP;
                        timer.Add(3, 10);
                        break;
                    case PrimaryOpCode.POP_DE:
                        yield return Expression.Assign(DynaRecExpressions.E, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadByte, DynaRecExpressions.SP));
                        yield return DynaRecExpressions.PopSP;
                        yield return Expression.Assign(DynaRecExpressions.D, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadByte, DynaRecExpressions.SP));
                        yield return DynaRecExpressions.PopSP;
                        timer.Add(3, 10);
                        break;
                    case PrimaryOpCode.POP_HL:
                        yield return Expression.Assign(index.RegisterLowOrder, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadByte, DynaRecExpressions.SP));
                        yield return DynaRecExpressions.PopSP;
                        yield return Expression.Assign(index.RegisterHighOrder, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadByte, DynaRecExpressions.SP));
                        yield return DynaRecExpressions.PopSP;
                        timer.Add(3, 10);
                        break;
                    case PrimaryOpCode.POP_AF:
                        yield return Expression.Assign(DynaRecExpressions.F, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadByte, DynaRecExpressions.SP));
                        yield return DynaRecExpressions.PopSP;
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadByte, DynaRecExpressions.SP));
                        yield return DynaRecExpressions.PopSP;
                        timer.Add(3, 10);
                        break;

                    // ********* Exchange *********
                    // EX DE, HL
                    case PrimaryOpCode.EX_DE_HL:
                        // This affects HL register directly, always ignoring index register prefixes
                        yield return Expression.Assign(DynaRecExpressions.LocalWord, DynaRecExpressions.DE);
                        yield return Expression.Assign(DynaRecExpressions.DE, DynaRecExpressions.HL);
                        yield return Expression.Assign(DynaRecExpressions.HL, DynaRecExpressions.LocalWord);
                        timer.Add(1, 4);
                        break;

                    // EX AF, AF′
                    case PrimaryOpCode.EX_AF:
                        yield return DynaRecExpressions.SwitchToAlternativeAccumulatorAndFlagsRegisters;
                        timer.Add(1, 4);
                        break;

                    // EXX
                    case PrimaryOpCode.EXX:
                        yield return DynaRecExpressions.SwitchToAlternativeGeneralPurposeRegisters;
                        timer.Add(1, 4);
                        break;

                    // EX (SP), HL
                    case PrimaryOpCode.EX_mSP_HL:
                        // Exchange L
                        yield return Expression.Assign(DynaRecExpressions.LocalByte, index.RegisterLowOrder);
                        yield return Expression.Assign(index.RegisterLowOrder, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadByte, DynaRecExpressions.SP));
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.SP, DynaRecExpressions.LocalByte);

                        // Exchange H
                        yield return Expression.Assign(DynaRecExpressions.LocalByte, index.RegisterHighOrder);
                        yield return Expression.Assign(DynaRecExpressions.LocalWord, Expression.Increment(DynaRecExpressions.SP));
                        yield return Expression.Assign(index.RegisterHighOrder, Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuReadByte, DynaRecExpressions.LocalWord));
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, DynaRecExpressions.LocalWord, DynaRecExpressions.LocalByte);

                        timer.Add(5, 19);
                        break;

                    // ********* 8-Bit Arithmetic *********
                    // ADD A, r
                    case PrimaryOpCode.ADD_A_A:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd, DynaRecExpressions.A, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_B:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd, DynaRecExpressions.A, DynaRecExpressions.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_C:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd, DynaRecExpressions.A, DynaRecExpressions.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_D:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd, DynaRecExpressions.A, DynaRecExpressions.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_E:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd, DynaRecExpressions.A, DynaRecExpressions.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_H:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd, DynaRecExpressions.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_L:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd, DynaRecExpressions.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // ADD A, n
                    case PrimaryOpCode.ADD_A_n:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd, DynaRecExpressions.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // ADD A, (HL)
                    case PrimaryOpCode.ADD_A_mHL:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd, DynaRecExpressions.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // ADC A, r
                    case PrimaryOpCode.ADC_A_A:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAddWithCarry, DynaRecExpressions.A, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_B:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAddWithCarry, DynaRecExpressions.A, DynaRecExpressions.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_C:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAddWithCarry, DynaRecExpressions.A, DynaRecExpressions.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_D:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAddWithCarry, DynaRecExpressions.A, DynaRecExpressions.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_E:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAddWithCarry, DynaRecExpressions.A, DynaRecExpressions.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_H:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAddWithCarry, DynaRecExpressions.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_L:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAddWithCarry, DynaRecExpressions.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // ADC A, n
                    case PrimaryOpCode.ADC_A_n:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAddWithCarry, DynaRecExpressions.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // ADC A, (HL)
                    case PrimaryOpCode.ADC_A_mHL:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAddWithCarry, DynaRecExpressions.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // SUB A, r
                    case PrimaryOpCode.SUB_A_A:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract, DynaRecExpressions.A, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_B:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract, DynaRecExpressions.A, DynaRecExpressions.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_C:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract, DynaRecExpressions.A, DynaRecExpressions.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_D:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract, DynaRecExpressions.A, DynaRecExpressions.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_E:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract, DynaRecExpressions.A, DynaRecExpressions.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_H:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract, DynaRecExpressions.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_L:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract, DynaRecExpressions.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // SUB A, n
                    case PrimaryOpCode.SUB_A_n:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract, DynaRecExpressions.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // SUB A, (HL)
                    case PrimaryOpCode.SUB_A_mHL:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtract, DynaRecExpressions.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // SBC A, r
                    case PrimaryOpCode.SBC_A_A:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtractWithCarry, DynaRecExpressions.A, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_B:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtractWithCarry, DynaRecExpressions.A, DynaRecExpressions.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_C:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtractWithCarry, DynaRecExpressions.A, DynaRecExpressions.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_D:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtractWithCarry, DynaRecExpressions.A, DynaRecExpressions.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_E:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtractWithCarry, DynaRecExpressions.A, DynaRecExpressions.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_H:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtractWithCarry, DynaRecExpressions.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_L:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtractWithCarry, DynaRecExpressions.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // SBC A, n
                    case PrimaryOpCode.SBC_A_n:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtractWithCarry, DynaRecExpressions.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // SBC A, (HL)
                    case PrimaryOpCode.SBC_A_mHL:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluSubtractWithCarry, DynaRecExpressions.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // AND r
                    case PrimaryOpCode.AND_A:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAnd, DynaRecExpressions.A, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_B:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAnd, DynaRecExpressions.A, DynaRecExpressions.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_C:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAnd, DynaRecExpressions.A, DynaRecExpressions.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_D:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAnd, DynaRecExpressions.A, DynaRecExpressions.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_E:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAnd, DynaRecExpressions.A, DynaRecExpressions.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_H:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAnd, DynaRecExpressions.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_L:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAnd, DynaRecExpressions.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // AND n
                    case PrimaryOpCode.AND_n:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAnd, DynaRecExpressions.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // AND (HL)
                    case PrimaryOpCode.AND_mHL:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAnd, DynaRecExpressions.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // OR r
                    case PrimaryOpCode.OR_A:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluOr, DynaRecExpressions.A, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_B:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluOr, DynaRecExpressions.A, DynaRecExpressions.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_C:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluOr, DynaRecExpressions.A, DynaRecExpressions.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_D:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluOr, DynaRecExpressions.A, DynaRecExpressions.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_E:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluOr, DynaRecExpressions.A, DynaRecExpressions.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_H:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluOr, DynaRecExpressions.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_L:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluOr, DynaRecExpressions.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // OR n
                    case PrimaryOpCode.OR_n:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluOr, DynaRecExpressions.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // OR (HL)
                    case PrimaryOpCode.OR_mHL:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluOr, DynaRecExpressions.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // XOR r
                    case PrimaryOpCode.XOR_A:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluXor, DynaRecExpressions.A, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_B:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluXor, DynaRecExpressions.A, DynaRecExpressions.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_C:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluXor, DynaRecExpressions.A, DynaRecExpressions.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_D:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluXor, DynaRecExpressions.A, DynaRecExpressions.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_E:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluXor, DynaRecExpressions.A, DynaRecExpressions.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_H:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluXor, DynaRecExpressions.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_L:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluXor, DynaRecExpressions.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // XOR n
                    case PrimaryOpCode.XOR_n:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluXor, DynaRecExpressions.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // XOR (HL)
                    case PrimaryOpCode.XOR_mHL:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluXor, DynaRecExpressions.A, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // CP r
                    case PrimaryOpCode.CP_A:
                        yield return Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluCompare, DynaRecExpressions.A, DynaRecExpressions.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_B:
                        yield return Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluCompare, DynaRecExpressions.A, DynaRecExpressions.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_C:
                        yield return Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluCompare, DynaRecExpressions.A, DynaRecExpressions.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_D:
                        yield return Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluCompare, DynaRecExpressions.A, DynaRecExpressions.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_E:
                        yield return Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluCompare, DynaRecExpressions.A, DynaRecExpressions.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_H:
                        yield return Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluCompare, DynaRecExpressions.A, index.RegisterHighOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_L:
                        yield return Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluCompare, DynaRecExpressions.A, index.RegisterLowOrder);
                        timer.Add(1, 4);
                        break;

                    // CP n
                    case PrimaryOpCode.CP_n:
                        yield return Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluCompare, DynaRecExpressions.A, NextByte);
                        timer.Add(2, 7);
                        break;

                    // CP (HL)
                    case PrimaryOpCode.CP_mHL:
                        yield return Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluCompare, DynaRecExpressions.A, index.ReadIndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // INC r
                    case PrimaryOpCode.INC_A:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluIncrement, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_B:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluIncrement, DynaRecExpressions.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_C:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluIncrement, DynaRecExpressions.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_D:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluIncrement, DynaRecExpressions.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_E:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluIncrement, DynaRecExpressions.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_H:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluIncrement, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_L:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluIncrement, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // INC (HL)
                    case PrimaryOpCode.INC_mHL:
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, index.IndexedAddress, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluIncrement, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(3, 11);
                        break;

                    // DEC r
                    case PrimaryOpCode.DEC_A:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluDecrement, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_B:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluDecrement, DynaRecExpressions.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_C:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluDecrement, DynaRecExpressions.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_D:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluDecrement, DynaRecExpressions.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_E:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluDecrement, DynaRecExpressions.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_H:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluDecrement, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_L:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluDecrement, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // DEC (HL)
                    case PrimaryOpCode.DEC_mHL:
                        yield return Expression.Call(DynaRecExpressions.Mmu, DynaRecExpressions.MmuWriteByte, index.IndexedAddress, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluDecrement, index.ReadIndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(3, 11);
                        break;

                    // ********* 16-Bit Arithmetic *********
                    // ADD HL, ss
                    case PrimaryOpCode.ADD_HL_BC:
                        yield return Expression.Assign(index.Register, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd16, index.Register, DynaRecExpressions.BC));
                        timer.Add(3, 11);
                        break;
                    case PrimaryOpCode.ADD_HL_DE:
                        yield return Expression.Assign(index.Register, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd16, index.Register, DynaRecExpressions.DE));
                        timer.Add(3, 11);
                        break;
                    case PrimaryOpCode.ADD_HL_HL:
                        yield return Expression.Assign(index.Register, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd16, index.Register, index.Register));
                        timer.Add(3, 11);
                        break;
                    case PrimaryOpCode.ADD_HL_SP:
                        yield return Expression.Assign(index.Register, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluAdd16, index.Register, DynaRecExpressions.SP));
                        timer.Add(3, 11);
                        break;

                    // INC ss (no flags changes so implemented directly)
                    case PrimaryOpCode.INC_BC:
                        yield return Expression.PreIncrementAssign(DynaRecExpressions.BC);
                        timer.Add(1, 6);
                        break;
                    case PrimaryOpCode.INC_DE:
                        yield return Expression.PreIncrementAssign(DynaRecExpressions.DE);
                        timer.Add(1, 6);
                        break;
                    case PrimaryOpCode.INC_HL:
                        yield return Expression.PreIncrementAssign(index.Register);
                        timer.Add(1, 6);
                        break;
                    case PrimaryOpCode.INC_SP:
                        yield return Expression.PreIncrementAssign(DynaRecExpressions.SP);
                        timer.Add(1, 6);
                        break;

                    // DEC ss (no flags changes so implemented directly)
                    case PrimaryOpCode.DEC_BC:
                        yield return Expression.PreDecrementAssign(DynaRecExpressions.BC);
                        timer.Add(1, 6);
                        break;
                    case PrimaryOpCode.DEC_DE:
                        yield return Expression.PreDecrementAssign(DynaRecExpressions.DE);
                        timer.Add(1, 6);
                        break;
                    case PrimaryOpCode.DEC_HL:
                        yield return Expression.PreDecrementAssign(index.Register);
                        timer.Add(1, 6);
                        break;
                    case PrimaryOpCode.DEC_SP:
                        yield return Expression.PreDecrementAssign(DynaRecExpressions.SP);
                        timer.Add(1, 6);
                        break;

                    // ********* General-Purpose Arithmetic *********
                    // DAA
                    case PrimaryOpCode.DAA:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluDecimalAdjust, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;

                    // CPL
                    case PrimaryOpCode.CPL:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Not(DynaRecExpressions.A));
                        yield return Expression.Call(DynaRecExpressions.Flags, DynaRecExpressions.SetUndocumentedFlags, DynaRecExpressions.A);
                        yield return Expression.Assign(DynaRecExpressions.HalfCarry, Expression.Constant(true));
                        yield return Expression.Assign(DynaRecExpressions.Subtract, Expression.Constant(true));
                        timer.Add(1, 4);
                        break;

                    // CCF
                    case PrimaryOpCode.CCF:
                        yield return Expression.Assign(DynaRecExpressions.HalfCarry, DynaRecExpressions.Carry);
                        yield return Expression.Assign(DynaRecExpressions.Subtract, Expression.Constant(false));
                        yield return Expression.Assign(DynaRecExpressions.Carry, Expression.Not(DynaRecExpressions.Carry));
                        timer.Add(1, 4);
                        break;

                    // SCF
                    case PrimaryOpCode.SCF:
                        yield return Expression.Assign(DynaRecExpressions.HalfCarry, Expression.Constant(false));
                        yield return Expression.Assign(DynaRecExpressions.Subtract, Expression.Constant(false));
                        yield return Expression.Assign(DynaRecExpressions.Carry, Expression.Constant(true));
                        timer.Add(1, 4);
                        break;

                    // DI
                    case PrimaryOpCode.DI:
                        yield return Expression.Assign(DynaRecExpressions.IFF1, Expression.Constant(false));
                        yield return Expression.Assign(DynaRecExpressions.IFF2, Expression.Constant(false));
                        timer.Add(1, 4);
                        break;

                    // EI
                    case PrimaryOpCode.EI:
                        yield return Expression.Assign(DynaRecExpressions.IFF1, Expression.Constant(true));
                        yield return Expression.Assign(DynaRecExpressions.IFF2, Expression.Constant(true));
                        timer.Add(1, 4);
                        break;

                    // ********* Rotate *********
                    // RLCA
                    case PrimaryOpCode.RLCA:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluRotateLeftWithCarry, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;

                    //RLA
                    case PrimaryOpCode.RLA:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluRotateLeft, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;

                    // RRCA
                    case PrimaryOpCode.RRCA:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluRotateRightWithCarry, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;

                    //RRA
                    case PrimaryOpCode.RRA:
                        yield return Expression.Assign(DynaRecExpressions.A, Expression.Call(DynaRecExpressions.Alu, DynaRecExpressions.AluRotateRight, DynaRecExpressions.A));
                        timer.Add(1, 4);
                        break;

                    // ********* Jump *********
                    case PrimaryOpCode.JP:
                        yield return Expression.Assign(DynaRecExpressions.PC, NextWord);
                        timer.Add(3, 10);
                        lastDecodeResult = DecodeResult.Finalize;
                        yield break;

                    default:
                        throw new NotImplementedException(opCode.ToString());
                }

                // Set index back
                if (this.index.IndexRegister != IndexRegister.HL)
                {
                    this.index = DynaRecExpressions.IndexRegisterExpressions[IndexRegister.HL];
                }
            }
        }

        /// <summary>
        /// Checks that when a DD or FD index prefix is applied whether the opcode will need a displacement adding to the index register i.e. (IX + d) and (IY + d) opcodes.
        /// </summary>
        /// <param name="opCode"></param>
        /// <returns></returns>
        private static bool OpCodeUsesDisplacedIndex(PrimaryOpCode opCode)
        {
            switch (opCode)
            {
                case PrimaryOpCode.LD_A_mHL:
                case PrimaryOpCode.LD_B_mHL:
                case PrimaryOpCode.LD_C_mHL:
                case PrimaryOpCode.LD_D_mHL:
                case PrimaryOpCode.LD_E_mHL:
                case PrimaryOpCode.LD_H_mHL:
                case PrimaryOpCode.LD_L_mHL:
                case PrimaryOpCode.LD_mHL_A:
                case PrimaryOpCode.LD_mHL_B:
                case PrimaryOpCode.LD_mHL_C:
                case PrimaryOpCode.LD_mHL_D:
                case PrimaryOpCode.LD_mHL_E:
                case PrimaryOpCode.LD_mHL_H:
                case PrimaryOpCode.LD_mHL_L:
                case PrimaryOpCode.LD_mHL_n:
                case PrimaryOpCode.ADD_A_mHL:
                case PrimaryOpCode.ADC_A_mHL:
                case PrimaryOpCode.SUB_A_mHL:
                case PrimaryOpCode.SBC_A_mHL:
                case PrimaryOpCode.AND_mHL:
                case PrimaryOpCode.OR_mHL:
                case PrimaryOpCode.XOR_mHL:
                case PrimaryOpCode.CP_mHL:
                case PrimaryOpCode.INC_mHL:
                case PrimaryOpCode.DEC_mHL:
                    return true;
                default:
                    return false;
            }
        }
    }
}
