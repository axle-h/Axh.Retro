namespace Axh.Emulation.CPU.X80.Tests.Core
{
    using System;

    using Axh.Emulation.CPU.X80.Contracts.OpCodes;

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

            this.cache.SetupSequence(x => x.NextByte()).Returns((byte)opcode).Returns((byte)PrimaryOpCode.HALT);

            var block = this.decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);

            Assert.AreEqual(2, block.MachineCycles);
            Assert.AreEqual(8, block.ThrottlingStates);

            block.Action(this.registers.Object, this.mmu.Object);

            this.cache.Verify(x => x.NextByte(), Times.Exactly(2));

            // 8-bit load doesn't set the flags.
            this.gpRegisters.Verify(x => x.Flags, Times.Never);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_A:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, gpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_A:
                    this.gpRegisters.Verify(x => x.A, Times.Once);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == A), Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, gpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_A:
                    this.gpRegisters.Verify(x => x.A, Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == A), Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, gpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_A:
                    this.gpRegisters.Verify(x => x.A, Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == A), Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, gpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_A:
                    this.gpRegisters.Verify(x => x.A, Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == A), Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, gpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_A:
                    this.gpRegisters.Verify(x => x.A, Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == A), Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(A, gpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_A:
                    this.gpRegisters.Verify(x => x.A, Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == A), Times.Once);
                    Assert.AreEqual(A, gpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_B:
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == B), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, gpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_B:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, gpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_B:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Once);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == B), Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, gpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_B:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == B), Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, gpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_B:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == B), Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, gpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_B:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == B), Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(B, gpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_B:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == B), Times.Once);
                    Assert.AreEqual(B, gpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_C:
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == C), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, gpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_C:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == C), Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, gpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_C:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, gpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_C:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Once);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == C), Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, gpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_C:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == C), Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, gpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_C:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == C), Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(C, gpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_C:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == C), Times.Once);
                    Assert.AreEqual(C, gpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_D:
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == D), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, gpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_D:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == D), Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, gpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_D:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == D), Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, gpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_D:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, gpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_D:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Once);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == D), Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, gpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_D:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == D), Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(D, gpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_D:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == D), Times.Once);
                    Assert.AreEqual(D, gpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_E:
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == E), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, gpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_E:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == E), Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, gpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_E:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == E), Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, gpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_E:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == E), Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, gpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_E:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, gpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_E:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Once);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == E), Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(E, gpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_E:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == E), Times.Once);
                    Assert.AreEqual(E, gpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_H:
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == H), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, gpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_H:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == H), Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, gpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_H:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == H), Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, gpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_H:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == H), Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, gpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_H:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == H), Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, gpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_H:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(H, gpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_H:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Once);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == H), Times.Once);
                    Assert.AreEqual(H, gpRegisters.Object.L);
                    break;
                case PrimaryOpCode.LD_A_L:
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == L), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, gpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_L:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == L), Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, gpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_L:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == L), Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, gpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_L:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == L), Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, gpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_L:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == L), Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, gpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_L:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == L), Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Once);
                    Assert.AreEqual(L, gpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_L:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    Assert.AreEqual(L, gpRegisters.Object.L);
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

            this.cache.SetupSequence(x => x.NextByte()).Returns((byte)opcode).Returns(value).Returns((byte)PrimaryOpCode.HALT);

            var block = this.decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);

            Assert.AreEqual(3, block.MachineCycles);
            Assert.AreEqual(11, block.ThrottlingStates);

            block.Action(this.registers.Object, this.mmu.Object);

            this.cache.Verify(x => x.NextByte(), Times.Exactly(3));

            // 8-bit load doesn't set the flags.
            this.gpRegisters.Verify(x => x.Flags, Times.Never);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_n:
                    Assert.AreEqual(value, gpRegisters.Object.A);
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == value), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_B_n:
                    Assert.AreEqual(value, gpRegisters.Object.B);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == value), Times.Once);
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_C_n:
                    Assert.AreEqual(value, gpRegisters.Object.C);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == value), Times.Once);
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_D_n:
                    Assert.AreEqual(value, gpRegisters.Object.D);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == value), Times.Once);
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_E_n:
                    Assert.AreEqual(value, gpRegisters.Object.E);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == value), Times.Once);
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_H_n:
                    Assert.AreEqual(value, gpRegisters.Object.H);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == value), Times.Once);
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_L_n:
                    Assert.AreEqual(value, gpRegisters.Object.L);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == value), Times.Once);
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
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

            this.mmu.Setup(x => x.ReadByte(HL)).Returns(Value);
            this.cache.SetupSequence(x => x.NextByte()).Returns((byte)opcode).Returns((byte)PrimaryOpCode.HALT);

            var block = this.decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);

            Assert.AreEqual(3, block.MachineCycles);
            Assert.AreEqual(11, block.ThrottlingStates);

            block.Action(this.registers.Object, this.mmu.Object);

            this.cache.Verify(x => x.NextByte(), Times.Exactly(2));

            // 8-bit load doesn't set the flags.
            this.gpRegisters.Verify(x => x.Flags, Times.Never);

            this.mmu.Verify(x => x.ReadByte(HL), Times.Once);
            this.gpRegisters.VerifyGet(x => x.HL, Times.Once);

            switch (opcode)
            {
                case PrimaryOpCode.LD_A_mHL:
                    Assert.AreEqual(Value, this.gpRegisters.Object.A);
                    break;
                case PrimaryOpCode.LD_B_mHL:
                    Assert.AreEqual(Value, this.gpRegisters.Object.B);
                    break;
                case PrimaryOpCode.LD_C_mHL:
                    Assert.AreEqual(Value, this.gpRegisters.Object.C);
                    break;
                case PrimaryOpCode.LD_D_mHL:
                    Assert.AreEqual(Value, this.gpRegisters.Object.D);
                    break;
                case PrimaryOpCode.LD_E_mHL:
                    Assert.AreEqual(Value, this.gpRegisters.Object.E);
                    break;
                case PrimaryOpCode.LD_H_mHL:
                    Assert.AreEqual(Value, this.gpRegisters.Object.H);
                    break;
                case PrimaryOpCode.LD_L_mHL:
                    Assert.AreEqual(Value, this.gpRegisters.Object.L);
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

            const sbyte d = -127;
            const byte db = unchecked((byte)d);
            const byte Value = 0x8f;

            this.mmu.Setup(x => x.ReadByte(IX + d)).Returns(Value);
            this.cache.SetupSequence(x => x.NextByte()).Returns((byte)PrimaryOpCode.Prefix_DD).Returns((byte)opcode).Returns(db).Returns((byte)PrimaryOpCode.HALT);

            var block = this.decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);

            Assert.AreEqual(5 + 1, block.MachineCycles);
            Assert.AreEqual(19 + 4, block.ThrottlingStates);

            block.Action(this.registers.Object, this.mmu.Object);

            this.cache.Verify(x => x.NextByte(), Times.Exactly(4));

            // 8-bit load doesn't set the flags.
            this.gpRegisters.Verify(x => x.Flags, Times.Never);

            this.mmu.Verify(x => x.ReadByte(IX + d), Times.Once);
            this.registers.VerifyGet(x => x.IX, Times.Once);
            
            switch (opcode)
            {
                case PrefixDdFdOpCode.LD_A_mIXYd:
                    Assert.AreEqual(Value, this.gpRegisters.Object.A);
                    break;
                case PrefixDdFdOpCode.LD_B_mIXYd:
                    Assert.AreEqual(Value, this.gpRegisters.Object.B);
                    break;
                case PrefixDdFdOpCode.LD_C_mIXYd:
                    Assert.AreEqual(Value, this.gpRegisters.Object.C);
                    break;
                case PrefixDdFdOpCode.LD_D_mIXYd:
                    Assert.AreEqual(Value, this.gpRegisters.Object.D);
                    break;
                case PrefixDdFdOpCode.LD_E_mIXYd:
                    Assert.AreEqual(Value, this.gpRegisters.Object.E);
                    break;
                case PrefixDdFdOpCode.LD_H_mIXYd:
                    Assert.AreEqual(Value, this.gpRegisters.Object.H);
                    break;
                case PrefixDdFdOpCode.LD_L_mIXYd:
                    Assert.AreEqual(Value, this.gpRegisters.Object.L);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("opcode");
            }
        }
    }
}
