using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.State;
using Axh.Retro.CPU.Z80.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Registers
{
    [TestFixture]
    public class Z80RegistersTests
    {
        private IZ80Registers z80Registers;

        private Mock<IGeneralPurposeRegisterSet> primaryGeneralPurposeRegisters;

        private Mock<IGeneralPurposeRegisterSet> alternativeGeneralPurposeRegisters;

        private Mock<IAccumulatorAndFlagsRegisterSet> primaryAccumulatorAndFlagsRegisters;

        private Mock<IAccumulatorAndFlagsRegisterSet> alternativeAccumulatorAndFlagsRegisters;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            primaryGeneralPurposeRegisters = new Mock<IGeneralPurposeRegisterSet>();
            alternativeGeneralPurposeRegisters = new Mock<IGeneralPurposeRegisterSet>();
            primaryAccumulatorAndFlagsRegisters = new Mock<IAccumulatorAndFlagsRegisterSet>();
            alternativeAccumulatorAndFlagsRegisters = new Mock<IAccumulatorAndFlagsRegisterSet>();
            var initialStateFactory = new Mock<IInitialStateFactory<Z80RegisterState>>();

            z80Registers = new Z80Registers(primaryGeneralPurposeRegisters.Object,
                                            alternativeGeneralPurposeRegisters.Object,
                                            primaryAccumulatorAndFlagsRegisters.Object,
                                            alternativeAccumulatorAndFlagsRegisters.Object,
                                            initialStateFactory.Object);
        }

        [Test]
        public void GetIxRegisterTest()
        {
            z80Registers.IXl = 0x34;
            z80Registers.IXh = 0x12;

            Assert.AreEqual(0x12, z80Registers.IXh);
            Assert.AreEqual(0x34, z80Registers.IXl);
            Assert.AreEqual(0x1234, z80Registers.IX);
        }

        [Test]
        public void GetIyRegisterTest()
        {
            z80Registers.IYl = 0x34;
            z80Registers.IYh = 0x12;

            Assert.AreEqual(0x12, z80Registers.IYh);
            Assert.AreEqual(0x34, z80Registers.IYl);
            Assert.AreEqual(0x1234, z80Registers.IY);
        }

        [Test]
        public void GetRegisterStateTest()
        {
            const byte I = 0x11, R = 0x22, IX = 0x33, IY = 0x44, ProgramCounter = 0x55, StackPointer = 0x66;
            const bool InterruptFlipFlop1 = true, InterruptFlipFlop2 = false;
            const InterruptMode InterruptMode = InterruptMode.InterruptMode1;

            z80Registers.I = I;
            z80Registers.R = R;
            z80Registers.IX = IX;
            z80Registers.IY = IY;
            z80Registers.InterruptFlipFlop1 = InterruptFlipFlop1;
            z80Registers.InterruptFlipFlop2 = InterruptFlipFlop2;
            z80Registers.InterruptMode = InterruptMode;
            z80Registers.ProgramCounter = ProgramCounter;
            z80Registers.StackPointer = StackPointer;

            var state = z80Registers.GetRegisterState();

            Assert.AreEqual(I, state.I);
            Assert.AreEqual(R, state.R);
            Assert.AreEqual(IX, state.IX);
            Assert.AreEqual(IY, state.IY);
            Assert.AreEqual(InterruptFlipFlop1, state.InterruptFlipFlop1);
            Assert.AreEqual(InterruptFlipFlop2, state.InterruptFlipFlop2);
            Assert.AreEqual(InterruptMode, state.InterruptMode);
            Assert.AreEqual(ProgramCounter, z80Registers.ProgramCounter);
            Assert.AreEqual(StackPointer, z80Registers.StackPointer);

            primaryGeneralPurposeRegisters.Verify(x => x.GetRegisterState(), Times.Once);
            alternativeGeneralPurposeRegisters.Verify(x => x.GetRegisterState(), Times.Once);

            primaryAccumulatorAndFlagsRegisters.Verify(x => x.GetRegisterState(), Times.Once);
            alternativeAccumulatorAndFlagsRegisters.Verify(x => x.GetRegisterState(), Times.Once);
        }

        [Test]
        public void ResetToStateTest()
        {
            var state = RegisterTestObjects.Z80RegisterState;

            z80Registers.ResetToState(state);

            Assert.AreEqual(state.I, z80Registers.I);
            Assert.AreEqual(state.R, z80Registers.R);
            Assert.AreEqual(state.IX, z80Registers.IX);
            Assert.AreEqual(state.IY, z80Registers.IY);
            Assert.AreEqual(state.InterruptFlipFlop1, z80Registers.InterruptFlipFlop1);
            Assert.AreEqual(state.InterruptFlipFlop2, z80Registers.InterruptFlipFlop2);
            Assert.AreEqual(state.InterruptMode, z80Registers.InterruptMode);
            Assert.AreEqual(state.ProgramCounter, z80Registers.ProgramCounter);
            Assert.AreEqual(state.StackPointer, z80Registers.StackPointer);

            primaryGeneralPurposeRegisters.Verify(x => x.ResetToState(state.PrimaryGeneralPurposeRegisterState));
            alternativeGeneralPurposeRegisters.Verify(x => x.ResetToState(state.AlternativeGeneralPurposeRegisterState));

            primaryAccumulatorAndFlagsRegisters.Verify(
                                                       x =>
                                                           x.ResetToState(state.PrimaryAccumulatorAndFlagsRegisterState));
            alternativeAccumulatorAndFlagsRegisters.Verify(
                                                           x =>
                                                               x.ResetToState(
                                                                              state
                                                                                  .AlternativeAccumulatorAndFlagsRegisterState));
        }

        [Test]
        public void SetIxRegisterTest()
        {
            z80Registers.IX = 0x1234;

            Assert.AreEqual(0x12, z80Registers.IXh);
            Assert.AreEqual(0x34, z80Registers.IXl);
            Assert.AreEqual(0x1234, z80Registers.IX);
        }

        [Test]
        public void SetIyRegisterTest()
        {
            z80Registers.IY = 0x1234;

            Assert.AreEqual(0x12, z80Registers.IYh);
            Assert.AreEqual(0x34, z80Registers.IYl);
            Assert.AreEqual(0x1234, z80Registers.IY);
        }

        [Test]
        public void SwitchToAlternativeAccumulatorAndFlagsRegistersTest()
        {
            z80Registers.Reset();

            Assert.AreSame(primaryAccumulatorAndFlagsRegisters.Object, z80Registers.AccumulatorAndFlagsRegisters);

            z80Registers.SwitchToAlternativeAccumulatorAndFlagsRegisters();

            Assert.AreSame(alternativeAccumulatorAndFlagsRegisters.Object, z80Registers.AccumulatorAndFlagsRegisters);
        }

        [Test]
        public void SwitchToAlternativeGeneralPurposeRegistersTest()
        {
            z80Registers.Reset();

            Assert.AreSame(primaryGeneralPurposeRegisters.Object, z80Registers.GeneralPurposeRegisters);

            z80Registers.SwitchToAlternativeGeneralPurposeRegisters();

            Assert.AreSame(alternativeGeneralPurposeRegisters.Object, z80Registers.GeneralPurposeRegisters);
        }
    }
}