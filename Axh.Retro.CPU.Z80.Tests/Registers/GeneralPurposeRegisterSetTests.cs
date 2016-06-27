using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Registers;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Registers
{
    [TestFixture]
    public class GeneralPurposeRegisterSetTests
    {
        private IGeneralPurposeRegisterSet generalPurposeRegisterSet;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            generalPurposeRegisterSet = new GeneralPurposeRegisterSet();
        }

        [Test]
        public void GetBcRegisterTest()
        {
            generalPurposeRegisterSet.B = 0x12;
            generalPurposeRegisterSet.C = 0x34;

            Assert.AreEqual(0x1234, generalPurposeRegisterSet.BC);
        }

        [Test]
        public void GetDeRegisterTest()
        {
            generalPurposeRegisterSet.D = 0x12;
            generalPurposeRegisterSet.E = 0x34;

            Assert.AreEqual(0x1234, generalPurposeRegisterSet.DE);
        }

        [Test]
        public void GetHlRegisterTest()
        {
            generalPurposeRegisterSet.H = 0x12;
            generalPurposeRegisterSet.L = 0x34;

            Assert.AreEqual(0x1234, generalPurposeRegisterSet.HL);
        }

        [Test]
        public void GetRegisterStateTest()
        {
            const byte B = 0xbb, C = 0xcc, D = 0xdd, E = 0xee, H = 0x11, L = 0x22;

            generalPurposeRegisterSet.B = B;
            generalPurposeRegisterSet.C = C;
            generalPurposeRegisterSet.D = D;
            generalPurposeRegisterSet.E = E;
            generalPurposeRegisterSet.H = H;
            generalPurposeRegisterSet.L = L;

            var state = generalPurposeRegisterSet.GetRegisterState();

            Assert.AreEqual(B, state.B);
            Assert.AreEqual(C, state.C);
            Assert.AreEqual(D, state.D);
            Assert.AreEqual(E, state.E);
            Assert.AreEqual(H, state.H);
            Assert.AreEqual(L, state.L);
        }

        [Test]
        public void ResetToStateTest()
        {
            var state = RegisterTestObjects.GeneralPurposeRegisterState;

            generalPurposeRegisterSet.ResetToState(state);

            Assert.AreEqual(state.B, generalPurposeRegisterSet.B);
            Assert.AreEqual(state.C, generalPurposeRegisterSet.C);
            Assert.AreEqual(state.D, generalPurposeRegisterSet.D);
            Assert.AreEqual(state.E, generalPurposeRegisterSet.E);
            Assert.AreEqual(state.H, generalPurposeRegisterSet.H);
            Assert.AreEqual(state.L, generalPurposeRegisterSet.L);
        }

        [Test]
        public void SetBcRegisterTest()
        {
            generalPurposeRegisterSet.BC = 0x1234;

            Assert.AreEqual(0x12, generalPurposeRegisterSet.B);
            Assert.AreEqual(0x34, generalPurposeRegisterSet.C);
        }

        [Test]
        public void SetDeRegisterTest()
        {
            generalPurposeRegisterSet.DE = 0x1234;

            Assert.AreEqual(0x12, generalPurposeRegisterSet.D);
            Assert.AreEqual(0x34, generalPurposeRegisterSet.E);
        }

        [Test]
        public void SetHlRegisterTest()
        {
            generalPurposeRegisterSet.HL = 0x1234;

            Assert.AreEqual(0x12, generalPurposeRegisterSet.H);
            Assert.AreEqual(0x34, generalPurposeRegisterSet.L);
        }
    }
}