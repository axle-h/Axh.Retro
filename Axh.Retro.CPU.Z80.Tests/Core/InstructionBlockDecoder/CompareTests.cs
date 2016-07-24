using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class CompareTests : InstructionBlockDecoderTestsBase
    {
        public CompareTests() : base(CpuMode.Z80)
        {
        }

        [Test]
        public void CPD()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xa7;

            Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPD);

            Assert.AreEqual(GpRegisters.HL, HL - 1);
            Assert.AreEqual(GpRegisters.BC, BC - 1);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Alu.Verify(x => x.Compare(A, Value), Times.Once);
        }

        [Test]
        public void CPDR_Match()
        {
            SetupRegisters();
            ResetMocks();

            var queue = new byte[10];
            for (var i = 0; i < queue.Length; i++)
            {
                var index = i;
                queue[i] = (byte) (A - queue.Length + i + 1);
                Mmu.Setup(x => x.ReadByte((ushort) (HL - index))).Returns(queue[i]);
            }

            // Replicate implementation. When values compared match the zero flag is set.
            Alu.Setup(x => x.Compare(A, A)).Callback(() => FlagsRegister.Object.Zero = true);

            RunWithHalt(4 + 5 * (queue.Length - 1), 16 + 21 * (queue.Length - 1), PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPDR);

            for (var i = 1; i < queue.Length; i++)
            {
                var index = i;
                Mmu.Verify(x => x.ReadByte((ushort) (HL - index)), Times.Once);
            }

            foreach (var item in queue)
            {
                Alu.Verify(x => x.Compare(A, item), Times.Once);
            }

            Assert.AreEqual(GpRegisters.HL, HL - queue.Length);
            Assert.AreEqual(GpRegisters.BC, BC - queue.Length);
        }

        [Test]
        public void CPDR_NoMatch()
        {
            const ushort Count = 20;
            const byte Value = A + 1;

            SetupRegisters(Count);
            ResetMocks();

            // Always return something from the MMU that is not A
            Mmu.Setup(x => x.ReadByte(It.IsAny<ushort>())).Returns(Value);

            // Replicate implementation. When values compared match the zero flag is set.
            Alu.Setup(x => x.Compare(A, A)).Callback(() => FlagsRegister.Object.Zero = true);

            RunWithHalt(4 + 5 * (Count - 1), 16 + 21 * (Count - 1), PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPDR);

            // We are expecting to keep looping until BC = 0.
            for (var i = 1; i < Count; i++)
            {
                var index = i;
                Mmu.Verify(x => x.ReadByte(unchecked((ushort) (HL - index))), Times.Once);
            }

            Alu.Verify(x => x.Compare(A, Value), Times.Exactly(Count));
            Assert.AreEqual(GpRegisters.HL, unchecked(HL - Count));
            Assert.AreEqual(GpRegisters.BC, 0);
        }

        [Test]
        public void CPI()
        {
            SetupRegisters();
            ResetMocks();

            const byte Value = 0xa7;

            Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPI);

            Assert.AreEqual(GpRegisters.HL, HL + 1);
            Assert.AreEqual(GpRegisters.BC, BC - 1);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            Alu.Verify(x => x.Compare(A, Value), Times.Once);
        }

        [Test]
        public void CPIR_Match()
        {
            SetupRegisters();
            ResetMocks();

            var queue = new byte[10];
            for (var i = 0; i < queue.Length; i++)
            {
                var index = i;
                queue[i] = (byte) (A - queue.Length + i + 1);
                Mmu.Setup(x => x.ReadByte((ushort) (index + HL))).Returns(queue[i]);
            }

            // Replicate implementation. When values compared match the zero flag is set.
            Alu.Setup(x => x.Compare(A, A)).Callback(() => FlagsRegister.Object.Zero = true);

            RunWithHalt(4 + 5 * (queue.Length - 1), 16 + 21 * (queue.Length - 1), PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPIR);

            for (var i = 1; i < queue.Length; i++)
            {
                var index = i;
                Mmu.Verify(x => x.ReadByte((ushort) (HL + index)), Times.Once);
            }

            foreach (var item in queue)
            {
                Alu.Verify(x => x.Compare(A, item), Times.Once);
            }

            Assert.AreEqual(GpRegisters.HL, HL + queue.Length);
            Assert.AreEqual(GpRegisters.BC, BC - queue.Length);
        }

        [Test]
        public void CPIR_NoMatch()
        {
            const ushort Count = 20;
            const byte Value = A + 1;

            SetupRegisters(Count);
            ResetMocks();

            // Always return something from the MMU that is not A
            Mmu.Setup(x => x.ReadByte(It.IsAny<ushort>())).Returns(Value);

            // Replicate implementation. When values compared match the zero flag is set.
            Alu.Setup(x => x.Compare(A, A)).Callback(() => FlagsRegister.Object.Zero = true);

            RunWithHalt(4 + 5 * (Count - 1), 16 + 21 * (Count - 1), PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPIR);

            // We are expecting to keep looping until BC = 0.
            for (var i = 1; i < Count; i++)
            {
                var index = i;
                Mmu.Verify(x => x.ReadByte(unchecked ((ushort) (HL + index))), Times.Once);
            }

            Alu.Verify(x => x.Compare(A, Value), Times.Exactly(Count));
            Assert.AreEqual(GpRegisters.HL, unchecked(HL + Count));
            Assert.AreEqual(GpRegisters.BC, 0);
        }
    }
}