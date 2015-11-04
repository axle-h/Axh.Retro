namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
    using System;

    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.OpCodes;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Load8BitTests : InstructionBlockDecoderTestsBase<IZ80Registers>
    {
        public Load8BitTests() : base(CpuMode.Z80)
        {
        }

        [TestCase(PrimaryOpCode.LD_A_A)]
        [TestCase(PrimaryOpCode.LD_B_A)]
        [TestCase(PrimaryOpCode.LD_C_A)]
        [TestCase(PrimaryOpCode.LD_D_A)]
        [TestCase(PrimaryOpCode.LD_E_A)]
        [TestCase(PrimaryOpCode.LD_H_A)]
        [TestCase(PrimaryOpCode.LD_L_A)]
        [TestCase(PrimaryOpCode.LD_A_B)]
        [TestCase(PrimaryOpCode.LD_B_B)]
        [TestCase(PrimaryOpCode.LD_C_B)]
        [TestCase(PrimaryOpCode.LD_D_B)]
        [TestCase(PrimaryOpCode.LD_E_B)]
        [TestCase(PrimaryOpCode.LD_H_B)]
        [TestCase(PrimaryOpCode.LD_L_B)]
        [TestCase(PrimaryOpCode.LD_A_C)]
        [TestCase(PrimaryOpCode.LD_B_C)]
        [TestCase(PrimaryOpCode.LD_C_C)]
        [TestCase(PrimaryOpCode.LD_D_C)]
        [TestCase(PrimaryOpCode.LD_E_C)]
        [TestCase(PrimaryOpCode.LD_H_C)]
        [TestCase(PrimaryOpCode.LD_L_C)]
        [TestCase(PrimaryOpCode.LD_A_D)]
        [TestCase(PrimaryOpCode.LD_B_D)]
        [TestCase(PrimaryOpCode.LD_C_D)]
        [TestCase(PrimaryOpCode.LD_D_D)]
        [TestCase(PrimaryOpCode.LD_E_D)]
        [TestCase(PrimaryOpCode.LD_H_D)]
        [TestCase(PrimaryOpCode.LD_L_D)]
        [TestCase(PrimaryOpCode.LD_A_E)]
        [TestCase(PrimaryOpCode.LD_B_E)]
        [TestCase(PrimaryOpCode.LD_C_E)]
        [TestCase(PrimaryOpCode.LD_D_E)]
        [TestCase(PrimaryOpCode.LD_E_E)]
        [TestCase(PrimaryOpCode.LD_H_E)]
        [TestCase(PrimaryOpCode.LD_L_E)]
        [TestCase(PrimaryOpCode.LD_A_H)]
        [TestCase(PrimaryOpCode.LD_B_H)]
        [TestCase(PrimaryOpCode.LD_C_H)]
        [TestCase(PrimaryOpCode.LD_D_H)]
        [TestCase(PrimaryOpCode.LD_E_H)]
        [TestCase(PrimaryOpCode.LD_H_H)]
        [TestCase(PrimaryOpCode.LD_L_H)]
        [TestCase(PrimaryOpCode.LD_A_L)]
        [TestCase(PrimaryOpCode.LD_B_L)]
        [TestCase(PrimaryOpCode.LD_C_L)]
        [TestCase(PrimaryOpCode.LD_D_L)]
        [TestCase(PrimaryOpCode.LD_E_L)]
        [TestCase(PrimaryOpCode.LD_H_L)]
        [TestCase(PrimaryOpCode.LD_L_L)]
        public void LD_r_r(PrimaryOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(1, 4, opcode);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_A:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, this.AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_A:
                    this.AfRegisters.Verify(x => x.A, Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == A), Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_A:
                    this.AfRegisters.Verify(x => x.A, Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == A), Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_A:
                    this.AfRegisters.Verify(x => x.A, Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == A), Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_A:
                    this.AfRegisters.Verify(x => x.A, Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == A), Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_A:
                    this.AfRegisters.Verify(x => x.A, Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == A), Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_A:
                    this.AfRegisters.Verify(x => x.A, Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == A), Times.Once);
                    Assert.AreEqual(A, this.GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_B:
                    this.AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == B), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, this.AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_B:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_B:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == B), Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_B:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == B), Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_B:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == B), Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_B:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == B), Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_B:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == B), Times.Once);
                    Assert.AreEqual(B, this.GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_C:
                    this.AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == C), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, this.AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_C:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == C), Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_C:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_C:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == C), Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_C:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == C), Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_C:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == C), Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_C:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == C), Times.Once);
                    Assert.AreEqual(C, this.GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_D:
                    this.AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == D), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, this.AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_D:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == D), Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_D:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == D), Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_D:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_D:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == D), Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_D:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == D), Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_D:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == D), Times.Once);
                    Assert.AreEqual(D, this.GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_E:
                    this.AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == E), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, this.AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_E:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == E), Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_E:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == E), Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_E:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == E), Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_E:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_E:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == E), Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_E:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == E), Times.Once);
                    Assert.AreEqual(E, this.GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_H:
                    this.AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == H), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, this.AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_H:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == H), Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_H:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == H), Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_H:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == H), Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_H:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == H), Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_H:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_H:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == H), Times.Once);
                    Assert.AreEqual(H, this.GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_L:
                    this.AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == L), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, this.AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_L:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == L), Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_L:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == L), Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_L:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == L), Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_L:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == L), Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_L:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == L), Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_L:
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(L, this.GpRegisters.Object.L);
                    break;
                default:
                    throw new NotImplementedException(opcode.ToString());
            }

        }

        [TestCase(PrimaryOpCode.LD_A_n, 0x12)]
        [TestCase(PrimaryOpCode.LD_B_n, 0x34)]
        [TestCase(PrimaryOpCode.LD_C_n, 0x56)]
        [TestCase(PrimaryOpCode.LD_D_n, 0x78)]
        [TestCase(PrimaryOpCode.LD_E_n, 0x9a)]
        [TestCase(PrimaryOpCode.LD_H_n, 0xbc)]
        [TestCase(PrimaryOpCode.LD_L_n, 0xde)]
        public void LD_r_n(PrimaryOpCode opcode, byte value)
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(2, 7, opcode, value);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_n:
                    Assert.AreEqual(value, this.AfRegisters.Object.A);
                    this.AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == value), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_B_n:
                    Assert.AreEqual(value, this.GpRegisters.Object.B);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == value), Times.Once);
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_C_n:
                    Assert.AreEqual(value, this.GpRegisters.Object.C);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == value), Times.Once);
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_D_n:
                    Assert.AreEqual(value, this.GpRegisters.Object.D);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == value), Times.Once);
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_E_n:
                    Assert.AreEqual(value, this.GpRegisters.Object.E);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == value), Times.Once);
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_H_n:
                    Assert.AreEqual(value, this.GpRegisters.Object.H);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == value), Times.Once);
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_L_n:
                    Assert.AreEqual(value, this.GpRegisters.Object.L);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == value), Times.Once);
                    this.AfRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }


        [TestCase(PrimaryOpCode.LD_A_mHL)]
        [TestCase(PrimaryOpCode.LD_B_mHL)]
        [TestCase(PrimaryOpCode.LD_C_mHL)]
        [TestCase(PrimaryOpCode.LD_D_mHL)]
        [TestCase(PrimaryOpCode.LD_E_mHL)]
        [TestCase(PrimaryOpCode.LD_H_mHL)]
        [TestCase(PrimaryOpCode.LD_L_mHL)]
        public void LD_r_mHL(PrimaryOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte Value = 0x8f;

            this.Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(2, 7, opcode);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.GpRegisters.VerifyGet(x => x.HL, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_mHL:
                    Assert.AreEqual(Value, this.AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_mHL:
                    Assert.AreEqual(Value, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_mHL:
                    Assert.AreEqual(Value, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_mHL:
                    Assert.AreEqual(Value, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_mHL:
                    Assert.AreEqual(Value, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_mHL:
                    Assert.AreEqual(Value, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_mHL:
                    Assert.AreEqual(Value, this.GpRegisters.Object.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }

        }

        [TestCase(PrimaryOpCode.LD_A_mHL)]
        [TestCase(PrimaryOpCode.LD_B_mHL)]
        [TestCase(PrimaryOpCode.LD_C_mHL)]
        [TestCase(PrimaryOpCode.LD_D_mHL)]
        [TestCase(PrimaryOpCode.LD_E_mHL)]
        [TestCase(PrimaryOpCode.LD_H_mHL)]
        [TestCase(PrimaryOpCode.LD_L_mHL)]
        public void LD_r_mIXd(PrimaryOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte SignedD = -127;
            const byte UnsignedD = unchecked((byte)SignedD);
            const byte ValueAtIXd = 0x8f;

            this.Mmu.Setup(x => x.ReadByte(IX + SignedD)).Returns(ValueAtIXd);
            RunWithHalt(5, 19, PrimaryOpCode.Prefix_DD, opcode, UnsignedD);

            this.Mmu.Verify(x => x.ReadByte(IX + SignedD), Times.Once);
            this.Registers.VerifyGet(x => x.IX, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_mHL:
                    Assert.AreEqual(ValueAtIXd, this.AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_mHL:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_mHL:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_mHL:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_mHL:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_mHL:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_mHL:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [TestCase(PrimaryOpCode.LD_A_mHL)]
        [TestCase(PrimaryOpCode.LD_B_mHL)]
        [TestCase(PrimaryOpCode.LD_C_mHL)]
        [TestCase(PrimaryOpCode.LD_D_mHL)]
        [TestCase(PrimaryOpCode.LD_E_mHL)]
        [TestCase(PrimaryOpCode.LD_H_mHL)]
        [TestCase(PrimaryOpCode.LD_L_mHL)]
        public void LD_r_mIYd(PrimaryOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte SignedD = 55;
            const byte UnsignedD = unchecked((byte)SignedD);
            const byte ValueAtIYd = 0x42;

            this.Mmu.Setup(x => x.ReadByte(IY + SignedD)).Returns(ValueAtIYd);

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_FD, opcode, UnsignedD);

            this.Mmu.Verify(x => x.ReadByte(IY + SignedD), Times.Once);
            this.Registers.VerifyGet(x => x.IY, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_mHL:
                    Assert.AreEqual(ValueAtIYd, this.AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_mHL:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_mHL:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_mHL:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_mHL:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_mHL:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_mHL:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [TestCase(PrimaryOpCode.LD_mHL_A)]
        [TestCase(PrimaryOpCode.LD_mHL_B)]
        [TestCase(PrimaryOpCode.LD_mHL_C)]
        [TestCase(PrimaryOpCode.LD_mHL_D)]
        [TestCase(PrimaryOpCode.LD_mHL_E)]
        [TestCase(PrimaryOpCode.LD_mHL_H)]
        [TestCase(PrimaryOpCode.LD_mHL_L)]
        public void LD_mHL_r(PrimaryOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(2, 7, opcode);

            switch (opcode)
            {
                case PrimaryOpCode.LD_mHL_A:
                    this.Mmu.Verify(x => x.WriteByte(HL, A), Times.Once);
                    this.AfRegisters.VerifyGet(x => x.A, Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_B:
                    this.Mmu.Verify(x => x.WriteByte(HL, B), Times.Once);
                    this.GpRegisters.VerifyGet(x => x.B, Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_C:
                    this.Mmu.Verify(x => x.WriteByte(HL, C), Times.Once);
                    this.GpRegisters.VerifyGet(x => x.C, Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_D:
                    this.Mmu.Verify(x => x.WriteByte(HL, D), Times.Once);
                    this.GpRegisters.VerifyGet(x => x.D, Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_E:
                    this.Mmu.Verify(x => x.WriteByte(HL, E), Times.Once);
                    this.GpRegisters.VerifyGet(x => x.E, Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_H:
                    this.Mmu.Verify(x => x.WriteByte(HL, H), Times.Once);
                    this.GpRegisters.VerifyGet(x => x.H, Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_L:
                    this.Mmu.Verify(x => x.WriteByte(HL, L), Times.Once);
                    this.GpRegisters.VerifyGet(x => x.L, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [TestCase(0x12)]
        [TestCase(0x34)]
        public void LD_mHL_n(byte value)
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(3, 10, PrimaryOpCode.LD_mHL_n, value);
            
            this.Mmu.Verify(x => x.WriteByte(HL, value), Times.Once);
        }

        [TestCase(PrimaryOpCode.LD_mHL_A)]
        [TestCase(PrimaryOpCode.LD_mHL_B)]
        [TestCase(PrimaryOpCode.LD_mHL_C)]
        [TestCase(PrimaryOpCode.LD_mHL_D)]
        [TestCase(PrimaryOpCode.LD_mHL_E)]
        [TestCase(PrimaryOpCode.LD_mHL_H)]
        [TestCase(PrimaryOpCode.LD_mHL_L)]
        public void LD_mIXd_r(PrimaryOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte SignedD = -66;
            const byte UnsignedD = unchecked((byte)SignedD);
            
            RunWithHalt(5, 19, PrimaryOpCode.Prefix_DD, opcode, UnsignedD);
            
            this.Registers.VerifyGet(x => x.IX, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_mHL_A:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, A), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_B:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, B), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_C:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, C), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_D:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, D), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_E:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, E), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_H:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, H), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_L:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [TestCase(PrimaryOpCode.LD_mHL_A)]
        [TestCase(PrimaryOpCode.LD_mHL_B)]
        [TestCase(PrimaryOpCode.LD_mHL_C)]
        [TestCase(PrimaryOpCode.LD_mHL_D)]
        [TestCase(PrimaryOpCode.LD_mHL_E)]
        [TestCase(PrimaryOpCode.LD_mHL_H)]
        [TestCase(PrimaryOpCode.LD_mHL_L)]
        public void LD_mIYd_r(PrimaryOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte SignedD = 88;
            const byte UnsignedD = unchecked((byte)SignedD);

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_FD, opcode, UnsignedD);

            this.Registers.VerifyGet(x => x.IY, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_mHL_A:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, A), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_B:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, B), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_C:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, C), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_D:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, D), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_E:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, E), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_H:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, H), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_L:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [Test]
        public void LD_mIXd_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte SignedD = 77;
            const byte UnsignedD = unchecked((byte)SignedD);
            const byte N = 0x73;
            
            RunWithHalt(5, 19, PrimaryOpCode.Prefix_DD, PrimaryOpCode.LD_mHL_n, UnsignedD, N);

            this.Registers.VerifyGet(x => x.IX, Times.Once);
            this.Mmu.Verify(x => x.WriteByte(IX + SignedD, N), Times.Once);
        }

        [Test]
        public void LD_mIYd_n()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte SignedD = -77;
            const byte UnsignedD = unchecked((byte)SignedD);
            const byte N = 0xF6;

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_FD, PrimaryOpCode.LD_mHL_n, UnsignedD, N);

            this.Registers.VerifyGet(x => x.IY, Times.Once);
            this.Mmu.Verify(x => x.WriteByte(IY + SignedD, N), Times.Once);
        }

        [Test]
        public void LD_A_mBC()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtBC = 0x61;

            this.Mmu.Setup(x => x.ReadByte(BC)).Returns(ValueAtBC);

            RunWithHalt(2, 7, PrimaryOpCode.LD_A_mBC);

            this.Mmu.Verify(x => x.ReadByte(BC), Times.Once);
            Assert.AreEqual(ValueAtBC, this.AfRegisters.Object.A);
        }

        [Test]
        public void LD_A_mDE()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtDE = 0x0c;

            this.Mmu.Setup(x => x.ReadByte(DE)).Returns(ValueAtDE);

            RunWithHalt(2, 7, PrimaryOpCode.LD_A_mDE);

            this.Mmu.Verify(x => x.ReadByte(DE), Times.Once);
            Assert.AreEqual(ValueAtDE, this.AfRegisters.Object.A);
        }


        [Test]
        public void LD_A_mnn()
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            const ushort NN = 0x827d;
            const byte ValueAtNN = 0xcf;

            this.Mmu.Setup(x => x.ReadByte(NN)).Returns(ValueAtNN);

            RunWithHalt(4, 13, PrimaryOpCode.LD_A_mnn, NN);

            this.Mmu.Verify(x => x.ReadByte(NN), Times.Once);
            Assert.AreEqual(ValueAtNN, this.AfRegisters.Object.A);
        }

        [Test]
        public void LD_mBC_A()
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(2, 7, PrimaryOpCode.LD_mBC_A);

            this.Mmu.Verify(x => x.WriteByte(BC, A), Times.Once);
            this.AfRegisters.VerifyGet(x => x.A, Times.Once);
        }

        [Test]
        public void LD_mDE_A()
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(2, 7, PrimaryOpCode.LD_mDE_A);

            this.Mmu.Verify(x => x.WriteByte(DE, A), Times.Once);
            this.AfRegisters.VerifyGet(x => x.A, Times.Once);
        }
        
        [Test]
        public void LD_mnn_A()
        {
            this.SetupRegisters();
            this.ResetMocks();
            const ushort N = 0x613b;

            RunWithHalt(2, 7, PrimaryOpCode.LD_mnn_A, N);

            this.Mmu.Verify(x => x.WriteByte(N, A), Times.Once);
            this.AfRegisters.VerifyGet(x => x.A, Times.Once);
        }

        [Test]
        public void LD_A_I()
        {
            this.SetupRegisters();
            this.ResetMocks();
            
            RunWithHalt(2, 9, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LD_A_I);
            
            this.AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == I), Times.Once);
            Assert.AreEqual(I, this.AfRegisters.Object.A);

            // Check flags
            this.FlagsRegister.Verify(x => x.SetResultFlags(It.Is<byte>(y => y == I)), Times.Once);
            this.FlagsRegister.VerifySet(x => x.HalfCarry = It.Is<bool>(y => !y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.ParityOverflow = IFF2, Times.Once);
            this.FlagsRegister.VerifySet(x => x.Subtract = It.Is<bool>(y => !y), Times.Once);
        }


        [Test]
        public void LD_A_R()
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(2, 9, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LD_A_R);

            this.AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == R), Times.Once);
            Assert.AreEqual(R, this.AfRegisters.Object.A);

            // Check flags
            this.FlagsRegister.Verify(x => x.SetResultFlags(It.Is<byte>(y => y == R)), Times.Once);
            this.FlagsRegister.VerifySet(x => x.HalfCarry = It.Is<bool>(y => !y), Times.Once);
            this.FlagsRegister.VerifySet(x => x.ParityOverflow = IFF2, Times.Once);
            this.FlagsRegister.VerifySet(x => x.Subtract = It.Is<bool>(y => !y), Times.Once);
        }


        [Test]
        public void LD_I_A()
        {
            this.SetupRegisters();
            this.ResetMocks();

            RunWithHalt(2, 9, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LD_I_A);

            this.Registers.VerifySet(x => x.I = It.Is<byte>(y => y == A), Times.Once);
            Assert.AreEqual(A, this.Registers.Object.I);
        }


        [Test]
        public void LD_R_A()
        {
            this.SetupRegisters();
            this.ResetMocks();

            // This is not all it seems.
            // 1. A is loaded to R
            // 2. LS 7-bytes of R are incremented 3 times for the 3 opcodes executed.
            // Note: this is not cycle accurate. R should only be incremented twice as Prefix_ED has already been run. However, it really doesn't matter. 
            const int ExprectedR = ((A + 3) & 0x7f) | ((A & 0x80) >> 8);

            RunWithHalt(2, 9, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LD_R_A);

            this.Registers.VerifySet(x => x.R = It.Is<byte>(y => y == A), Times.Once);
            
            Assert.AreEqual(ExprectedR, this.Registers.Object.R);
        }

    }
}
