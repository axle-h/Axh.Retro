namespace Axh.Retro.CPU.X80.Tests.Core
{
    using System;

    using Axh.Retro.CPU.X80.Contracts.OpCodes;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoder8BitLoadTests : Z80InstructionDecoderBase
    {

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

            this.SetCacheForSingleBytes(opcode, PrimaryOpCode.HALT);
            Run8BitLoadGroup(2, 8, opcode, PrimaryOpCode.HALT);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_A:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, this.GpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_A:
                    this.GpRegisters.Verify(x => x.A, Times.Once);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == A), Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_A:
                    this.GpRegisters.Verify(x => x.A, Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == A), Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_A:
                    this.GpRegisters.Verify(x => x.A, Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == A), Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_A:
                    this.GpRegisters.Verify(x => x.A, Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == A), Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_A:
                    this.GpRegisters.Verify(x => x.A, Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == A), Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_A:
                    this.GpRegisters.Verify(x => x.A, Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == A), Times.Once);
                    Assert.AreEqual(A, this.GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_B:
                    this.GpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == B), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, this.GpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_B:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_B:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Once);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == B), Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_B:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == B), Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_B:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == B), Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_B:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == B), Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_B:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == B), Times.Once);
                    Assert.AreEqual(B, this.GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_C:
                    this.GpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == C), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, this.GpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_C:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == C), Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_C:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_C:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Once);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == C), Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_C:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == C), Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_C:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == C), Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_C:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == C), Times.Once);
                    Assert.AreEqual(C, this.GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_D:
                    this.GpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == D), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, this.GpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_D:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == D), Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_D:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == D), Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_D:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_D:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Once);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == D), Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_D:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == D), Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_D:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == D), Times.Once);
                    Assert.AreEqual(D, this.GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_E:
                    this.GpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == E), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, this.GpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_E:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == E), Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_E:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == E), Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_E:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == E), Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_E:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_E:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Once);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == E), Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_E:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == E), Times.Once);
                    Assert.AreEqual(E, this.GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_H:
                    this.GpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == H), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, this.GpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_H:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == H), Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_H:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == H), Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_H:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == H), Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_H:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == H), Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_H:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_H:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Once);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == H), Times.Once);
                    Assert.AreEqual(H, this.GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_L:
                    this.GpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == L), Times.Once);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, this.GpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_L:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == L), Times.Once);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, this.GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_L:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == L), Times.Once);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, this.GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_L:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == L), Times.Once);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, this.GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_L:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == L), Times.Once);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, this.GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_L:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == L), Times.Once);
                    this.GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, this.GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_L:
                    this.GpRegisters.Verify(x => x.A, Times.Never);
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

            Run8BitLoadGroup(3, 11, opcode, value, PrimaryOpCode.HALT);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_n:
                    Assert.AreEqual(value, this.GpRegisters.Object.A);
                    this.GpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == value), Times.Once);
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
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_C_n:
                    Assert.AreEqual(value, this.GpRegisters.Object.C);
                    this.GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == value), Times.Once);
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_D_n:
                    Assert.AreEqual(value, this.GpRegisters.Object.D);
                    this.GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == value), Times.Once);
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_E_n:
                    Assert.AreEqual(value, this.GpRegisters.Object.E);
                    this.GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == value), Times.Once);
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_H_n:
                    Assert.AreEqual(value, this.GpRegisters.Object.H);
                    this.GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == value), Times.Once);
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_L_n:
                    Assert.AreEqual(value, this.GpRegisters.Object.L);
                    this.GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == value), Times.Once);
                    this.GpRegisters.Verify(x => x.A, Times.Never);
                    this.GpRegisters.Verify(x => x.B, Times.Never);
                    this.GpRegisters.Verify(x => x.C, Times.Never);
                    this.GpRegisters.Verify(x => x.D, Times.Never);
                    this.GpRegisters.Verify(x => x.E, Times.Never);
                    this.GpRegisters.Verify(x => x.H, Times.Never);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("opcode");
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

            Run8BitLoadGroup(3, 11, opcode, PrimaryOpCode.HALT);

            this.Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.GpRegisters.VerifyGet(x => x.HL, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_mHL:
                    Assert.AreEqual(Value, this.GpRegisters.Object.A);
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
            }

        }

        [TestCase(PrefixDdFdOpCode.LD_A_mIXYd)]
        [TestCase(PrefixDdFdOpCode.LD_B_mIXYd)]
        [TestCase(PrefixDdFdOpCode.LD_C_mIXYd)]
        [TestCase(PrefixDdFdOpCode.LD_D_mIXYd)]
        [TestCase(PrefixDdFdOpCode.LD_E_mIXYd)]
        [TestCase(PrefixDdFdOpCode.LD_H_mIXYd)]
        [TestCase(PrefixDdFdOpCode.LD_L_mIXYd)]
        public void LD_r_mIXd(PrefixDdFdOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte SignedD = -127;
            const byte UnsignedD = unchecked((byte)SignedD);
            const byte ValueAtIXd = 0x8f;

            this.Mmu.Setup(x => x.ReadByte(IX + SignedD)).Returns(ValueAtIXd);
            Run8BitLoadGroup(5 + 1, 19 + 4, PrimaryOpCode.Prefix_DD, opcode, UnsignedD, PrimaryOpCode.HALT);

            this.Mmu.Verify(x => x.ReadByte(IX + SignedD), Times.Once);
            this.Registers.VerifyGet(x => x.IX, Times.Once);

            switch (opcode)
            {
                case PrefixDdFdOpCode.LD_A_mIXYd:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.A);
                    break;
                case PrefixDdFdOpCode.LD_B_mIXYd:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.B);
                    break;
                case PrefixDdFdOpCode.LD_C_mIXYd:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.C);
                    break;
                case PrefixDdFdOpCode.LD_D_mIXYd:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.D);
                    break;
                case PrefixDdFdOpCode.LD_E_mIXYd:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.E);
                    break;
                case PrefixDdFdOpCode.LD_H_mIXYd:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.H);
                    break;
                case PrefixDdFdOpCode.LD_L_mIXYd:
                    Assert.AreEqual(ValueAtIXd, this.GpRegisters.Object.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("opcode");
            }
        }

        [TestCase(PrefixDdFdOpCode.LD_A_mIXYd)]
        [TestCase(PrefixDdFdOpCode.LD_B_mIXYd)]
        [TestCase(PrefixDdFdOpCode.LD_C_mIXYd)]
        [TestCase(PrefixDdFdOpCode.LD_D_mIXYd)]
        [TestCase(PrefixDdFdOpCode.LD_E_mIXYd)]
        [TestCase(PrefixDdFdOpCode.LD_H_mIXYd)]
        [TestCase(PrefixDdFdOpCode.LD_L_mIXYd)]
        public void LD_r_mIYd(PrefixDdFdOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte SignedD = 55;
            const byte UnsignedD = unchecked((byte)SignedD);
            const byte ValueAtIYd = 0x42;

            this.Mmu.Setup(x => x.ReadByte(IY + SignedD)).Returns(ValueAtIYd);

            Run8BitLoadGroup(5 + 1, 19 + 4, PrimaryOpCode.Prefix_FD, opcode, UnsignedD, PrimaryOpCode.HALT);

            this.Mmu.Verify(x => x.ReadByte(IY + SignedD), Times.Once);
            this.Registers.VerifyGet(x => x.IY, Times.Once);

            switch (opcode)
            {
                case PrefixDdFdOpCode.LD_A_mIXYd:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.A);
                    break;
                case PrefixDdFdOpCode.LD_B_mIXYd:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.B);
                    break;
                case PrefixDdFdOpCode.LD_C_mIXYd:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.C);
                    break;
                case PrefixDdFdOpCode.LD_D_mIXYd:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.D);
                    break;
                case PrefixDdFdOpCode.LD_E_mIXYd:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.E);
                    break;
                case PrefixDdFdOpCode.LD_H_mIXYd:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.H);
                    break;
                case PrefixDdFdOpCode.LD_L_mIXYd:
                    Assert.AreEqual(ValueAtIYd, this.GpRegisters.Object.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("opcode");
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

            Run8BitLoadGroup(2 + 1, 7 + 4, opcode, PrimaryOpCode.HALT);

            switch (opcode)
            {
                case PrimaryOpCode.LD_mHL_A:
                    this.Mmu.Verify(x => x.WriteByte(HL, A), Times.Once);
                    this.GpRegisters.VerifyGet(x => x.A, Times.Once);
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
            }
        }

        [TestCase(0x12)]
        [TestCase(0x34)]
        public void LD_mHL_n(byte value)
        {
            this.SetupRegisters();
            this.ResetMocks();

            Run8BitLoadGroup(3 + 1, 10 + 4, PrimaryOpCode.LD_mHL_n, value, PrimaryOpCode.HALT);
            
            this.Mmu.Verify(x => x.WriteByte(HL, value), Times.Once);
        }

        [TestCase(PrefixDdFdOpCode.LD_mIXYd_A)]
        [TestCase(PrefixDdFdOpCode.LD_mIXYd_B)]
        [TestCase(PrefixDdFdOpCode.LD_mIXYd_C)]
        [TestCase(PrefixDdFdOpCode.LD_mIXYd_D)]
        [TestCase(PrefixDdFdOpCode.LD_mIXYd_E)]
        [TestCase(PrefixDdFdOpCode.LD_mIXYd_H)]
        [TestCase(PrefixDdFdOpCode.LD_mIXYd_L)]
        public void LD_mIXd_r(PrefixDdFdOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte SignedD = -66;
            const byte UnsignedD = unchecked((byte)SignedD);
            
            Run8BitLoadGroup(5 + 1, 19 + 4, PrimaryOpCode.Prefix_DD, opcode, UnsignedD, PrimaryOpCode.HALT);
            
            this.Registers.VerifyGet(x => x.IX, Times.Once);

            switch (opcode)
            {
                case PrefixDdFdOpCode.LD_mIXYd_A:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, A), Times.Once);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_B:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, B), Times.Once);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_C:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, C), Times.Once);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_D:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, D), Times.Once);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_E:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, E), Times.Once);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_H:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, H), Times.Once);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_L:
                    this.Mmu.Verify(x => x.WriteByte(IX + SignedD, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("opcode");
            }
        }

        [TestCase(PrefixDdFdOpCode.LD_mIXYd_A)]
        [TestCase(PrefixDdFdOpCode.LD_mIXYd_B)]
        [TestCase(PrefixDdFdOpCode.LD_mIXYd_C)]
        [TestCase(PrefixDdFdOpCode.LD_mIXYd_D)]
        [TestCase(PrefixDdFdOpCode.LD_mIXYd_E)]
        [TestCase(PrefixDdFdOpCode.LD_mIXYd_H)]
        [TestCase(PrefixDdFdOpCode.LD_mIXYd_L)]
        public void LD_mIYd_r(PrefixDdFdOpCode opcode)
        {
            this.SetupRegisters();
            this.ResetMocks();

            const sbyte SignedD = 88;
            const byte UnsignedD = unchecked((byte)SignedD);

            Run8BitLoadGroup(5 + 1, 19 + 4, PrimaryOpCode.Prefix_FD, opcode, UnsignedD, PrimaryOpCode.HALT);

            this.Registers.VerifyGet(x => x.IY, Times.Once);

            switch (opcode)
            {
                case PrefixDdFdOpCode.LD_mIXYd_A:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, A), Times.Once);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_B:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, B), Times.Once);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_C:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, C), Times.Once);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_D:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, D), Times.Once);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_E:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, E), Times.Once);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_H:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, H), Times.Once);
                    break;
                case PrefixDdFdOpCode.LD_mIXYd_L:
                    this.Mmu.Verify(x => x.WriteByte(IY + SignedD, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("opcode");
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
            
            Run8BitLoadGroup(5 + 1, 19 + 4, PrimaryOpCode.Prefix_DD, PrefixDdFdOpCode.LD_mIXYd_n, UnsignedD, N, PrimaryOpCode.HALT);

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

            Run8BitLoadGroup(5 + 1, 19 + 4, PrimaryOpCode.Prefix_FD, PrefixDdFdOpCode.LD_mIXYd_n, UnsignedD, N, PrimaryOpCode.HALT);

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

            Run8BitLoadGroup(2 + 1, 7 + 4, PrimaryOpCode.LD_A_mBC, PrimaryOpCode.HALT);

            this.Mmu.Verify(x => x.ReadByte(BC), Times.Once);
            Assert.AreEqual(ValueAtBC, this.GpRegisters.Object.A);
        }

        [Test]
        public void LD_A_mDE()
        {
            this.SetupRegisters();
            this.ResetMocks();

            const byte ValueAtDE = 0x0c;

            this.Mmu.Setup(x => x.ReadByte(DE)).Returns(ValueAtDE);

            Run8BitLoadGroup(2 + 1, 7 + 4, PrimaryOpCode.LD_A_mDE, PrimaryOpCode.HALT);

            this.Mmu.Verify(x => x.ReadByte(DE), Times.Once);
            Assert.AreEqual(ValueAtDE, this.GpRegisters.Object.A);
        }

        private void Run8BitLoadGroup(int expectedMachineCycles, int expectedThrottlingStates, params object[] bytes)
        {
            this.SetCacheForSingleBytes(bytes);

            var block = this.BlockDecoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);

            Assert.AreEqual(expectedMachineCycles, block.MachineCycles);
            Assert.AreEqual(expectedThrottlingStates, block.ThrottlingStates);

            block.Action(this.Registers.Object, this.Mmu.Object);

            this.Cache.Verify(x => x.NextByte(), Times.Exactly(bytes.Length));

            // 8-bit load doesn't set the flags.
            this.GpRegisters.Verify(x => x.Flags, Times.Never);
        }

    }
}
