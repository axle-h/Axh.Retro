namespace Axh.Retro.CPU.X80.Tests.Core
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoder16BitArithmeticTests : Z80InstructionDecoderBase
    {
        [TestCase(PrimaryOpCode.ADD_HL_BC)]
        [TestCase(PrimaryOpCode.ADD_HL_DE)]
        [TestCase(PrimaryOpCode.ADD_HL_HL)]
        [TestCase(PrimaryOpCode.ADD_HL_SP)]
        public void ADD_HL_s(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x5353;
            this.Alu.Setup(x => x.Add(HL, It.IsAny<ushort>())).Returns(Value);

            Run(3, 11, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.ADD_HL_BC:
                    this.Alu.Verify(x => x.Add(HL, BC), Times.Once);
                    break;
                case PrimaryOpCode.ADD_HL_DE:
                    this.Alu.Verify(x => x.Add(HL, DE), Times.Once);
                    break;
                case PrimaryOpCode.ADD_HL_HL:
                    this.Alu.Verify(x => x.Add(HL, HL), Times.Once);
                    break;
                case PrimaryOpCode.ADD_HL_SP:
                    this.Alu.Verify(x => x.Add(HL, SP), Times.Once);
                    break;
            }
            
            this.GpRegisters.VerifySet(x => x.HL = Value, Times.Once);
        }
        
        [TestCase(PrimaryOpCode.Prefix_DD, PrimaryOpCode.ADD_HL_BC)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrimaryOpCode.ADD_HL_DE)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrimaryOpCode.ADD_HL_HL)]
        [TestCase(PrimaryOpCode.Prefix_DD, PrimaryOpCode.ADD_HL_SP)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrimaryOpCode.ADD_HL_BC)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrimaryOpCode.ADD_HL_DE)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrimaryOpCode.ADD_HL_HL)]
        [TestCase(PrimaryOpCode.Prefix_FD, PrimaryOpCode.ADD_HL_SP)]
        public void ADD_IXY_s(PrimaryOpCode prefix, PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            var indexRegister = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            const ushort Value = 0x5353;
            this.Alu.Setup(x => x.Add(indexRegister, It.IsAny<ushort>())).Returns(Value);

            Run(4, 15, prefix, opCode);
            
            switch (opCode)
            {
                case PrimaryOpCode.ADD_HL_BC:
                    this.Alu.Verify(x => x.Add(indexRegister, BC), Times.Once);
                    break;
                case PrimaryOpCode.ADD_HL_DE:
                    this.Alu.Verify(x => x.Add(indexRegister, DE), Times.Once);
                    break;
                case PrimaryOpCode.ADD_HL_HL:
                    this.Alu.Verify(x => x.Add(indexRegister, indexRegister), Times.Once);
                    break;
                case PrimaryOpCode.ADD_HL_SP:
                    this.Alu.Verify(x => x.Add(indexRegister, SP), Times.Once);
                    break;
            }

            if (prefix == PrimaryOpCode.Prefix_DD)
            {
                this.Registers.VerifySet(x => x.IX = Value, Times.Once);
            }
            else
            {
                this.Registers.VerifySet(x => x.IY = Value, Times.Once);
            }
        }

        [TestCase(PrefixEdOpCode.ADC_HL_BC)]
        [TestCase(PrefixEdOpCode.ADC_HL_DE)]
        [TestCase(PrefixEdOpCode.ADC_HL_HL)]
        [TestCase(PrefixEdOpCode.ADC_HL_SP)]
        public void ADC_HL_s(PrefixEdOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x5353;
            this.Alu.Setup(x => x.AddWithCarry(HL, It.IsAny<ushort>())).Returns(Value);

            Run(4, 15, PrimaryOpCode.Prefix_ED, opCode);

            switch (opCode)
            {
                case PrefixEdOpCode.ADC_HL_BC:
                    this.Alu.Verify(x => x.AddWithCarry(HL, BC), Times.Once);
                    break;
                case PrefixEdOpCode.ADC_HL_DE:
                    this.Alu.Verify(x => x.AddWithCarry(HL, DE), Times.Once);
                    break;
                case PrefixEdOpCode.ADC_HL_HL:
                    this.Alu.Verify(x => x.AddWithCarry(HL, HL), Times.Once);
                    break;
                case PrefixEdOpCode.ADC_HL_SP:
                    this.Alu.Verify(x => x.AddWithCarry(HL, SP), Times.Once);
                    break;
            }

            this.GpRegisters.VerifySet(x => x.HL = Value, Times.Once);
        }
    }
}
