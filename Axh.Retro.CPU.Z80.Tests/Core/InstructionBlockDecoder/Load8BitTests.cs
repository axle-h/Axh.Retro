using System;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.OpCodes;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core.InstructionBlockDecoder
{
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
            SetupRegisters();
            ResetMocks();

            RunWithHalt(1, 4, opcode);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_A:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_A:
                    AfRegisters.Verify(x => x.A, Times.Once);
                    GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == A), Times.Once);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_A:
                    AfRegisters.Verify(x => x.A, Times.Once);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == A), Times.Once);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_A:
                    AfRegisters.Verify(x => x.A, Times.Once);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == A), Times.Once);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_A:
                    AfRegisters.Verify(x => x.A, Times.Once);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == A), Times.Once);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_A:
                    AfRegisters.Verify(x => x.A, Times.Once);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == A), Times.Once);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_A:
                    AfRegisters.Verify(x => x.A, Times.Once);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == A), Times.Once);
                    Assert.AreEqual(A, GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_B:
                    AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == B), Times.Once);
                    GpRegisters.Verify(x => x.B, Times.Once);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_B:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_B:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Once);
                    GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == B), Times.Once);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_B:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Once);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == B), Times.Once);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_B:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Once);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == B), Times.Once);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_B:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Once);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == B), Times.Once);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_B:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Once);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == B), Times.Once);
                    Assert.AreEqual(B, GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_C:
                    AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == C), Times.Once);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Once);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_C:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == C), Times.Once);
                    GpRegisters.Verify(x => x.C, Times.Once);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_C:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_C:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Once);
                    GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == C), Times.Once);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_C:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Once);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == C), Times.Once);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_C:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Once);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == C), Times.Once);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_C:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Once);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == C), Times.Once);
                    Assert.AreEqual(C, GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_D:
                    AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == D), Times.Once);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Once);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_D:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == D), Times.Once);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Once);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_D:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == D), Times.Once);
                    GpRegisters.Verify(x => x.D, Times.Once);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_D:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_D:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Once);
                    GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == D), Times.Once);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_D:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Once);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == D), Times.Once);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_D:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Once);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == D), Times.Once);
                    Assert.AreEqual(D, GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_E:
                    AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == E), Times.Once);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Once);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_E:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == E), Times.Once);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Once);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_E:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == E), Times.Once);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Once);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_E:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == E), Times.Once);
                    GpRegisters.Verify(x => x.E, Times.Once);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_E:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_E:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Once);
                    GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == E), Times.Once);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_E:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Once);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == E), Times.Once);
                    Assert.AreEqual(E, GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_H:
                    AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == H), Times.Once);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Once);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_H:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == H), Times.Once);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Once);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_H:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == H), Times.Once);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Once);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_H:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == H), Times.Once);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Once);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_H:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == H), Times.Once);
                    GpRegisters.Verify(x => x.H, Times.Once);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_H:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_H:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Once);
                    GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == H), Times.Once);
                    Assert.AreEqual(H, GpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_L:
                    AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == L), Times.Once);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_L:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == L), Times.Once);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_L:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == L), Times.Once);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_L:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == L), Times.Once);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_L:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == L), Times.Once);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_L:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == L), Times.Once);
                    GpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_L:
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(L, GpRegisters.Object.L);
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
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 7, opcode, value);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_n:
                    Assert.AreEqual(value, AfRegisters.Object.A);
                    AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == value), Times.Once);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_B_n:
                    Assert.AreEqual(value, GpRegisters.Object.B);
                    GpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == value), Times.Once);
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_C_n:
                    Assert.AreEqual(value, GpRegisters.Object.C);
                    GpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == value), Times.Once);
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_D_n:
                    Assert.AreEqual(value, GpRegisters.Object.D);
                    GpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == value), Times.Once);
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_E_n:
                    Assert.AreEqual(value, GpRegisters.Object.E);
                    GpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == value), Times.Once);
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_H_n:
                    Assert.AreEqual(value, GpRegisters.Object.H);
                    GpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == value), Times.Once);
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_L_n:
                    Assert.AreEqual(value, GpRegisters.Object.L);
                    GpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == value), Times.Once);
                    AfRegisters.Verify(x => x.A, Times.Never);
                    GpRegisters.Verify(x => x.B, Times.Never);
                    GpRegisters.Verify(x => x.C, Times.Never);
                    GpRegisters.Verify(x => x.D, Times.Never);
                    GpRegisters.Verify(x => x.E, Times.Never);
                    GpRegisters.Verify(x => x.H, Times.Never);
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
            SetupRegisters();
            ResetMocks();

            const byte Value = 0x8f;

            Mmu.Setup(x => x.ReadByte(HL)).Returns(Value);

            RunWithHalt(2, 7, opcode);

            Mmu.Verify(x => x.ReadByte(HL), Times.Once);
            GpRegisters.VerifyGet(x => x.HL, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_mHL:
                    Assert.AreEqual(Value, AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_mHL:
                    Assert.AreEqual(Value, GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_mHL:
                    Assert.AreEqual(Value, GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_mHL:
                    Assert.AreEqual(Value, GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_mHL:
                    Assert.AreEqual(Value, GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_mHL:
                    Assert.AreEqual(Value, GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_mHL:
                    Assert.AreEqual(Value, GpRegisters.Object.L);
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
            SetupRegisters();
            ResetMocks();

            const sbyte SignedD = -127;
            const byte UnsignedD = unchecked((byte) SignedD);
            const byte ValueAtIXd = 0x8f;

            Mmu.Setup(x => x.ReadByte(IX + SignedD)).Returns(ValueAtIXd);
            RunWithHalt(5, 19, PrimaryOpCode.Prefix_DD, opcode, UnsignedD);

            Mmu.Verify(x => x.ReadByte(IX + SignedD), Times.Once);
            Registers.VerifyGet(x => x.IX, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_mHL:
                    Assert.AreEqual(ValueAtIXd, AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_mHL:
                    Assert.AreEqual(ValueAtIXd, GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_mHL:
                    Assert.AreEqual(ValueAtIXd, GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_mHL:
                    Assert.AreEqual(ValueAtIXd, GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_mHL:
                    Assert.AreEqual(ValueAtIXd, GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_mHL:
                    Assert.AreEqual(ValueAtIXd, GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_mHL:
                    Assert.AreEqual(ValueAtIXd, GpRegisters.Object.L);
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
            SetupRegisters();
            ResetMocks();

            const sbyte SignedD = 55;
            const byte UnsignedD = unchecked((byte) SignedD);
            const byte ValueAtIYd = 0x42;

            Mmu.Setup(x => x.ReadByte(IY + SignedD)).Returns(ValueAtIYd);

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_FD, opcode, UnsignedD);

            Mmu.Verify(x => x.ReadByte(IY + SignedD), Times.Once);
            Registers.VerifyGet(x => x.IY, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_mHL:
                    Assert.AreEqual(ValueAtIYd, AfRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_mHL:
                    Assert.AreEqual(ValueAtIYd, GpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_mHL:
                    Assert.AreEqual(ValueAtIYd, GpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_mHL:
                    Assert.AreEqual(ValueAtIYd, GpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_mHL:
                    Assert.AreEqual(ValueAtIYd, GpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_mHL:
                    Assert.AreEqual(ValueAtIYd, GpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_mHL:
                    Assert.AreEqual(ValueAtIYd, GpRegisters.Object.L);
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
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 7, opcode);

            switch (opcode)
            {
                case PrimaryOpCode.LD_mHL_A:
                    Mmu.Verify(x => x.WriteByte(HL, A), Times.Once);
                    AfRegisters.VerifyGet(x => x.A, Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_B:
                    Mmu.Verify(x => x.WriteByte(HL, B), Times.Once);
                    GpRegisters.VerifyGet(x => x.B, Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_C:
                    Mmu.Verify(x => x.WriteByte(HL, C), Times.Once);
                    GpRegisters.VerifyGet(x => x.C, Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_D:
                    Mmu.Verify(x => x.WriteByte(HL, D), Times.Once);
                    GpRegisters.VerifyGet(x => x.D, Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_E:
                    Mmu.Verify(x => x.WriteByte(HL, E), Times.Once);
                    GpRegisters.VerifyGet(x => x.E, Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_H:
                    Mmu.Verify(x => x.WriteByte(HL, H), Times.Once);
                    GpRegisters.VerifyGet(x => x.H, Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_L:
                    Mmu.Verify(x => x.WriteByte(HL, L), Times.Once);
                    GpRegisters.VerifyGet(x => x.L, Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [TestCase(0x12)]
        [TestCase(0x34)]
        public void LD_mHL_n(byte value)
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(3, 10, PrimaryOpCode.LD_mHL_n, value);

            Mmu.Verify(x => x.WriteByte(HL, value), Times.Once);
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
            SetupRegisters();
            ResetMocks();

            const sbyte SignedD = -66;
            const byte UnsignedD = unchecked((byte) SignedD);

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_DD, opcode, UnsignedD);

            Registers.VerifyGet(x => x.IX, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_mHL_A:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, A), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_B:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, B), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_C:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, C), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_D:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, D), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_E:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, E), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_H:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, H), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_L:
                    Mmu.Verify(x => x.WriteByte(IX + SignedD, L), Times.Once);
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
            SetupRegisters();
            ResetMocks();

            const sbyte SignedD = 88;
            const byte UnsignedD = unchecked((byte) SignedD);

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_FD, opcode, UnsignedD);

            Registers.VerifyGet(x => x.IY, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_mHL_A:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, A), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_B:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, B), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_C:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, C), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_D:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, D), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_E:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, E), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_H:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, H), Times.Once);
                    break;
                case PrimaryOpCode.LD_mHL_L:
                    Mmu.Verify(x => x.WriteByte(IY + SignedD, L), Times.Once);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode));
            }
        }

        [Test]
        public void LD_A_I()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 9, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LD_A_I);

            AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == I), Times.Once);
            Assert.AreEqual(I, AfRegisters.Object.A);

            // Check flags
            FlagsRegister.Verify(x => x.SetResultFlags(It.Is<byte>(y => y == I)), Times.Once);
            FlagsRegister.VerifySet(x => x.HalfCarry = It.Is<bool>(y => !y), Times.Once);
            FlagsRegister.VerifySet(x => x.ParityOverflow = IFF2, Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = It.Is<bool>(y => !y), Times.Once);
        }

        [Test]
        public void LD_A_mBC()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtBC = 0x61;

            Mmu.Setup(x => x.ReadByte(BC)).Returns(ValueAtBC);

            RunWithHalt(2, 7, PrimaryOpCode.LD_A_mBC);

            Mmu.Verify(x => x.ReadByte(BC), Times.Once);
            Assert.AreEqual(ValueAtBC, AfRegisters.Object.A);
        }

        [Test]
        public void LD_A_mDE()
        {
            SetupRegisters();
            ResetMocks();

            const byte ValueAtDE = 0x0c;

            Mmu.Setup(x => x.ReadByte(DE)).Returns(ValueAtDE);

            RunWithHalt(2, 7, PrimaryOpCode.LD_A_mDE);

            Mmu.Verify(x => x.ReadByte(DE), Times.Once);
            Assert.AreEqual(ValueAtDE, AfRegisters.Object.A);
        }

        [Test]
        public void LD_A_mnn()
        {
            SetupRegisters();
            ResetMocks();

            const ushort NN = 0x827d;
            const byte ValueAtNN = 0xcf;

            Mmu.Setup(x => x.ReadByte(NN)).Returns(ValueAtNN);

            RunWithHalt(4, 13, PrimaryOpCode.LD_A_mnn, NN);

            Mmu.Verify(x => x.ReadByte(NN), Times.Once);
            Assert.AreEqual(ValueAtNN, AfRegisters.Object.A);
        }

        [Test]
        public void LD_A_R()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 9, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LD_A_R);

            AfRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == R), Times.Once);
            Assert.AreEqual(R, AfRegisters.Object.A);

            // Check flags
            FlagsRegister.Verify(x => x.SetResultFlags(It.Is<byte>(y => y == R)), Times.Once);
            FlagsRegister.VerifySet(x => x.HalfCarry = It.Is<bool>(y => !y), Times.Once);
            FlagsRegister.VerifySet(x => x.ParityOverflow = IFF2, Times.Once);
            FlagsRegister.VerifySet(x => x.Subtract = It.Is<bool>(y => !y), Times.Once);
        }

        [Test]
        public void LD_I_A()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 9, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LD_I_A);

            Registers.VerifySet(x => x.I = It.Is<byte>(y => y == A), Times.Once);
            Assert.AreEqual(A, Registers.Object.I);
        }

        [Test]
        public void LD_mBC_A()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 7, PrimaryOpCode.LD_mBC_A);

            Mmu.Verify(x => x.WriteByte(BC, A), Times.Once);
            AfRegisters.VerifyGet(x => x.A, Times.Once);
        }

        [Test]
        public void LD_mDE_A()
        {
            SetupRegisters();
            ResetMocks();

            RunWithHalt(2, 7, PrimaryOpCode.LD_mDE_A);

            Mmu.Verify(x => x.WriteByte(DE, A), Times.Once);
            AfRegisters.VerifyGet(x => x.A, Times.Once);
        }

        [Test]
        public void LD_mIXd_n()
        {
            SetupRegisters();
            ResetMocks();

            const sbyte SignedD = 77;
            const byte UnsignedD = unchecked((byte) SignedD);
            const byte N = 0x73;

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_DD, PrimaryOpCode.LD_mHL_n, UnsignedD, N);

            Registers.VerifyGet(x => x.IX, Times.Once);
            Mmu.Verify(x => x.WriteByte(IX + SignedD, N), Times.Once);
        }

        [Test]
        public void LD_mIYd_n()
        {
            SetupRegisters();
            ResetMocks();

            const sbyte SignedD = -77;
            const byte UnsignedD = unchecked((byte) SignedD);
            const byte N = 0xF6;

            RunWithHalt(5, 19, PrimaryOpCode.Prefix_FD, PrimaryOpCode.LD_mHL_n, UnsignedD, N);

            Registers.VerifyGet(x => x.IY, Times.Once);
            Mmu.Verify(x => x.WriteByte(IY + SignedD, N), Times.Once);
        }

        [Test]
        public void LD_mnn_A()
        {
            SetupRegisters();
            ResetMocks();
            const ushort N = 0x613b;

            RunWithHalt(4, 13, PrimaryOpCode.LD_mnn_A, N);

            Mmu.Verify(x => x.WriteByte(N, A), Times.Once);
            AfRegisters.VerifyGet(x => x.A, Times.Once);
        }

        [Test]
        public void LD_R_A()
        {
            SetupRegisters();
            ResetMocks();

            // This is not all it seems.
            // 1. A is loaded to R
            // 2. LS 7-bytes of R are incremented 3 times for the 3 opcodes executed.
            // Note: this is not cycle accurate. R should only be incremented twice as Prefix_ED has already been run. However, it really doesn't matter. 
            const int ExprectedR = ((A + 3) & 0x7f) | ((A & 0x80) >> 8);

            RunWithHalt(2, 9, PrimaryOpCode.Prefix_ED, PrefixEdOpCode.LD_R_A);

            Registers.VerifySet(x => x.R = It.Is<byte>(y => y == A), Times.Once);

            Assert.AreEqual(ExprectedR, Registers.Object.R);
        }
    }
}