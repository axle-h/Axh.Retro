namespace Axh.Retro.CPU.X80.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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

        private static readonly Expression IXl;
        private static readonly Expression IXh;
        private static readonly Expression IYl;
        private static readonly Expression IYh;

        // Interrupt stuff
        private static readonly Expression IFF1;
        private static readonly Expression IFF2;
        private static readonly Expression IM;

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
        private static readonly MethodInfo SetUndocumentedFlags;

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
        private static readonly MethodInfo AluAnd;
        private static readonly MethodInfo AluOr;
        private static readonly MethodInfo AluXor;
        private static readonly MethodInfo AluDecimalAdjust;

        private static readonly IDictionary<IndexRegister, IndexRegisterExpressions> IndexRegisterExpressions;

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
            IXl = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.IXl);
            IXh = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.IXh);
            IY = Registers.GetPropertyExpression<IZ80Registers, ushort>(r => r.IY);
            IYl = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.IYl);
            IYh = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.IYh);

            // Interrupt stuff
            IFF1 = Registers.GetPropertyExpression<IZ80Registers, bool>(r => r.InterruptFlipFlop1);
            IFF2 = Registers.GetPropertyExpression<IZ80Registers, bool>(r => r.InterruptFlipFlop2);
            IM = Registers.GetPropertyExpression<IZ80Registers, InterruptMode>(r => r.InterruptMode);

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
            SetUndocumentedFlags = ExpressionHelpers.GetMethodInfo<IFlagsRegister, byte>((flags, result) => flags.SetUndocumentedFlags(result));

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
            AluAnd = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a, b) => alu.And(a, b));
            AluOr = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a, b) => alu.Or(a, b));
            AluXor = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a, b) => alu.Xor(a, b));
            AluDecimalAdjust = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte>((alu, a) => alu.DecimalAdjust(a));

            IndexRegisterExpressions = new Dictionary<IndexRegister, IndexRegisterExpressions>
                                       {
                                           {
                                               IndexRegister.HL,
                                               new IndexRegisterExpressions
                                               {
                                                   Register = HL,
                                                   RegisterLowOrder = L,
                                                   RegisterHighOrder = H,
                                                   IndexedAddress = HL,
                                                   // HL indexes don't have a displacement
                                                   IndexedValue = ReadByteAtHL,
                                                   UsesDisplacedIndexTimings = false
                                               }
                                           },
                                           {
                                               IndexRegister.IX,
                                               new IndexRegisterExpressions
                                               {
                                                   Register = IX,
                                                   RegisterLowOrder = IXl,
                                                   RegisterHighOrder = IXh,
                                                   IndexedAddress = IXd,
                                                   IndexedValue = ReadByteAtIXd,
                                                   UsesDisplacedIndexTimings = true
                                               }
                                           },
                                           {
                                               IndexRegister.IY,
                                               new IndexRegisterExpressions
                                               {
                                                   Register = IY,
                                                   RegisterLowOrder = IYl,
                                                   RegisterHighOrder = IYh,
                                                   IndexedAddress = IYd,
                                                   IndexedValue = ReadByteAtIYd,
                                                   UsesDisplacedIndexTimings = true
                                               }
                                           }
                                       };
        }

        private static Expression GetMemoryRefreshDeltaExpression(Expression deltaExpression)
        {
            var increment7LsbR = Expression.And(Expression.Add(Expression.Convert(R, typeof(int)), deltaExpression), Expression.Constant(0x7f));
            return Expression.Assign(R, Expression.Convert(increment7LsbR, typeof(byte)));
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

        private readonly List<Expression> expressions;

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

        /// <summary>
        /// Decode an opcode.
        /// </summary>
        /// <param name="currentIndexRegister">The current index prefix set from the last opcode</param>
        /// <returns></returns>
        public DecodeResult TryDecodeNextOperation(IndexRegister currentIndexRegister = IndexRegister.HL)
        {
            var opCode = (PrimaryOpCode)this.mmuCache.NextByte();
            var index = IndexRegisterExpressions[currentIndexRegister];
            
            if (index.UsesDisplacedIndexTimings && OpCodeUsesDisplacedIndex(opCode))
            {
                // Read the displacement as the next byte
                expressions.Add(Expression.Assign(LocalByte, NextByte));
            }
            
            switch (opCode)
            {
                case PrimaryOpCode.NOP:
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.HALT:
                    timer.Add(1, 4);
                    return DecodeResult.FinalizeAndSync;

                // ********* Prefixes *********
                case PrimaryOpCode.Prefix_DD:
                    // TODO: potential stack overflow
                    // Add a NOP timing for now and take a NOP off known indexed timings later.
                    timer.Add(1, 4);
                    return TryDecodeNextOperation(IndexRegister.IX);

                case PrimaryOpCode.Prefix_FD:
                    // TODO: potential stack overflow
                    // Add a NOP timing for now and take a NOP off known indexed timings later.
                    timer.Add(1, 4);
                    return TryDecodeNextOperation(IndexRegister.IY);

                case PrimaryOpCode.Prefix_ED:
                    return TryDecodeNextEdPrefixOperation();

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
                    expressions.Add(Expression.Assign(index.RegisterHighOrder, A));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_A:
                    expressions.Add(Expression.Assign(index.RegisterLowOrder, A));
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
                    expressions.Add(Expression.Assign(index.RegisterHighOrder, B));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_B:
                    expressions.Add(Expression.Assign(index.RegisterLowOrder, B));
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
                    expressions.Add(Expression.Assign(index.RegisterHighOrder, C));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_C:
                    expressions.Add(Expression.Assign(index.RegisterLowOrder, C));
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
                    expressions.Add(Expression.Assign(index.RegisterHighOrder, D));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_D:
                    expressions.Add(Expression.Assign(index.RegisterLowOrder, D));
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
                    expressions.Add(Expression.Assign(index.RegisterHighOrder, E));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_E:
                    expressions.Add(Expression.Assign(index.RegisterLowOrder, E));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_A_H:
                    expressions.Add(Expression.Assign(A, index.RegisterHighOrder));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_B_H:
                    expressions.Add(Expression.Assign(B, index.RegisterHighOrder));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_C_H:
                    expressions.Add(Expression.Assign(C, index.RegisterHighOrder));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_D_H:
                    expressions.Add(Expression.Assign(D, index.RegisterHighOrder));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_E_H:
                    expressions.Add(Expression.Assign(E, index.RegisterHighOrder));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_H_H:
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_L_H:
                    expressions.Add(Expression.Assign(index.RegisterLowOrder, index.RegisterHighOrder));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_A_L:
                    expressions.Add(Expression.Assign(A, index.RegisterLowOrder));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_B_L:
                    expressions.Add(Expression.Assign(B, index.RegisterLowOrder));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_C_L:
                    expressions.Add(Expression.Assign(C, index.RegisterLowOrder));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_D_L:
                    expressions.Add(Expression.Assign(D, index.RegisterLowOrder));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_E_L:
                    expressions.Add(Expression.Assign(E, index.RegisterLowOrder));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.LD_H_L:
                    expressions.Add(Expression.Assign(index.RegisterHighOrder, index.RegisterLowOrder));
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
                    expressions.Add(Expression.Assign(index.RegisterHighOrder, NextByte));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_L_n:
                    expressions.Add(Expression.Assign(index.RegisterLowOrder, NextByte));
                    timer.Add(2, 7);
                    break;

                // LD r, (HL)
                case PrimaryOpCode.LD_A_mHL:
                    expressions.Add(Expression.Assign(A, index.IndexedValue));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_B_mHL:
                    expressions.Add(Expression.Assign(B, index.IndexedValue));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_C_mHL:
                    expressions.Add(Expression.Assign(C, index.IndexedValue));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_D_mHL:
                    expressions.Add(Expression.Assign(D, index.IndexedValue));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_E_mHL:
                    expressions.Add(Expression.Assign(E, index.IndexedValue));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_H_mHL:
                    // H register is always assigned here
                    expressions.Add(Expression.Assign(H, index.IndexedValue));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_L_mHL:
                    // L register is always assigned here
                    expressions.Add(Expression.Assign(L, index.IndexedValue));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;

                // LD (HL), r
                case PrimaryOpCode.LD_mHL_A:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, index.IndexedAddress, A));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_B:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, index.IndexedAddress, B));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_C:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, index.IndexedAddress, C));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_D:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, index.IndexedAddress, D));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_E:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, index.IndexedAddress, E));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_H:
                    // Value of H register is always used here
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, index.IndexedAddress, H));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_L:
                    // Value of L register is always used here
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, index.IndexedAddress, L));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;

                // LD (HL), n
                case PrimaryOpCode.LD_mHL_n:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, index.IndexedAddress, NextByte));
                    if (index.UsesDisplacedIndexTimings) timer.Add(1, 5);
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
                    expressions.Add(Expression.Assign(index.Register, NextWord));
                    timer.Add(index.UsesDisplacedIndexTimings ? 3 : 2, 10);
                    break;
                case PrimaryOpCode.LD_SP_nn:
                    expressions.Add(Expression.Assign(SP, NextWord));
                    timer.Add(2, 10);
                    break;

                // LD HL, (nn)
                case PrimaryOpCode.LD_HL_mnn:
                    expressions.Add(Expression.Assign(index.Register, Expression.Call(Mmu, MmuReadWord, NextWord)));
                    timer.Add(5, 16);
                    break;

                // LD (nn), HL
                case PrimaryOpCode.LD_mnn_HL:
                    expressions.Add(Expression.Call(Mmu, MmuWriteWord, NextWord, index.Register));
                    timer.Add(5, 16);
                    break;

                // LD SP, HL
                case PrimaryOpCode.LD_SP_HL:
                    expressions.Add(Expression.Assign(SP, index.Register));
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
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, SP, index.RegisterHighOrder));
                    expressions.Add(PushSP);
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, SP, index.RegisterLowOrder));
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
                    expressions.Add(Expression.Assign(index.RegisterLowOrder, Expression.Call(Mmu, MmuReadByte, SP)));
                    expressions.Add(PopSP);
                    expressions.Add(Expression.Assign(index.RegisterHighOrder, Expression.Call(Mmu, MmuReadByte, SP)));
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
                    // This affects HL register directly, always ignoring index register prefixes
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
                    expressions.Add(Expression.Assign(LocalByte, index.RegisterLowOrder));
                    expressions.Add(Expression.Assign(index.RegisterLowOrder, Expression.Call(Mmu, MmuReadByte, SP)));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, SP, LocalByte));

                    // Exchange H
                    expressions.Add(Expression.Assign(LocalByte, index.RegisterHighOrder));
                    expressions.Add(Expression.Assign(LocalWord, Expression.Increment(SP)));
                    expressions.Add(Expression.Assign(index.RegisterHighOrder, Expression.Call(Mmu, MmuReadByte, LocalWord)));
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, LocalWord, LocalByte));
                    
                    timer.Add(5, 19);
                    break;

                // ********* 8-Bit Arithmetic *********
                // ADD A, r
                case PrimaryOpCode.ADD_A_A:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAdd, A, A)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.ADD_A_B:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAdd, A, B)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.ADD_A_C:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAdd, A, C)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.ADD_A_D:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAdd, A, D)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.ADD_A_E:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAdd, A, E)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.ADD_A_H:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAdd, A, index.RegisterHighOrder)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.ADD_A_L:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAdd, A, index.RegisterLowOrder)));
                    timer.Add(1, 4);
                    break;

                // ADD A, n
                case PrimaryOpCode.ADD_A_n:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAdd, A, NextByte)));
                    timer.Add(2, 7);
                    break;

                // ADD A, (HL)
                case PrimaryOpCode.ADD_A_mHL:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAdd, A, index.IndexedValue)));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;

                // ADC A, r
                case PrimaryOpCode.ADC_A_A:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAddWithCarry, A, A)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.ADC_A_B:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAddWithCarry, A, B)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.ADC_A_C:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAddWithCarry, A, C)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.ADC_A_D:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAddWithCarry, A, D)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.ADC_A_E:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAddWithCarry, A, E)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.ADC_A_H:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAddWithCarry, A, index.RegisterHighOrder)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.ADC_A_L:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAddWithCarry, A, index.RegisterLowOrder)));
                    timer.Add(1, 4);
                    break;

                // ADC A, n
                case PrimaryOpCode.ADC_A_n:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAddWithCarry, A, NextByte)));
                    timer.Add(2, 7);
                    break;

                // ADC A, (HL)
                case PrimaryOpCode.ADC_A_mHL:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAddWithCarry, A, index.IndexedValue)));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;

                // SUB A, r
                case PrimaryOpCode.SUB_A_A:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtract, A, A)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.SUB_A_B:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtract, A, B)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.SUB_A_C:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtract, A, C)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.SUB_A_D:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtract, A, D)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.SUB_A_E:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtract, A, E)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.SUB_A_H:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtract, A, index.RegisterHighOrder)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.SUB_A_L:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtract, A, index.RegisterLowOrder)));
                    timer.Add(1, 4);
                    break;

                // SUB A, n
                case PrimaryOpCode.SUB_A_n:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtract, A, NextByte)));
                    timer.Add(2, 7);
                    break;

                // SUB A, (HL)
                case PrimaryOpCode.SUB_A_mHL:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtract, A, index.IndexedValue)));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;

                // SBC A, r
                case PrimaryOpCode.SBC_A_A:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtractWithCarry, A, A)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.SBC_A_B:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtractWithCarry, A, B)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.SBC_A_C:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtractWithCarry, A, C)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.SBC_A_D:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtractWithCarry, A, D)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.SBC_A_E:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtractWithCarry, A, E)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.SBC_A_H:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtractWithCarry, A, index.RegisterHighOrder)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.SBC_A_L:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtractWithCarry, A, index.RegisterLowOrder)));
                    timer.Add(1, 4);
                    break;

                // SBC A, n
                case PrimaryOpCode.SBC_A_n:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtractWithCarry, A, NextByte)));
                    timer.Add(2, 7);
                    break;

                // SBC A, (HL)
                case PrimaryOpCode.SBC_A_mHL:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtractWithCarry, A, index.IndexedValue)));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;

                // AND r
                case PrimaryOpCode.AND_A:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAnd, A, A)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.AND_B:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAnd, A, B)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.AND_C:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAnd, A, C)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.AND_D:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAnd, A, D)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.AND_E:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAnd, A, E)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.AND_H:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAnd, A, index.RegisterHighOrder)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.AND_L:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAnd, A, index.RegisterLowOrder)));
                    timer.Add(1, 4);
                    break;

                // AND n
                case PrimaryOpCode.AND_n:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAnd, A, NextByte)));
                    timer.Add(2, 7);
                    break;

                // AND (HL)
                case PrimaryOpCode.AND_mHL:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluAnd, A, index.IndexedValue)));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;

                // OR r
                case PrimaryOpCode.OR_A:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluOr, A, A)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.OR_B:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluOr, A, B)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.OR_C:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluOr, A, C)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.OR_D:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluOr, A, D)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.OR_E:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluOr, A, E)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.OR_H:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluOr, A, index.RegisterHighOrder)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.OR_L:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluOr, A, index.RegisterLowOrder)));
                    timer.Add(1, 4);
                    break;

                // OR n
                case PrimaryOpCode.OR_n:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluOr, A, NextByte)));
                    timer.Add(2, 7);
                    break;

                // OR (HL)
                case PrimaryOpCode.OR_mHL:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluOr, A, index.IndexedValue)));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;

                // XOR r
                case PrimaryOpCode.XOR_A:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluXor, A, A)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.XOR_B:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluXor, A, B)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.XOR_C:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluXor, A, C)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.XOR_D:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluXor, A, D)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.XOR_E:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluXor, A, E)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.XOR_H:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluXor, A, index.RegisterHighOrder)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.XOR_L:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluXor, A, index.RegisterLowOrder)));
                    timer.Add(1, 4);
                    break;

                // XOR n
                case PrimaryOpCode.XOR_n:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluXor, A, NextByte)));
                    timer.Add(2, 7);
                    break;

                // XOR (HL)
                case PrimaryOpCode.XOR_mHL:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluXor, A, index.IndexedValue)));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;

                // CP r
                case PrimaryOpCode.CP_A:
                    expressions.Add(Expression.Call(Alu, AluCompare, A, A));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.CP_B:
                    expressions.Add(Expression.Call(Alu, AluCompare, A, B));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.CP_C:
                    expressions.Add(Expression.Call(Alu, AluCompare, A, C));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.CP_D:
                    expressions.Add(Expression.Call(Alu, AluCompare, A, D));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.CP_E:
                    expressions.Add(Expression.Call(Alu, AluCompare, A, E));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.CP_H:
                    expressions.Add(Expression.Call(Alu, AluCompare, A, index.RegisterHighOrder));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.CP_L:
                    expressions.Add(Expression.Call(Alu, AluCompare, A, index.RegisterLowOrder));
                    timer.Add(1, 4);
                    break;

                // CP n
                case PrimaryOpCode.CP_n:
                    expressions.Add(Expression.Call(Alu, AluCompare, A, NextByte));
                    timer.Add(2, 7);
                    break;

                // CP (HL)
                case PrimaryOpCode.CP_mHL:
                    expressions.Add(Expression.Call(Alu, AluCompare, A, index.IndexedValue));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(2, 7);
                    break;

                // INC r
                case PrimaryOpCode.INC_A:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluIncrement, A)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.INC_B:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluIncrement, B)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.INC_C:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluIncrement, C)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.INC_D:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluIncrement, D)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.INC_E:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluIncrement, E)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.INC_H:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluIncrement, index.RegisterHighOrder)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.INC_L:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluIncrement, index.RegisterLowOrder)));
                    timer.Add(1, 4);
                    break;
                    
                // INC (HL)
                case PrimaryOpCode.INC_mHL:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, index.IndexedAddress, Expression.Call(Alu, AluIncrement, index.IndexedValue)));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(3, 11);
                    break;

                // DEC r
                case PrimaryOpCode.DEC_A:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluDecrement, A)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.DEC_B:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluDecrement, B)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.DEC_C:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluDecrement, C)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.DEC_D:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluDecrement, D)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.DEC_E:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluDecrement, E)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.DEC_H:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluDecrement, index.RegisterHighOrder)));
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.DEC_L:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluDecrement, index.RegisterLowOrder)));
                    timer.Add(1, 4);
                    break;

                // DEC (HL)
                case PrimaryOpCode.DEC_mHL:
                    expressions.Add(Expression.Call(Mmu, MmuWriteByte, index.IndexedAddress, Expression.Call(Alu, AluDecrement, index.IndexedValue)));
                    if (index.UsesDisplacedIndexTimings) timer.Add(2, 8);
                    timer.Add(3, 11);
                    break;

                // ********* General-Purpose Arithmetic *********
                // DAA
                case PrimaryOpCode.DAA:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluDecimalAdjust, A)));
                    timer.Add(1, 4);
                    break;

                // CPL
                case PrimaryOpCode.CPL:
                    expressions.Add(Expression.Assign(A, Expression.Not(A)));
                    expressions.Add(Expression.Call(Flags, SetUndocumentedFlags, A));
                    expressions.Add(Expression.Assign(HalfCarry, Expression.Constant(true)));
                    expressions.Add(Expression.Assign(Subtract, Expression.Constant(true)));
                    timer.Add(1, 4);
                    break;

                // CCF
                case PrimaryOpCode.CCF:
                    expressions.Add(Expression.Assign(HalfCarry, Carry));
                    expressions.Add(Expression.Assign(Subtract, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(Carry, Expression.Not(Carry)));
                    timer.Add(1, 4);
                    break;

                // SCF
                case PrimaryOpCode.SCF:
                    expressions.Add(Expression.Assign(HalfCarry, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(Subtract, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(Carry, Expression.Constant(true)));
                    timer.Add(1, 4);
                    break;

                // DI
                case PrimaryOpCode.DI:
                    expressions.Add(Expression.Assign(IFF1, Expression.Constant(false)));
                    expressions.Add(Expression.Assign(IFF2, Expression.Constant(false)));
                    timer.Add(1, 4);
                    break;

                // EI
                case PrimaryOpCode.EI:
                    expressions.Add(Expression.Assign(IFF1, Expression.Constant(true)));
                    expressions.Add(Expression.Assign(IFF2, Expression.Constant(true)));
                    timer.Add(1, 4);
                    break;
                    
                // ********* Jump *********
                case PrimaryOpCode.JP:
                    expressions.Add(Expression.Assign(PC, NextWord));
                    timer.Add(3, 10);
                    return DecodeResult.Finalize;

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
                    expressions.AddRange(GetLdExpressions());
                    timer.Add(4, 16);
                    break;

                // LDIR
                case PrefixEdOpCode.LDIR:
                    expressions.AddRange(GetLdrExpressions());
                    timer.Add(4, 16);
                    break;

                // LDD
                case PrefixEdOpCode.LDD:
                    expressions.AddRange(GetLdExpressions(true));
                    timer.Add(4, 16);
                    break;

                // LDDR
                case PrefixEdOpCode.LDDR:
                    expressions.AddRange(GetLdrExpressions(true));
                    timer.Add(4, 16);
                    break;

                // ********* Search *********
                case PrefixEdOpCode.CPI:
                    expressions.AddRange(GetCpExpressions());
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPIR:
                    expressions.Add(GetCprExpression());
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPD:
                    expressions.AddRange(GetCpExpressions(true));
                    timer.Add(4, 16);
                    break;

                case PrefixEdOpCode.CPDR:
                    expressions.Add(GetCprExpression(true));
                    timer.Add(4, 16);
                    break;

                // ********* General-Purpose Arithmetic *********
                // NEG
                case PrefixEdOpCode.NEG:
                    expressions.Add(Expression.Assign(A, Expression.Call(Alu, AluSubtract, Expression.Constant((byte)0), A)));
                    timer.Add(2, 8);
                    break;

                // IM 0
                case PrefixEdOpCode.IM0:
                    expressions.Add(Expression.Assign(IM, Expression.Constant(InterruptMode.InterruptMode0)));
                    timer.Add(2, 8);
                    break;

                // IM 1
                case PrefixEdOpCode.IM1:
                    expressions.Add(Expression.Assign(IM, Expression.Constant(InterruptMode.InterruptMode1)));
                    timer.Add(2, 8);
                    break;

                // IM 2
                case PrefixEdOpCode.IM2:
                    expressions.Add(Expression.Assign(IM, Expression.Constant(InterruptMode.InterruptMode2)));
                    timer.Add(2, 8);
                    break;

                default:
                    throw new NotImplementedException(opCode.ToString());
            }

            return DecodeResult.Continue;
        }

        private static IEnumerable<Expression> GetLdExpressions(bool decrement = false)
        {
            yield return Expression.Call(Mmu, MmuTransferByte, HL, DE);
            yield return decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL);
            yield return decrement ? Expression.PreDecrementAssign(DE) : Expression.PreIncrementAssign(DE);
            yield return Expression.PreDecrementAssign(BC);
            yield return Expression.Assign(HalfCarry, Expression.Constant(false));
            yield return Expression.Assign(ParityOverflow, Expression.NotEqual(BC, Expression.Constant((ushort)0)));
            yield return Expression.Assign(Subtract, Expression.Constant(false));
        }

        private static IEnumerable<Expression> GetLdrExpressions(bool decrement = false)
        {
            var breakLabel = Expression.Label();
            yield return
                Expression.Loop(
                    Expression.Block(
                        Expression.Call(Mmu, MmuTransferByte, HL, DE),
                        decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL),
                        decrement ? Expression.PreDecrementAssign(DE) : Expression.PreIncrementAssign(DE),
                        Expression.PreDecrementAssign(BC),
                        Expression.IfThen(Expression.Equal(BC, Expression.Constant((ushort)0)), Expression.Break(breakLabel)),
                        Expression.Call(DynamicTimer, DynamicTimerAdd, Expression.Constant(5), Expression.Constant(21)),
                        GetMemoryRefreshDeltaExpression(Expression.Constant(2))), // This function actually decreases the PC by two for each 'loop' hence need more refresh cycles.
                    breakLabel);

            yield return Expression.Assign(HalfCarry, Expression.Constant(false));
            yield return Expression.Assign(ParityOverflow, Expression.Constant(false));
            yield return Expression.Assign(Subtract, Expression.Constant(false));
        }

        private static IEnumerable<Expression> GetCpExpressions(bool decrement = false)
        {
            yield return Expression.Call(Alu, AluCompare, A, Expression.Call(Mmu, MmuReadByte, HL));
            yield return decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL);
            yield return Expression.PreDecrementAssign(BC);
        }

        private static Expression GetCprExpression(bool decrement = false)
        {
            var breakLabel = Expression.Label();
            return
                Expression.Loop(
                    Expression.Block(
                        Expression.Call(Alu, AluCompare, A, Expression.Call(Mmu, MmuReadByte, HL)),
                        decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL),
                        Expression.PreDecrementAssign(BC),
                        Expression.IfThen(Expression.OrElse(Expression.Equal(BC, Expression.Constant((ushort)0)), Zero), Expression.Break(breakLabel)),
                        Expression.Call(DynamicTimer, DynamicTimerAdd, Expression.Constant(5), Expression.Constant(21)),
                        GetMemoryRefreshDeltaExpression(Expression.Constant(2))), // This function actually decreases the PC by two for each 'loop' hence need more refresh cycles.
                    breakLabel);
        }
    }
}
