namespace Axh.Retro.CPU.X80.Tests.Core.InstructionBlockDecoder
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class InputOutputTests : InstructionBlockDecoderTestsBase
    {
        [Test]
        public void IN_A_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Port = 0xac;
            const byte Value = 0x5e;
            this.Io.Setup(x => x.ReadByte(Port, A)).Returns(Value);

            RunWithNOP(3, 11, PrimaryOpCode.IN_A_n, Port);

            this.Io.Verify(x => x.ReadByte(Port, A), Times.Once);
            this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
        }

        [TestCase(PrefixEdOpCode.IN_A_C)]
        [TestCase(PrefixEdOpCode.IN_B_C)]
        [TestCase(PrefixEdOpCode.IN_C_C)]
        [TestCase(PrefixEdOpCode.IN_D_C)]
        [TestCase(PrefixEdOpCode.IN_E_C)]
        [TestCase(PrefixEdOpCode.IN_F_C)]
        [TestCase(PrefixEdOpCode.IN_H_C)]
        [TestCase(PrefixEdOpCode.IN_L_C)]
        public void IN_r_C(PrefixEdOpCode opCode)
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            const byte Value = 0x5e;
            this.Io.Setup(x => x.ReadByte(C, B)).Returns(Value);

            RunWithNOP(3, 12, PrimaryOpCode.Prefix_ED, opCode);

            this.Io.Verify(x => x.ReadByte(C, B), Times.Once);

            switch (opCode)
            {
                case PrefixEdOpCode.IN_A_C:
                    this.AfRegisters.VerifySet(x => x.A = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_B_C:
                    this.GpRegisters.VerifySet(x => x.B = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_C_C:
                    this.GpRegisters.VerifySet(x => x.C = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_D_C:
                    this.GpRegisters.VerifySet(x => x.D = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_E_C:
                    this.GpRegisters.VerifySet(x => x.E = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_F_C:
                    this.FlagsRegister.VerifySet(x => x.Register = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_H_C:
                    this.GpRegisters.VerifySet(x => x.H = Value, Times.Once);
                    break;
                case PrefixEdOpCode.IN_L_C:
                    this.GpRegisters.VerifySet(x => x.L = Value, Times.Once);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
