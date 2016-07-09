using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Registers;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Registers
{
    [TestFixture]
    public class GeneralPurposeRegisterSetTests
    {
        private GeneralPurposeRegisterSet _generalPurposeRegisterSet;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _generalPurposeRegisterSet = new GeneralPurposeRegisterSet();
        }

        [Test]
        public void GetBcRegisterTest()
        {
            _generalPurposeRegisterSet.B = 0x12;
            _generalPurposeRegisterSet.C = 0x34;

            Assert.AreEqual(0x1234, _generalPurposeRegisterSet.BC);
        }

        [Test]
        public void GetDeRegisterTest()
        {
            _generalPurposeRegisterSet.D = 0x12;
            _generalPurposeRegisterSet.E = 0x34;

            Assert.AreEqual(0x1234, _generalPurposeRegisterSet.DE);
        }

        [Test]
        public void GetHlRegisterTest()
        {
            _generalPurposeRegisterSet.H = 0x12;
            _generalPurposeRegisterSet.L = 0x34;

            Assert.AreEqual(0x1234, _generalPurposeRegisterSet.HL);
        }

        [Test]
        public void GetRegisterStateTest()
        {
            const byte B = 0xbb, C = 0xcc, D = 0xdd, E = 0xee, H = 0x11, L = 0x22;

            _generalPurposeRegisterSet.B = B;
            _generalPurposeRegisterSet.C = C;
            _generalPurposeRegisterSet.D = D;
            _generalPurposeRegisterSet.E = E;
            _generalPurposeRegisterSet.H = H;
            _generalPurposeRegisterSet.L = L;

            var state = _generalPurposeRegisterSet.GetRegisterState();

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

            _generalPurposeRegisterSet.ResetToState(state);

            Assert.AreEqual(state.B, _generalPurposeRegisterSet.B);
            Assert.AreEqual(state.C, _generalPurposeRegisterSet.C);
            Assert.AreEqual(state.D, _generalPurposeRegisterSet.D);
            Assert.AreEqual(state.E, _generalPurposeRegisterSet.E);
            Assert.AreEqual(state.H, _generalPurposeRegisterSet.H);
            Assert.AreEqual(state.L, _generalPurposeRegisterSet.L);
        }

        [Test]
        public void SetBcRegisterTest()
        {
            _generalPurposeRegisterSet.BC = 0x1234;

            Assert.AreEqual(0x12, _generalPurposeRegisterSet.B);
            Assert.AreEqual(0x34, _generalPurposeRegisterSet.C);
        }

        [Test]
        public void SetDeRegisterTest()
        {
            _generalPurposeRegisterSet.DE = 0x1234;

            Assert.AreEqual(0x12, _generalPurposeRegisterSet.D);
            Assert.AreEqual(0x34, _generalPurposeRegisterSet.E);
        }

        [Test]
        public void SetHlRegisterTest()
        {
            _generalPurposeRegisterSet.HL = 0x1234;

            Assert.AreEqual(0x12, _generalPurposeRegisterSet.H);
            Assert.AreEqual(0x34, _generalPurposeRegisterSet.L);
        }
    }
}