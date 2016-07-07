using System;
using Axh.Retro.CPU.Common.Contracts.Config;
using Axh.Retro.CPU.Common.Contracts.Exceptions;
using Axh.Retro.CPU.Common.Memory;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Common.Tests.Memory
{
    [TestFixture]
    public class ArrayBackedMemoryTests
    {
        private const ushort Address = 0xf00d;
        private const ushort Length = 0x0bad;

        private static Mock<IMemoryBankConfig> SetupMemoryBankConfigMock()
        {
            var memoryBankConfig = new Mock<IMemoryBankConfig>();
            memoryBankConfig.Setup(x => x.Address).Returns(Address);
            memoryBankConfig.Setup(x => x.Length).Returns(Length);
            memoryBankConfig.Setup(x => x.InitialState).Returns(null as byte[]);
            return memoryBankConfig;
        }

        [Test]
        public void CanReadAndWriteByteArrays()
        {
            var memoryBankConfig = SetupMemoryBankConfigMock();
            var memory = new ArrayBackedMemoryBank(memoryBankConfig.Object);
            const ushort ByteArrayAddress = Length / 4;
            const ushort ByteArrayLength = Length / 2;

            var random = new Random(1234);
            var bytes = new byte[ByteArrayLength];
            random.NextBytes(bytes);

            memory.WriteBytes(ByteArrayAddress, bytes);
            var observed = memory.ReadBytes(ByteArrayAddress, ByteArrayLength);

            CollectionAssert.AreEqual(bytes, observed);
        }

        [Test]
        public void CanReadAndWriteSingleBytes()
        {
            var memoryBankConfig = SetupMemoryBankConfigMock();
            var memory = new ArrayBackedMemoryBank(memoryBankConfig.Object);
            var random = new Random(1234);
            for (ushort i = 0; i < Length; i++)
            {
                var expected = (byte) random.Next(byte.MaxValue);
                memory.WriteByte(i, expected);
                var actual = memory.ReadByte(i);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CanReadAndWriteSingleWords()
        {
            var memoryBankConfig = SetupMemoryBankConfigMock();
            var memory = new ArrayBackedMemoryBank(memoryBankConfig.Object);
            var random = new Random(1234);
            for (ushort i = 0; i < Length - 1; i++)
            {
                var expected = (ushort) random.Next(ushort.MaxValue);
                memory.WriteWord(i, expected);
                var actual = memory.ReadWord(i);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void StateArrayTooBigThrowsMemoryConfigStateException()
        {
            const ushort BadLength = Length + 1;
            var memoryBankConfig = SetupMemoryBankConfigMock();
            memoryBankConfig.Setup(x => x.InitialState).Returns(new byte[BadLength]);
            var exception = Assert.Throws<MemoryConfigStateException>(() => new ArrayBackedMemoryBank(memoryBankConfig.Object));

            Assert.AreEqual(Address, exception.Adddress);
            Assert.AreEqual(Length, exception.SegmentLength);
            Assert.AreEqual(BadLength, exception.StateLength);
        }

        [Test]
        public void StateArrayTooSmallThrowsMemoryConfigStateException()
        {
            const ushort BadLength = Length - 1;
            var memoryBankConfig = SetupMemoryBankConfigMock();
            memoryBankConfig.Setup(x => x.InitialState).Returns(new byte[BadLength]);
            var exception = Assert.Throws<MemoryConfigStateException>(() => new ArrayBackedMemoryBank(memoryBankConfig.Object));

            Assert.AreEqual(Address, exception.Adddress);
            Assert.AreEqual(Length, exception.SegmentLength);
            Assert.AreEqual(BadLength, exception.StateLength);
        }

        [Test]
        public void StateIsSetInConstructor()
        {
            var random = new Random(1234);
            var state = new byte[Length];
            random.NextBytes(state);

            var memoryBankConfig = SetupMemoryBankConfigMock();
            memoryBankConfig.Setup(x => x.InitialState).Returns(state);
            var memory = new ArrayBackedMemoryBank(memoryBankConfig.Object);

            var memoryBytes = memory.ReadBytes(0, Length);
            CollectionAssert.AreEqual(state, memoryBytes);
        }
    }
}