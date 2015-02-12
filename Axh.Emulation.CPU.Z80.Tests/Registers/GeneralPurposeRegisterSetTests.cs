namespace Axh.Emulation.CPU.Z80.Tests.Registers
{
    using Axh.Emulation.CPU.Z80.Contracts.Registers;
    using Axh.Emulation.CPU.Z80.Registers;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class GeneralPurposeRegisterSetTests
    {
        private IGeneralPurposeRegisterSet generalPurposeRegisterSet;

        private Mock<IFlagsRegister> flagsRegister;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.flagsRegister = new Mock<IFlagsRegister>();

            this.generalPurposeRegisterSet = new GeneralPurposeRegisterSet(flagsRegister.Object);
        }

        [Test]
        public void SetAfRegisterTest()
        {
            this.generalPurposeRegisterSet.AF = 0x1234;
            Assert.AreEqual(0x12, this.generalPurposeRegisterSet.A);
            this.flagsRegister.VerifySet(x => x.Register = It.Is<byte>(y => y == 0x34));
        }

        [Test]
        public void SetBcRegisterTest()
        {
            this.generalPurposeRegisterSet.BC = 0x1234;

            Assert.AreEqual(0x12, this.generalPurposeRegisterSet.B);
            Assert.AreEqual(0x34, this.generalPurposeRegisterSet.C);
        }

        [Test]
        public void SetDeRegisterTest()
        {
            this.generalPurposeRegisterSet.DE = 0x1234;

            Assert.AreEqual(0x12, this.generalPurposeRegisterSet.D);
            Assert.AreEqual(0x34, this.generalPurposeRegisterSet.E);
        }

        [Test]
        public void SetHlRegisterTest()
        {
            this.generalPurposeRegisterSet.HL = 0x1234;

            Assert.AreEqual(0x12, this.generalPurposeRegisterSet.H);
            Assert.AreEqual(0x34, this.generalPurposeRegisterSet.L);
        }

        [Test]
        public void GetAfRegisterTest()
        {
            this.generalPurposeRegisterSet.A = 0x12;
            this.flagsRegister.Setup(x => x.Register).Returns(0x34);

            Assert.AreEqual(0x1234, this.generalPurposeRegisterSet.AF);
        }

        [Test]
        public void GetBcRegisterTest()
        {
            this.generalPurposeRegisterSet.B = 0x12;
            this.generalPurposeRegisterSet.C = 0x34;

            Assert.AreEqual(0x1234, this.generalPurposeRegisterSet.BC);
        }

        [Test]
        public void GetDeRegisterTest()
        {
            this.generalPurposeRegisterSet.D = 0x12;
            this.generalPurposeRegisterSet.E = 0x34;

            Assert.AreEqual(0x1234, this.generalPurposeRegisterSet.DE);
        }

        [Test]
        public void GetHlRegisterTest()
        {
            this.generalPurposeRegisterSet.H = 0x12;
            this.generalPurposeRegisterSet.L = 0x34;

            Assert.AreEqual(0x1234, this.generalPurposeRegisterSet.HL);
        }

        [Test]
        public void ResetToStateTest()
        {
            var state = RegisterTestObjects.Z80GeneralPurposeRegisterState;

            this.generalPurposeRegisterSet.ResetToState(state);

            Assert.AreEqual(state.A, this.generalPurposeRegisterSet.A);
            Assert.AreEqual(state.B, this.generalPurposeRegisterSet.B);
            Assert.AreEqual(state.C, this.generalPurposeRegisterSet.C);
            Assert.AreEqual(state.D, this.generalPurposeRegisterSet.D);
            Assert.AreEqual(state.E, this.generalPurposeRegisterSet.E);
            Assert.AreEqual(state.H, this.generalPurposeRegisterSet.H);
            Assert.AreEqual(state.L, this.generalPurposeRegisterSet.L);

            this.flagsRegister.VerifySet(x => x.Register = It.Is<byte>(y => y == state.F));
        }

    }
}
