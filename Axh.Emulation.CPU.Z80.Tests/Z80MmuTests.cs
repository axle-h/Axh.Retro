namespace Axh.Emulation.CPU.Z80.Tests
{
    using System.Configuration;
    using Axh.Emulation.CPU.Z80.Contracts;
    using Axh.Emulation.CPU.Z80.Contracts.Config;
    using Axh.Emulation.CPU.Z80.Contracts.Exceptions;
    using Axh.Emulation.CPU.Z80.Contracts.Memory;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class Z80MmuTests
    {
        private Mock<IZ80PlatformConfig> config;

        private Mock<IAddressSegment> segment0;
        private Mock<IAddressSegment> segment1;
        private Mock<IAddressSegment> segment2;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            config = new Mock<IZ80PlatformConfig>();
            segment0 = new Mock<IAddressSegment>();
            segment1 = new Mock<IAddressSegment>();
            segment2 = new Mock<IAddressSegment>();
        }

        private IMmu GetMmu(ushort address0, ushort length0, ushort address1, ushort length1, ushort address2, ushort length2)
        {
            segment0.Setup(x => x.Address).Returns(address0);
            segment0.Setup(x => x.Length).Returns(length0);

            segment1.Setup(x => x.Address).Returns(address1);
            segment1.Setup(x => x.Length).Returns(length1);

            segment2.Setup(x => x.Address).Returns(address2);
            segment2.Setup(x => x.Length).Returns(length2);

            return new Z80Mmu(config.Object, new[] { segment0.Object, segment1.Object, segment2.Object });
        }


        [Test]
        [ExpectedException(typeof(Z80ConfigurationException))]
        public void SegmentWith0LengthThrowsZ80ConfigurationException()
        {
            GetMmu(0x0000, 0x1000, 0x1000, 0x0000, 0x1000, 0xefff);
        }

        [Test]
        public void NotFillingFullAddressSpaceThrowsMmuAddressSegmentGapException()
        {
            var exception = Assert.Throws<MmuAddressSegmentGapException>(() => GetMmu(0x0000, 0x1000, 0x1000, 0x1000, 0x2000, 0xd000));
            Assert.AreEqual(0xf000, exception.AddressFrom);
            Assert.AreEqual(0xffff, exception.AddressTo);
        }


    }
}
