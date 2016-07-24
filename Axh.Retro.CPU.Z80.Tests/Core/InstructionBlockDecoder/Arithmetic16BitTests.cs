using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    [TestFixture]
    public class Arithmetic16BitTests : InstructionBlockDecoderTestsBase
    {
        public Arithmetic16BitTests() : base(CpuMode.Z80)
        {
        }

        [TestCase(PrimaryOpCode.ADD_HL_BC)]
        [TestCase(PrimaryOpCode.ADD_HL_DE)]
        [TestCase(PrimaryOpCode.ADD_HL_HL)]
        [TestCase(PrimaryOpCode.ADD_HL_SP)]
        public void ADD_HL_s(PrimaryOpCode opCode)
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0x5353;
            Alu.Setup(x => x.Add(HL, It.IsAny<ushort>())).Returns(Value);

            RunWithHalt(3, 11, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.ADD_HL_BC:
                    Alu.Verify(x => x.Add(HL, BC), Times.Once);
                    break;
                case PrimaryOpCode.ADD_HL_DE:
                    Alu.Verify(x => x.Add(HL, DE), Times.Once);
                    break;
                case PrimaryOpCode.ADD_HL_HL:
                    Alu.Verify(x => x.Add(HL, HL), Times.Once);
                    break;
                case PrimaryOpCode.ADD_HL_SP:
                    Alu.Verify(x => x.Add(HL, SP), Times.Once);
                    break;
            }

            Assert.AreEqual(Value, GpRegisters.HL);
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
            SetupRegisters();
            ResetMocks();

            var indexRegister = prefix == PrimaryOpCode.Prefix_DD ? IX : IY;
            const ushort Value = 0x5353;
            Alu.Setup(x => x.Add(indexRegister, It.IsAny<ushort>())).Returns(Value);

            RunWithHalt(4, 15, prefix, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.ADD_HL_BC:
                    Alu.Verify(x => x.Add(indexRegister, BC), Times.Once);
                    break;
                case PrimaryOpCode.ADD_HL_DE:
                    Alu.Verify(x => x.Add(indexRegister, DE), Times.Once);
                    break;
                case PrimaryOpCode.ADD_HL_HL:
                    Alu.Verify(x => x.Add(indexRegister, indexRegister), Times.Once);
                    break;
                case PrimaryOpCode.ADD_HL_SP:
                    Alu.Verify(x => x.Add(indexRegister, SP), Times.Once);
                    break;
            }

            if (prefix == PrimaryOpCode.Prefix_DD)
            {
                Registers.VerifySet(x => x.IX = Value, Times.Once);
            }
            else
            {
                Registers.VerifySet(x => x.IY = Value, Times.Once);
            }
        }

        [TestCase(PrefixEdOpCode.ADC_HL_BC)]
        [TestCase(PrefixEdOpCode.ADC_HL_DE)]
        [TestCase(PrefixEdOpCode.ADC_HL_HL)]
        [TestCase(PrefixEdOpCode.ADC_HL_SP)]
        public void ADC_HL_s(PrefixEdOpCode opCode)
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0x5353;
            Alu.Setup(x => x.AddWithCarry(HL, It.IsAny<ushort>())).Returns(Value);

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_ED, opCode);

            switch (opCode)
            {
                case PrefixEdOpCode.ADC_HL_BC:
                    Alu.Verify(x => x.AddWithCarry(HL, BC), Times.Once);
                    break;
                case PrefixEdOpCode.ADC_HL_DE:
                    Alu.Verify(x => x.AddWithCarry(HL, DE), Times.Once);
                    break;
                case PrefixEdOpCode.ADC_HL_HL:
                    Alu.Verify(x => x.AddWithCarry(HL, HL), Times.Once);
                    break;
                case PrefixEdOpCode.ADC_HL_SP:
                    Alu.Verify(x => x.AddWithCarry(HL, SP), Times.Once);
                    break;
            }

            Assert.AreEqual(Value, GpRegisters.HL);
        }

        [TestCase(PrefixEdOpCode.SBC_HL_BC)]
        [TestCase(PrefixEdOpCode.SBC_HL_DE)]
        [TestCase(PrefixEdOpCode.SBC_HL_HL)]
        [TestCase(PrefixEdOpCode.SBC_HL_SP)]
        public void SBC_HL_s(PrefixEdOpCode opCode)
        {
            SetupRegisters();
            ResetMocks();

            const ushort Value = 0x5353;
            Alu.Setup(x => x.SubtractWithCarry(HL, It.IsAny<ushort>())).Returns(Value);

            RunWithHalt(4, 15, PrimaryOpCode.Prefix_ED, opCode);

            switch (opCode)
            {
                case PrefixEdOpCode.SBC_HL_BC:
                    Alu.Verify(x => x.SubtractWithCarry(HL, BC), Times.Once);
                    break;
                case PrefixEdOpCode.SBC_HL_DE:
                    Alu.Verify(x => x.SubtractWithCarry(HL, DE), Times.Once);
                    break;
                case PrefixEdOpCode.SBC_HL_HL:
                    Alu.Verify(x => x.SubtractWithCarry(HL, HL), Times.Once);
                    break;
                case PrefixEdOpCode.SBC_HL_SP:
                    Alu.Verify(x => x.SubtractWithCarry(HL, SP), Times.Once);
                    break;
            }

            Assert.AreEqual(Value, GpRegisters.HL);
        }

        [TestCase(PrimaryOpCode.INC_BC)]
        [TestCase(PrimaryOpCode.INC_DE)]
        [TestCase(PrimaryOpCode.INC_HL)]
        [TestCase(PrimaryOpCode.INC_SP)]
        public void INC_ss(PrimaryOpCode opCode)
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(1, 6, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.INC_BC:
                    Assert.AreEqual(unchecked(BC + 1), GpRegisters.BC);
                    break;
                case PrimaryOpCode.INC_DE:
                    Assert.AreEqual(unchecked(DE + 1), GpRegisters.DE);
                    break;
                case PrimaryOpCode.INC_HL:
                    Assert.AreEqual(unchecked(HL + 1), GpRegisters.HL);
                    break;
                case PrimaryOpCode.INC_SP:
                    Registers.VerifySet(x => x.StackPointer = unchecked((ushort) (SP + 1)), Times.Once);
                    break;
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void INC_IXY(PrimaryOpCode prefix)
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 10, prefix, PrimaryOpCode.INC_HL);

            switch (prefix)
            {
                case PrimaryOpCode.Prefix_DD:
                    Registers.VerifySet(x => x.IX = unchecked((ushort) (IX + 1)), Times.Once);
                    break;
                case PrimaryOpCode.Prefix_FD:
                    Registers.VerifySet(x => x.IY = unchecked((ushort) (IY + 1)), Times.Once);
                    break;
            }
        }

        [TestCase(PrimaryOpCode.DEC_BC)]
        [TestCase(PrimaryOpCode.DEC_DE)]
        [TestCase(PrimaryOpCode.DEC_HL)]
        [TestCase(PrimaryOpCode.DEC_SP)]
        public void DEC_ss(PrimaryOpCode opCode)
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(1, 6, opCode);

            switch (opCode)
            {
                case PrimaryOpCode.DEC_BC:
                    Assert.AreEqual(unchecked(BC - 1), GpRegisters.BC);
                    break;
                case PrimaryOpCode.DEC_DE:
                    Assert.AreEqual(unchecked(DE - 1), GpRegisters.DE);
                    break;
                case PrimaryOpCode.DEC_HL:
                    Assert.AreEqual(unchecked(HL - 1), GpRegisters.HL);
                    break;
                case PrimaryOpCode.DEC_SP:
                    Registers.VerifySet(x => x.StackPointer = unchecked((ushort) (SP - 1)), Times.Once);
                    break;
            }
        }

        [TestCase(PrimaryOpCode.Prefix_DD)]
        [TestCase(PrimaryOpCode.Prefix_FD)]
        public void DEC_IXY(PrimaryOpCode prefix)
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 10, prefix, PrimaryOpCode.DEC_HL);

            switch (prefix)
            {
                case PrimaryOpCode.Prefix_DD:
                    Registers.VerifySet(x => x.IX = unchecked((ushort) (IX - 1)), Times.Once);
                    break;
                case PrimaryOpCode.Prefix_FD:
                    Registers.VerifySet(x => x.IY = unchecked((ushort) (IY - 1)), Times.Once);
                    break;
            }
        }
    }
}