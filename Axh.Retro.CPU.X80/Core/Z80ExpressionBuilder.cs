namespace Axh.Retro.CPU.X80.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Util;

    internal class Z80ExpressionBuilder
    {
        private static readonly ParameterExpression Registers;
        private static readonly ParameterExpression Mmu;
        private static readonly ParameterExpression Alu;

        /// <summary>
        /// Byte parameter b
        /// </summary>
        private static readonly ParameterExpression LocalByte;

        /// <summary>
        /// Word parameter w
        /// </summary>
        private static readonly ParameterExpression LocalWord;

        /// <summary>
        /// The dynamic instruction timer that is updated at runtime.
        /// This is required for instructions that don't have compile time known timings e.g. LDIR.
        /// </summary>
        private static readonly ParameterExpression DynamicTimer;
        private static readonly MethodInfo DynamicTimerAdd;

        // Register expressions
        private static readonly Expression A;
        private static readonly Expression B;
        private static readonly Expression C;
        private static readonly Expression D;
        private static readonly Expression E;
        private static readonly Expression F;
        private static readonly Expression H;
        private static readonly Expression L;
        private static readonly Expression BC;
        private static readonly Expression DE;
        private static readonly Expression HL;
        private static readonly Expression PC;

        // Stack pointer stuff
        private static readonly Expression SP;
        private static readonly Expression PushSP;
        private static readonly Expression PushPushSP;
        private static readonly Expression PopSP;
        private static readonly Expression PopPopSP;

        // Z80 specific register expressions
        private static readonly Expression I;
        private static readonly Expression R;
        private static readonly Expression IX;
        private static readonly Expression IY;

        // Interrupt stuff
        private static readonly Expression IFF1;
        private static readonly Expression IFF2;

        // Flags
        private static readonly Expression Flags;
        private static readonly Expression Sign;
        private static readonly Expression Zero;
        private static readonly Expression Flag5;
        private static readonly Expression HalfCarry;
        private static readonly Expression Flag3;
        private static readonly Expression ParityOverflow;
        private static readonly Expression Subtract;
        private static readonly Expression Carry;
        private static readonly MethodInfo SetResultFlags;

        // Register methods
        private static readonly Expression SwitchToAlternativeGeneralPurposeRegisters;
        private static readonly Expression SwitchToAlternativeAccumulatorAndFlagsRegisters;

        /// <summary>
        /// IX + d
        /// d is read from LocalByte
        /// </summary>
        private static readonly Expression IXd;

        /// <summary>
        /// IY + d
        /// d is read from LocalByte
        /// </summary>
        private static readonly Expression IYd;

        /// <summary>
        /// Reads a byte from the mmu at the address at LocalWord
        /// </summary>
        private static readonly Expression ReadByteAtLocalWord;

        /// <summary>
        /// Reads a byte from the mmu at the address in HL
        /// </summary>
        private static readonly Expression ReadByteAtHL;

        /// <summary>
        /// Reads a byte from the mmu at the address in BC
        /// </summary>
        private static readonly Expression ReadByteAtBC;

        /// <summary>
        /// Reads a byte from the mmu at the address in DE
        /// </summary>
        private static readonly Expression ReadByteAtDE;

        /// <summary>
        /// Reads a byte from the mmu at the address at IX + b (using 2's compliant addition)
        /// </summary>
        private static readonly Expression ReadByteAtIXd;
        
        /// <summary>
        /// Reads a byte from the mmu at the address at IY + b (using 2's compliant addition)
        /// </summary>
        private static readonly Expression ReadByteAtIYd;

        // MMU methods
        private static readonly MethodInfo MmuReadByte;
        private static readonly MethodInfo MmuReadWord;
        private static readonly MethodInfo MmuWriteByte;
        private static readonly MethodInfo MmuWriteWord;
        private static readonly MethodInfo MmuTransferByte;

        // ALU methods
        private static readonly MethodInfo AluIncrement;
        private static readonly MethodInfo AluDecrement;
        private static readonly MethodInfo AluAdd;
        private static readonly MethodInfo AluAddWithCarry;
        private static readonly MethodInfo AluSubtract;
        private static readonly MethodInfo AluSubtractWithCarry;
        private static readonly MethodInfo AluCompare;

        static Z80ExpressionBuilder()
        {
            Registers = Expression.Parameter(typeof(IZ80Registers), "registers");
            Mmu = Expression.Parameter(typeof(IMmu), "mmu");
            Alu = Expression.Parameter(typeof(IArithmeticLogicUnit), "alu");
            LocalByte = Expression.Parameter(typeof(byte), "b");
            LocalWord = Expression.Parameter(typeof(ushort), "w");
            DynamicTimer = Expression.Parameter(typeof(IInstructionTimer), "timer");
            DynamicTimerAdd = ExpressionHelpers.GetMethodInfo<IInstructionTimer, int, int>((dt, m, t) => dt.Add(m, t));
            
            // General purpose register expressions
            var generalPurposeRegisters = Registers.GetPropertyExpression<IZ80Registers, IGeneralPurposeRegisterSet>(r => r.GeneralPurposeRegisters);
            B = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.B);
            C = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.C);
            D = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.D);
            E = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.E);
            H = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.H);
            L = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.L);
            BC = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, ushort>(r => r.BC);
            DE = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, ushort>(r => r.DE);
            HL = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, ushort>(r => r.HL);
            PC = Registers.GetPropertyExpression<IZ80Registers, ushort>(r => r.ProgramCounter);

            // Stack pointer stuff
            SP = Registers.GetPropertyExpression<IZ80Registers, ushort>(r => r.StackPointer);
            PushSP = Expression.PreDecrementAssign(SP);
            PushPushSP = Expression.SubtractAssign(SP, Expression.Constant((ushort)2));
            PopSP = Expression.PreIncrementAssign(SP);
            PopPopSP = Expression.AddAssign(SP, Expression.Constant((ushort)2));

            // Z80 specific register expressions
            I = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.I);
            R = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.R);
            IX = Registers.GetPropertyExpression<IZ80Registers, ushort>(r => r.IX);
            IY = Registers.GetPropertyExpression<IZ80Registers, ushort>(r => r.IY);

            // Interrupt stuff
            IFF1 = Registers.GetPropertyExpression<IZ80Registers, bool>(r => r.InterruptFlipFlop1);
            IFF2 = Registers.GetPropertyExpression<IZ80Registers, bool>(r => r.InterruptFlipFlop2);

            // Accumulator & Flags register expressions
            var accumulatorAndFlagsRegisters = Registers.GetPropertyExpression<IZ80Registers, IAccumulatorAndFlagsRegisterSet>(r => r.AccumulatorAndFlagsRegisters);
            A = accumulatorAndFlagsRegisters.GetPropertyExpression<IAccumulatorAndFlagsRegisterSet, byte>(r => r.A);
            Flags = accumulatorAndFlagsRegisters.GetPropertyExpression<IAccumulatorAndFlagsRegisterSet, IFlagsRegister>(r => r.Flags);
            F = Flags.GetPropertyExpression<IFlagsRegister, byte>(r => r.Register);
            Sign = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Sign);
            Zero = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Zero);
            Flag5 = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Flag5);
            HalfCarry = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.HalfCarry);
            Flag3 = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Flag3);
            ParityOverflow = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.ParityOverflow);
            Subtract = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Subtract);
            Carry = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Carry);
            SetResultFlags = ExpressionHelpers.GetMethodInfo<IFlagsRegister, byte>((flags, result) => flags.SetResultFlags(result));
            
            // Index register expressions i.e. IX+d and IY+d where d is LocalByte (expression local value, which must be initialised before running these)
            IXd = Expression.Convert(Expression.Add(Expression.Convert(IX, typeof(int)), Expression.Convert(Expression.Convert(LocalByte, typeof(sbyte)), typeof(int))), typeof(ushort));
            IYd = Expression.Convert(Expression.Add(Expression.Convert(IY, typeof(int)), Expression.Convert(Expression.Convert(LocalByte, typeof(sbyte)), typeof(int))), typeof(ushort));

            // Register methods
            SwitchToAlternativeGeneralPurposeRegisters = Expression.Call(Registers, ExpressionHelpers.GetMethodInfo<IZ80Registers>((registers) => registers.SwitchToAlternativeGeneralPurposeRegisters()));
            SwitchToAlternativeAccumulatorAndFlagsRegisters = Expression.Call(Registers, ExpressionHelpers.GetMethodInfo<IZ80Registers>((registers) => registers.SwitchToAlternativeAccumulatorAndFlagsRegisters()));
            
            // MMU expressions
            MmuReadByte = ExpressionHelpers.GetMethodInfo<IMmu, ushort, byte>((mmu, address) => mmu.ReadByte(address));
            MmuReadWord = ExpressionHelpers.GetMethodInfo<IMmu, ushort, ushort>((mmu, address) => mmu.ReadWord(address));
            MmuWriteByte = ExpressionHelpers.GetMethodInfo<IMmu, ushort, byte>((mmu, address, value) => mmu.WriteByte(address, value));
            MmuWriteWord = ExpressionHelpers.GetMethodInfo<IMmu, ushort, ushort>((mmu, address, value) => mmu.WriteWord(address, value));
            MmuTransferByte = ExpressionHelpers.GetMethodInfo<IMmu, ushort, ushort>((mmu, addressFrom, addressTo) => mmu.TransferByte(addressFrom, addressTo));

            ReadByteAtLocalWord = Expression.Call(Mmu, MmuReadByte, LocalWord);
            ReadByteAtHL = Expression.Call(Mmu, MmuReadByte, HL);
            ReadByteAtBC = Expression.Call(Mmu, MmuReadByte, BC);
            ReadByteAtDE = Expression.Call(Mmu, MmuReadByte, DE);
            ReadByteAtIXd = Expression.Call(Mmu, MmuReadByte, IXd);
            ReadByteAtIYd = Expression.Call(Mmu, MmuReadByte, IYd);

            // ALU expressions
            AluIncrement = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, b) => alu.Increment(b));
            AluDecrement = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, b) => alu.Decrement(b));
            AluAdd = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte, byte>((alu, a, b) => alu.Add(a, b));
            AluAddWithCarry = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte, byte>((alu, a, b) => alu.AddWithCarry(a, b));
            AluSubtract = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte, byte>((alu, a, b) => alu.Subtract(a, b));
            AluSubtractWithCarry = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte, byte>((alu, a, b) => alu.SubtractWithCarry(a, b));
            AluCompare = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a, b) => alu.Compare(a, b));
        }

        private readonly ICollection<Expression> expressions;

        private readonly IInstructionTimer timer;

        private readonly IMmuCache mmuCache;

        private ConstantExpression NextByte => Expression.Constant(mmuCache.NextByte());
        private ConstantExpression NextWord => Expression.Constant(mmuCache.NextWord(), typeof(ushort));

        public Z80ExpressionBuilder(IMmuCache mmuCache, IInstructionTimer timer)
        {
            this.expressions = new List<Expression> { Expression.Assign(DynamicTimer, Expression.New(typeof(InstructionTimer))) };
            this.timer = timer;
            this.mmuCache = mmuCache;
        }

        public Expression<Func<IZ80Registers, IMmu, IArithmeticLogicUnit, InstructionTimings>> FinalizeBlock(DecodeResult lastResult)
        {
            if (lastResult == DecodeResult.FinalizeAndSync)
            {
                // Increment the program counter by how many bytes were read.
                var expression = Expression.Assign(PC, Expression.Convert(Expression.Add(Expression.Convert(PC, typeof(int)), Expression.Constant(this.mmuCache.TotalBytesRead)), typeof(ushort)));
                this.expressions.Add(expression);
            }

            // Add the block length to the 7 lsb of memory refresh register.
            var blockLengthExpression = Expression.Constant(this.mmuCache.TotalBytesRead, typeof(int));
            this.expressions.Add(GetMemoryRefreshDeltaExpression(blockLengthExpression));
            
            var getInstructionTimings = ExpressionHelpers.GetMethodInfo<IInstructionTimer>(dt => dt.GetInstructionTimings());
            var returnTarget = Expression.Label(typeof(InstructionTimings), "InstructionTimings_Return");
            var returnLabel = Expression.Label(returnTarget, Expression.Constant(default(InstructionTimings)));
            this.expressions.Add(Expression.Return(returnTarget, Expression.Call(DynamicTimer, getInstructionTimings), typeof(InstructionTimings)));
            this.expressions.Add(returnLabel);

            var expressionBlock = Expression.Block(new[] { LocalByte, LocalWord, DynamicTimer }, this.expressions);
            var lambda = Expression.Lambda<Func<IZ80Registers, IMmu, IArithmeticLogicUnit, InstructionTimings>>(expressionBlock, Registers, Mmu, Alu);

            return lambda;
        }

        private static Expression GetMemoryRefreshDeltaExpression(Expression deltaExpression)
        {
            var increment7LsbR = Expression.And(Expression.Add(Expression.Convert(R, typeof(int)), deltaExpression), Expression.Constant(0x7f));
            return Expression.Assign(R, Expression.Convert(increment7LsbR, typeof(byte)));
        }
        
        public DecodeResult TryDecodeNextOperation()
        {
            var opCode = (PrimaryOpCode)this.mmuCache.NextByte();

            switch (opCode)
            {
                case PrimaryOpCode.NOP:
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.HALT:
                    timer.Add(1, 4);
                    return DecodeResult.FinalizeAndSync;

                // ********* 8-bit load *********
                // LD r, r'
                case PrimaryOpCode.LD_A_A:
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_B_A:
                    expressions.Add(Expression.Assign(B, A));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_C_A:
                    expressions.Add(Expression.Assign(C, A));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_D_A:
                    expressions.Add(Expression.Assign(D, A));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_E_A:
                    expressions.Add(Expression.Assign(E, A));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_H_A:
                    expressions.Add(Expression.Assign(H, A));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_A:
                    expressions.Add(Expression.Assign(L, A));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_A_B:
                    expressions.Add(Expression.Assign(A, B));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_B_B:
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_C_B:
                    expressions.Add(Expression.Assign(C, B));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_D_B:
                    expressions.Add(Expression.Assign(D, B));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_E_B:
                    expressions.Add(Expression.Assign(E, B));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_H_B:
                    expressions.Add(Expression.Assign(H, B));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_B:
                    expressions.Add(Expression.Assign(L, B));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_A_C:
                    expressions.Add(Expression.Assign(A, C));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_B_C:
                    expressions.Add(Expression.Assign(B, C));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_C_C:
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_D_C:
                    expressions.Add(Expression.Assign(D, C));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_E_C:
                    expressions.Add(Expression.Assign(E, C));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_H_C:
                    expressions.Add(Expression.Assign(H, C));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_C:
                    expressions.Add(Expression.Assign(L, C));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_A_D:
                    expressions.Add(Expression.Assign(A, D));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_B_D:
                    expressions.Add(Expression.Assign(B, D));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_C_D:
                    expressions.Add(Expression.Assign(C, D));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_D_D:
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_E_D:
                    expressions.Add(Expression.Assign(E, D));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_H_D:
                    expressions.Add(Expression.Assign(H, D));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_D:
                    expressions.Add(Expression.Assign(L, D));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_A_E:
                    expressions.Add(Expression.Assign(A, E));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_B_E:
                    expressions.Add(Expression.Assign(B, E));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_C_E:
                    expressions.Add(Expression.Assign(C, E));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_D_E:
                    expressions.Add(Expression.Assign(D, E));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_E_E:
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_H_E:
                    expressions.Add(Expression.Assign(H, E));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_E:
                    expressions.Add(Expression.Assign(L, E));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_A_H:
                    expressions.Add(Expression.Assign(A, H));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_B_H:
                    expressions.Add(Expression.Assign(B, H));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_C_H:
                    expressions.Add(Expression.Assign(C, H));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_D_H:
                    expressions.Add(Expression.Assign(D, H));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_E_H:
                    expressions.Add(Expression.Assign(E, H));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_H_H:
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_H:
                    expressions.Add(Expression.Assign(L, H));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_A_L:
                    expressions.Add(Expression.Assign(A, L));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_B_L:
                    expressions.Add(Expression.Assign(B, L));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_C_L:
                    expressions.Add(Expression.Assign(C, L));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_D_L:
                    expressions.Add(Expression.Assign(D, L));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_E_L:
                    expressions.Add(Expression.Assign(E, L));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_H_L:
                    expressions.Add(Expression.Assign(H, L));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_L:
                    timer.Add(1, 4);
                    break;

                // LD r,n
                case PrimaryOpCode.LD_A_n:
                    expressions.Add(Expression.Assign(A, NextByte));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_B_n:
                    expressions.Add(Expression.Assign(B, NextByte));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_C_n:
                    expressions.Add(Expression.Assign(C, NextByte));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_D_n:
                    expressions.Add(Expression.Assign(D, NextByte));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_E_n:
                    expressions.Add(Expression.Assign(E, NextByte));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_H_n:
                    expressions.Add(Expression.Assign(H, NextByte));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_L_n:
                    expressions.Add(Expression.Assign(L, NextByte));
                    timer.Add(2, 7);
                    break;

                // LD r, (HL)
                case PrimaryOpCode.LD_A_mHL:
                    expressions.Add(Expression.Assign(A, ReadByteAtHL));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_B_mHL:
                    expressions.Add(Expression.Assign(B, ReadByteAtHL));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_C_mHL:
                    expressions.Add(Expression.Assign(C, ReadByteAtHL));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_D_mHL:
                    expressions.Add(Expression.Assign(D, ReadByteAtHL));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_E_mHL:
                    expressions.Add(Expression.Assign(E, ReadByteAtHL));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_H_mHL:
                    expressions.Add(Expression.Assign(H, ReadByteAtHL));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_L_mHL:
                    expressions.Add(Expression.Assign(L, ReadByteAtHL));
                    timer.Add(2, 7);
                    break;

                // LD (HL), r
                case PrimaryOpCode.LD_mHL_A:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, HL, A));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_B:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, HL, B));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_C:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, HL, C));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_D:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, HL, D));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_E:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, HL, E));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_H:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, HL, H));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_L:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, HL, L));
                    timer.Add(2, 7);
                    break;

                // LD (HL), n
                case PrimaryOpCode.LD_mHL_n:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, HL, NextByte));
                    timer.Add(3, 10);
                    break;

                // LD A, (BC)
                case PrimaryOpCode.LD_A_mBC:
                    expressions.Add(Expression.Assign(A, ReadByteAtBC));
                    timer.Add(2, 7);
                    break;

                // LD A, (DE)
                case PrimaryOpCode.LD_A_mDE:
                    expressions.Add(Expression.Assign(A, ReadByteAtDE));
                    timer.Add(2, 7);
                    break;

                // LD A, (nn)
                case PrimaryOpCode.LD_A_mnn:
                    expressions.Add(Expression.Assign(A, Expression.Call(Mmu, MmuReadByte, NextWord)));
                    timer.Add(4, 13);
                    break;

                // LD (BC), A
                case PrimaryOpCode.LD_mBC_A:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, BC, A));
                    timer.Add(2, 7);
                    break;

                // LD (DE), A
                case PrimaryOpCode.LD_mDE_A:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, DE, A));
                    timer.Add(2, 7);
                    break;

                // LD (nn), A
                case PrimaryOpCode.LD_mnn_A:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, NextWord, A));
                    timer.Add(2, 7);
                    break;

                // ********* 16-bit load *********
                // LD dd, nn
                case PrimaryOpCode.LD_BC_nn:
                    expressions.Add(Expression.Assign(BC, NextWord));
                    timer.Add(2, 10);
                    break;
                case PrimaryOpCode.LD_DE_nn:
                    expressions.Add(Expression.Assign(DE, NextWord));
                    timer.Add(2, 10);
                    break;
                case PrimaryOpCode.LD_HL_nn:
                    expressions.Add(Expression.Assign(HL, NextWord));
                    timer.Add(2, 10);
                    break;
                case PrimaryOpCode.LD_SP_nn:
                    expressions.Add(Expression.Assign(SP, NextWord));
                    timer.Add(2, 10);
                    break;

                // LD HL, (nn)
                case PrimaryOpCode.LD_HL_mnn:
                    expressions.Add(Expression.Assign(HL, Expression.Call(Mmu, MmuReadWord, NextWord)));
                    timer.Add(5, 16);
                    break;

                // LD (nn), HL
                case PrimaryOpCode.LD_mnn_HL:
                    expressions.Add(Expression.Call(Mmu, MmuWriteWord, NextWord, HL));
                    timer.Add(6, 20);
                    break;

                // LD SP, HL
                case PrimaryOpCode.LD_SP_HL:
                    expressions.Add(Expression.Assign(SP, HL));
                    timer.Add(1, 6);
                    break;

                // PUSH qq
                case PrimaryOpCode.PUSH_BC:
                    expressions.Add(PushSP);
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, SP, B));
                    expressions.Add(PushSP);
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, SP, C));
                    timer.Add(3, 11);
                    break;
                case PrimaryOpCode.PUSH_DE:
                    expressions.Add(PushSP);
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, SP, D));
                    expressions.Add(PushSP);
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, SP, E));
                    timer.Add(3, 11);
                    break;
                case PrimaryOpCode.PUSH_HL:
                    expressions.Add(PushSP);
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, SP, H));
                    expressions.Add(PushSP);
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, SP, L));
                    timer.Add(3, 11);
                    break;
                case PrimaryOpCode.PUSH_AF:
                    expressions.Add(PushSP);
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, SP, A));
                    expressions.Add(PushSP);
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, SP, F));
                    timer.Add(3, 11);
                    break;

                // POP qq
                case PrimaryOpCode.POP_BC:
                    expressions.Add(Expression.Assign(C, Expression.Call(Mmu, MmuReadByte, SP)));
                    expressions.Add(PopSP);
                    expressions.Add(Expression.Assign(B, Expression.Call(Mmu, MmuReadByte, SP)));
                    expressions.Add(PopSP);
                    timer.Add(3, 10);
                    break;
                case PrimaryOpCode.POP_DE:
                    expressions.Add(Expression.Assign(E, Expression.Call(Mmu, MmuReadByte, SP)));
                    expressions.Add(PopSP);
                    expressions.Add(Expression.Assign(D, Expression.Call(Mmu, MmuReadByte, SP)));
                    expressions.Add(PopSP);
                    timer.Add(3, 10);
                    break;
                case PrimaryOpCode.POP_HL:
                    expressions.Add(Expression.Assign(L, Expression.Call(Mmu, MmuReadByte, SP)));
                    expressions.Add(PopSP);
                    expressions.Add(Expression.Assign(H, Expression.Call(Mmu, MmuReadByte, SP)));
                    expressions.Add(PopSP);
                    timer.Add(3, 10);
                    break;
                case PrimaryOpCode.POP_AF:
                    expressions.Add(Expression.Assign(F, Expression.Call(Mmu, MmuReadByte, SP)));
                    expressions.Add(PopSP);
                    expressions.Add(Expression.Assign(A, Expression.Call(Mmu, MmuReadByte, SP)));
                    expressions.Add(PopSP);
                    timer.Add(3, 10);
                    break;

                // ********* Exchange *********
                // EX DE, HL
                case PrimaryOpCode.EX_DE_HL:
                    expressions.Add(Expression.Assign(LocalWord, DE));
                    expressions.Add(Expression.Assign(DE, HL));
                    expressions.Add(Expression.Assign(HL, LocalWord));
                    timer.Add(1, 4);
                    break;

                // EX AF, AF′
                case PrimaryOpCode.EX_AF:
                    expressions.Add(SwitchToAlternativeAccumulatorAndFlagsRegisters);
                    timer.Add(1, 4);
                    break;

                // EXX
                case PrimaryOpCode.EXX:
                    expressions.Add(SwitchToAlternativeGeneralPurposeRegisters);
                    timer.Add(1, 4);
                    break;

                // EX (SP), HL
                case PrimaryOpCode.EX_mSP_HL:
                    // Exchange L
                    expressions.Add(Expression.Assign(LocalByte, L));
                    expressions.Add(Expression.Assign(L, Expression.Call(Mmu, MmuReadByte, SP)));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, SP, LocalByte));

                    // Exchange H
                    expressions.Add(Expression.Assign(LocalByte, H));
                    expressions.Add(Expression.Assign(LocalWord, Expression.Increment(SP)));
                    expressions.Add(Expression.Assign(H, Expression.Call(Mmu, MmuReadByte, LocalWord)));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, LocalWord, LocalByte));
                    
                    timer.Add(5, 19);
                    break;
                    
                // ********* Jump *********
                case PrimaryOpCode.JP:
                    expressions.Add(Expression.Assign(PC, NextWord));
                    timer.Add(3, 10);
                    return DecodeResult.Finalize;

                // ********* Prefixes *********
                case PrimaryOpCode.Prefix_DD:
                    return TryDecodeNextDdPrefixOperation();

                case PrimaryOpCode.Prefix_FD:
                    return TryDecodeNextFdPrefixOperation();

                case PrimaryOpCode.Prefix_ED:
                    return TryDecodeNextEdPrefixOperation();

                default:
                    throw new NotImplementedException(opCode.ToString());
            }

            return DecodeResult.Continue;
        }
        
        public DecodeResult TryDecodeNextDdPrefixOperation()
        {
            var opCode = (PrefixDdFdOpCode)mmuCache.NextByte();

            switch (opCode)
            {
                // ********* Stray Prefixes (same timings as NOP) *********
                case PrefixDdFdOpCode.Prefix_DD:
                    timer.Add(1, 4);
                    return TryDecodeNextDdPrefixOperation();

                case PrefixDdFdOpCode.Prefix_FD:
                    timer.Add(1, 4);
                    return TryDecodeNextFdPrefixOperation();
                    
                case PrefixDdFdOpCode.Prefix_ED:
                    timer.Add(1, 4);
                    return TryDecodeNextEdPrefixOperation();

                // ********* 8-bit load *********
                // LD r, (IX+d)
                // We have defined this using ReadByteAtIXd for when we set the local parameter b to d.
                case PrefixDdFdOpCode.LD_A_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(A, ReadByteAtIXd));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_B_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(B, ReadByteAtIXd));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_C_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(C, ReadByteAtIXd));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_D_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(D, ReadByteAtIXd));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_E_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(E, ReadByteAtIXd));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_H_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(H, ReadByteAtIXd));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_L_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(L, ReadByteAtIXd));
                    timer.Add(5, 19);
                    break;

                // LD (IX+d), r
                case PrefixDdFdOpCode.LD_mIXYd_A:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IXd, A));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_B:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IXd, B));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_C:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IXd, C));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_D:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IXd, D));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_E:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IXd, E));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_H:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IXd, H));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_L:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IXd, L));
                    timer.Add(5, 19);
                    break;

                // LD (IX+d), n
                case PrefixDdFdOpCode.LD_mIXYd_n:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IXd, NextByte));
                    timer.Add(5, 19);
                    break;

                // ********* 16-bit load *********
                // LD IX, nn
                case PrefixDdFdOpCode.LD_IXY_nn:
                    expressions.Add(Expression.Assign(IX, NextWord));
                    timer.Add(4, 14);
                    break;

                // LD IX, (nn)
                case PrefixDdFdOpCode.LD_IXY_mnn:
                    expressions.Add(Expression.Assign(IX, Expression.Call(Mmu, MmuReadWord, NextWord)));
                    timer.Add(6, 20);
                    break;

                // LD (nn), IX
                case PrefixDdFdOpCode.LD_mnn_IXY:
                    expressions.Add(Expression.Call(Mmu, MmuWriteWord, NextWord, IX));
                    timer.Add(6, 20);
                    break;

                // LD SP, IX
                case PrefixDdFdOpCode.LD_SP_IXY:
                    expressions.Add(Expression.Assign(SP, IX));
                    timer.Add(2, 10);
                    break;

                // PUSH IX
                case PrefixDdFdOpCode.PUSH_IXY:
                    expressions.Add(PushPushSP);
                    expressions.Add(Expression.Call(Mmu, MmuWriteWord, SP, IX));
                    timer.Add(4, 15);
                    break;

                // POP IX
                case PrefixDdFdOpCode.POP_IXY:
                    expressions.Add(Expression.Assign(IX, Expression.Call(Mmu, MmuReadWord, SP)));
                    expressions.Add(PopPopSP);
                    timer.Add(4, 14);
                    break;

                // ********* Exchange *********
                // EX (SP), IX
                case PrefixDdFdOpCode.EX_mSP_IXY:
                    // Read temp word at SP
                    expressions.Add(Expression.Assign(LocalWord, Expression.Call(Mmu, MmuReadWord, SP)));

                    // Write IX at SP
                    expressions.Add(Expression.Call(Mmu, MmuWriteWord, SP, IX));

                    // Assign IX to temp word
                    expressions.Add(Expression.Assign(IX, LocalWord));
                    timer.Add(6, 23);
                    break;

                default:
                    throw new NotImplementedException(opCode.ToString());
            }

            return DecodeResult.Continue;
        }

        public DecodeResult TryDecodeNextFdPrefixOperation()
        {
            var opCode = (PrefixDdFdOpCode)mmuCache.NextByte();

            switch (opCode)
            {
                // ********* Stray Prefixes (same timings as NOP) *********
                case PrefixDdFdOpCode.Prefix_DD:
                    timer.Add(1, 4);
                    return TryDecodeNextDdPrefixOperation();

                case PrefixDdFdOpCode.Prefix_FD:
                    timer.Add(1, 4);
                    return TryDecodeNextFdPrefixOperation();

                case PrefixDdFdOpCode.Prefix_ED:
                    timer.Add(1, 4);
                    return TryDecodeNextEdPrefixOperation();

                // ********* 8-bit load *********
                // LD r, (IY+d)
                // We have defined this using ReadByteByIxIndexRegisterExpression for when we set the local parameter b to d.
                case PrefixDdFdOpCode.LD_A_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(A, ReadByteAtIYd));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_B_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(B, ReadByteAtIYd));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_C_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(C, ReadByteAtIYd));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_D_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(D, ReadByteAtIYd));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_E_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(E, ReadByteAtIYd));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_H_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(H, ReadByteAtIYd));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_L_mIXYd:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Assign(L, ReadByteAtIYd));
                    timer.Add(5, 19);
                    break;
                    
                // LD (IY+d), r
                case PrefixDdFdOpCode.LD_mIXYd_A:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IYd, A));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_B:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IYd, B));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_C:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IYd, C));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_D:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IYd, D));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_E:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IYd, E));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_H:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IYd, H));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_L:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IYd, L));
                    timer.Add(5, 19);
                    break;
                    
                // LD (IX+d), n
                case PrefixDdFdOpCode.LD_mIXYd_n:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, IYd, NextByte));
                    timer.Add(5, 19);
                    break;

                // ********* 16-bit load *********
                // LD IY, nn
                case PrefixDdFdOpCode.LD_IXY_nn:
                    expressions.Add(Expression.Assign(IY, NextWord));
                    timer.Add(4, 14);
                    break;

                // LD IY, (nn)
                case PrefixDdFdOpCode.LD_IXY_mnn:
                    expressions.Add(Expression.Assign(IY, Expression.Call(Mmu, MmuReadWord, NextWord)));
                    timer.Add(6, 20);
                    break;

                // LD (nn), IY
                case PrefixDdFdOpCode.LD_mnn_IXY:
                    expressions.Add(Expression.Call(Mmu, MmuWriteWord, NextWord, IY));
                    timer.Add(6, 20);
                    break;

                // LD SP, IY
                case PrefixDdFdOpCode.LD_SP_IXY:
                    expressions.Add(Expression.Assign(SP, IY));
                    timer.Add(2, 10);
                    break;
                    
                // PUSH IY
                case PrefixDdFdOpCode.PUSH_IXY:
                    expressions.Add(PushPushSP);
                    expressions.Add(Expression.Call(Mmu, MmuWriteWord, SP, IY));
                    timer.Add(4, 15);
                    break;
                    
                // POP IY
                case PrefixDdFdOpCode.POP_IXY:
                    expressions.Add(Expression.Assign(IY, Expression.Call(Mmu, MmuReadWord, SP)));
                    expressions.Add(PopPopSP);
                    timer.Add(4, 14);
                    break;

                // ********* Exchange *********
                // EX (SP), IY
                case PrefixDdFdOpCode.EX_mSP_IXY:
                    // Read temp word at SP
                    expressions.Add(Expression.Assign(LocalWord, Expression.Call(Mmu, MmuReadWord, SP)));

                    // Write IY at SP
                    expressions.Add(Expression.Call(Mmu, MmuWriteWord, SP, IY));

                    // Assign IY to temp word
                    expressions.Add(Expression.Assign(IY, LocalWord));
                    timer.Add(6, 23);
                    break;

                default:
                    throw new NotImplementedException(opCode.ToString());
            }

            return DecodeResult.Continue;
        }

        public DecodeResult TryDecodeNextEdPrefixOperation()
        {
            var opCode = (PrefixEdOpCode)mmuCache.NextByte();
            
            switch (opCode)
            {
                // ********* 8-bit load *********
                // LD A, I
                case PrefixEdOpCode.LD_A_I:
                    expressions.Add(Expression.Assign(A, I));
                    expressions.Add(Expression.Call(Flags, SetResultFlags, A));

                    // Also reset H & N and copy IFF2 to P/V
                    expressions.Add(Expression.Assign(HalfCarry, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(Subtract, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(ParityOverflow, IFF2));
                    timer.Add(2, 9);
                    break;

                // LD A, R
                case PrefixEdOpCode.LD_A_R:
                    expressions.Add(Expression.Assign(A, R));
                    expressions.Add(Expression.Call(Flags, SetResultFlags, A));

                    // Also reset H & N and copy IFF2 to P/V
                    expressions.Add(Expression.Assign(HalfCarry, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(Subtract, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(ParityOverflow, IFF2));
                    timer.Add(2, 9);
                    break;

                // LD I, A
                case PrefixEdOpCode.LD_I_A:
                    expressions.Add(Expression.Assign(I, A));
                    timer.Add(2, 9);
                    break;

                // LD R, A
                case PrefixEdOpCode.LD_R_A:
                    expressions.Add(Expression.Assign(R, A));
                    timer.Add(2, 9);
                    break;

                // ********* 16-bit load *********
                // LD dd, (nn)
                case PrefixEdOpCode.LD_BC_mnn:
                    expressions.Add(Expression.Assign(BC, Expression.Call(Mmu, MmuReadWord, NextWord)));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_DE_mnn:
                    expressions.Add(Expression.Assign(DE, Expression.Call(Mmu, MmuReadWord, NextWord)));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_HL_mnn:
                    expressions.Add(Expression.Assign(HL, Expression.Call(Mmu, MmuReadWord, NextWord)));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_SP_mnn:
                    expressions.Add(Expression.Assign(SP, Expression.Call(Mmu, MmuReadWord, NextWord)));
                    timer.Add(6, 20);
                    break;

                // LD (nn), dd
                case PrefixEdOpCode.LD_mnn_BC:
                    expressions.Add(Expression.Call(Mmu, MmuWriteWord, NextWord, BC));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_DE:
                    expressions.Add(Expression.Call(Mmu, MmuWriteWord, NextWord, DE));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_HL:
                    expressions.Add(Expression.Call(Mmu, MmuWriteWord, NextWord, HL));
                    timer.Add(6, 20);
                    break;
                case PrefixEdOpCode.LD_mnn_SP:
                    expressions.Add(Expression.Call(Mmu, MmuWriteWord, NextWord, SP));
                    timer.Add(6, 20);
                    break;


                // ********* Block Transfer *********
                // LDI
                case PrefixEdOpCode.LDI:
                    expressions.Add(Expression.Call(Mmu, MmuTransferByte, HL, DE));
                    expressions.Add(Expression.PreIncrementAssign(HL));
                    expressions.Add(Expression.PreIncrementAssign(DE));
                    expressions.Add(Expression.PreDecrementAssign(BC));
                    expressions.Add(Expression.Assign(HalfCarry, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(ParityOverflow, Expression.NotEqual(BC, Expression.Constant((ushort)0))));
                    expressions.Add(Expression.Assign(Subtract, Expression.Constant(false)));
                    timer.Add(4, 16);
                    break;

                // LDIR
                case PrefixEdOpCode.LDIR:
                    {
                        var breakLabel = Expression.Label();
                        expressions.Add(
                            Expression.Loop(
                                Expression.Block(
                                    Expression.Call(Mmu, MmuTransferByte, HL, DE),
                                    Expression.PreIncrementAssign(HL),
                                    Expression.PreIncrementAssign(DE),
                                    Expression.PreDecrementAssign(BC),
                                    Expression.IfThen(Expression.Equal(BC, Expression.Constant((ushort)0)), Expression.Break(breakLabel)),
                                    Expression.Call(DynamicTimer, DynamicTimerAdd, Expression.Constant(5), Expression.Constant(21)),
                                    GetMemoryRefreshDeltaExpression(Expression.Constant(2))), // This function actually decreases the PC by two for each 'loop' hence need more refresh cycles.
                                breakLabel));
                    }

                    expressions.Add(Expression.Assign(HalfCarry, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(ParityOverflow, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(Subtract, Expression.Constant(false)));

                    timer.Add(4, 16);
                    break;

                // LDD
                case PrefixEdOpCode.LDD:
                    expressions.Add(Expression.Call(Mmu, MmuTransferByte, HL, DE));
                    expressions.Add(Expression.PreDecrementAssign(HL));
                    expressions.Add(Expression.PreDecrementAssign(DE));
                    expressions.Add(Expression.PreDecrementAssign(BC));
                    expressions.Add(Expression.Assign(HalfCarry, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(ParityOverflow, Expression.NotEqual(BC, Expression.Constant((ushort)0))));
                    expressions.Add(Expression.Assign(Subtract, Expression.Constant(false)));
                    timer.Add(4, 16);
                    break;

                // LDDR
                case PrefixEdOpCode.LDDR:
                    {
                        var breakLabel = Expression.Label();
                        expressions.Add(
                            Expression.Loop(
                                Expression.Block(
                                    Expression.Call(Mmu, MmuTransferByte, HL, DE),
                                    Expression.PreDecrementAssign(HL),
                                    Expression.PreDecrementAssign(DE),
                                    Expression.PreDecrementAssign(BC),
                                    Expression.IfThen(Expression.Equal(BC, Expression.Constant((ushort)0)), Expression.Break(breakLabel)),
                                    Expression.Call(DynamicTimer, DynamicTimerAdd, Expression.Constant(5), Expression.Constant(21)),
                                    GetMemoryRefreshDeltaExpression(Expression.Constant(2))), // This function actually decreases the PC by two for each 'loop' hence need more refresh cycles.
                                breakLabel));
                    }
                    
                    expressions.Add(Expression.Assign(HalfCarry, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(ParityOverflow, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(Subtract, Expression.Constant(false)));

                    timer.Add(4, 16);
                    break;

                // ********* Search *********
                case PrefixEdOpCode.CPI:
                    expressions.Add(Expression.Call(Alu, AluCompare, A, Expression.Call(Mmu, MmuReadByte, HL)));
                    expressions.Add(Expression.PreIncrementAssign(HL));
                    expressions.Add(Expression.PreDecrementAssign(BC));
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPIR:
                    {
                        var breakLabel = Expression.Label();
                        expressions.Add(
                            Expression.Loop(
                                Expression.Block(
                                    Expression.Call(Alu, AluCompare, A, Expression.Call(Mmu, MmuReadByte, HL)),
                                    Expression.PreIncrementAssign(HL),
                                    Expression.PreDecrementAssign(BC),
                                    Expression.IfThen(Expression.OrElse(Expression.Equal(BC, Expression.Constant((ushort)0)), Zero), Expression.Break(breakLabel)),
                                    Expression.Call(DynamicTimer, DynamicTimerAdd, Expression.Constant(5), Expression.Constant(21)),
                                    GetMemoryRefreshDeltaExpression(Expression.Constant(2))), // This function actually decreases the PC by two for each 'loop' hence need more refresh cycles.
                                breakLabel));
                    }

                    timer.Add(4, 16);
                    break;

                default:
                    throw new NotImplementedException(opCode.ToString());
            }

            return DecodeResult.Continue;
        }
    }
}
