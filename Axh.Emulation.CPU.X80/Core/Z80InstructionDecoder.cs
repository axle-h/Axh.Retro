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

    public class Z80InstructionDecoder : IZ80InstructionDecoder
    {
        private static readonly ParameterExpression RegistersParameterExpression;
        private static readonly ParameterExpression MmuParameterExpression;

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

        private static readonly Expression IncrementMemoryRefreshRegisterExpression;

        private static readonly Expression IncrementProgramCounterRegisterExpression;

        static Z80InstructionDecoder()
        {
            RegistersParameterExpression = Expression.Parameter(typeof(IZ80Registers), "registers");
            MmuParameterExpression = Expression.Parameter(typeof(IMmu), "mmu");
            R = Expression.Property(RegistersParameterExpression, GetPropertyInfo<IZ80Registers, byte>(r => r.R));
            Pc = Expression.Property(RegistersParameterExpression, GetPropertyInfo<IZ80Registers, ushort>(r => r.ProgramCounter));

            var generalPurposeRegisters = Expression.Property(RegistersParameterExpression, GetPropertyInfo<IZ80Registers, IGeneralPurposeRegisterSet>(r => r.GeneralPurposeRegisters));
            A = Expression.Property(generalPurposeRegisters, GetPropertyInfo<IGeneralPurposeRegisterSet, byte>(r => r.A));
            B = Expression.Property(generalPurposeRegisters, GetPropertyInfo<IGeneralPurposeRegisterSet, byte>(r => r.B));
            C = Expression.Property(generalPurposeRegisters, GetPropertyInfo<IGeneralPurposeRegisterSet, byte>(r => r.C));
            D = Expression.Property(generalPurposeRegisters, GetPropertyInfo<IGeneralPurposeRegisterSet, byte>(r => r.D));
            E = Expression.Property(generalPurposeRegisters, GetPropertyInfo<IGeneralPurposeRegisterSet, byte>(r => r.E));
            H = Expression.Property(generalPurposeRegisters, GetPropertyInfo<IGeneralPurposeRegisterSet, byte>(r => r.H));
            L = Expression.Property(generalPurposeRegisters, GetPropertyInfo<IGeneralPurposeRegisterSet, byte>(r => r.L));

            var flagsRegister = Expression.Property(generalPurposeRegisters, GetPropertyInfo<IGeneralPurposeRegisterSet, IFlagsRegister>(r => r.Flags));
            F = Expression.Property(flagsRegister, GetPropertyInfo<IFlagsRegister, byte>(r => r.Register));


            // Increment 7 lsb of memory refresh register.
            var increment7LsbR = Expression.And(Expression.Increment(Expression.Convert(R, typeof(int))), Expression.Constant(0x7f));
            IncrementMemoryRefreshRegisterExpression = Expression.Assign(R, Expression.Convert(increment7LsbR, typeof(byte)));

            // Increment program counter register
            var incrementPc = Expression.Convert(Expression.Increment(Expression.Convert(Pc, typeof(int))), typeof(ushort));
            IncrementProgramCounterRegisterExpression = Expression.Assign(Pc, incrementPc);

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

            var expressionBlock = Expression.Block(expressions);
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

            switch (opCode)
            {
                case PrimaryOpCode.NOP:
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.HALT:
                    timer.Add(1, 4);
                    return false;

                // ********* 8-bit load *********
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
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_H_A:
                    expressions.Add(Expression.Assign(H, A));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_L_A:
                    expressions.Add(Expression.Assign(L, A));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_A_B:
                    expressions.Add(Expression.Assign(A, B));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_B_B:
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_C_B:
                    expressions.Add(Expression.Assign(C, B));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_D_B:
                    expressions.Add(Expression.Assign(D, B));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_E_B:
                    expressions.Add(Expression.Assign(E, B));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_H_B:
                    expressions.Add(Expression.Assign(H, B));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_L_B:
                    expressions.Add(Expression.Assign(L, B));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_A_C:
                    expressions.Add(Expression.Assign(A, C));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_B_C:
                    expressions.Add(Expression.Assign(B, C));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_C_C:
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_D_C:
                    expressions.Add(Expression.Assign(D, C));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_E_C:
                    expressions.Add(Expression.Assign(E, C));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_H_C:
                    expressions.Add(Expression.Assign(H, C));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_L_C:
                    expressions.Add(Expression.Assign(L, C));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_A_D:
                    expressions.Add(Expression.Assign(A, D));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_B_D:
                    expressions.Add(Expression.Assign(B, D));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_C_D:
                    expressions.Add(Expression.Assign(C, D));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_D_D:
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_E_D:
                    expressions.Add(Expression.Assign(E, D));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_H_D:
                    expressions.Add(Expression.Assign(H, D));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_L_D:
                    expressions.Add(Expression.Assign(L, D));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_A_E:
                    expressions.Add(Expression.Assign(A, E));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_B_E:
                    expressions.Add(Expression.Assign(B, E));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_C_E:
                    expressions.Add(Expression.Assign(C, E));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_D_E:
                    expressions.Add(Expression.Assign(D, E));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_E_E:
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_H_E:
                    expressions.Add(Expression.Assign(H, E));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_L_E:
                    expressions.Add(Expression.Assign(L, E));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_A_H:
                    expressions.Add(Expression.Assign(A, H));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_B_H:
                    expressions.Add(Expression.Assign(B, H));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_C_H:
                    expressions.Add(Expression.Assign(C, H));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_D_H:
                    expressions.Add(Expression.Assign(D, H));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_E_H:
                    expressions.Add(Expression.Assign(E, H));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_H_H:
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_L_H:
                    expressions.Add(Expression.Assign(L, H));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_A_L:
                    expressions.Add(Expression.Assign(A, L));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_B_L:
                    expressions.Add(Expression.Assign(B, L));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_C_L:
                    expressions.Add(Expression.Assign(C, L));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_D_L:
                    expressions.Add(Expression.Assign(D, L));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_E_L:
                    expressions.Add(Expression.Assign(E, L));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_H_L:
                    expressions.Add(Expression.Assign(H, L));
                    timer.Add(1, 4); break;
                case PrimaryOpCode.LD_L_L:
                    timer.Add(1, 4); break;

                // ********* Jump *********
                case PrimaryOpCode.JP:
                    var address = mmuCache.NextWord();
                    expressions.Add(Expression.Assign(Pc, Expression.Constant(address, typeof(ushort))));
                    timer.Add(3, 10);
                    return false;
                default:
                    throw new NotImplementedException(opCode.ToString());
            }

            return true;
        }
        
        private static PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", propertyLambda));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", propertyLambda));
            }

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format("Expresion '{0}' refers to a property that is not from type {1}.", propertyLambda, type));
            }

            return propInfo;
        }
    }
}
