namespace Axh.Emulation.CPU.X80.Tests.Memory
{
    using System;
    using System.Linq;

    using Axh.Emulation.CPU.X80.Contracts.Config;
    using Axh.Emulation.CPU.X80.Contracts.Exceptions;
    using Axh.Emulation.CPU.X80.Memory;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ReadOnlyMemoryBankTests
    {
        private const ushort Address = 0xf00d;
        private const ushort Length = 0x0bad;

        private static readonly byte[] ByteContent;
        private static readonly ushort[] WordContent;

        static ReadOnlyMemoryBankTests()
        {
            ByteContent = new byte[Length];
            var random = new Random(1234);
            random.NextBytes(ByteContent);

            WordContent = new ushort[Length / 2];
            for (var i = 0; i < Length / 2; i++)
            {
                WordContent[i] = BitConverter.ToUInt16(ByteContent, i * 2);
            }
        }

        private static Mock<IMemoryBankConfig> SetupMemoryBankConfigMock()
        {
            var memoryBankConfig = new Mock<IMemoryBankConfig>();
            memoryBankConfig.Setup(x => x.Address).Returns(Address);
            memoryBankConfig.Setup(x => x.Length).Returns(Length);
            memoryBankConfig.Setup(x => x.State).Returns(ByteContent);
            return memoryBankConfig;
        }

        [Test]
        public void StateArrayTooSmallThrowsMemoryConfigStateException()
        {
            const ushort BadLength = Length - 1;
            var memoryBankConfig = SetupMemoryBankConfigMock();
            memoryBankConfig.Setup(x => x.State).Returns(new byte[BadLength]);
            var exception = Assert.Throws<MemoryConfigStateException>(() => new ArrayBackedMemoryBank(memoryBankConfig.Object));

            Assert.AreEqual(Address, exception.Adddress);
            Assert.AreEqual(Length, exception.SegmentLength);
            Assert.AreEqual(BadLength, exception.StateLength);
        }

        [Test]
        public void StateArrayTooBigThrowsMemoryConfigStateException()
        {
            const ushort BadLength = Length + 1;
            var memoryBankConfig = SetupMemoryBankConfigMock();
            memoryBankConfig.Setup(x => x.State).Returns(new byte[BadLength]);
            var exception = Assert.Throws<MemoryConfigStateException>(() => new ArrayBackedMemoryBank(memoryBankConfig.Object));

            Assert.AreEqual(Address, exception.Adddress);
            Assert.AreEqual(Length, exception.SegmentLength);
            Assert.AreEqual(BadLength, exception.StateLength);
        }

        [Test]
        public void ReadByteReturnsCorrectContent()
        {
            var memoryBankConfig = SetupMemoryBankConfigMock();
            var memory = new ReadOnlyMemoryBank(memoryBankConfig.Object);

            var readAllBytes = Enumerable.Range(0, Length).Select(i => memory.ReadByte((ushort)i)).ToArray();
            CollectionAssert.AreEqual(ByteContent, readAllBytes);
        }

        [Test]
        public void ReadWordReturnsCorrectContent()
        {
            var memoryBankConfig = SetupMemoryBankConfigMock();
            var memory = new ReadOnlyMemoryBank(memoryBankConfig.Object);

            var readAllWords = Enumerable.Range(0, Length / 2).Select(i => memory.ReadWord((ushort)(i * 2))).ToArray();
            CollectionAssert.AreEqual(WordContent, readAllWords);
        }

        [Test]
        public void ReadBytesReturnsCorrectContent()
        {
            var memoryBankConfig = SetupMemoryBankConfigMock();
            var memory = new ReadOnlyMemoryBank(memoryBankConfig.Object);

            var readAllBytes = memory.ReadBytes(0, Length);
            CollectionAssert.AreEqual(ByteContent, readAllBytes);
        }

    }
}
