using Axh.Retro.CPU.Common.Contracts.Exceptions;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Common.Memory;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Common.Tests.Mmu
{
    [TestFixture]
    public class MmuConfigTests
    {
        private Mock<IReadableAddressSegment> _segment0R;
        private Mock<IWriteableAddressSegment> _segment0W;
        private Mock<IReadableWriteableAddressSegment> _segment1;
        private Mock<IReadableWriteableAddressSegment> _segment2;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _segment0R = new Mock<IReadableAddressSegment>();
            _segment0W = new Mock<IWriteableAddressSegment>();
            _segment1 = new Mock<IReadableWriteableAddressSegment>();
            _segment2 = new Mock<IReadableWriteableAddressSegment>();
        }

        private IMmu GetMmu(ushort address0, ushort length0, ushort address1, ushort length1, ushort address2, ushort length2)
        {
            _segment0R.Setup(x => x.Address).Returns(address0);
            _segment0R.Setup(x => x.Length).Returns(length0);
            _segment0W.Setup(x => x.Address).Returns(address0);
            _segment0W.Setup(x => x.Length).Returns(length0);

            _segment1.Setup(x => x.Address).Returns(address1);
            _segment1.Setup(x => x.Length).Returns(length1);

            _segment2.Setup(x => x.Address).Returns(address2);
            _segment2.Setup(x => x.Length).Returns(length2);

            var dmaController = new Mock<IDmaController>();
            var instructionTimer = new Mock<IInstructionTimer>();

            return
                new SegmentMmu(
                    new IAddressSegment[] { _segment0R.Object, _segment0W.Object, _segment1.Object, _segment2.Object },
                    dmaController.Object,
                    instructionTimer.Object);
        }

        [Test]
        public void LeavingGapInAddressSpaceThrowsMmuAddressSegmentGapException()
        {
            var exception =
                Assert.Throws<MmuAddressSegmentGapException>(() => GetMmu(0x0000, 0x1000, 0x2000, 0x1000, 0x3000, 0xcfff));
            Assert.AreEqual(0x1000, exception.AddressFrom);
            Assert.AreEqual(0x2000, exception.AddressTo);
        }

        [Test]
        public void NotFillingFullAddressSpaceThrowsMmuAddressSegmentGapException()
        {
            var exception =
                Assert.Throws<MmuAddressSegmentGapException>(() => GetMmu(0x0000, 0x1000, 0x1000, 0x1000, 0x2000, 0xd000));
            Assert.AreEqual(0xf000, exception.AddressFrom);
            Assert.AreEqual(0xffff, exception.AddressTo);
        }

        [Test]
        public void NotStartingAddressSpaceAt0ThrowsMmuAddressSegmentGapException()
        {
            var exception =
                Assert.Throws<MmuAddressSegmentGapException>(() => GetMmu(0x1000, 0x1000, 0x2000, 0x1000, 0x3000, 0xcfff));
            Assert.AreEqual(0x0000, exception.AddressFrom);
            Assert.AreEqual(0x1000, exception.AddressTo);
        }

        [Test]
        public void OverlappingSegmentsThrowsMmuAddressSegmentOverlapException()
        {
            var exception =
                Assert.Throws<MmuAddressSegmentOverlapException>(() => GetMmu(0x0000, 0x2000, 0x1000, 0x2000, 0x3000, 0xcfff));
            Assert.AreEqual(0x1000, exception.AddressFrom);
            Assert.AreEqual(0x2000, exception.AddressTo);
        }

        [Test]
        [ExpectedException(typeof (PlatformConfigurationException))]
        public void SegmentWith0LengthThrowsConfigurationException()
        {
            GetMmu(0x0000, 0x1000, 0x1000, 0x0000, 0x1000, 0xefff);
        }
    }
}