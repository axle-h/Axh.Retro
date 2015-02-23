namespace Axh.Emulation.CPU.X80.Z80
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using Axh.Emulation.CPU.X80.Contracts.Memory;
    using Axh.Emulation.CPU.X80.Contracts.OpCodes;
    using Axh.Emulation.CPU.X80.Contracts.Registers;
    using Axh.Emulation.CPU.X80.Contracts.Z80;

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

        public Expression<Action<IZ80Registers, IMmu>> DecodeSingleOperation(PrimaryOpCode opCode)
        {
            var operationExpressions = DecodeOperation(opCode);
            var expressionBlock = Expression.Block(operationExpressions);
            return Expression.Lambda<Action<IZ80Registers, IMmu>>(expressionBlock, RegistersParameterExpression, MmuParameterExpression);
        }

        private static IEnumerable<Expression> DecodeOperation(PrimaryOpCode opCode)
        {
            yield return IncrementMemoryRefreshRegisterExpression;
            yield return IncrementProgramCounterRegisterExpression;

            switch (opCode)
            {
                case PrimaryOpCode.NOP:
                    break;
                case PrimaryOpCode.LD_A_A:
                    break;
                case PrimaryOpCode.LD_B_A:
                    yield return Expression.Assign(B, A);
                    break;
                case PrimaryOpCode.LD_C_A:
                    yield return Expression.Assign(C, A);
                    break;
                case PrimaryOpCode.LD_D_A:
                    yield return Expression.Assign(D, A);
                    break;
                case PrimaryOpCode.LD_E_A:
                    yield return Expression.Assign(E, A);
                    break;
                case PrimaryOpCode.LD_H_A:
                    yield return Expression.Assign(H, A);
                    break;
                case PrimaryOpCode.LD_L_A:
                    yield return Expression.Assign(L, A);
                    break;
                case PrimaryOpCode.LD_A_B:
                    yield return Expression.Assign(A, B);
                    break;
                case PrimaryOpCode.LD_B_B:
                    break;
                case PrimaryOpCode.LD_C_B:
                    yield return Expression.Assign(C, B);
                    break;
                case PrimaryOpCode.LD_D_B:
                    yield return Expression.Assign(D, B);
                    break;
                case PrimaryOpCode.LD_E_B:
                    yield return Expression.Assign(E, B);
                    break;
                case PrimaryOpCode.LD_H_B:
                    yield return Expression.Assign(H, B);
                    break;
                case PrimaryOpCode.LD_L_B:
                    yield return Expression.Assign(L, B);
                    break;
                case PrimaryOpCode.LD_A_C:
                    yield return Expression.Assign(A, C);
                    break;
                case PrimaryOpCode.LD_B_C:
                    yield return Expression.Assign(B, C);
                    break;
                case PrimaryOpCode.LD_C_C:
                    break;
                case PrimaryOpCode.LD_D_C:
                    yield return Expression.Assign(D, C);
                    break;
                case PrimaryOpCode.LD_E_C:
                    yield return Expression.Assign(E, C);
                    break;
                case PrimaryOpCode.LD_H_C:
                    yield return Expression.Assign(H, C);
                    break;
                case PrimaryOpCode.LD_L_C:
                    yield return Expression.Assign(L, C);
                    break;
                case PrimaryOpCode.LD_A_D:
                    yield return Expression.Assign(A, D);
                    break;
                case PrimaryOpCode.LD_B_D:
                    yield return Expression.Assign(B, D);
                    break;
                case PrimaryOpCode.LD_C_D:
                    yield return Expression.Assign(C, D);
                    break;
                case PrimaryOpCode.LD_D_D:
                    break;
                case PrimaryOpCode.LD_E_D:
                    yield return Expression.Assign(E, D);
                    break;
                case PrimaryOpCode.LD_H_D:
                    yield return Expression.Assign(H, D);
                    break;
                case PrimaryOpCode.LD_L_D:
                    yield return Expression.Assign(L, D);
                    break;
                case PrimaryOpCode.LD_A_E:
                    yield return Expression.Assign(A, E);
                    break;
                case PrimaryOpCode.LD_B_E:
                    yield return Expression.Assign(B, E);
                    break;
                case PrimaryOpCode.LD_C_E:
                    yield return Expression.Assign(C, E);
                    break;
                case PrimaryOpCode.LD_D_E:
                    yield return Expression.Assign(D, E);
                    break;
                case PrimaryOpCode.LD_E_E:
                    break;
                case PrimaryOpCode.LD_H_E:
                    yield return Expression.Assign(H, E);
                    break;
                case PrimaryOpCode.LD_L_E:
                    yield return Expression.Assign(L, E);
                    break;
                case PrimaryOpCode.LD_A_H:
                    yield return Expression.Assign(A, H);
                    break;
                case PrimaryOpCode.LD_B_H:
                    yield return Expression.Assign(B, H);
                    break;
                case PrimaryOpCode.LD_C_H:
                    yield return Expression.Assign(C, H);
                    break;
                case PrimaryOpCode.LD_D_H:
                    yield return Expression.Assign(D, H);
                    break;
                case PrimaryOpCode.LD_E_H:
                    yield return Expression.Assign(E, H);
                    break;
                case PrimaryOpCode.LD_H_H:
                    break;
                case PrimaryOpCode.LD_L_H:
                    yield return Expression.Assign(L, H);
                    break;
                case PrimaryOpCode.LD_A_L:
                    yield return Expression.Assign(A, L);
                    break;
                case PrimaryOpCode.LD_B_L:
                    yield return Expression.Assign(B, L);
                    break;
                case PrimaryOpCode.LD_C_L:
                    yield return Expression.Assign(C, L);
                    break;
                case PrimaryOpCode.LD_D_L:
                    yield return Expression.Assign(D, L);
                    break;
                case PrimaryOpCode.LD_E_L:
                    yield return Expression.Assign(E, L);
                    break;
                case PrimaryOpCode.LD_H_L:
                    yield return Expression.Assign(H, L);
                    break;
                case PrimaryOpCode.LD_L_L:
                    break;
                default:
                    throw new NotImplementedException(opCode.ToString());
            }
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
