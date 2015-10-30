namespace Axh.Retro.CPU.X80.Tests.OpCodes
{
    using System;
    using System.Linq;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using NUnit.Framework;

    [TestFixture]
    public class OpCodeDistributionTests
    {
        [Test]
        public void PrimaryOpCode()
        {
            AssertOpCodes<PrimaryOpCode>();
        }

        [Test]
        [Ignore]
        public void PrefixEdOpCode()
        {
            AssertOpCodes<PrefixEdOpCode>();
        }

        [Test]
        public void PrefixCbOpCode()
        {
            AssertOpCodes<PrefixCbOpCode>();
        }

        private static void AssertOpCodes<TOpCode>()
        {
            var opCodes = Enum.GetValues(typeof(TOpCode)).Cast<byte>().OrderBy(x => x).ToArray();
            var allBytes = Enumerable.Range(0, 0x100).Select(x => (byte)x).ToArray();

            Assert.AreEqual(allBytes.Length, opCodes.Length);
            CollectionAssert.AreEquivalent(allBytes, opCodes);
        }
    }
}
