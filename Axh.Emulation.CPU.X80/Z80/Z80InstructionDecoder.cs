namespace Axh.Emulation.CPU.X80.Z80
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    using Axh.Emulation.CPU.X80.Contracts.Memory;
    using Axh.Emulation.CPU.X80.Contracts.OpCodes;
    using Axh.Emulation.CPU.X80.Contracts.Registers;
    using Axh.Emulation.CPU.X80.Contracts.Z80;

    public class Z80InstructionDecoder : IZ80InstructionDecoder
    {
        private static readonly PropertyInfo R = GetPropertyInfo<IZ80Registers, byte>(r => r.R);
        private static readonly PropertyInfo ProgramCounter = GetPropertyInfo<IZ80Registers, ushort>(r => r.ProgramCounter);

        private static readonly ParameterExpression RegistersParameterExpression = Expression.Parameter(typeof(IZ80Registers), "registers");
        private static readonly ParameterExpression MmuParameterExpression = Expression.Parameter(typeof(IMmu), "mmu");
        private static readonly BlockExpression AdjustRegistersForNextOpcodeExpression = GetAdjustRegistersForNextOpcodeExpression();

        public Expression<Action<IZ80Registers, IMmu>> DecodeOperation(PrimaryOpCode opCode)
        {

            return Expression.Lambda<Action<IZ80Registers, IMmu>>(AdjustRegistersForNextOpcodeExpression, RegistersParameterExpression, MmuParameterExpression);
        }

        private static BlockExpression GetAdjustRegistersForNextOpcodeExpression()
        {
            // Increment 7 lsb of memory refresh register.
            var memoryRefreshRegister = Expression.Property(RegistersParameterExpression, R);
            var increment7LsbR = Expression.And(Expression.Increment(Expression.Convert(memoryRefreshRegister, typeof(int))), Expression.Constant(0x7f));
            var assignR = Expression.Assign(memoryRefreshRegister, Expression.Convert(increment7LsbR, typeof(byte)));

            // Increment program counter register
            var programCounterRegister = Expression.Property(RegistersParameterExpression, ProgramCounter);
            var incrementPc = Expression.Convert(Expression.Increment(Expression.Convert(programCounterRegister, typeof(int))), typeof(ushort));
            var assignPc = Expression.Assign(programCounterRegister, incrementPc);

            return Expression.Block(assignR, assignPc);
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
