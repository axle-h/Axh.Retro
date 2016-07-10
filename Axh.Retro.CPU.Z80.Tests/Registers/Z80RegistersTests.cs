using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.State;
using Axh.Retro.CPU.Z80.Registers;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Registers
{
    [TestFixture]
    public class Z80RegistersTests
    {
        private IZ80Registers _z80Registers;
        
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _z80Registers = new Z80Registers(Z80RegisterState.Zero);
        }

        [Test]
        public void GetIxRegisterTest()
        {
            _z80Registers.IXl = 0x34;
            _z80Registers.IXh = 0x12;

            Assert.AreEqual(0x12, _z80Registers.IXh);
            Assert.AreEqual(0x34, _z80Registers.IXl);
            Assert.AreEqual(0x1234, _z80Registers.IX);
        }

        [Test]
        public void GetIyRegisterTest()
        {
            _z80Registers.IYl = 0x34;
            _z80Registers.IYh = 0x12;

            Assert.AreEqual(0x12, _z80Registers.IYh);
            Assert.AreEqual(0x34, _z80Registers.IYl);
            Assert.AreEqual(0x1234, _z80Registers.IY);
        }

        [Test]
        public void GetRegisterStateTest()
        {
            const byte I = 0x11, R = 0x22, IX = 0x33, IY = 0x44, ProgramCounter = 0x55, StackPointer = 0x66;
            const bool InterruptFlipFlop1 = true, InterruptFlipFlop2 = false;
            const InterruptMode InterruptMode = InterruptMode.InterruptMode1;

            _z80Registers.I = I;
            _z80Registers.R = R;
            _z80Registers.IX = IX;
            _z80Registers.IY = IY;
            _z80Registers.InterruptFlipFlop1 = InterruptFlipFlop1;
            _z80Registers.InterruptFlipFlop2 = InterruptFlipFlop2;
            _z80Registers.InterruptMode = InterruptMode;
            _z80Registers.ProgramCounter = ProgramCounter;
            _z80Registers.StackPointer = StackPointer;

            var state = _z80Registers.GetZ80RegisterState();

            Assert.AreEqual(I, state.I);
            Assert.AreEqual(R, state.R);
            Assert.AreEqual(IX, state.IX);
            Assert.AreEqual(IY, state.IY);
            Assert.AreEqual(InterruptFlipFlop1, state.InterruptFlipFlop1);
            Assert.AreEqual(InterruptFlipFlop2, state.InterruptFlipFlop2);
            Assert.AreEqual(InterruptMode, state.InterruptMode);
            Assert.AreEqual(ProgramCounter, _z80Registers.ProgramCounter);
            Assert.AreEqual(StackPointer, _z80Registers.StackPointer);
        }

        [Test]
        public void ResetToStateTest()
        {
            var state = RegisterTestObjects.Z80RegisterState;

            _z80Registers.ResetToState(state);

            Assert.AreEqual(state.I, _z80Registers.I);
            Assert.AreEqual(state.R, _z80Registers.R);
            Assert.AreEqual(state.IX, _z80Registers.IX);
            Assert.AreEqual(state.IY, _z80Registers.IY);
            Assert.AreEqual(state.InterruptFlipFlop1, _z80Registers.InterruptFlipFlop1);
            Assert.AreEqual(state.InterruptFlipFlop2, _z80Registers.InterruptFlipFlop2);
            Assert.AreEqual(state.InterruptMode, _z80Registers.InterruptMode);
            Assert.AreEqual(state.ProgramCounter, _z80Registers.ProgramCounter);
            Assert.AreEqual(state.StackPointer, _z80Registers.StackPointer);
        }

        [Test]
        public void SetIxRegisterTest()
        {
            _z80Registers.IX = 0x1234;

            Assert.AreEqual(0x12, _z80Registers.IXh);
            Assert.AreEqual(0x34, _z80Registers.IXl);
            Assert.AreEqual(0x1234, _z80Registers.IX);
        }

        [Test]
        public void SetIyRegisterTest()
        {
            _z80Registers.IY = 0x1234;

            Assert.AreEqual(0x12, _z80Registers.IYh);
            Assert.AreEqual(0x34, _z80Registers.IYl);
            Assert.AreEqual(0x1234, _z80Registers.IY);
        }

        [Test]
        public void SwitchToAlternativeAccumulatorAndFlagsRegistersTest()
        {
            _z80Registers.Reset();

            var primary = _z80Registers.AccumulatorAndFlagsRegisters;
            _z80Registers.SwitchToAlternativeAccumulatorAndFlagsRegisters();

            Assert.AreNotSame(primary, _z80Registers.AccumulatorAndFlagsRegisters);
        }

        [Test]
        public void SwitchToAlternativeGeneralPurposeRegistersTest()
        {
            _z80Registers.Reset();

            var primary = _z80Registers.GeneralPurposeRegisters;
            _z80Registers.SwitchToAlternativeGeneralPurposeRegisters();

            Assert.AreNotSame(primary, _z80Registers.GeneralPurposeRegisters);
        }
    }
}