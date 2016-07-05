using System;
using System.Linq.Expressions;
using System.Reflection;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Registers;
using Axh.Retro.GameBoy.Registers.Interfaces;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.GameBoy.Tests.Devices
{
    [TestFixture]
    public class JoyPadTests
    {
        private const byte P10 = 0x01;
        private const byte P11 = 0x02;
        private const byte P12 = 0x04;
        private const byte P13 = 0x08;

        private const byte P14 = 0x10;
        private const byte P15 = 0x20;

        private IJoyPadRegister _joyPad;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var mockInterruptManager = new Mock<IInterruptFlagsRegister>();
            _joyPad = new JoyPad(mockInterruptManager.Object);
        }

        private void AssertRegisterMatrix(Expression<Func<IJoyPad, bool>> buttonExpression,
            byte regOut,
            byte expectedMask)
        {
            var joyPadExpression = Expression.Parameter(typeof (IJoyPad), "joyPad");
            var property = GetPropertyExpression(joyPadExpression, buttonExpression);

            var getLambda = Expression.Lambda<Func<IJoyPad, bool>>(property, joyPadExpression).Compile();
            var setLambda =
                Expression.Lambda<Action<IJoyPad>>(Expression.Assign(property, Expression.Constant(true)),
                    joyPadExpression).Compile();
            var resetLambda =
                Expression.Lambda<Action<IJoyPad>>(Expression.Assign(property, Expression.Constant(false)),
                    joyPadExpression).Compile();

            // Assert button pressed
            setLambda(_joyPad);
            _joyPad.Register = regOut;
            // Button is pressed then flag is reset
            Assert.IsTrue((_joyPad.Register & expectedMask) == 0);

            // Assert button released
            resetLambda(_joyPad);
            _joyPad.Register = regOut;
            // Button is released then flag is set
            Assert.IsTrue((_joyPad.Register & expectedMask) > 0);
        }

        public static MemberExpression GetPropertyExpression<TSource, TProperty>(Expression instance,
            Expression<Func<TSource, TProperty>>
                propertyLambda)
        {
            var type = typeof (TSource);

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

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType) &&
                !propInfo.ReflectedType.IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    $"Expresion '{propertyLambda}' refers to a property that is not from type {type}.");
            }

            return Expression.Property(instance, propInfo);
        }

        [Test]
        public void A()
        {
            AssertRegisterMatrix(j => j.A, P15, P10);
        }

        [Test]
        public void B()
        {
            AssertRegisterMatrix(j => j.B, P15, P11);
        }

        [Test]
        public void Down()
        {
            AssertRegisterMatrix(j => j.Down, P14, P13);
        }

        [Test]
        public void Left()
        {
            AssertRegisterMatrix(j => j.Left, P14, P11);
        }

        [Test]
        public void Right()
        {
            AssertRegisterMatrix(j => j.Right, P14, P10);
        }

        [Test]
        public void Select()
        {
            AssertRegisterMatrix(j => j.Select, P15, P12);
        }

        [Test]
        public void Start()
        {
            AssertRegisterMatrix(j => j.Start, P15, P13);
        }

        [Test]
        public void Up()
        {
            AssertRegisterMatrix(j => j.Up, P14, P12);
        }
    }
}