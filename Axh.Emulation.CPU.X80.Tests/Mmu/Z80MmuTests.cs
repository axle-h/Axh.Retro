namespace Axh.Emulation.CPU.X80.Tests.Mmu
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Emulation.CPU.X80.Contracts;
    using Axh.Emulation.CPU.X80.Contracts.Memory;
    using Axh.Emulation.CPU.X80.Z80;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80MmuTests
    {
        private const ushort Address0 = 0x0000;
        private const ushort Address1 = 0x8000;
        private const ushort Address2 = 0xf000;

        private const ushort Length0 = Address1 - Address0;
        private const ushort Length1 = Address2 - Address1;
        private const ushort Length2 = 0xffff - Address2;
        
        private Mock<IReadableAddressSegment> segment0R;
        private Mock<IWriteableAddressSegment> segment0W;
        private Mock<IReadableWriteableAddressSegment> segment1;
        private Mock<IReadableWriteableAddressSegment> segment2;

        private readonly Random random = new Random(1234);

        private IMmu mmu;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.segment0R = new Mock<IReadableAddressSegment>();
            this.segment0W = new Mock<IWriteableAddressSegment>();
            this.segment1 = new Mock<IReadableWriteableAddressSegment>();
            this.segment2 = new Mock<IReadableWriteableAddressSegment>();

            this.segment0R.Setup(x => x.Address).Returns(Address0);
            this.segment0R.Setup(x => x.Length).Returns(Length0);
            this.segment0W.Setup(x => x.Address).Returns(Address0);
            this.segment0W.Setup(x => x.Length).Returns(Length0);

            this.segment1.Setup(x => x.Address).Returns(Address1);
            this.segment1.Setup(x => x.Length).Returns(Length1);

            this.segment2.Setup(x => x.Address).Returns(Address2);
            this.segment2.Setup(x => x.Length).Returns(Length2);

            this.mmu = new Z80Mmu(new IAddressSegment[] { this.segment0R.Object, this.segment0W.Object, this.segment1.Object, this.segment2.Object });
        }
        
        [Test]
        public void ReadFromSegment0()
        {
            VerifyRead(Address0, Length0, segment0R, segment1, segment2);
        }

        [Test]
        public void WriteToSegment0()
        {
            VerifyWrite(Address0, Length0, segment0W, segment1, segment2);
        }

        [Test]
        public void ReadFromSegment1()
        {
            VerifyRead(Address1, Length1, segment1, segment0R, segment2);
        }

        [Test]
        public void WriteToSegment1()
        {
            VerifyWrite(Address1, Length1, segment1, segment0W, segment2);
        }

        [Test]
        public void ReadFromSegment2()
        {
            VerifyRead(Address2, Length2, segment2, segment0R, segment1);
        }

        [Test]
        public void WriteToSegment2()
        {
            VerifyWrite(Address2, Length2, segment2, segment0W, segment1);
        }

        private void VerifyRead<TAddressSegment, TAddressSegment1, TAddressSegment2>(
            ushort segmentAddress,
            ushort length,
            Mock<TAddressSegment> segment,
            Mock<TAddressSegment1> otherSegment1,
            Mock<TAddressSegment2> otherSegment2)
            where TAddressSegment : class, IReadableAddressSegment
            where TAddressSegment1 : class, IReadableAddressSegment
            where TAddressSegment2 : class, IReadableAddressSegment
        {
            segment.ResetCalls();
            otherSegment1.ResetCalls();
            otherSegment2.ResetCalls();

            foreach (var percentile in GetAddressRangePercentiles(segmentAddress, length))
            {
                this.mmu.ReadByte(percentile);
                var address = (ushort)(percentile - segmentAddress);
                segment.Verify(x => x.ReadByte(address), Times.Once);
            }

            otherSegment1.Verify(x => x.ReadByte(It.IsAny<ushort>()), Times.Never);
            otherSegment2.Verify(x => x.ReadByte(It.IsAny<ushort>()), Times.Never);
        }

        private void VerifyWrite<TAddressSegment, TAddressSegment1, TAddressSegment2>(
            ushort segmentAddress,
            ushort length,
            Mock<TAddressSegment> segment,
            Mock<TAddressSegment1> otherSegment1,
            Mock<TAddressSegment2> otherSegment2)
            where TAddressSegment : class, IWriteableAddressSegment
            where TAddressSegment1 : class, IWriteableAddressSegment
            where TAddressSegment2 : class, IWriteableAddressSegment
        {
            segment.ResetCalls();
            otherSegment1.ResetCalls();
            otherSegment2.ResetCalls();

            foreach (var percentile in GetAddressRangePercentiles(segmentAddress, length))
            {
                var value = (byte)this.random.Next(0xff);
                this.mmu.WriteByte(percentile, value);
                var address = (ushort)(percentile - segmentAddress);
                segment.Verify(x => x.WriteByte(address, value), Times.Once);
            }

            otherSegment1.Verify(x => x.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
            otherSegment2.Verify(x => x.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        private static IEnumerable<ushort> GetAddressRangePercentiles(ushort baseAddress, ushort length)
        {
            return Enumerable.Range(0, 100).Select(x => (ushort)Math.Round(baseAddress + length * x / 100.0));
        }
    }
}
