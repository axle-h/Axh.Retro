namespace Axh.Retro.CPU.X80.Tests.Registers
{
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Registers;

    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class AccumulatorAndFlagsRegisterSetTests
    {
        private IAccumulatorAndFlagsRegisterSet accumulatorAndFlagsRegisterSet;

        private Mock<IFlagsRegister> flagsRegister;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.flagsRegister = new Mock<IFlagsRegister>();

            this.accumulatorAndFlagsRegisterSet = new AccumulatorAndFlagsRegisterSet(flagsRegister.Object);
        }


        [Test]
        public void SetAfRegisterTest()
        {
            this.accumulatorAndFlagsRegisterSet.AF = 0x1234;
            Assert.AreEqual(0x12, this.accumulatorAndFlagsRegisterSet.A);
            this.flagsRegister.VerifySet(x => x.Register = It.Is<byte>(y => y == 0x34));
        }
        
        [Test]
        public void GetAfRegisterTest()
        {
            this.accumulatorAndFlagsRegisterSet.A = 0x12;
            this.flagsRegister.Setup(x => x.Register).Returns(0x34);

            Assert.AreEqual(0x1234, this.accumulatorAndFlagsRegisterSet.AF);
        }


        [Test]
        public void ResetToStateTest()
        {
            var state = RegisterTestObjects.AccumulatorAndFlagsRegisterState;

            this.accumulatorAndFlagsRegisterSet.ResetToState(state);

            Assert.AreEqual(state.A, this.accumulatorAndFlagsRegisterSet.A);

            this.flagsRegister.VerifySet(x => x.Register = It.Is<byte>(y => y == state.F));
        }

        [Test]
        public void GetRegisterStateTest()
        {
            const byte A = 0xaa, F = 0xff;

            this.accumulatorAndFlagsRegisterSet.A = A;
            this.flagsRegister.Setup(x => x.Register).Returns(F);

            var state = this.accumulatorAndFlagsRegisterSet.GetRegisterState();

            Assert.AreEqual(A, state.A);
            Assert.AreEqual(F, state.F);
        }
    }
}
