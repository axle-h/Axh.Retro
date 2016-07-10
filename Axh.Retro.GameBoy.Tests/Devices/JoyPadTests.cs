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

        private void AssertRegisterMatrix(JoyPadButton button, byte regOut, byte expectedMask)
        {
            // Assert button pressed
            _joyPad.Buttons = button;
            _joyPad.Register = regOut;
            // Button is pressed then flag is reset
            Assert.IsTrue((_joyPad.Register & expectedMask) == 0);

            // Assert button released
            _joyPad.Buttons = JoyPadButton.None;
            _joyPad.Register = regOut;
            // Button is released then flag is set
            Assert.IsTrue((_joyPad.Register & expectedMask) > 0);
        }

        [Test]
        public void A() => AssertRegisterMatrix(JoyPadButton.A, P15, P10);

        [Test]
        public void B() => AssertRegisterMatrix(JoyPadButton.B, P15, P11);

        [Test]
        public void Down() => AssertRegisterMatrix(JoyPadButton.Down, P14, P13);

        [Test]
        public void Left() => AssertRegisterMatrix(JoyPadButton.Left, P14, P11);

        [Test]
        public void Right() => AssertRegisterMatrix(JoyPadButton.Right, P14, P10);

        [Test]
        public void Select() => AssertRegisterMatrix(JoyPadButton.Select, P15, P12);

        [Test]
        public void Start() => AssertRegisterMatrix(JoyPadButton.Start, P15, P13);

        [Test]
        public void Up() => AssertRegisterMatrix(JoyPadButton.Up, P14, P12);
    }
}