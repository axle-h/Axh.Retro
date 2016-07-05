using System;
using System.Linq;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Memory;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Common.Tests.Mmu
{
    [TestFixture]
    public class PrefetchQueueTests
    {
        private const int Address = 0x1000;

        private Mock<IMmu> _mmu;

        private static readonly byte[] Bytes;

        private static readonly ushort[] Words;

        static PrefetchQueueTests()
        {
            Bytes = new byte[ushort.MaxValue];
            var random = new Random();
            random.NextBytes(Bytes);

            Words = new ushort[ushort.MaxValue / 2];
            for (var i = 0; i < Words.Length; i++)
            {
                Words[i] = BitConverter.ToUInt16(Bytes, i * 2);
            }
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _mmu = new Mock<IMmu>();
            _mmu.Setup(x => x.ReadBytes(It.IsAny<ushort>(), It.IsAny<int>()))
                .Returns((ushort address, int length) => Bytes.Skip(address).Take(length).ToArray());
            _mmu.Setup(x => x.ReadWord(It.IsAny<ushort>()))
                .Returns((ushort address) => BitConverter.ToUInt16(Bytes, address));
            _mmu.Setup(x => x.ReadByte(It.IsAny<ushort>())).Returns((ushort address) => Bytes[address]);
        }

        [Test]
        public void NextByte()
        {
            var cache = new PrefetchQueue(_mmu.Object);
            cache.ReBuildCache(Address);

            // Read 1k bytes
            var bytes = Enumerable.Range(0, 1000).Select(i => cache.NextByte()).ToArray();

            var expected = Bytes.Skip(Address).Take(1000).ToArray();

            CollectionAssert.AreEqual(expected, bytes);
        }

        [Test]
        public void NextBytes()
        {
            var cache = new PrefetchQueue(_mmu.Object);
            cache.ReBuildCache(Address);

            // Read 1k bytes
            var bytes = cache.NextBytes(1000);

            var expected = Bytes.Skip(Address).Take(1000).ToArray();

            CollectionAssert.AreEqual(expected, bytes);
        }

        [Test]
        public void NextBytesThenNextByte()
        {
            var cache = new PrefetchQueue(_mmu.Object);
            cache.ReBuildCache(Address);

            // Read 1k bytes
            var bytes = cache.NextBytes(1000);

            var expected = Bytes.Skip(Address).Take(1000).ToArray();

            CollectionAssert.AreEqual(expected, bytes);

            // Now read some more bytes
            bytes = Enumerable.Range(0, 1000).Select(i => cache.NextByte()).ToArray();

            expected = Bytes.Skip(Address + 1000).Take(1000).ToArray();

            CollectionAssert.AreEqual(expected, bytes);
        }

        [Test]
        public void NextWord()
        {
            var cache = new PrefetchQueue(_mmu.Object);
            cache.ReBuildCache(Address);

            // Read 1k words
            var words = Enumerable.Range(0, 1000).Select(i => cache.NextWord()).ToArray();

            var expected = Words.Skip(Address / 2).Take(1000).ToArray();

            CollectionAssert.AreEqual(expected, words);
        }
    }
}