using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Registers
{
    [TestFixture]
    public class AccumulatorAndFlagsRegisterSetTests
    {
        private IAccumulatorAndFlagsRegisterSet accumulatorAndFlagsRegisterSet;

        private Mock<IFlagsRegister> flagsRegister;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            flagsRegister = new Mock<IFlagsRegister>();

            accumulatorAndFlagsRegisterSet = new AccumulatorAndFlagsRegisterSet(flagsRegister.Object);
        }

        [Test]
        public void GetAfRegisterTest()
        {
            accumulatorAndFlagsRegisterSet.A = 0x12;
            flagsRegister.Setup(x => x.Register).Returns(0x34);

            Assert.AreEqual(0x1234, accumulatorAndFlagsRegisterSet.AF);
        }

        [Test]
        public void GetRegisterStateTest()
        {
            const byte A = 0xaa, F = 0xff;

            accumulatorAndFlagsRegisterSet.A = A;
            flagsRegister.Setup(x => x.Register).Returns(F);

            var state = accumulatorAndFlagsRegisterSet.GetRegisterState();

            Assert.AreEqual(A, state.A);
            Assert.AreEqual(F, state.F);
        }


        [Test]
        public void ResetToStateTest()
        {
            var state = RegisterTestObjects.AccumulatorAndFlagsRegisterState;

            accumulatorAndFlagsRegisterSet.ResetToState(state);

            Assert.AreEqual(state.A, accumulatorAndFlagsRegisterSet.A);

            flagsRegister.VerifySet(x => x.Register = It.Is<byte>(y => y == state.F));
        }


        [Test]
        public void SetAfRegisterTest()
        {
            accumulatorAndFlagsRegisterSet.AF = 0x1234;
            Assert.AreEqual(0x12, accumulatorAndFlagsRegisterSet.A);
            flagsRegister.VerifySet(x => x.Register = It.Is<byte>(y => y == 0x34));
        }
    }
}