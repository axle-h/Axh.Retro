using System;
using System.Linq;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.OpCodes
{
    [TestFixture]
    public class OpCodeDistributionTests
    {
        private static void AssertOpCodes<TOpCode>()
        {
            var opCodes = Enum.GetValues(typeof (TOpCode)).Cast<byte>().OrderBy(x => x).ToArray();
            var allBytes = Enumerable.Range(0, 0x100).Select(x => (byte) x).ToArray();

            Assert.AreEqual(allBytes.Length, opCodes.Length);
            CollectionAssert.AreEquivalent(allBytes, opCodes);
        }

        [Test]
        public void PrefixCbOpCode()
        {
            AssertOpCodes<PrefixCbOpCode>();
        }

        [Test]
        public void PrimaryOpCode()
        {
            AssertOpCodes<PrimaryOpCode>();
        }
    }
}