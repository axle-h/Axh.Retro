namespace Axh.Emulation.CPU.Z80.Tests.Memory
{
    using System;
    using System.Linq;
    using Axh.Emulation.CPU.Z80.Contracts.Config;
    using Axh.Emulation.CPU.Z80.Contracts.Memory;
    using Axh.Emulation.CPU.Z80.Memory;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ArrayBackedMemoryTests
    {
        private const ushort Address = 0xf00d;
        private const ushort Length = 0x0bad;
        
        private IAddressSegment memory;


        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var memoryBankConfig = new Mock<IMemoryBankConfig>();
            memoryBankConfig.Setup(x => x.Address).Returns(Address);
            memoryBankConfig.Setup(x => x.Length).Returns(Length);

            memory = new ArrayBackedMemoryBank(memoryBankConfig.Object);
        }

        [Test]
        public void CanReadAndWriteSingleBytes()
        {
            var random = new Random(1234);
            for (ushort i = 0; i < Length; i++)
            {
                var expected = (byte)random.Next(byte.MaxValue);
                memory.WriteByte(i, expected);
                var actual = memory.ReadByte(i);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CanReadAndWriteSingleWords()
        {
            var random = new Random(1234);
            for (ushort i = 0; i < Length - 1; i++)
            {
                var expected = (ushort)random.Next(ushort.MaxValue);
                memory.WriteWord(i, expected);
                var actual = memory.ReadWord(i);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CanReadAndWriteByteArrays()
        {
            const ushort ByteArrayAddress = Length / 4;
            const ushort ByteArrayLength = Length / 2;

            var random = new Random(1234);
            var bytes = new byte[ByteArrayLength];
            random.NextBytes(bytes);

            this.memory.WriteBytes(ByteArrayAddress, bytes);
            var observed = this.memory.ReadBytes(ByteArrayAddress, ByteArrayLength);

            CollectionAssert.AreEqual(bytes, observed);
        }

        [Test]
        public void CanReadAndWriteWordArrays()
        {
            const ushort WordArrayAddress = Length / 4;
            const ushort WordArrayLength = Length / 4;

            var random = new Random(1234);
            var words = Enumerable.Range(0, WordArrayLength).Select(x => (ushort)random.Next(ushort.MaxValue)).ToArray();

            this.memory.WriteWords(WordArrayAddress, words);
            var observed = this.memory.ReadWords(WordArrayAddress, WordArrayLength);

            CollectionAssert.AreEqual(words, observed);
        }

    }
}
