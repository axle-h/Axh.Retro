namespace Axh.Retro.CPU.X80.Tests.Core
{
    using System.Collections;
    using System.Collections.Generic;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoderCompareTests : Z80InstructionDecoderBase
    {
        [Test]
        public void CPI()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0xa7;

            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            Run(4, 16, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPI);
            
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
                queue[i] = (byte)(A - queue.Length + i + 1);
                this.Mmu.Setup(x => x.ReadByte((ushort)(i + HL))).Returns(queue[i]);
            }

            // Replicate implementation. When values compared match the zero flag is set.
            this.Alu.Setup(x => x.Compare(A, A)).Callback(() => FlagsRegister.Object.Zero = true);

            Run(4 + 5 * (queue.Length - 1), 16 + 21 * (queue.Length - 1), PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPIR);

            for (var i = 1; i < queue.Length; i++)
            {
                this.Mmu.Verify(x => x.ReadByte((ushort)(HL + i)), Times.Once);
                this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == HL + i), Times.Once);
                this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == BC - i), Times.Once);
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

            Run(4 + 5 * (Count - 1), 16 + 21 * (Count - 1), PrimaryOpCode.Prefix_ED, PrefixEdOpCode.CPIR);

            // We are expecting to keep looping until BC = 0.
            for (var i = 1; i < Count; i++)
            {
                this.Mmu.Verify(x => x.ReadByte(unchecked ((ushort)(HL + i))), Times.Once);
                this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == unchecked(HL + i)), Times.Once);
                this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == unchecked(Count - i)), Times.Once);
            }

            this.Alu.Verify(x => x.Compare(A, Value), Times.Exactly(Count));
        }
    }
}
