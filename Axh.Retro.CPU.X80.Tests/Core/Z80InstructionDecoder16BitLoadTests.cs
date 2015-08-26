namespace Axh.Retro.CPU.X80.Tests.Core
{
    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoder16BitLoadTests : Z80InstructionDecoderBase
    {
        [TestCase(PrimaryOpCode.LD_BC_nn)]
        [TestCase(PrimaryOpCode.LD_DE_nn)]
        [TestCase(PrimaryOpCode.LD_HL_nn)]
        [TestCase(PrimaryOpCode.LD_SP_nn)]
        public void LD_dd_nn(PrimaryOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x9178;

            Run(2, 10, opcode, Value);

            switch (opcode)
            {
                case PrimaryOpCode.LD_BC_nn:
                    this.GpRegisters.VerifySet(x => x.BC = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, this.GpRegisters.Object.BC);
                    break;
                case PrimaryOpCode.LD_DE_nn:
                    this.GpRegisters.VerifySet(x => x.DE = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, this.GpRegisters.Object.DE);
                    break;
                case PrimaryOpCode.LD_HL_nn:
                    this.GpRegisters.VerifySet(x => x.HL = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, this.GpRegisters.Object.HL);
                    break;
                case PrimaryOpCode.LD_SP_nn:
                    this.Registers.VerifySet(x => x.StackPointer = It.Is<ushort>(y => y == Value), Times.Once);
                    Assert.AreEqual(Value, this.Registers.Object.StackPointer);
                    break;
            }
        }
    }
}
