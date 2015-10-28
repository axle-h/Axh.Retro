namespace Axh.Retro.CPU.X80.Tests.Core.Z80InstructionDecoder
{
    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class CompareTests : Z80InstructionDecoderTestsBase
    {
        [Test]
        public void CPI()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xa7;

            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithNOP(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPI);
            
            this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == HL + 1), Times.Once);
            this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == BC - 1), Times.Once);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.Compare(A, Value), Times.Once);
        }

        [Test]
        public void CPIR_Match()
        {
            this.SetupRegisters();
            this.ResetMocks();

            var queue = new byte[10];
            for (var i = 0; i < queue.Length; i++)
            {
                var index = i;
                queue[i] = (byte)(A - queue.Length + i + 1);
                this.Mmu.Setup(x => x.ReadByte((ushort)(index + HL))).Returns(queue[i]);
            }

            // Replicate implementation. When values compared match the zero flag is set.
            this.Alu.Setup(x => x.Compare(A, A)).Callback(() => FlagsRegister.Object.Zero = true);

            RunWithNOP(4 + 5 * (queue.Length - 1), 16 + 21 * (queue.Length - 1), PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPIR);

            for (var i = 1; i < queue.Length; i++)
            {
                var index = i;
                this.Mmu.Verify(x => x.ReadByte((ushort)(HL + index)), Times.Once);
                this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == HL + index), Times.Once);
                this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == BC - index), Times.Once);
            }

            foreach (var item in queue)
            {
                this.Alu.Verify(x => x.Compare(A, item), Times.Once);
            }
        }

        [Test]
        public void CPIR_NoMatch()
        {
            const ushort Count = 20;
            const byte Value = A + 1;

            this.SetupRegisters(bc: Count);
            this.ResetMocks();

            // Always return something from the MMU that is not A
            this.Mmu.Setup(x => x.ReadByte(It.IsAny<ushort>())).Returns(Value);

            // Replicate implementation. When values compared match the zero flag is set.
            this.Alu.Setup(x => x.Compare(A, A)).Callback(() => FlagsRegister.Object.Zero = true);

            RunWithNOP(4 + 5 * (Count - 1), 16 + 21 * (Count - 1), PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPIR);

            // We are expecting to keep looping until BC = 0.
            for (var i = 1; i < Count; i++)
            {
                var index = i;
                this.Mmu.Verify(x => x.ReadByte(unchecked ((ushort)(HL + index))), Times.Once);
                this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == unchecked(HL + index)), Times.Once);
                this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == unchecked(Count - index)), Times.Once);
            }

            this.Alu.Verify(x => x.Compare(A, Value), Times.Exactly(Count));
        }

        [Test]
        public void CPD()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xa7;

            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithNOP(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPD);

            this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == HL - 1), Times.Once);
            this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == BC - 1), Times.Once);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.Alu.Verify(x => x.Compare(A, Value), Times.Once);
        }

        [Test]
        public void CPDR_Match()
        {
            this.SetupRegisters();
            this.ResetMocks();

            var queue = new byte[10];
            for (var i = 0; i < queue.Length; i++)
            {
                var index = i;
                queue[i] = (byte)(A - queue.Length + i + 1);
                this.Mmu.Setup(x => x.ReadByte((ushort)(HL - index))).Returns(queue[i]);
            }

            // Replicate implementation. When values compared match the zero flag is set.
            this.Alu.Setup(x => x.Compare(A, A)).Callback(() => FlagsRegister.Object.Zero = true);

            RunWithNOP(4 + 5 * (queue.Length - 1), 16 + 21 * (queue.Length - 1), PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPDR);

            for (var i = 1; i < queue.Length; i++)
            {
                var index = i;
                this.Mmu.Verify(x => x.ReadByte((ushort)(HL - index)), Times.Once);
                this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == HL - index), Times.Once);
                this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == BC - index), Times.Once);
            }

            foreach (var item in queue)
            {
                this.Alu.Verify(x => x.Compare(A, item), Times.Once);
            }
        }

        [Test]
        public void CPDR_NoMatch()
        {
            const ushort Count = 20;
            const byte Value = A + 1;

            this.SetupRegisters(bc: Count);
            this.ResetMocks();

            // Always return something from the MMU that is not A
            this.Mmu.Setup(x => x.ReadByte(It.IsAny<ushort>())).Returns(Value);

            // Replicate implementation. When values compared match the zero flag is set.
            this.Alu.Setup(x => x.Compare(A, A)).Callback(() => FlagsRegister.Object.Zero = true);

            RunWithNOP(4 + 5 * (Count - 1), 16 + 21 * (Count - 1), PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPDR);

            // We are expecting to keep looping until BC = 0.
            for (var i = 1; i < Count; i++)
            {
                var index = i;
                this.Mmu.Verify(x => x.ReadByte(unchecked((ushort)(HL - index))), Times.Once);
                this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == unchecked(HL - index)), Times.Once);
                this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == unchecked(Count - index)), Times.Once);
            }

            this.Alu.Verify(x => x.Compare(A, Value), Times.Exactly(Count));
        }
    }
}
