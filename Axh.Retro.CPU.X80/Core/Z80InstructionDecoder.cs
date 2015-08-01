using Axh.Retro.CPU.X80.Contracts.Core;
using Axh.Retro.CPU.X80.Contracts.Factories;
using Axh.Retro.CPU.X80.Contracts.Memory;
using Axh.Retro.CPU.X80.Contracts.OpCodes;
using Axh.Retro.CPU.X80.Contracts.Registers;
using Axh.Retro.CPU.X80.Util;

namespace Axh.Retro.CPU.X80.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using Retro.CPU.X80.Contracts.Factories;
    using Retro.CPU.X80.Contracts.Memory;
    using Retro.CPU.X80.Contracts.OpCodes;
    using Retro.CPU.X80.Contracts.Registers;
    using Retro.CPU.X80.Contracts.Core;
    using Retro.CPU.X80.Util;

    public class Z80InstructionDecoder : IZ80InstructionDecoder
    {
        private static readonly ParameterExpression RegistersExpression;
        private static readonly ParameterExpression MmuExpression;

        /// <summary>
        /// Byte parameter b
        /// </summary>
        private static readonly ParameterExpression LocalBExpression;

        /// <summary>
        /// Word parameter w
        /// </summary>
        private static readonly ParameterExpression LocalWExpression;

        // Register expressions
        private static readonly MemberExpression R;
        private static readonly MemberExpression PC;
        private static readonly MemberExpression A;
        private static readonly MemberExpression B;
        private static readonly MemberExpression C;
        private static readonly MemberExpression D;
        private static readonly MemberExpression E;
        private static readonly MemberExpression F;
        private static readonly MemberExpression H;
        private static readonly MemberExpression L;
        private static readonly MemberExpression HL;
        private static readonly MemberExpression IX;
        private static readonly MemberExpression IY;

        /// <summary>
        /// Reads a byte from the mmu at the address at TempWordParameterExpression
        /// </summary>
        private static readonly MethodCallExpression ReadByteByTempMethodExpression;

        /// <summary>
        /// Reads a byte from the mmu at the address in HL
        /// </summary>
        private static readonly MethodCallExpression ReadByteByHlRegisterMethodExpression;

        /// <summary>
        /// Reads a byte from the mmu at the address at IX + b (using 2's compliant addition)
        /// </summary>
        private static readonly MethodCallExpression ReadByteByIxIndexRegisterExpression;

        /// <summary>
        /// Reads a byte from the mmu at the address at IY + b (using 2's compliant addition)
        /// </summary>
        private static readonly MethodCallExpression ReadByteByIyIndexRegisterExpression;

        private static readonly MethodInfo MmuWriteByteMethodInfo;

        static Z80InstructionDecoder()
        {
            RegistersExpression = Expression.Parameter(typeof(IZ80Registers), "registers");
            MmuExpression = Expression.Parameter(typeof(IMmu), "mmu");
            LocalBExpression = Expression.Parameter(typeof(byte), "b");
            LocalWExpression = Expression.Parameter(typeof(ushort), "w");

            R = RegistersExpression.GetPropertyExpression<IZ80Registers, byte>(r => r.R);
            PC = RegistersExpression.GetPropertyExpression<IZ80Registers, ushort>(r => r.ProgramCounter);
            IX = RegistersExpression.GetPropertyExpression<IZ80Registers, ushort>(r => r.IX);
            IY = RegistersExpression.GetPropertyExpression<IZ80Registers, ushort>(r => r.IY);

            var generalPurposeRegisters = RegistersExpression.GetPropertyExpression<IZ80Registers, IGeneralPurposeRegisterSet>(r => r.GeneralPurposeRegisters);
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
            
            ReadByteByTempMethodExpression = MmuExpression.GetMethodExpression<IMmu, ushort, byte>((mmu, address) => mmu.ReadByte(address), LocalWExpression);
            ReadByteByHlRegisterMethodExpression = MmuExpression.GetMethodExpression<IMmu, ushort, byte>((mmu, address) => mmu.ReadByte(address), HL);

            var getIxIndexRegisterAddressExpression =
                Expression.Convert(Expression.Add(Expression.Convert(IX, typeof(int)), Expression.Convert(Expression.Convert(LocalBExpression, typeof(sbyte)), typeof(int))), typeof(ushort));
            ReadByteByIxIndexRegisterExpression = MmuExpression.GetMethodExpression<IMmu, ushort, byte>((mmu, address) => mmu.ReadByte(address), getIxIndexRegisterAddressExpression);

            var getIyIndexRegisterAddressExpression =
                Expression.Convert(Expression.Add(Expression.Convert(IY, typeof(int)), Expression.Convert(Expression.Convert(LocalBExpression, typeof(sbyte)), typeof(int))), typeof(ushort));
            ReadByteByIyIndexRegisterExpression = MmuExpression.GetMethodExpression<IMmu, ushort, byte>((mmu, address) => mmu.ReadByte(address), getIyIndexRegisterAddressExpression);

            MmuWriteByteMethodInfo = ExpressionHelpers.GetMethodInfo<IMmu, ushort, byte>((mmu, address, value) => mmu.WriteByte(address, value));
        }

        private static Expression GetAddToProgramCounterExpression(int value)
        {
            return Expression.Assign(PC, Expression.Convert(Expression.Add(Expression.Convert(PC, typeof(int)), Expression.Constant(value)), typeof(ushort)));
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
                if (!TryDecodeNextOperation(mmuCache, expressions, timer))
                {
                    break;
                }

                if (mmuCache.TotalBytesRead >= ushort.MaxValue)
                {
                    // Since we are not breaking the block inside the decode step we need to adjust the program counter here.
                    expressions.Add(GetAddToProgramCounterExpression(mmuCache.TotalBytesRead));
                    break;
                }
            }

            // Add the block length to the 7 lsb of memory refresh register.
            var blockLengthExpression = Expression.Constant(mmuCache.TotalBytesRead, typeof(int));
            var increment7LsbR = Expression.And(Expression.Add(Expression.Convert(R, typeof(int)), blockLengthExpression), Expression.Constant(0x7f));
            expressions.Add(Expression.Assign(R, Expression.Convert(increment7LsbR, typeof(byte))));
            
            var expressionBlock = Expression.Block(new [] { LocalBExpression, LocalWExpression }, expressions);
            var lambda = Expression.Lambda<Action<IZ80Registers, IMmu>>(expressionBlock, RegistersExpression, MmuExpression);
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
        /// 
        /// </summary>
        /// <param name="mmuCache"></param>
        /// <param name="expressions"></param>
        /// <param name="timer"></param>
        /// <returns>True if we can continue to decode operations sequentially, false if it can't e.g. a jumo</returns>
        private static bool TryDecodeNextOperation(IMmuCache mmuCache, ICollection<Expression> expressions, InstructionTimer timer)
        {
            var opCode = (PrimaryOpCode)mmuCache.NextByte();
            
            switch (opCode)
            {
                case PrimaryOpCode.NOP:
                    timer.Add(1, 4);
                    break;
                case PrimaryOpCode.HALT:
                    expressions.Add(GetAddToProgramCounterExpression(mmuCache.TotalBytesRead));
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
                    expressions.Add(Expression.Assign(A, Expression.Constant(mmuCache.NextByte())));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_B_n:
                    expressions.Add(Expression.Assign(B, Expression.Constant(mmuCache.NextByte())));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_C_n:
                    expressions.Add(Expression.Assign(C, Expression.Constant(mmuCache.NextByte())));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_D_n:
                    expressions.Add(Expression.Assign(D, Expression.Constant(mmuCache.NextByte())));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_E_n:
                    expressions.Add(Expression.Assign(E, Expression.Constant(mmuCache.NextByte())));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_H_n:
                    expressions.Add(Expression.Assign(H, Expression.Constant(mmuCache.NextByte())));
                    timer.Add(2, 7);
                    break;
                case PrimaryOpCode.LD_L_n:
                    expressions.Add(Expression.Assign(L, Expression.Constant(mmuCache.NextByte())));
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

                    //LD (HL), r
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

                    // ********* Jump *********
                case PrimaryOpCode.JP:
                    expressions.Add(Expression.Assign(PC, Expression.Constant(mmuCache.NextWord(), typeof(ushort))));
                    timer.Add(3, 10);
                    return false;

                    // ********* Prefixes *********
                case PrimaryOpCode.Prefix_DD:
                    return TryDecodeNextDdPrefixOperation(mmuCache, expressions, timer);

                case PrimaryOpCode.Prefix_FD:
                    return TryDecodeNextFdPrefixOperation(mmuCache, expressions, timer);

                default:
                    throw new NotImplementedException(opCode.ToString());
            }

            return true;
        }


        private static bool TryDecodeNextDdPrefixOperation(IMmuCache mmuCache, ICollection<Expression> expressions, InstructionTimer timer)
        {
            var opCode = (PrefixDdFdOpCode)mmuCache.NextByte();
            
            switch (opCode)
            {
                    // LD r, (IX+d)
                    // We have defined this using ReadByteByIxIndexRegisterExpression for when we set the local parameter b to d.
                case PrefixDdFdOpCode.LD_A_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(A, ReadByteByIxIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_B_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(B, ReadByteByIxIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_C_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(C, ReadByteByIxIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_D_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(D, ReadByteByIxIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_E_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(E, ReadByteByIxIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_H_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(H, ReadByteByIxIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_L_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(L, ReadByteByIxIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                default:
                    throw new NotImplementedException(opCode.ToString());
            }

            return true;
        }

        private static bool TryDecodeNextFdPrefixOperation(IMmuCache mmuCache, ICollection<Expression> expressions, InstructionTimer timer)
        {
            var opCode = (PrefixDdFdOpCode)mmuCache.NextByte();

            switch (opCode)
            {
                // LD r, (IY+d)
                // We have defined this using ReadByteByIxIndexRegisterExpression for when we set the local parameter b to d.
                case PrefixDdFdOpCode.LD_A_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(A, ReadByteByIyIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_B_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(B, ReadByteByIyIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_C_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(C, ReadByteByIyIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_D_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(D, ReadByteByIyIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_E_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(E, ReadByteByIyIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_H_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(H, ReadByteByIyIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                case PrefixDdFdOpCode.LD_L_mIXYd:
                    expressions.Add(Expression.Assign(LocalBExpression, Expression.Constant(mmuCache.NextByte())));
                    expressions.Add(Expression.Assign(L, ReadByteByIyIndexRegisterExpression));
                    timer.Add(5, 19);
                    break;
                default:
                    throw new NotImplementedException(opCode.ToString());
            }

            return true;
        }
        
    }
}
