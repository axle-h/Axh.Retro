namespace Axh.Retro.CPU.X80.Tests.Core
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Core;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ArithmeticLogicUnitTests
    {
        private Mock<IFlagsRegister> flags;

        private ArithmeticLogicUnit alu;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            flags = new Mock<IFlagsRegister>();
            alu = new ArithmeticLogicUnit(flags.Object);
        }

        private void Reset()
        {
            this.flags.ResetCalls();
            this.flags.SetupProperty(x => x.Sign, false);
            this.flags.SetupProperty(x => x.Zero, false);
            this.flags.SetupProperty(x => x.Flag5, false);
            this.flags.SetupProperty(x => x.HalfCarry, false);
            this.flags.SetupProperty(x => x.ParityOverflow, false);
            this.flags.SetupProperty(x => x.Flag3, false);
            this.flags.SetupProperty(x => x.Subtract, false);
            this.flags.SetupProperty(x => x.Carry, false);
        }

        [TestCase(0x4e, 0x4f, false, false)]
        [TestCase(0x0f, 0x10, true, false)]
        [TestCase(0x7f, 0x80, true, true)]
        public void Increment(byte a, byte expected, bool halfCarry, bool overflow)
        {
            Reset();
            var result = alu.Increment(a);

            Assert.AreEqual(expected, result);

            AssertFlags(result, halfCarry, overflow, false, null);
        }

        [TestCase(0x4e, 0x4d, false, false)]
        [TestCase(0xf0, 0xef, true, false)]
        [TestCase(0x80, 0x7f, true, true)]
        public void Decrement(byte a, byte expected, bool halfCarry, bool overflow)
        {
            Reset();

            var result = alu.Decrement(a);

            Assert.AreEqual(expected, result);

            AssertFlags(result, halfCarry, overflow, true, null);
        }
        
        [TestCase(0, 0, 0, false, false, false)]
        [TestCase(0, 1, 1, false, false, false)]
        [TestCase(0, 127, 127, false, false, false)]
        [TestCase(0, 128, 128, false, false, false)]
        [TestCase(0, 129, 129, false, false, false)]
        [TestCase(0, 255, 255, false, false, false)]
        [TestCase(1, 0, 1, false, false, false)]
        [TestCase(1, 1, 2, false, false, false)]
        [TestCase(1, 127, 128, true, false, true)]
        [TestCase(1, 128, 129, false, false, false)]
        [TestCase(1, 129, 130, false, false, false)]
        [TestCase(1, 255, 0, true, true, false)]
        [TestCase(127, 0, 127, false, false, false)]
        [TestCase(127, 1, 128, true, false, true)]
        [TestCase(127, 127, 254, true, false, true)]
        [TestCase(127, 128, 255, false, false, false)]
        [TestCase(127, 129, 0, true, true, false)]
        [TestCase(127, 255, 126, true, true, false)]
        [TestCase(128, 0, 128, false, false, false)]
        [TestCase(128, 1, 129, false, false, false)]
        [TestCase(128, 127, 255, false, false, false)]
        [TestCase(128, 128, 0, false, true, true)]
        [TestCase(128, 129, 1, false, true, true)]
        [TestCase(128, 255, 127, false, true, true)]
        [TestCase(129, 0, 129, false, false, false)]
        [TestCase(129, 1, 130, false, false, false)]
        [TestCase(129, 127, 0, true, true, false)]
        [TestCase(129, 128, 1, false, true, true)]
        [TestCase(129, 129, 2, false, true, true)]
        [TestCase(129, 255, 128, true, true, false)]
        [TestCase(255, 0, 255, false, false, false)]
        [TestCase(255, 1, 0, true, true, false)]
        [TestCase(255, 127, 126, true, true, false)]
        [TestCase(255, 128, 127, false, true, true)]
        [TestCase(255, 129, 128, true, true, false)]
        [TestCase(255, 255, 254, true, true, false)]
        public void Add(byte a, byte b, byte expected, bool halfCarry, bool carry, bool overflow)
        {
            Reset();

            var result = alu.Add(a, b);

            Assert.AreEqual(expected, result);

            AssertFlags(result, halfCarry, overflow, false, carry);
        }
        
        [TestCase(0, 0, 1, false, false, false)]
        [TestCase(0, 1, 2, false, false, false)]
        [TestCase(0, 127, 128, true, false, true)]
        [TestCase(0, 128, 129, false, false, false)]
        [TestCase(0, 129, 130, false, false, false)]
        [TestCase(0, 255, 0, true, true, false)]
        [TestCase(1, 0, 2, false, false, false)]
        [TestCase(1, 1, 3, false, false, false)]
        [TestCase(1, 127, 129, true, false, true)]
        [TestCase(1, 128, 130, false, false, false)]
        [TestCase(1, 129, 131, false, false, false)]
        [TestCase(1, 255, 1, true, true, false)]
        [TestCase(127, 0, 128, true, false, true)]
        [TestCase(127, 1, 129, true, false, true)]
        [TestCase(127, 127, 255, true, false, true)]
        [TestCase(127, 128, 0, true, true, false)]
        [TestCase(127, 129, 1, true, true, false)]
        [TestCase(127, 255, 127, true, true, false)]
        [TestCase(128, 0, 129, false, false, false)]
        [TestCase(128, 1, 130, false, false, false)]
        [TestCase(128, 127, 0, true, true, false)]
        [TestCase(128, 128, 1, false, true, true)]
        [TestCase(128, 129, 2, false, true, true)]
        [TestCase(128, 255, 128, true, true, false)]
        [TestCase(129, 0, 130, false, false, false)]
        [TestCase(129, 1, 131, false, false, false)]
        [TestCase(129, 127, 1, true, true, false)]
        [TestCase(129, 128, 2, false, true, true)]
        [TestCase(129, 129, 3, false, true, true)]
        [TestCase(129, 255, 129, true, true, false)]
        [TestCase(255, 0, 0, true, true, false)]
        [TestCase(255, 1, 1, true, true, false)]
        [TestCase(255, 127, 127, true, true, false)]
        [TestCase(255, 128, 128, true, true, false)]
        [TestCase(255, 129, 129, true, true, false)]
        [TestCase(255, 255, 255, true, true, false)]
        public void AddWithCarry(byte a, byte b, byte expected, bool halfCarry, bool carry, bool overflow)
        {
            Reset();

            this.flags.SetupProperty(x => x.Carry, true);

            var result = alu.AddWithCarry(a, b);

            Assert.AreEqual(expected, result);

            AssertFlags(result, halfCarry, overflow, false, carry);
        }

        [TestCase(0, 0, 0, false, false, false)]
        [TestCase(0, 1, 255, true, true, false)]
        [TestCase(0, 127, 129, true, true, false)]
        [TestCase(0, 128, 128, false, true, true)]
        [TestCase(0, 129, 127, true, true, false)]
        [TestCase(0, 255, 1, true, true, false)]
        [TestCase(1, 0, 1, false, false, false)]
        [TestCase(1, 1, 0, false, false, false)]
        [TestCase(1, 127, 130, true, true, false)]
        [TestCase(1, 128, 129, false, true, true)]
        [TestCase(1, 129, 128, false, true, true)]
        [TestCase(1, 255, 2, true, true, false)]
        [TestCase(127, 0, 127, false, false, false)]
        [TestCase(127, 1, 126, false, false, false)]
        [TestCase(127, 127, 0, false, false, false)]
        [TestCase(127, 128, 255, false, true, true)]
        [TestCase(127, 129, 254, false, true, true)]
        [TestCase(127, 255, 128, false, true, true)]
        [TestCase(128, 0, 128, false, false, false)]
        [TestCase(128, 1, 127, true, false, true)]
        [TestCase(128, 127, 1, true, false, true)]
        [TestCase(128, 128, 0, false, false, false)]
        [TestCase(128, 129, 255, true, true, false)]
        [TestCase(128, 255, 129, true, true, false)]
        [TestCase(129, 0, 129, false, false, false)]
        [TestCase(129, 1, 128, false, false, false)]
        [TestCase(129, 127, 2, true, false, true)]
        [TestCase(129, 128, 1, false, false, false)]
        [TestCase(129, 129, 0, false, false, false)]
        [TestCase(129, 255, 130, true, true, false)]
        [TestCase(255, 0, 255, false, false, false)]
        [TestCase(255, 1, 254, false, false, false)]
        [TestCase(255, 127, 128, false, false, false)]
        [TestCase(255, 128, 127, false, false, false)]
        [TestCase(255, 129, 126, false, false, false)]
        [TestCase(255, 255, 0, false, false, false)]
        public void Subtract(byte a, byte b, byte expected, bool halfCarry, bool carry, bool overflow)
        {
            Reset();

            var result = alu.Subtract(a, b);

            Assert.AreEqual(expected, result);

            AssertFlags(result, halfCarry, overflow, true, carry);
        }

        [TestCase(0, 0, 255, true, true, false)]
        [TestCase(0, 1, 254, true, true, false)]
        [TestCase(0, 127, 128, true, true, false)]
        [TestCase(0, 128, 127, true, true, false)]
        [TestCase(0, 129, 126, true, true, false)]
        [TestCase(0, 255, 0, true, true, false)]
        [TestCase(1, 0, 0, false, false, false)]
        [TestCase(1, 1, 255, true, true, false)]
        [TestCase(1, 127, 129, true, true, false)]
        [TestCase(1, 128, 128, false, true, true)]
        [TestCase(1, 129, 127, true, true, false)]
        [TestCase(1, 255, 1, true, true, false)]
        [TestCase(127, 0, 126, false, false, false)]
        [TestCase(127, 1, 125, false, false, false)]
        [TestCase(127, 127, 255, true, true, false)]
        [TestCase(127, 128, 254, false, true, true)]
        [TestCase(127, 129, 253, false, true, true)]
        [TestCase(127, 255, 127, true, true, false)]
        [TestCase(128, 0, 127, true, false, true)]
        [TestCase(128, 1, 126, true, false, true)]
        [TestCase(128, 127, 0, true, false, true)]
        [TestCase(128, 128, 255, true, true, false)]
        [TestCase(128, 129, 254, true, true, false)]
        [TestCase(128, 255, 128, true, true, false)]
        [TestCase(129, 0, 128, false, false, false)]
        [TestCase(129, 1, 127, true, false, true)]
        [TestCase(129, 127, 1, true, false, true)]
        [TestCase(129, 128, 0, false, false, false)]
        [TestCase(129, 129, 255, true, true, false)]
        [TestCase(129, 255, 129, true, true, false)]
        [TestCase(255, 0, 254, false, false, false)]
        [TestCase(255, 1, 253, false, false, false)]
        [TestCase(255, 127, 127, true, false, true)]
        [TestCase(255, 128, 126, false, false, false)]
        [TestCase(255, 129, 125, false, false, false)]
        [TestCase(255, 255, 255, true, true, false)]
        public void SubtractWithCarry(byte a, byte b, byte expected, bool halfCarry, bool carry, bool overflow)
        {
            Reset();

            this.flags.SetupProperty(x => x.Carry, true);

            var result = alu.SubtractWithCarry(a, b);

            Assert.AreEqual(expected, result);

            AssertFlags(result, halfCarry, overflow, true, carry);
        }

        [TestCase(0, 0, 0, false, false)]
        [TestCase(0, 1, 255, true, false)]
        [TestCase(0, 127, 129, true, false)]
        [TestCase(0, 128, 128, false, true)]
        [TestCase(0, 129, 127, true, false)]
        [TestCase(0, 255, 1, true,  false)]
        [TestCase(1, 0, 1, false, false)]
        [TestCase(1, 1, 0, false, false)]
        [TestCase(1, 127, 130, true, false)]
        [TestCase(1, 128, 129, false, true)]
        [TestCase(1, 129, 128, false, true)]
        [TestCase(1, 255, 2, true, false)]
        [TestCase(127, 0, 127, false, false)]
        [TestCase(127, 1, 126, false, false)]
        [TestCase(127, 127, 0, false, false)]
        [TestCase(127, 128, 255, false, true)]
        [TestCase(127, 129, 254, false, true)]
        [TestCase(127, 255, 128, false, true)]
        [TestCase(128, 0, 128, false, false)]
        [TestCase(128, 1, 127, true, true)]
        [TestCase(128, 127, 1, true, true)]
        [TestCase(128, 128, 0, false, false)]
        [TestCase(128, 129, 255, true, false)]
        [TestCase(128, 255, 129, true, false)]
        [TestCase(129, 0, 129, false, false)]
        [TestCase(129, 1, 128, false, false)]
        [TestCase(129, 127, 2, true, true)]
        [TestCase(129, 128, 1, false, false)]
        [TestCase(129, 129, 0, false, false)]
        [TestCase(129, 255, 130, true, false)]
        [TestCase(255, 0, 255, false, false)]
        [TestCase(255, 1, 254, false, false)]
        [TestCase(255, 127, 128, false, false)]
        [TestCase(255, 128, 127, false, false)]
        [TestCase(255, 129, 126, false, false)]
        [TestCase(255, 255, 0, false, false)]
        public void Compare(byte a, byte b, byte expected, bool halfCarry, bool overflow)
        {
            Reset();

            alu.Compare(a, b);
            
            AssertFlags(expected, halfCarry, overflow, true, null);
        }

        [TestCase(0x7b, 0xc3, 0x43)]
        public void And(byte a, byte b, byte expected)
        {
            Reset();

            var result = alu.And(a, b);

            Assert.AreEqual(expected, result);
            flags.Verify(x => x.SetParityFlags(expected), Times.Once);
        }
        
        [TestCase(0x48, 0x12, 0x5a)]
        public void Or(byte a, byte b, byte expected)
        {
            Reset();

            var result = alu.Or(a, b);

            Assert.AreEqual(expected, result);
            flags.Verify(x => x.SetParityFlags(expected), Times.Once);
        }
        
        [TestCase(0x96, 0x5d, 0xcb)]
        public void Xor(byte a, byte b, byte expected)
        {
            Reset();

            var result = alu.Xor(a, b);

            Assert.AreEqual(expected, result);
            flags.Verify(x => x.SetParityFlags(expected), Times.Once);
        }

        private void AssertFlags(byte result, bool? halfCarry, bool? parityOverflow, bool? subtract, bool? carry)
        {
            flags.Verify(x => x.SetResultFlags(result), Times.Once);
            VerifyFlag(x => x.HalfCarry, halfCarry);
            VerifyFlag(x => x.ParityOverflow, parityOverflow);
            VerifyFlag(x => x.Subtract, subtract);
            VerifyFlag(x => x.Carry, carry);
        }

        private void VerifyFlag(Expression<Func<IFlagsRegister, bool>> propertyLambda, bool? value)
        {
            var flagsExpression = Expression.Parameter(typeof(IFlagsRegister), "flags");
            var property = GetPropertyExpression(flagsExpression, propertyLambda);
            var getLambda = Expression.Lambda<Func<IFlagsRegister, bool>>(property, flagsExpression);
            
            if (!value.HasValue)
            {
                flags.Verify(getLambda, Times.Never);
                return;
            }
            
            var setExpression = Expression.Assign(property, Expression.Constant(value.Value));
            var setLambda = Expression.Lambda<Action<IFlagsRegister>>(setExpression, flagsExpression);

            flags.VerifySet(setLambda.Compile(), Times.AtLeastOnce);

            var flag = getLambda.Compile()(flags.Object);
            if (value.Value)
            {
                Assert.IsTrue(flag, getLambda.ToString());
                return;
            }

            Assert.IsFalse(flag, getLambda.ToString());
        }

        private static MemberExpression GetPropertyExpression<TSource, TProperty>(Expression instance, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
            }

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType) && !propInfo.ReflectedType.IsAssignableFrom(type))
            {
                throw new ArgumentException($"Expresion '{propertyLambda}' refers to a property that is not from type {type}.");
            }

            return Expression.Property(instance, propInfo);
        }
    }
}
