namespace Axh.Retro.CPU.Z80.Tests.Registers
{
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Registers;

    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class GeneralPurposeRegisterSetTests
    {
        private IGeneralPurposeRegisterSet generalPurposeRegisterSet;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.generalPurposeRegisterSet = new GeneralPurposeRegisterSet();
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
            var state = RegisterTestObjects.GeneralPurposeRegisterState;

            this.generalPurposeRegisterSet.ResetToState(state);
            
            Assert.AreEqual(state.B, this.generalPurposeRegisterSet.B);
            Assert.AreEqual(state.C, this.generalPurposeRegisterSet.C);
            Assert.AreEqual(state.D, this.generalPurposeRegisterSet.D);
            Assert.AreEqual(state.E, this.generalPurposeRegisterSet.E);
            Assert.AreEqual(state.H, this.generalPurposeRegisterSet.H);
            Assert.AreEqual(state.L, this.generalPurposeRegisterSet.L);
        }

        [Test]
        public void GetRegisterStateTest()
        {
            const byte B = 0xbb, C = 0xcc, D = 0xdd, E = 0xee, H = 0x11, L = 0x22;
            
            this.generalPurposeRegisterSet.B = B;
            this.generalPurposeRegisterSet.C = C;
            this.generalPurposeRegisterSet.D = D;
            this.generalPurposeRegisterSet.E = E;
            this.generalPurposeRegisterSet.H = H;
            this.generalPurposeRegisterSet.L = L;

            var state = this.generalPurposeRegisterSet.GetRegisterState();
            
            Assert.AreEqual(B, state.B);
            Assert.AreEqual(C, state.C);
            Assert.AreEqual(D, state.D);
            Assert.AreEqual(E, state.E);
            Assert.AreEqual(H, state.H);
            Assert.AreEqual(L, state.L);

        }


    }
}
