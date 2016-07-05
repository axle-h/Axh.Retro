using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Registers
{
    [TestFixture]
    public class AccumulatorAndFlagsRegisterSetTests
    {
        private IAccumulatorAndFlagsRegisterSet _accumulatorAndFlagsRegisterSet;

        private Mock<IFlagsRegister> _flagsRegister;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _flagsRegister = new Mock<IFlagsRegister>();

            _accumulatorAndFlagsRegisterSet = new AccumulatorAndFlagsRegisterSet(_flagsRegister.Object);
        }

        [Test]
        public void GetAfRegisterTest()
        {
            _accumulatorAndFlagsRegisterSet.A = 0x12;
            _flagsRegister.Setup(x => x.Register).Returns(0x34);

            Assert.AreEqual(0x1234, _accumulatorAndFlagsRegisterSet.AF);
        }

        [Test]
        public void GetRegisterStateTest()
        {
            const byte A = 0xaa, F = 0xff;

            _accumulatorAndFlagsRegisterSet.A = A;
            _flagsRegister.Setup(x => x.Register).Returns(F);

            var state = _accumulatorAndFlagsRegisterSet.GetRegisterState();

            Assert.AreEqual(A, state.A);
            Assert.AreEqual(F, state.F);
        }


        [Test]
        public void ResetToStateTest()
        {
            var state = RegisterTestObjects.AccumulatorAndFlagsRegisterState;

            _accumulatorAndFlagsRegisterSet.ResetToState(state);

            Assert.AreEqual(state.A, _accumulatorAndFlagsRegisterSet.A);

            _flagsRegister.VerifySet(x => x.Register = It.Is<byte>(y => y == state.F));
        }


        [Test]
        public void SetAfRegisterTest()
        {
            _accumulatorAndFlagsRegisterSet.AF = 0x1234;
            Assert.AreEqual(0x12, _accumulatorAndFlagsRegisterSet.A);
            _flagsRegister.VerifySet(x => x.Register = It.Is<byte>(y => y == 0x34));
        }
    }
}