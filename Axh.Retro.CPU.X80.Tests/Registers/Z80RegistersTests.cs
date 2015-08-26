namespace Axh.Retro.CPU.X80.Tests.Registers
{
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Registers;

    using Moq;
    using NUnit.Framework;

    using IGeneralPurposeRegisterSet = Contracts.Registers.IGeneralPurposeRegisterSet;
    using IZ80Registers = Contracts.Registers.IZ80Registers;

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

            this.z80Registers = new Z80Registers(
                primaryGeneralPurposeRegisters.Object,
                alternativeGeneralPurposeRegisters.Object,
                primaryAccumulatorAndFlagsRegisters.Object,
                alternativeAccumulatorAndFlagsRegisters.Object);
        }

        [Test]
        public void SwitchToAlternativeGeneralPurposeRegistersTest()
        {
            this.z80Registers.Reset();

            Assert.AreSame(this.primaryGeneralPurposeRegisters.Object, z80Registers.GeneralPurposeRegisters);

            this.z80Registers.SwitchToAlternativeGeneralPurposeRegisters();

            Assert.AreSame(this.alternativeGeneralPurposeRegisters.Object, z80Registers.GeneralPurposeRegisters);
        }

        [Test]
        public void SwitchToAlternativeAccumulatorAndFlagsRegistersTest()
        {
            this.z80Registers.Reset();

            Assert.AreSame(this.primaryAccumulatorAndFlagsRegisters.Object, z80Registers.AccumulatorAndFlagsRegisters);

            this.z80Registers.SwitchToAlternativeAccumulatorAndFlagsRegisters();

            Assert.AreSame(this.alternativeAccumulatorAndFlagsRegisters.Object, z80Registers.AccumulatorAndFlagsRegisters);
        }

        [Test]
        public void ResetToStateTest()
        {
            var state = RegisterTestObjects.Z80RegisterState;

            this.z80Registers.ResetToState(state);

            Assert.AreEqual(state.I, z80Registers.I);
            Assert.AreEqual(state.R, z80Registers.R);
            Assert.AreEqual(state.IX, z80Registers.IX);
            Assert.AreEqual(state.IY, z80Registers.IY);
            Assert.AreEqual(state.InterruptFlipFlop1, z80Registers.InterruptFlipFlop1);
            Assert.AreEqual(state.InterruptFlipFlop2, z80Registers.InterruptFlipFlop2);
            Assert.AreEqual(state.InterruptMode, z80Registers.InterruptMode);
            Assert.AreEqual(state.ProgramCounter, z80Registers.ProgramCounter);
            Assert.AreEqual(state.StackPointer, z80Registers.StackPointer);

            this.primaryGeneralPurposeRegisters.Verify(x => x.ResetToState(state.PrimaryGeneralPurposeRegisterState));
            this.alternativeGeneralPurposeRegisters.Verify(x => x.ResetToState(state.AlternativeGeneralPurposeRegisterState));

            this.primaryAccumulatorAndFlagsRegisters.Verify(x => x.ResetToState(state.PrimaryAccumulatorAndFlagsRegisterState));
            this.alternativeAccumulatorAndFlagsRegisters.Verify(x => x.ResetToState(state.AlternativeAccumulatorAndFlagsRegisterState));
        }

        [Test]
        public void GetRegisterStateTest()
        {
            const byte I = 0x11, R = 0x22, IX = 0x33, IY = 0x44, ProgramCounter = 0x55, StackPointer = 0x66;
            const bool InterruptFlipFlop1 = true, InterruptFlipFlop2 = false;
            const InterruptMode InterruptMode = InterruptMode.InterruptMode1;

            this.z80Registers.I = I;
            this.z80Registers.R = R;
            this.z80Registers.IX = IX;
            this.z80Registers.IY = IY;
            this.z80Registers.InterruptFlipFlop1 = InterruptFlipFlop1;
            this.z80Registers.InterruptFlipFlop2 = InterruptFlipFlop2;
            this.z80Registers.InterruptMode = InterruptMode;
            this.z80Registers.ProgramCounter = ProgramCounter;
            this.z80Registers.StackPointer = StackPointer;

            var state = this.z80Registers.GetRegisterState();

            Assert.AreEqual(I, state.I);
            Assert.AreEqual(R, state.R);
            Assert.AreEqual(IX, state.IX);
            Assert.AreEqual(IY, state.IY);
            Assert.AreEqual(InterruptFlipFlop1, state.InterruptFlipFlop1);
            Assert.AreEqual(InterruptFlipFlop2, state.InterruptFlipFlop2);
            Assert.AreEqual(InterruptMode, state.InterruptMode);
            Assert.AreEqual(ProgramCounter, z80Registers.ProgramCounter);
            Assert.AreEqual(StackPointer, z80Registers.StackPointer);

            this.primaryGeneralPurposeRegisters.Verify(x => x.GetRegisterState(), Times.Once);
            this.alternativeGeneralPurposeRegisters.Verify(x => x.GetRegisterState(), Times.Once);

            this.primaryAccumulatorAndFlagsRegisters.Verify(x => x.GetRegisterState(), Times.Once);
            this.alternativeAccumulatorAndFlagsRegisters.Verify(x => x.GetRegisterState(), Times.Once);

        }
    }
}
