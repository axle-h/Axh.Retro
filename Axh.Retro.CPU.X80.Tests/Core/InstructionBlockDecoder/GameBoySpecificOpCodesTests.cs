namespace Axh.Retro.CPU.X80.Tests.Core.InstructionBlockDecoder
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.OpCodes;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class GameBoySpecificOpCodesTests : InstructionBlockDecoderTestsBase<IIntel8080Registers>
    {
        public GameBoySpecificOpCodesTests() : base(CpuMode.GameBoy)
        {
        }

        [Test]
        public void LD_mnn_SP()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort NN = 0x3836;

            RunWithHalt(6, 20, GameBoySpecificOpCode.LD_mnn_SP, NN);

            this.Mmu.Verify(x => x.WriteWord(NN, SP), Times.Once);
        }

        [Test]
        public void STOP()
        {
            this.ResetMocks();
            
            this.Run(1, 4, GameBoySpecificOpCode.STOP);

            this.Cache.Verify(x => x.NextByte(), Times.Once);
        }
    }
}
