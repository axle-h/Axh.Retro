﻿namespace Axh.Emulation.CPU.X80.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using Axh.Emulation.CPU.X80.Contracts.Factories;
    using Axh.Emulation.CPU.X80.Contracts.Memory;
    using Axh.Emulation.CPU.X80.Contracts.OpCodes;
    using Axh.Emulation.CPU.X80.Contracts.Registers;
    using Axh.Emulation.CPU.X80.Contracts.Core;
    using Axh.Emulation.CPU.X80.Util;

    public class Z80InstructionDecoder : IZ80InstructionDecoder
    {
        private static readonly ParameterExpression RegistersParameterExpression;
        private static readonly ParameterExpression MmuParameterExpression;
        private static readonly ParameterExpression TempByteParameterExpression;
        private static readonly ParameterExpression TempWordParameterExpression;

        // Register expressions
        private static readonly MemberExpression R;
        private static readonly MemberExpression Pc;
        private static readonly MemberExpression A;
        private static readonly MemberExpression B;
        private static readonly MemberExpression C;
        private static readonly MemberExpression D;
        private static readonly MemberExpression E;
        private static readonly MemberExpression F;
        private static readonly MemberExpression H;
        private static readonly MemberExpression L;
        private static readonly MemberExpression HL;

        private static readonly Expression IncrementMemoryRefreshRegisterExpression;

        private static readonly Expression IncrementProgramCounterRegisterExpression;

        /// <summary>
        /// Reads a byte from the mmu at the address at TempWordParameterExpression
        /// </summary>
        private static readonly MethodCallExpression ReadByteByTempMethodExpression;

        /// <summary>
        /// Reads a byte from the mmu at the address at HL
        /// </summary>
        private static readonly MethodCallExpression ReadByteByHlRegisterMethodExpression;

        static Z80InstructionDecoder()
        {
            RegistersParameterExpression = Expression.Parameter(typeof(IZ80Registers), "registers");
            MmuParameterExpression = Expression.Parameter(typeof(IMmu), "mmu");
            TempByteParameterExpression = Expression.Parameter(typeof(byte), "b");
            TempWordParameterExpression = Expression.Parameter(typeof(ushort), "w");

            R = RegistersParameterExpression.GetPropertyExpression<IZ80Registers, byte>(r => r.R);
            Pc = RegistersParameterExpression.GetPropertyExpression<IZ80Registers, ushort>(r => r.ProgramCounter);

            var generalPurposeRegisters = RegistersParameterExpression.GetPropertyExpression<IZ80Registers, IGeneralPurposeRegisterSet>(r => r.GeneralPurposeRegisters);
            A = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.A);

            B = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.B);
            C = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.C);
            D = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.D);
            E = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.E);
            H = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.H);
            L = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.L);
            HL = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, ushort>(r => r.HL);

            var flagsRegister = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, IFlagsRegister>(r => r.Flags);
            F = flagsRegister.GetPropertyExpression<IFlagsRegister, byte>(r => r.Register);

            // Increment 7 lsb of memory refresh register.
            var increment7LsbR = Expression.And(Expression.Increment(Expression.Convert(R, typeof(int))), Expression.Constant(0x7f));
            IncrementMemoryRefreshRegisterExpression = Expression.Assign(R, Expression.Convert(increment7LsbR, typeof(byte)));

            // Increment program counter register
            var incrementPc = Expression.Convert(Expression.Increment(Expression.Convert(Pc, typeof(int))), typeof(ushort));
            IncrementProgramCounterRegisterExpression = Expression.Assign(Pc, incrementPc);

            ReadByteByTempMethodExpression = MmuParameterExpression.GetMethodExpression<IMmu, ushort, byte>((mmu, address) => mmu.ReadByte(address), TempWordParameterExpression);
            ReadByteByHlRegisterMethodExpression = MmuParameterExpression.GetMethodExpression<IMmu, ushort, byte>((mmu, address) => mmu.ReadByte(address), HL);

        }

        private readonly IMmuFactory mmuFactory;

        private readonly IMmu mmu;

        public Z80InstructionDecoder(IMmuFactory mmuFactory, IMmu mmu)
        {
            this.mmuFactory = mmuFactory;
            this.mmu = mmu;
        }

        public Z80DynamicallyRecompiledBlock DecodeNextBlock(ushort address)
        {
            var mmuCache = mmuFactory.GetMmuCache(mmu, address);

            var expressions = new List<Expression>();
            var timer = new InstructionTimer();

            while (true)
            {
                var canContinue = TryDecodeNextOperation(mmuCache, expressions, timer);

                if (!canContinue || mmuCache.TotalBytesRead >= ushort.MaxValue)
                {
                    break;
                }
            }

            var expressionBlock = Expression.Block(new [] { TempByteParameterExpression, TempWordParameterExpression }, expressions);
            var lambda = Expression.Lambda<Action<IZ80Registers, IMmu>>(expressionBlock, RegistersParameterExpression, MmuParameterExpression);
            return new Z80DynamicallyRecompiledBlock
                   {
                       Address = address,
                       Action = lambda.Compile(),
                       Length = (ushort)mmuCache.TotalBytesRead,
                       MachineCycles = timer.MachineCycles,
                       ThrottlingStates = timer.ThrottlingStates
                   };
        }

        /// <summary>
        /// This is just to clean up incrementing the cycle counters per instruction. Calls to Add 'should' be inlined by the JIT compiler.
        /// </summary>
        private class InstructionTimer
        {
            public InstructionTimer()
            {
                MachineCycles = 0;
                ThrottlingStates = 0;
            }

            public int MachineCycles { get; private set; }
            public int ThrottlingStates { get; private set; }

            public void Add(int mCycles, int tStates)
            {
                MachineCycles += mCycles;
                ThrottlingStates += tStates;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mmuCache"></param>
        /// <param name="expressions"></param>
        /// <param name="timer"></param>
        /// <returns>True if we can continue to decode operations sequentially, false if it can't e.g. a jumo</returns>
        private static bool TryDecodeNextOperation(IMmuCache mmuCache, ICollection<Expression> expressions, InstructionTimer timer)
        {
            var opCode = (PrimaryOpCode)mmuCache.NextByte();

            expressions.Add(IncrementMemoryRefreshRegisterExpression);
            expressions.Add(IncrementProgramCounterRegisterExpression);

            // Predefine some fields to hold arguements. Gets around implementing cases as blocks.
            ushort wordArg;
            byte byteArg;

            switch (opCode)
            {
                case PrimaryOpCode.NOP:
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.HALT:
                    timer.Add(1, 4);
                    return false;

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
                    byteArg = mmuCache.NextByte();
                    expressions.Add(Expression.Assign(A, Expression.Constant(byteArg)));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_B_n:
                    byteArg = mmuCache.NextByte();
                    expressions.Add(Expression.Assign(B, Expression.Constant(byteArg)));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_C_n:
                    byteArg = mmuCache.NextByte();
                    expressions.Add(Expression.Assign(C, Expression.Constant(byteArg)));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_D_n:
                    byteArg = mmuCache.NextByte();
                    expressions.Add(Expression.Assign(D, Expression.Constant(byteArg)));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_E_n:
                    byteArg = mmuCache.NextByte();
                    expressions.Add(Expression.Assign(E, Expression.Constant(byteArg)));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_H_n:
                    byteArg = mmuCache.NextByte();
                    expressions.Add(Expression.Assign(H, Expression.Constant(byteArg)));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_L_n:
                    byteArg = mmuCache.NextByte();
                    expressions.Add(Expression.Assign(L, Expression.Constant(byteArg)));
                    timer.Add(2, 7);
                    break;

                    // LD r, (HL)
                case PrimaryOpCode.LD_A_mHL:
                    expressions.Add(Expression.Assign(A, ReadByteByHlRegisterMethodExpression));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_B_mHL:
                    expressions.Add(Expression.Assign(B, ReadByteByHlRegisterMethodExpression));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_C_mHL:
                    expressions.Add(Expression.Assign(C, ReadByteByHlRegisterMethodExpression));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_D_mHL:
                    expressions.Add(Expression.Assign(D, ReadByteByHlRegisterMethodExpression));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_E_mHL:
                    expressions.Add(Expression.Assign(E, ReadByteByHlRegisterMethodExpression));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_H_mHL:
                    expressions.Add(Expression.Assign(H, ReadByteByHlRegisterMethodExpression));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_L_mHL:
                    expressions.Add(Expression.Assign(L, ReadByteByHlRegisterMethodExpression));
                    timer.Add(2, 7);
                    break;


                    // ********* Jump *********
                case PrimaryOpCode.JP:
                    wordArg = mmuCache.NextWord();
                    expressions.Add(Expression.Assign(Pc, Expression.Constant(wordArg, typeof(ushort))));
                    timer.Add(3, 10);
                    return false;
                default:
                    throw new NotImplementedException(opCode.ToString());
            }

            return true;
        }
    }
}