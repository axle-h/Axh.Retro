using System;
using System.Collections.Generic;
using System.Linq;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Common.Memory;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Common.Tests.Mmu
{
    [TestFixture]
    public class SegmentMmuTests
    {
        private const ushort Address0 = 0x0000;
        private const ushort Address1 = 0x8000;
        private const ushort Address2 = 0xf000;

        private const ushort Length0 = Address1 - Address0;
        private const ushort Length1 = Address2 - Address1;
        private const ushort Length2 = 0x10000 - Address2;

        private const int MinByteLength = 24;
        private const int MaxByteLength = 32;

        private Mock<IReadableAddressSegment> _segment0R;
        private Mock<IWriteableAddressSegment> _segment0W;
        private Mock<IReadableWriteableAddressSegment> _segment1;
        private Mock<IReadableWriteableAddressSegment> _segment2;

        private readonly Random _random = new Random(1234);

        private IMmu _mmu;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _segment0R = new Mock<IReadableAddressSegment>();
            _segment0W = new Mock<IWriteableAddressSegment>();
            _segment1 = new Mock<IReadableWriteableAddressSegment>();
            _segment2 = new Mock<IReadableWriteableAddressSegment>();

            SetupSegmentForRead(_segment0R, Address0, Length0);

            SetupSegment(_segment0W, Address0, Length0);

            SetupSegmentForRead(_segment1, Address1, Length1);

            SetupSegmentForRead(_segment2, Address2, Length2);

            var dmaController = new Mock<IDmaController>();
            var instructionTimer = new Mock<IInstructionTimer>();

            _mmu =
                new SegmentMmu(
                    new IAddressSegment[] { _segment0R.Object, _segment0W.Object, _segment1.Object, _segment2.Object },
                    dmaController.Object,
                    instructionTimer.Object);
        }

        private static void SetupSegment<TAddressSegment>(Mock<TAddressSegment> segment, ushort address, ushort length)
            where TAddressSegment : class, IAddressSegment
        {
            segment.Setup(x => x.Address).Returns(address);
            segment.Setup(x => x.Length).Returns(length);
        }

        private static void SetupSegmentForRead<TReadableAddressSegment>(Mock<TReadableAddressSegment> segment,
            ushort address,
            ushort length) where TReadableAddressSegment : class, IReadableAddressSegment
        {
            SetupSegment(segment, address, length);

            // Need to defien for all lengths, not just Min - Max due to segment splitting
            for (var i = 1; i <= MaxByteLength; i++)
            {
                var arrayLength = i;
                var maxAddress = length - arrayLength;
                segment.Setup(x => x.ReadBytes(It.Is<ushort>(y => y <= maxAddress), arrayLength)).Returns(new byte[arrayLength]);
                segment.Setup(x => x.ReadBytes(It.Is<ushort>(y => y > maxAddress), arrayLength))
                       .Throws(new Exception("Tried reading over the array boundary"));
            }
        }

        private void VerifyReadByte<TAddressSegment, TAddressSegment1, TAddressSegment2>(ushort segmentAddress,
            ushort length,
            Mock<TAddressSegment> segment,
            Mock<TAddressSegment1> otherSegment1,
            Mock<TAddressSegment2> otherSegment2) where TAddressSegment : class, IReadableAddressSegment
            where TAddressSegment1 : class, IReadableAddressSegment where TAddressSegment2 : class, IReadableAddressSegment
        {
            segment.ResetCalls();
            otherSegment1.ResetCalls();
            otherSegment2.ResetCalls();

            foreach (var percentile in GetAddressRangePercentiles(segmentAddress, length))
            {
                _mmu.ReadByte(percentile);
                var address = (ushort) (percentile - segmentAddress);
                segment.Verify(x => x.ReadByte(address), Times.Once);
            }

            otherSegment1.Verify(x => x.ReadByte(It.IsAny<ushort>()), Times.Never);
            otherSegment2.Verify(x => x.ReadByte(It.IsAny<ushort>()), Times.Never);
        }

        private void VerifyWriteByte<TAddressSegment, TAddressSegment1, TAddressSegment2>(ushort segmentAddress,
            ushort length,
            Mock<TAddressSegment> segment,
            Mock<TAddressSegment1> otherSegment1,
            Mock<TAddressSegment2> otherSegment2) where TAddressSegment : class, IWriteableAddressSegment
            where TAddressSegment1 : class, IWriteableAddressSegment where TAddressSegment2 : class, IWriteableAddressSegment
        {
            segment.ResetCalls();
            otherSegment1.ResetCalls();
            otherSegment2.ResetCalls();

            foreach (var percentile in GetAddressRangePercentiles(segmentAddress, length))
            {
                var value = (byte) _random.Next(0xff);
                _mmu.WriteByte(percentile, value);
                var address = (ushort) (percentile - segmentAddress);
                segment.Verify(x => x.WriteByte(address, value), Times.Once);
            }

            otherSegment1.Verify(x => x.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
            otherSegment2.Verify(x => x.WriteByte(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Never);
        }

        private void VerifyReadWord<TAddressSegment, TAddressSegment1, TAddressSegment2>(ushort segmentAddress,
            ushort length,
            Mock<TAddressSegment> segment,
            Mock<TAddressSegment1> otherSegment1,
            Mock<TAddressSegment2> otherSegment2) where TAddressSegment : class, IReadableAddressSegment
            where TAddressSegment1 : class, IReadableAddressSegment where TAddressSegment2 : class, IReadableAddressSegment
        {
            segment.ResetCalls();
            otherSegment1.ResetCalls();
            otherSegment2.ResetCalls();

            var boundaryCheckComplete = false;

            foreach (var percentile in GetAddressRangePercentiles(segmentAddress, length))
            {
                _mmu.ReadWord(percentile);
                var address = (ushort) (percentile - segmentAddress);

                if (address < length - 1)
                {
                    segment.Verify(x => x.ReadWord(address), Times.Once);
                }
                else
                {
                    segment.Verify(x => x.ReadByte(address), Times.Once);
                    otherSegment1.Verify(x => x.ReadByte(0), Times.Once);
                    boundaryCheckComplete = true;
                }
            }

            Assert.IsTrue(boundaryCheckComplete, "Boundary check was not performed");
            otherSegment1.Verify(x => x.ReadWord(It.IsAny<ushort>()), Times.Never);
            otherSegment2.Verify(x => x.ReadWord(It.IsAny<ushort>()), Times.Never);
        }

        private void VerifyWriteWord<TAddressSegment, TAddressSegment1, TAddressSegment2>(ushort segmentAddress,
            ushort length,
            Mock<TAddressSegment> segment,
            Mock<TAddressSegment1> otherSegment1,
            Mock<TAddressSegment2> otherSegment2) where TAddressSegment : class, IWriteableAddressSegment
            where TAddressSegment1 : class, IWriteableAddressSegment where TAddressSegment2 : class, IWriteableAddressSegment
        {
            segment.ResetCalls();
            otherSegment1.ResetCalls();
            otherSegment2.ResetCalls();

            var boundaryCheckComplete = false;

            foreach (var percentile in GetAddressRangePercentiles(segmentAddress, length))
            {
                var value = (ushort) _random.Next(0xffff);
                _mmu.WriteWord(percentile, value);
                var address = (ushort) (percentile - segmentAddress);

                var message = string.Format("MMU Address: {0}, Segment Address: {1}, Value: {2}", percentile, address, value);

                if (address < length - 1)
                {
                    segment.Verify(x => x.WriteWord(address, value), Times.Once, message);
                }
                else
                {
                    var bytes = BitConverter.GetBytes(value);
                    segment.Verify(x => x.WriteByte(address, bytes[0]), Times.Once, message);
                    otherSegment1.Verify(x => x.WriteByte(0, bytes[1]), Times.Once, message);
                    boundaryCheckComplete = true;
                }
            }

            Assert.IsTrue(boundaryCheckComplete, "Boundary check was not performed");
            otherSegment1.Verify(x => x.WriteWord(It.IsAny<ushort>(), It.IsAny<ushort>()), Times.Never);
            otherSegment2.Verify(x => x.WriteWord(It.IsAny<ushort>(), It.IsAny<ushort>()), Times.Never);
        }

        private void VerifyReadBytes<TAddressSegment, TAddressSegment1, TAddressSegment2>(ushort segmentAddress,
            ushort length,
            Mock<TAddressSegment> segment,
            Mock<TAddressSegment1> otherSegment1,
            Mock<TAddressSegment2> otherSegment2) where TAddressSegment : class, IReadableAddressSegment
            where TAddressSegment1 : class, IReadableAddressSegment where TAddressSegment2 : class, IReadableAddressSegment
        {
            segment.ResetCalls();
            otherSegment1.ResetCalls();
            otherSegment2.ResetCalls();

            var boundaryCheckComplete = false;

            foreach (var percentile in GetAddressRangePercentiles(segmentAddress, length))
            {
                var readLength = _random.Next(MinByteLength, MaxByteLength + 1);
                _mmu.ReadBytes(percentile, readLength);
                var address = (ushort) (percentile - segmentAddress);

                if (address + readLength < length - 1)
                {
                    segment.Verify(x => x.ReadBytes(address, readLength), Times.Once);
                }
                else
                {
                    var length0 = length - address;
                    var length1 = readLength - length0;
                    segment.Verify(x => x.ReadBytes(address, length0),
                                   Times.Once,
                                   string.Format("Expected address: {0}, length: {1}", address, length0));
                    otherSegment1.Verify(x => x.ReadBytes(0, length1),
                                         Times.Once,
                                         string.Format("Expected address: {0}, length: {1}", 0, length1));
                    boundaryCheckComplete = true;
                }
            }

            Assert.IsTrue(boundaryCheckComplete, "Boundary check was not performed");
            otherSegment2.Verify(x => x.ReadBytes(It.IsAny<ushort>(), It.IsAny<int>()), Times.Never);
        }

        private void VerifyWriteBytes<TAddressSegment, TAddressSegment1, TAddressSegment2>(ushort segmentAddress,
            ushort length,
            Mock<TAddressSegment> segment,
            Mock<TAddressSegment1> otherSegment1,
            Mock<TAddressSegment2> otherSegment2) where TAddressSegment : class, IWriteableAddressSegment
            where TAddressSegment1 : class, IWriteableAddressSegment where TAddressSegment2 : class, IWriteableAddressSegment
        {
            segment.ResetCalls();
            otherSegment1.ResetCalls();
            otherSegment2.ResetCalls();

            var boundaryCheckComplete = false;

            foreach (var percentile in GetAddressRangePercentiles(segmentAddress, length))
            {
                var writeLength = _random.Next(MinByteLength, MaxByteLength + 1);
                var bytes = new byte[writeLength];
                _random.NextBytes(bytes);
                _mmu.WriteBytes(percentile, bytes);
                var address = (ushort) (percentile - segmentAddress);

                if (address + writeLength < length - 1)
                {
                    segment.Verify(x => x.WriteBytes(address, bytes), Times.Once);
                }
                else
                {
                    var length0 = length - address;
                    var length1 = writeLength - length0;
                    segment.Verify(x => x.WriteBytes(address, It.Is<byte[]>(y => AreEqual(bytes.Take(length0), y))),
                                   Times.Once,
                                   string.Format("Expected address: {0}, length: {1}", address, length0));
                    otherSegment1.Verify(x => x.WriteBytes(0, It.Is<byte[]>(y => AreEqual(bytes.Skip(length0), y))),
                                         Times.Once,
                                         string.Format("Expected address: {0}, length: {1}", 0, length1));
                    boundaryCheckComplete = true;
                }
            }

            Assert.IsTrue(boundaryCheckComplete, "Boundary check was not performed");
            otherSegment2.Verify(x => x.WriteBytes(It.IsAny<ushort>(), It.IsAny<byte[]>()), Times.Never);
        }

        private static bool AreEqual(IEnumerable<byte> bytes0, IEnumerable<byte> bytes1)
        {
            return bytes0.Zip(bytes1, (b0, b1) => b0 == b1).All(x => x);
        }

        private static IEnumerable<ushort> GetAddressRangePercentiles(ushort baseAddress, ushort length)
        {
            return
                Enumerable.Range(0, 100)
                          .Select(x => (ushort) Math.Round(baseAddress + length * x / 100.0))
                          .Concat(new[] { (ushort) (baseAddress + length - 1) });
        }

        [Test]
        public void ReadByteFromSegment0()
        {
            VerifyReadByte(Address0, Length0, _segment0R, _segment1, _segment2);
        }

        [Test]
        public void ReadByteFromSegment1()
        {
            VerifyReadByte(Address1, Length1, _segment1, _segment0R, _segment2);
        }

        [Test]
        public void ReadByteFromSegment2()
        {
            VerifyReadByte(Address2, Length2, _segment2, _segment0R, _segment1);
        }

        [Test]
        public void ReadBytesFromSegment0()
        {
            VerifyReadBytes(Address0, Length0, _segment0R, _segment1, _segment2);
        }

        [Test]
        public void ReadBytesFromSegment1()
        {
            VerifyReadBytes(Address1, Length1, _segment1, _segment2, _segment0R);
        }

        [Test]
        public void ReadBytesFromSegment2()
        {
            VerifyReadBytes(Address2, Length2, _segment2, _segment0R, _segment1);
        }

        [Test]
        public void ReadWordFromSegment0()
        {
            VerifyReadWord(Address0, Length0, _segment0R, _segment1, _segment2);
        }

        [Test]
        public void ReadWordFromSegment1()
        {
            VerifyReadWord(Address1, Length1, _segment1, _segment2, _segment0R);
        }

        [Test]
        public void ReadWordFromSegment2()
        {
            VerifyReadWord(Address2, Length2, _segment2, _segment0R, _segment1);
        }

        [Test]
        public void WriteBytesToSegment0()
        {
            VerifyWriteBytes(Address0, Length0, _segment0W, _segment1, _segment2);
        }

        [Test]
        public void WriteBytesToSegment1()
        {
            VerifyWriteBytes(Address1, Length1, _segment1, _segment2, _segment0W);
        }

        [Test]
        public void WriteBytesToSegment2()
        {
            VerifyWriteBytes(Address2, Length2, _segment2, _segment0W, _segment1);
        }

        [Test]
        public void WriteByteToSegment0()
        {
            VerifyWriteByte(Address0, Length0, _segment0W, _segment1, _segment2);
        }

        [Test]
        public void WriteByteToSegment1()
        {
            VerifyWriteByte(Address1, Length1, _segment1, _segment0W, _segment2);
        }

        [Test]
        public void WriteByteToSegment2()
        {
            VerifyWriteByte(Address2, Length2, _segment2, _segment0W, _segment1);
        }

        [Test]
        public void WriteWordToSegment0()
        {
            VerifyWriteWord(Address0, Length0, _segment0W, _segment1, _segment2);
        }

        [Test]
        public void WriteWordToSegment1()
        {
            VerifyWriteWord(Address1, Length1, _segment1, _segment2, _segment0W);
        }

        [Test]
        public void WriteWordToSegment2()
        {
            VerifyWriteWord(Address2, Length2, _segment2, _segment0W, _segment1);
        }
    }
}