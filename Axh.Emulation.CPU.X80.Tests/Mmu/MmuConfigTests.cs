namespace Axh.Emulation.CPU.X80.Tests.Mmu
{
    using Axh.Emulation.CPU.X80.Contracts.Exceptions;
    using Axh.Emulation.CPU.X80.Contracts.Memory;
    using Axh.Emulation.CPU.X80.Mmu;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class MmuConfigTests
    {
        private Mock<IReadableAddressSegment> segment0R;
        private Mock<IWriteableAddressSegment> segment0W;
        private Mock<IReadableWriteableAddressSegment> segment1;
        private Mock<IReadableWriteableAddressSegment> segment2;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.segment0R = new Mock<IReadableAddressSegment>();
            this.segment0W = new Mock<IWriteableAddressSegment>();
            this.segment1 = new Mock<IReadableWriteableAddressSegment>();
            this.segment2 = new Mock<IReadableWriteableAddressSegment>();
        }

        private IMmu GetMmu(ushort address0, ushort length0, ushort address1, ushort length1, ushort address2, ushort length2)
        {
            this.segment0R.Setup(x => x.Address).Returns(address0);
            this.segment0R.Setup(x => x.Length).Returns(length0);
            this.segment0W.Setup(x => x.Address).Returns(address0);
            this.segment0W.Setup(x => x.Length).Returns(length0);

            this.segment1.Setup(x => x.Address).Returns(address1);
            this.segment1.Setup(x => x.Length).Returns(length1);

            this.segment2.Setup(x => x.Address).Returns(address2);
            this.segment2.Setup(x => x.Length).Returns(length2);

            return new SegmentMmu(new IAddressSegment[] { this.segment0R.Object, this.segment0W.Object, this.segment1.Object, this.segment2.Object });
        }

        [Test]
        [ExpectedException(typeof(PlatformConfigurationException))]
        public void SegmentWith0LengthThrowsConfigurationException()
        {
            this.GetMmu(0x0000, 0x1000, 0x1000, 0x0000, 0x1000, 0xefff);
        }

        [Test]
        public void NotFillingFullAddressSpaceThrowsMmuAddressSegmentGapException()
        {
            var exception = Assert.Throws<MmuAddressSegmentGapException>(() => this.GetMmu(0x0000, 0x1000, 0x1000, 0x1000, 0x2000, 0xd000));
            Assert.AreEqual(0xf000, exception.AddressFrom);
            Assert.AreEqual(0xffff, exception.AddressTo);
        }

        [Test]
        public void LeavingGapInAddressSpaceThrowsMmuAddressSegmentGapException()
        {
            var exception = Assert.Throws<MmuAddressSegmentGapException>(() => this.GetMmu(0x0000, 0x1000, 0x2000, 0x1000, 0x3000, 0xcfff));
            Assert.AreEqual(0x1000, exception.AddressFrom);
            Assert.AreEqual(0x2000, exception.AddressTo);
        }

        [Test]
        public void NotStartingAddressSpaceAt0ThrowsMmuAddressSegmentGapException()
        {
            var exception = Assert.Throws<MmuAddressSegmentGapException>(() => this.GetMmu(0x1000, 0x1000, 0x2000, 0x1000, 0x3000, 0xcfff));
            Assert.AreEqual(0x0000, exception.AddressFrom);
            Assert.AreEqual(0x1000, exception.AddressTo);
        }

        [Test]
        public void OverlappingSegmentsThrowsMmuAddressSegmentOverlapException()
        {
            var exception = Assert.Throws<MmuAddressSegmentOverlapException>(() => this.GetMmu(0x0000, 0x2000, 0x1000, 0x2000, 0x3000, 0xcfff));
            Assert.AreEqual(0x1000, exception.AddressFrom);
            Assert.AreEqual(0x2000, exception.AddressTo);
        }
    }
}
