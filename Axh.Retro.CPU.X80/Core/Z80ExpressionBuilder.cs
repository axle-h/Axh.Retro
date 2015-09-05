namespace Axh.Retro.CPU.X80.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Util;

    using Xpr = Z80Expressions;

    internal class Z80ExpressionBuilder
    {
        private readonly IInstructionTimer timer;
        private readonly IMmuCache mmuCache;

        private ConstantExpression NextByte => Expression.Constant(mmuCache.NextByte(), typeof(byte));
        private ConstantExpression NextWord => Expression.Constant(mmuCache.NextWord(), typeof(ushort));
        
        public Z80ExpressionBuilder(IMmuCache mmuCache, IInstructionTimer timer)
        {
            this.timer = timer;
            this.mmuCache = mmuCache;
        }

        public Expression<Func<IZ80Registers, IMmu, IArithmeticLogicUnit, InstructionTimings>> DecodeNextBlock()
        {
            var expressions = GetBlockExpressions().ToArray();

            var expressionBlock = Expression.Block(new[] { Xpr.LocalByte, Xpr.LocalWord, Xpr.DynamicTimer }, expressions);
            var lambda = Expression.Lambda<Func<IZ80Registers, IMmu, IArithmeticLogicUnit, InstructionTimings>>(expressionBlock, Xpr.Registers, Xpr.Mmu, Xpr.Alu);

            return lambda;
        }
        
        private IEnumerable<Expression> GetBlockExpressions()
        {
            // Create a new dynamic timer to record any timings calculated at runtime.
            yield return Expression.Assign(Xpr.DynamicTimer, Expression.New(typeof(InstructionTimer)));

            var currentIndexRegister = IndexRegister.HL;
            var currentDecodeResult = DecodeResult.Continue;

            while (currentDecodeResult == DecodeResult.Continue)
            {
                if (mmuCache.TotalBytesRead == ushort.MaxValue)
                {
                    currentDecodeResult = DecodeResult.FinalizeAndSync;
                    break;
                }

                var opCode = (PrimaryOpCode)this.mmuCache.NextByte();
                var index = Xpr.IndexRegisterExpressions[currentIndexRegister];

                if (index.UsesDisplacedIndexTimings && OpCodeUsesDisplacedIndex(opCode))
                {
                    // Read the displacement as the next byte
                    yield return Expression.Assign(Xpr.LocalByte, NextByte);
                }

                switch (opCode)
                {
                    case PrimaryOpCode.NOP:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.HALT:
                        timer.Add(1, 4);
                        currentDecodeResult = DecodeResult.FinalizeAndSync;
                        break;

                    // ********* Prefixes *********
                    case PrimaryOpCode.Prefix_DD:
                        // Add a NOP timing for now and take a NOP off known indexed timings later.
                        timer.Add(1, 4);
                        currentIndexRegister = IndexRegister.IX;
                        continue;

                    case PrimaryOpCode.Prefix_FD:
                        // Add a NOP timing for now and take a NOP off known indexed timings later.
                        timer.Add(1, 4);
                        currentIndexRegister = IndexRegister.IY;
                        continue;

                    case PrimaryOpCode.Prefix_ED:
                        foreach (var expression in TryDecodeNextEdPrefixOperation())
                        {
                            yield return expression;
                        }
                        break;

                    // ********* 8-bit load *********
                    // LD r, r'
                    case PrimaryOpCode.LD_A_A:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_A:
                        yield return Expression.Assign(Xpr.B, Xpr.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_A:
                        yield return Expression.Assign(Xpr.C, Xpr.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_A:
                        yield return Expression.Assign(Xpr.D, Xpr.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_A:
                        yield return Expression.Assign(Xpr.E, Xpr.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_A:
                        yield return Expression.Assign(index.RegisterHighOrder, Xpr.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_A:
                        yield return Expression.Assign(index.RegisterLowOrder, Xpr.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_B:
                        yield return Expression.Assign(Xpr.A, Xpr.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_B:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_B:
                        yield return Expression.Assign(Xpr.C, Xpr.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_B:
                        yield return Expression.Assign(Xpr.D, Xpr.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_B:
                        yield return Expression.Assign(Xpr.E, Xpr.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_B:
                        yield return Expression.Assign(index.RegisterHighOrder, Xpr.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_B:
                        yield return Expression.Assign(index.RegisterLowOrder, Xpr.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_C:
                        yield return Expression.Assign(Xpr.A, Xpr.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_C:
                        yield return Expression.Assign(Xpr.B, Xpr.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_C:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_C:
                        yield return Expression.Assign(Xpr.D, Xpr.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_C:
                        yield return Expression.Assign(Xpr.E, Xpr.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_C:
                        yield return Expression.Assign(index.RegisterHighOrder, Xpr.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_C:
                        yield return Expression.Assign(index.RegisterLowOrder, Xpr.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_D:
                        yield return Expression.Assign(Xpr.A, Xpr.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_D:
                        yield return Expression.Assign(Xpr.B, Xpr.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_D:
                        yield return Expression.Assign(Xpr.C, Xpr.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_D:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_D:
                        yield return Expression.Assign(Xpr.E, Xpr.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_D:
                        yield return Expression.Assign(index.RegisterHighOrder, Xpr.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_D:
                        yield return Expression.Assign(index.RegisterLowOrder, Xpr.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_E:
                        yield return Expression.Assign(Xpr.A, Xpr.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_E:
                        yield return Expression.Assign(Xpr.B, Xpr.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_E:
                        yield return Expression.Assign(Xpr.C, Xpr.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_E:
                        yield return Expression.Assign(Xpr.D, Xpr.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_E:
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_H_E:
                        yield return Expression.Assign(index.RegisterHighOrder, Xpr.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_L_E:
                        yield return Expression.Assign(index.RegisterLowOrder, Xpr.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_A_H:
                        yield return Expression.Assign(Xpr.A, index.RegisterHighOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_H:
                        yield return Expression.Assign(Xpr.B, index.RegisterHighOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_H:
                        yield return Expression.Assign(Xpr.C, index.RegisterHighOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_H:
                        yield return Expression.Assign(Xpr.D, index.RegisterHighOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_H:
                        yield return Expression.Assign(Xpr.E, index.RegisterHighOrder);
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
                        yield return Expression.Assign(Xpr.A, index.RegisterLowOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_B_L:
                        yield return Expression.Assign(Xpr.B, index.RegisterLowOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_C_L:
                        yield return Expression.Assign(Xpr.C, index.RegisterLowOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_D_L:
                        yield return Expression.Assign(Xpr.D, index.RegisterLowOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.LD_E_L:
                        yield return Expression.Assign(Xpr.E, index.RegisterLowOrder);
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
                        yield return Expression.Assign(Xpr.A, NextByte);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_B_n:
                        yield return Expression.Assign(Xpr.B, NextByte);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_C_n:
                        yield return Expression.Assign(Xpr.C, NextByte);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_D_n:
                        yield return Expression.Assign(Xpr.D, NextByte);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_E_n:
                        yield return Expression.Assign(Xpr.E, NextByte);
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
                        yield return Expression.Assign(Xpr.A, index.IndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_B_mHL:
                        yield return Expression.Assign(Xpr.B, index.IndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_C_mHL:
                        yield return Expression.Assign(Xpr.C, index.IndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_D_mHL:
                        yield return Expression.Assign(Xpr.D, index.IndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_E_mHL:
                        yield return Expression.Assign(Xpr.E, index.IndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_H_mHL:
                        // H register is always assigned here
                        yield return Expression.Assign(Xpr.H, index.IndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_L_mHL:
                        // L register is always assigned here
                        yield return Expression.Assign(Xpr.L, index.IndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // LD (HL), r
                    case PrimaryOpCode.LD_mHL_A:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.A);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_B:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.B);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_C:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.C);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_D:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.D);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_E:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.E);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_H:
                        // Value of H register is always used here
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.H);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;
                    case PrimaryOpCode.LD_mHL_L:
                        // Value of L register is always used here
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Xpr.L);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // LD (HL), n
                    case PrimaryOpCode.LD_mHL_n:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, NextByte);
                        if (index.UsesDisplacedIndexTimings) timer.Add(1, 5);
                        timer.Add(3, 10);
                        break;

                    // LD A, (BC)
                    case PrimaryOpCode.LD_A_mBC:
                        yield return Expression.Assign(Xpr.A, Xpr.ReadByteAtBC);
                        timer.Add(2, 7);
                        break;

                    // LD A, (DE)
                    case PrimaryOpCode.LD_A_mDE:
                        yield return Expression.Assign(Xpr.A, Xpr.ReadByteAtDE);
                        timer.Add(2, 7);
                        break;

                    // LD A, (nn)
                    case PrimaryOpCode.LD_A_mnn:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, NextWord));
                        timer.Add(4, 13);
                        break;

                    // LD (BC), A
                    case PrimaryOpCode.LD_mBC_A:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.BC, Xpr.A);
                        timer.Add(2, 7);
                        break;

                    // LD (DE), A
                    case PrimaryOpCode.LD_mDE_A:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.DE, Xpr.A);
                        timer.Add(2, 7);
                        break;

                    // LD (nn), A
                    case PrimaryOpCode.LD_mnn_A:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, NextWord, Xpr.A);
                        timer.Add(2, 7);
                        break;

                    // ********* 16-bit load *********
                    // LD dd, nn
                    case PrimaryOpCode.LD_BC_nn:
                        yield return Expression.Assign(Xpr.BC, NextWord);
                        timer.Add(2, 10);
                        break;
                    case PrimaryOpCode.LD_DE_nn:
                        yield return Expression.Assign(Xpr.DE, NextWord);
                        timer.Add(2, 10);
                        break;
                    case PrimaryOpCode.LD_HL_nn:
                        yield return Expression.Assign(index.Register, NextWord);
                        timer.Add(index.UsesDisplacedIndexTimings ? 3 : 2, 10);
                        break;
                    case PrimaryOpCode.LD_SP_nn:
                        yield return Expression.Assign(Xpr.SP, NextWord);
                        timer.Add(2, 10);
                        break;

                    // LD HL, (nn)
                    case PrimaryOpCode.LD_HL_mnn:
                        yield return Expression.Assign(index.Register, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                        timer.Add(5, 16);
                        break;

                    // LD (nn), HL
                    case PrimaryOpCode.LD_mnn_HL:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, index.Register);
                        timer.Add(5, 16);
                        break;

                    // LD SP, HL
                    case PrimaryOpCode.LD_SP_HL:
                        yield return Expression.Assign(Xpr.SP, index.Register);
                        timer.Add(1, 6);
                        break;

                    // PUSH qq
                    case PrimaryOpCode.PUSH_BC:
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.B);
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.C);
                        timer.Add(3, 11);
                        break;
                    case PrimaryOpCode.PUSH_DE:
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.D);
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.E);
                        timer.Add(3, 11);
                        break;
                    case PrimaryOpCode.PUSH_HL:
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, index.RegisterHighOrder);
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, index.RegisterLowOrder);
                        timer.Add(3, 11);
                        break;
                    case PrimaryOpCode.PUSH_AF:
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.A);
                        yield return Xpr.PushSP;
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.F);
                        timer.Add(3, 11);
                        break;

                    // POP qq
                    case PrimaryOpCode.POP_BC:
                        yield return Expression.Assign(Xpr.C, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        yield return Expression.Assign(Xpr.B, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        timer.Add(3, 10);
                        break;
                    case PrimaryOpCode.POP_DE:
                        yield return Expression.Assign(Xpr.E, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        yield return Expression.Assign(Xpr.D, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        timer.Add(3, 10);
                        break;
                    case PrimaryOpCode.POP_HL:
                        yield return Expression.Assign(index.RegisterLowOrder, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        yield return Expression.Assign(index.RegisterHighOrder, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        timer.Add(3, 10);
                        break;
                    case PrimaryOpCode.POP_AF:
                        yield return Expression.Assign(Xpr.F, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Xpr.PopSP;
                        timer.Add(3, 10);
                        break;

                    // ********* Exchange *********
                    // EX DE, HL
                    case PrimaryOpCode.EX_DE_HL:
                        // This affects HL register directly, always ignoring index register prefixes
                        yield return Expression.Assign(Xpr.LocalWord, Xpr.DE);
                        yield return Expression.Assign(Xpr.DE, Xpr.HL);
                        yield return Expression.Assign(Xpr.HL, Xpr.LocalWord);
                        timer.Add(1, 4);
                        break;

                    // EX AF, AF′
                    case PrimaryOpCode.EX_AF:
                        yield return Xpr.SwitchToAlternativeAccumulatorAndFlagsRegisters;
                        timer.Add(1, 4);
                        break;

                    // EXX
                    case PrimaryOpCode.EXX:
                        yield return Xpr.SwitchToAlternativeGeneralPurposeRegisters;
                        timer.Add(1, 4);
                        break;

                    // EX (SP), HL
                    case PrimaryOpCode.EX_mSP_HL:
                        // Exchange L
                        yield return Expression.Assign(Xpr.LocalByte, index.RegisterLowOrder);
                        yield return Expression.Assign(index.RegisterLowOrder, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.SP));
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.SP, Xpr.LocalByte);

                        // Exchange H
                        yield return Expression.Assign(Xpr.LocalByte, index.RegisterHighOrder);
                        yield return Expression.Assign(Xpr.LocalWord, Expression.Increment(Xpr.SP));
                        yield return Expression.Assign(index.RegisterHighOrder, Expression.Call(Xpr.Mmu, Xpr.MmuReadByte, Xpr.LocalWord));
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, Xpr.LocalWord, Xpr.LocalByte);

                        timer.Add(5, 19);
                        break;

                    // ********* 8-Bit Arithmetic *********
                    // ADD A, r
                    case PrimaryOpCode.ADD_A_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, Xpr.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, Xpr.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, Xpr.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, Xpr.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, Xpr.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADD_A_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // ADD A, n
                    case PrimaryOpCode.ADD_A_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // ADD A, (HL)
                    case PrimaryOpCode.ADD_A_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAdd, Xpr.A, index.IndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // ADC A, r
                    case PrimaryOpCode.ADC_A_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, Xpr.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, Xpr.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, Xpr.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, Xpr.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, Xpr.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.ADC_A_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // ADC A, n
                    case PrimaryOpCode.ADC_A_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // ADC A, (HL)
                    case PrimaryOpCode.ADC_A_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAddWithCarry, Xpr.A, index.IndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // SUB A, r
                    case PrimaryOpCode.SUB_A_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, Xpr.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, Xpr.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, Xpr.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, Xpr.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, Xpr.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SUB_A_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // SUB A, n
                    case PrimaryOpCode.SUB_A_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // SUB A, (HL)
                    case PrimaryOpCode.SUB_A_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Xpr.A, index.IndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // SBC A, r
                    case PrimaryOpCode.SBC_A_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, Xpr.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, Xpr.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, Xpr.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, Xpr.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, Xpr.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.SBC_A_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // SBC A, n
                    case PrimaryOpCode.SBC_A_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // SBC A, (HL)
                    case PrimaryOpCode.SBC_A_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtractWithCarry, Xpr.A, index.IndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // AND r
                    case PrimaryOpCode.AND_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, Xpr.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, Xpr.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, Xpr.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, Xpr.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, Xpr.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.AND_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // AND n
                    case PrimaryOpCode.AND_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // AND (HL)
                    case PrimaryOpCode.AND_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluAnd, Xpr.A, index.IndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // OR r
                    case PrimaryOpCode.OR_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, Xpr.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, Xpr.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, Xpr.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, Xpr.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, Xpr.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.OR_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // OR n
                    case PrimaryOpCode.OR_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // OR (HL)
                    case PrimaryOpCode.OR_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluOr, Xpr.A, index.IndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // XOR r
                    case PrimaryOpCode.XOR_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, Xpr.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, Xpr.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, Xpr.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, Xpr.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, Xpr.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.XOR_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // XOR n
                    case PrimaryOpCode.XOR_n:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, NextByte));
                        timer.Add(2, 7);
                        break;

                    // XOR (HL)
                    case PrimaryOpCode.XOR_mHL:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluXor, Xpr.A, index.IndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // CP r
                    case PrimaryOpCode.CP_A:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, Xpr.A);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_B:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, Xpr.B);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_C:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, Xpr.C);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_D:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, Xpr.D);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_E:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, Xpr.E);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_H:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, index.RegisterHighOrder);
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.CP_L:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, index.RegisterLowOrder);
                        timer.Add(1, 4);
                        break;

                    // CP n
                    case PrimaryOpCode.CP_n:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, NextByte);
                        timer.Add(2, 7);
                        break;

                    // CP (HL)
                    case PrimaryOpCode.CP_mHL:
                        yield return Expression.Call(Xpr.Alu, Xpr.AluCompare, Xpr.A, index.IndexedValue);
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(2, 7);
                        break;

                    // INC r
                    case PrimaryOpCode.INC_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, Xpr.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, Xpr.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, Xpr.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, Xpr.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, Xpr.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.INC_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluIncrement, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // INC (HL)
                    case PrimaryOpCode.INC_mHL:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluIncrement, index.IndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(3, 11);
                        break;

                    // DEC r
                    case PrimaryOpCode.DEC_A:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, Xpr.A));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_B:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, Xpr.B));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_C:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, Xpr.C));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_D:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, Xpr.D));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_E:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, Xpr.E));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_H:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, index.RegisterHighOrder));
                        timer.Add(1, 4);
                        break;
                    case PrimaryOpCode.DEC_L:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecrement, index.RegisterLowOrder));
                        timer.Add(1, 4);
                        break;

                    // DEC (HL)
                    case PrimaryOpCode.DEC_mHL:
                        yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluDecrement, index.IndexedValue));
                        if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                        timer.Add(3, 11);
                        break;

                    // ********* 16-Bit Arithmetic *********
                    // ADD HL, ss
                    case PrimaryOpCode.ADD_HL_BC:
                        yield return Expression.Assign(index.Register, Expression.Call(Xpr.Alu, Xpr.AluAdd16, index.Register, Xpr.BC));
                        timer.Add(3, 11);
                        break;
                    case PrimaryOpCode.ADD_HL_DE:
                        yield return Expression.Assign(index.Register, Expression.Call(Xpr.Alu, Xpr.AluAdd16, index.Register, Xpr.DE));
                        timer.Add(3, 11);
                        break;
                    case PrimaryOpCode.ADD_HL_HL:
                        yield return Expression.Assign(index.Register, Expression.Call(Xpr.Alu, Xpr.AluAdd16, index.Register, index.Register));
                        timer.Add(3, 11);
                        break;
                    case PrimaryOpCode.ADD_HL_SP:
                        yield return Expression.Assign(index.Register, Expression.Call(Xpr.Alu, Xpr.AluAdd16, index.Register, Xpr.SP));
                        timer.Add(3, 11);
                        break;

                    // ********* General-Purpose Arithmetic *********
                    // DAA
                    case PrimaryOpCode.DAA:
                        yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluDecimalAdjust, Xpr.A));
                        timer.Add(1, 4);
                        break;

                    // CPL
                    case PrimaryOpCode.CPL:
                        yield return Expression.Assign(Xpr.A, Expression.Not(Xpr.A));
                        yield return Expression.Call(Xpr.Flags, Xpr.SetUndocumentedFlags, Xpr.A);
                        yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(true));
                        yield return Expression.Assign(Xpr.Subtract, Expression.Constant(true));
                        timer.Add(1, 4);
                        break;

                    // CCF
                    case PrimaryOpCode.CCF:
                        yield return Expression.Assign(Xpr.HalfCarry, Xpr.Carry);
                        yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                        yield return Expression.Assign(Xpr.Carry, Expression.Not(Xpr.Carry));
                        timer.Add(1, 4);
                        break;

                    // SCF
                    case PrimaryOpCode.SCF:
                        yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(false));
                        yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                        yield return Expression.Assign(Xpr.Carry, Expression.Constant(true));
                        timer.Add(1, 4);
                        break;

                    // DI
                    case PrimaryOpCode.DI:
                        yield return Expression.Assign(Xpr.IFF1, Expression.Constant(false));
                        yield return Expression.Assign(Xpr.IFF2, Expression.Constant(false));
                        timer.Add(1, 4);
                        break;

                    // EI
                    case PrimaryOpCode.EI:
                        yield return Expression.Assign(Xpr.IFF1, Expression.Constant(true));
                        yield return Expression.Assign(Xpr.IFF2, Expression.Constant(true));
                        timer.Add(1, 4);
                        break;
                        
                    // ********* Jump *********
                    case PrimaryOpCode.JP:
                        yield return Expression.Assign(Xpr.PC, NextWord);
                        timer.Add(3, 10);
                        currentDecodeResult = DecodeResult.Finalize;
                        break;

                    default:
                        throw new NotImplementedException(opCode.ToString());
                }
            }

            // Finalize
            if (currentDecodeResult == DecodeResult.FinalizeAndSync)
            {
                // Increment the program counter by how many bytes were read.
                yield return Expression.Assign(Xpr.PC, Expression.Convert(Expression.Add(Expression.Convert(Xpr.PC, typeof(int)), Expression.Constant(this.mmuCache.TotalBytesRead)), typeof(ushort)));
            }

            // Add the block length to the 7 lsb of memory refresh register.
            var blockLengthExpression = Expression.Constant(this.mmuCache.TotalBytesRead, typeof(int));
            yield return Xpr.GetMemoryRefreshDeltaExpression(blockLengthExpression);

            // Return the dynamic timings.
            var getInstructionTimings = ExpressionHelpers.GetMethodInfo<IInstructionTimer>(dt => dt.GetInstructionTimings());
            var returnTarget = Expression.Label(typeof(InstructionTimings), "InstructionTimings_Return");
            yield return Expression.Return(returnTarget, Expression.Call(Xpr.DynamicTimer, getInstructionTimings), typeof(InstructionTimings));
            yield return Expression.Label(returnTarget, Expression.Constant(default(InstructionTimings)));
        }
        
        private IEnumerable<Expression> TryDecodeNextEdPrefixOperation()
        {
            var opCode = (PrefixEdOpCode)mmuCache.NextByte();
            
            switch (opCode)
            {
                // ********* 8-bit load *********
                // LD A, I
                case PrefixEdOpCode.LD_A_I:
                    yield return Expression.Assign(Xpr.A, Xpr.I);
                    yield return Expression.Call(Xpr.Flags, Xpr.SetResultFlags, Xpr.A);

                    // Also reset H & N and copy IFF2 to P/V
                    yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.ParityOverflow, Xpr.IFF2);
                    timer.Add(2, 9);
                    break;

                // LD A, R
                case PrefixEdOpCode.LD_A_R:
                    yield return Expression.Assign(Xpr.A, Xpr.R);
                    yield return Expression.Call(Xpr.Flags, Xpr.SetResultFlags, Xpr.A);

                    // Also reset H & N and copy IFF2 to P/V
                    yield return Expression.Assign(Xpr.HalfCarry, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.Subtract, Expression.Constant(false));
                    yield return Expression.Assign(Xpr.ParityOverflow, Xpr.IFF2);
                    timer.Add(2, 9);
                    break;

                // LD I, A
                case PrefixEdOpCode.LD_I_A:
                    yield return Expression.Assign(Xpr.I, Xpr.A);
                    timer.Add(2, 9);
                    break;

                // LD R, A
                case PrefixEdOpCode.LD_R_A:
                    yield return Expression.Assign(Xpr.R, Xpr.A);
                    timer.Add(2, 9);
                    break;

                // ********* 16-bit load *********
                // LD dd, (nn)
                case PrefixEdOpCode.LD_BC_mnn:
                    yield return Expression.Assign(Xpr.BC, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_DE_mnn:
                    yield return Expression.Assign(Xpr.DE, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_HL_mnn:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_SP_mnn:
                    yield return Expression.Assign(Xpr.SP, Expression.Call(Xpr.Mmu, Xpr.MmuReadWord, NextWord));
                    timer.Add(6, 20);
                    break;

                // LD (nn), dd
                case PrefixEdOpCode.LD_mnn_BC:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.BC);
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_DE:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.DE);
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_HL:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.HL);
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_SP:
                    yield return Expression.Call(Xpr.Mmu, Xpr.MmuWriteWord, NextWord, Xpr.SP);
                    timer.Add(6, 20);
                    break;


                // ********* Block Transfer *********
                // LDI
                case PrefixEdOpCode.LDI:
                    foreach (var expression in Xpr.GetLdExpressions())
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                // LDIR
                case PrefixEdOpCode.LDIR:
                    foreach (var expression in Xpr.GetLdrExpressions())
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                // LDD
                case PrefixEdOpCode.LDD:
                    foreach (var expression in Xpr.GetLdExpressions(true))
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                // LDDR
                case PrefixEdOpCode.LDDR:
                    foreach (var expression in Xpr.GetLdrExpressions(true))
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                // ********* Search *********
                case PrefixEdOpCode.CPI:
                    foreach (var expression in Xpr.GetCpExpressions())
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPIR:
                    yield return Xpr.GetCprExpression();
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPD:
                    foreach (var expression in Xpr.GetCpExpressions(true))
                    {
                        yield return expression;
                    }
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPDR:
                    yield return Xpr.GetCprExpression(true);
                    timer.Add(4, 16);
                    break;

                // ********* 16-Bit Arithmetic *********
                // ADC HL, ss
                case PrefixEdOpCode.ADC_HL_BC:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, Xpr.HL, Xpr.BC));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.ADC_HL_DE:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, Xpr.HL, Xpr.DE));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.ADC_HL_HL:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, Xpr.HL, Xpr.HL));
                    timer.Add(4, 15);
                    break;
                case PrefixEdOpCode.ADC_HL_SP:
                    yield return Expression.Assign(Xpr.HL, Expression.Call(Xpr.Alu, Xpr.AluAdd16WithCarry, Xpr.HL, Xpr.SP));
                    timer.Add(4, 15);
                    break;

                // ********* General-Purpose Arithmetic *********
                // NEG
                case PrefixEdOpCode.NEG:
                    yield return Expression.Assign(Xpr.A, Expression.Call(Xpr.Alu, Xpr.AluSubtract, Expression.Constant((byte)0), Xpr.A));
                    timer.Add(2, 8);
                    break;

                // IM 0
                case PrefixEdOpCode.IM0:
                    yield return Expression.Assign(Xpr.IM, Expression.Constant(InterruptMode.InterruptMode0));
                    timer.Add(2, 8);
                    break;

                // IM 1
                case PrefixEdOpCode.IM1:
                    yield return Expression.Assign(Xpr.IM, Expression.Constant(InterruptMode.InterruptMode1));
                    timer.Add(2, 8);
                    break;

                // IM 2
                case PrefixEdOpCode.IM2:
                    yield return Expression.Assign(Xpr.IM, Expression.Constant(InterruptMode.InterruptMode2));
                    timer.Add(2, 8);
                    break;

                default:
                    throw new NotImplementedException(opCode.ToString());
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
