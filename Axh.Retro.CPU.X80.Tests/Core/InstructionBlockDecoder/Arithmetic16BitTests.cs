namespace Axh.Retro.CPU.X80.Tests.Core.InstructionBlockDecoder
{
    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Arithmetic16BitTests : InstructionBlockDecoderTestsBase
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

            RunWithHalt(3, 11, opCode);

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

            RunWithHalt(4, 15, prefix, opCode);
            
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

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_ED, opCode);

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

        [TestCase(PrefixEdOpCode.SBC_HL_BC)]
        [TestCase(PrefixEdOpCode.SBC_HL_DE)]
        [TestCase(PrefixEdOpCode.SBC_HL_HL)]
        [TestCase(PrefixEdOpCode.SBC_HL_SP)]
        public void SBC_HL_s(PrefixEdOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const ushort Value = 0x5353;
            this.Alu.Setup(x => x.SubtractWithCarry(HL, It.IsAny<ushort>())).Returns(Value);

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_ED, opCode);

            switch (opCode)
            {
                case PrefixEdOpCode.SBC_HL_BC:
                    this.Alu.Verify(x => x.SubtractWithCarry(HL, BC), Times.Once);
                    break;
                case PrefixEdOpCode.SBC_HL_DE:
                    this.Alu.Verify(x => x.SubtractWithCarry(HL, DE), Times.Once);
                    break;
                case PrefixEdOpCode.SBC_HL_HL:
                    this.Alu.Verify(x => x.SubtractWithCarry(HL, HL), Times.Once);
                    break;
                case PrefixEdOpCode.SBC_HL_SP:
                    this.Alu.Verify(x => x.SubtractWithCarry(HL, SP), Times.Once);
                    break;
            }

            this.GpRegisters.VerifySet(x => x.HL = Value, Times.Once);
        }


        [TestCase(PrimaryOpCode.INC_BC)]
        [TestCase(PrimaryOpCode.INC_DE)]
        [TestCase(PrimaryOpCode.INC_HL)]
        [TestCase(PrimaryOpCode.INC_SP)]
        public void INC_ss(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(1, 6, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.INC_BC:
                    this.GpRegisters.VerifySet(x => x.BC = unchecked ((ushort)(BC + 1)), Times.Once);
                    break;
                case PrimaryOpCode.INC_DE:
                    this.GpRegisters.VerifySet(x => x.DE = unchecked((ushort)(DE + 1)), Times.Once);
                    break;
                case PrimaryOpCode.INC_HL:
                    this.GpRegisters.VerifySet(x => x.HL = unchecked((ushort)(HL + 1)), Times.Once);
                    break;
                case PrimaryOpCode.INC_SP:
                    this.Registers.VerifySet(x => x.StackPointer = unchecked((ushort)(SP + 1)), Times.Once);
                    break;
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void INC_IXY(PrimaryOpCode prefix)
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            RunWithHalt(2, 10, prefix, PrimaryOpCode.INC_HL);

            switch (prefix)
            {
                case PrimaryOpCode.Prefix_DD:
                    this.Registers.VerifySet(x => x.IX = unchecked((ushort)(IX + 1)), Times.Once);
                    break;
                case PrimaryOpCode.Prefix_FD:
                    this.Registers.VerifySet(x => x.IY = unchecked((ushort)(IY + 1)), Times.Once);
                    break;
            }
        }

        [TestCase(PrimaryOpCode.DEC_BC)]
        [TestCase(PrimaryOpCode.DEC_DE)]
        [TestCase(PrimaryOpCode.DEC_HL)]
        [TestCase(PrimaryOpCode.DEC_SP)]
        public void DEC_ss(PrimaryOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(1, 6, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.DEC_BC:
                    this.GpRegisters.VerifySet(x => x.BC = unchecked((ushort)(BC - 1)), Times.Once);
                    break;
                case PrimaryOpCode.DEC_DE:
                    this.GpRegisters.VerifySet(x => x.DE = unchecked((ushort)(DE - 1)), Times.Once);
                    break;
                case PrimaryOpCode.DEC_HL:
                    this.GpRegisters.VerifySet(x => x.HL = unchecked((ushort)(HL - 1)), Times.Once);
                    break;
                case PrimaryOpCode.DEC_SP:
                    this.Registers.VerifySet(x => x.StackPointer = unchecked((ushort)(SP - 1)), Times.Once);
                    break;
            }
        }


        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void DEC_IXY(PrimaryOpCode prefix)
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(2, 10, prefix, PrimaryOpCode.DEC_HL);

            switch (prefix)
            {
                case PrimaryOpCode.Prefix_DD:
                    this.Registers.VerifySet(x => x.IX = unchecked((ushort)(IX - 1)), Times.Once);
                    break;
                case PrimaryOpCode.Prefix_FD:
                    this.Registers.VerifySet(x => x.IY = unchecked((ushort)(IY - 1)), Times.Once);
                    break;
            }
        }
    }
}
