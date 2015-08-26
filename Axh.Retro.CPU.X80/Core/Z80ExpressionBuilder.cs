namespace Axh.Retro.CPU.X80.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Util;

    internal class Z80ExpressionBuilder
    {
        private static readonly ParameterExpression RegistersExpression;
        private static readonly ParameterExpression MmuExpression;

        /// <summary>
        /// Byte parameter b
        /// </summary>
        private static readonly ParameterExpression LocalByte;

        /// <summary>
        /// Word parameter w
        /// </summary>
        private static readonly ParameterExpression LocalWord;

        // Register expressions
        private static readonly Expression PC;
        private static readonly Expression SP;
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

        private static readonly MethodInfo MmuReadByteMethodInfo;
        private static readonly MethodInfo MmuReadWordMethodInfo;
        private static readonly MethodInfo MmuWriteByteMethodInfo;

        static Z80ExpressionBuilder()
        {
            RegistersExpression = Expression.Parameter(typeof(IZ80Registers), "registers");
            MmuExpression = Expression.Parameter(typeof(IMmu), "mmu");
            LocalByte = Expression.Parameter(typeof(byte), "b");
            LocalWord = Expression.Parameter(typeof(ushort), "w");

            // General purpose register expressions
            var generalPurposeRegisters = RegistersExpression.GetPropertyExpression<IZ80Registers, IGeneralPurposeRegisterSet>(r => r.GeneralPurposeRegisters);
            A = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.A);
            B = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.B);
            C = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.C);
            D = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.D);
            E = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.E);
            H = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.H);
            L = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.L);
            BC = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, ushort>(r => r.BC);
            DE = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, ushort>(r => r.DE);
            HL = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, ushort>(r => r.HL);
            PC = RegistersExpression.GetPropertyExpression<IZ80Registers, ushort>(r => r.ProgramCounter);
            SP = RegistersExpression.GetPropertyExpression<IZ80Registers, ushort>(r => r.StackPointer);
            
            // Z80 specific register expressions
            I = RegistersExpression.GetPropertyExpression<IZ80Registers, byte>(r => r.I);
            R = RegistersExpression.GetPropertyExpression<IZ80Registers, byte>(r => r.R);
            IX = RegistersExpression.GetPropertyExpression<IZ80Registers, ushort>(r => r.IX);
            IY = RegistersExpression.GetPropertyExpression<IZ80Registers, ushort>(r => r.IY);

            // Interrupt stuff
            IFF1 = RegistersExpression.GetPropertyExpression<IZ80Registers, bool>(r => r.InterruptFlipFlop1);
            IFF2 = RegistersExpression.GetPropertyExpression<IZ80Registers, bool>(r => r.InterruptFlipFlop2);

            // Flags register expressions
            Flags = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, IFlagsRegister>(r => r.Flags);
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

            // MMU expressions
            MmuReadByteMethodInfo = ExpressionHelpers.GetMethodInfo<IMmu, ushort, byte>((mmu, address) => mmu.ReadByte(address));
            MmuReadWordMethodInfo = ExpressionHelpers.GetMethodInfo<IMmu, ushort, ushort>((mmu, address) => mmu.ReadWord(address));
            MmuWriteByteMethodInfo = ExpressionHelpers.GetMethodInfo<IMmu, ushort, byte>((mmu, address, value) => mmu.WriteByte(address, value));
            
            ReadByteAtLocalWord = Expression.Call(MmuExpression, MmuReadByteMethodInfo, LocalWord);
            ReadByteAtHL = Expression.Call(MmuExpression, MmuReadByteMethodInfo, HL);
            ReadByteAtBC = Expression.Call(MmuExpression, MmuReadByteMethodInfo, BC);
            ReadByteAtDE = Expression.Call(MmuExpression, MmuReadByteMethodInfo, DE);
            ReadByteAtIXd = Expression.Call(MmuExpression, MmuReadByteMethodInfo, IXd);
            ReadByteAtIYd = Expression.Call(MmuExpression, MmuReadByteMethodInfo, IYd);
        }

        private readonly ICollection<Expression> expressions;

        private readonly InstructionTimer timer;

        private readonly IMmuCache mmuCache;

        private ConstantExpression NextByte => Expression.Constant(mmuCache.NextByte());
        private ConstantExpression NextWord => Expression.Constant(mmuCache.NextWord(), typeof(ushort));

        public Z80ExpressionBuilder(IMmuCache mmuCache, InstructionTimer timer)
        {
            this.expressions = new List<Expression>();
            this.timer = timer;
            this.mmuCache = mmuCache;
        }

        public Expression<Action<IZ80Registers, IMmu>> FinalizeBlock(DecodeResult lastResult)
        {
            if (lastResult == DecodeResult.FinalizeAndSync)
            {
                // Increment the program counter by how many bytes were read.
                var expression = Expression.Assign(PC, Expression.Convert(Expression.Add(Expression.Convert(PC, typeof(int)), Expression.Constant(this.mmuCache.TotalBytesRead)), typeof(ushort)));
                this.expressions.Add(expression);
            }

            // Add the block length to the 7 lsb of memory refresh register.
            var blockLengthExpression = Expression.Constant(this.mmuCache.TotalBytesRead, typeof(int));
            var increment7LsbR = Expression.And(Expression.Add(Expression.Convert(R, typeof(int)), blockLengthExpression), Expression.Constant(0x7f));
            this.expressions.Add(Expression.Assign(R, Expression.Convert(increment7LsbR, typeof(byte))));

            var expressionBlock = Expression.Block(new[] { LocalByte, LocalWord }, this.expressions);
            var lambda = Expression.Lambda<Action<IZ80Registers, IMmu>>(expressionBlock, RegistersExpression, MmuExpression);

            return lambda;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mmuCache"></param>
        /// <param name="expressions"></param>
        /// <param name="timer"></param>
        /// <returns>True if we can continue to decode operations sequentially, false if it can't e.g. a jumo</returns>
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
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, HL, A));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_B:
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, HL, B));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_C:
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, HL, C));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_D:
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, HL, D));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_E:
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, HL, E));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_H:
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, HL, H));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_mHL_L:
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, HL, L));
                    timer.Add(2, 7);
                    break;

                // LD (HL), n
                case PrimaryOpCode.LD_mHL_n:
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, HL, NextByte));
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
                    expressions.Add(Expression.Assign(A, Expression.Call(MmuExpression, MmuReadByteMethodInfo, NextWord)));
                    timer.Add(4, 13);
                    break;

                // LD (BC), A
                case PrimaryOpCode.LD_mBC_A:
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, BC, A));
                    timer.Add(2, 7);
                    break;

                // LD (DE), A
                case PrimaryOpCode.LD_mDE_A:
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, DE, A));
                    timer.Add(2, 7);
                    break;

                // LD (nn), A
                case PrimaryOpCode.LD_mnn_A:
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, NextWord, A));
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
                    expressions.Add(Expression.Assign(HL, Expression.Call(MmuExpression, MmuReadWordMethodInfo, NextWord)));
                    timer.Add(5, 16);
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
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IXd, A));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_B:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IXd, B));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_C:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IXd, C));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_D:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IXd, D));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_E:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IXd, E));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_H:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IXd, H));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_L:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IXd, L));
                    timer.Add(5, 19);
                    break;

                // LD (IX+d), n
                case PrefixDdFdOpCode.LD_mIXYd_n:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IXd, NextByte));
                    timer.Add(5, 19);
                    break;

                // ********* 16-bit load *********
                // LD IX, nn
                case PrefixDdFdOpCode.LD_IXY_nn:
                    expressions.Add(Expression.Assign(IX, NextWord));
                    timer.Add(4, 14);
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
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IYd, A));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_B:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IYd, B));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_C:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IYd, C));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_D:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IYd, D));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_E:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IYd, E));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_H:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IYd, H));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_L:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IYd, L));
                    timer.Add(5, 19);
                    break;
                    
                // LD (IX+d), n
                case PrefixDdFdOpCode.LD_mIXYd_n:
                    expressions.Add(Expression.Assign(LocalByte, NextByte));
                    expressions.Add(Expression.Call(MmuExpression, MmuWriteByteMethodInfo, IYd, NextByte));
                    timer.Add(5, 19);
                    break;

                // ********* 16-bit load *********
                // LD IY, nn
                case PrefixDdFdOpCode.LD_IXY_nn:
                    expressions.Add(Expression.Assign(IY, NextWord));
                    timer.Add(4, 14);
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

                default:
                    throw new NotImplementedException(opCode.ToString());
            }

            return DecodeResult.Continue;
        }
    }
}
