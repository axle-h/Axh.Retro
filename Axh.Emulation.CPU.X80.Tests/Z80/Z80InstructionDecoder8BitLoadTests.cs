namespace Axh.Emulation.CPU.X80.Tests.Z80
{
    using System;

    using Axh.Emulation.CPU.X80.Contracts.Factories;
    using Axh.Emulation.CPU.X80.Contracts.Memory;
    using Axh.Emulation.CPU.X80.Contracts.OpCodes;
    using Axh.Emulation.CPU.X80.Contracts.Registers;
    using Axh.Emulation.CPU.X80.Contracts.Core;
    using Axh.Emulation.CPU.X80.Core;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class Z80InstructionDecoder8BitLoadTests
    {
        private const ushort Address = 0x0000;

        private const byte A = 0xaa;
        private const byte B = 0xbb;
        private const byte C = 0xcc;
        private const byte D = 0xdd;
        private const byte E = 0xee;
        private const byte H = 0x44;
        private const byte L = 0x77;

        private IZ80InstructionDecoder decoder;

        private Mock<IZ80Registers> registers;

        private Mock<IMmu> mmu;

        private Mock<IGeneralPurposeRegisterSet> gpRegisters;

        private Mock<IMmuCache> cache;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            registers = new Mock<IZ80Registers>();
            gpRegisters = new Mock<IGeneralPurposeRegisterSet>();
            registers.Setup(x => x.GeneralPurposeRegisters).Returns(gpRegisters.Object);
            gpRegisters.Setup(x => x.A).Returns(A);
            gpRegisters.Setup(x => x.B).Returns(B);
            gpRegisters.Setup(x => x.C).Returns(C);
            gpRegisters.Setup(x => x.D).Returns(D);
            gpRegisters.Setup(x => x.E).Returns(E);
            gpRegisters.Setup(x => x.H).Returns(H);
            gpRegisters.Setup(x => x.L).Returns(L);

            mmu = new Mock<IMmu>();

            cache = new Mock<IMmuCache>();

            var mmuFactory = new Mock<IMmuFactory>();
            mmuFactory.Setup(x => x.GetMmuCache(mmu.Object, Address)).Returns(cache.Object);

            decoder = new Z80InstructionDecoder(mmuFactory.Object, mmu.Object);
            
        }

        [Test]
        public void HaltIncrentsProgramCounterAndMemoryRefreshRegisters()
        {
            cache.Setup(x => x.GetNextByte()).Returns((byte)PrimaryOpCode.HALT);
            var block = decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);

            registers.ResetCalls();
            mmu.ResetCalls();

            registers.Setup(x => x.R).Returns(0x5f);
            registers.Setup(x => x.ProgramCounter).Returns(0x1234);

            block.Action(registers.Object, mmu.Object);
            
            registers.VerifySet(x => x.R = It.Is<byte>(y => y == 0x60), Times.Once);
            registers.VerifySet(x => x.ProgramCounter = It.Is<ushort>(y => y == 0x1235), Times.Once);
        }

        [Test]
        public void NopIncrentsProgramCounterAndMemoryRefreshRegistersWithCorrectOverflow()
        {
            cache.Setup(x => x.GetNextByte()).Returns((byte)PrimaryOpCode.HALT);
            var block = decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);

            registers.ResetCalls();
            mmu.ResetCalls();

            registers.Setup(x => x.R).Returns(0x7f);
            registers.Setup(x => x.ProgramCounter).Returns(0xffff);

            block.Action(registers.Object, mmu.Object);

            registers.VerifySet(x => x.R = It.Is<byte>(y => y == 0x00), Times.Once);
            registers.VerifySet(x => x.ProgramCounter = It.Is<ushort>(y => y == 0x0000), Times.Once);
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
        public void Test8BitLoadOperations(PrimaryOpCode opcode)
        {
            registers.ResetCalls();
            gpRegisters.ResetCalls();

            cache.SetupSequence(x => x.GetNextByte()).Returns((byte)opcode).Returns((byte)PrimaryOpCode.HALT);

            var block = decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);

            block.Action(registers.Object, mmu.Object);

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
                    break;
                case PrimaryOpCode.LD_B_A:
                    this.gpRegisters.Verify(x => x.A, Times.Once);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == A), Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_C_A:
                    this.gpRegisters.Verify(x => x.A, Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == A), Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_D_A:
                    this.gpRegisters.Verify(x => x.A, Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == A), Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_E_A:
                    this.gpRegisters.Verify(x => x.A, Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == A), Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_H_A:
                    this.gpRegisters.Verify(x => x.A, Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == A), Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_L_A:
                    this.gpRegisters.Verify(x => x.A, Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == A), Times.Once);
                    break;
                case PrimaryOpCode.LD_A_B:
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == B), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_B_B:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_C_B:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Once);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == B), Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_D_B:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == B), Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_E_B:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == B), Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_H_B:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == B), Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_L_B:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == B), Times.Once);
                    break;
                case PrimaryOpCode.LD_A_C:
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == C), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_B_C:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == C), Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_C_C:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_D_C:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Once);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == C), Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_E_C:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == C), Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_H_C:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == C), Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_L_C:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == C), Times.Once);
                    break;
                case PrimaryOpCode.LD_A_D:
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == D), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_B_D:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == D), Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_C_D:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == D), Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_D_D:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_E_D:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Once);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == D), Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_H_D:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == D), Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_L_D:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == D), Times.Once);
                    break;
                case PrimaryOpCode.LD_A_E:
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == E), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_B_E:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == E), Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_C_E:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == E), Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_D_E:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == E), Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_E_E:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_H_E:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Once);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == E), Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_L_E:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == E), Times.Once);
                    break;
                case PrimaryOpCode.LD_A_H:
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == H), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_B_H:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == H), Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_C_H:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == H), Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_D_H:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == H), Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_E_H:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == H), Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_H_H:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;
                case PrimaryOpCode.LD_L_H:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Once);
                    this.gpRegisters.VerifySet(x => x.L = It.Is<byte>(y => y == H), Times.Once);
                    break;
                case PrimaryOpCode.LD_A_L:
                    this.gpRegisters.VerifySet(x => x.A = It.Is<byte>(y => y == L), Times.Once);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Once);
                    break;
                case PrimaryOpCode.LD_B_L:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.VerifySet(x => x.B = It.Is<byte>(y => y == L), Times.Once);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Once);
                    break;
                case PrimaryOpCode.LD_C_L:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.VerifySet(x => x.C = It.Is<byte>(y => y == L), Times.Once);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Once);
                    break;
                case PrimaryOpCode.LD_D_L:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.VerifySet(x => x.D = It.Is<byte>(y => y == L), Times.Once);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Once);
                    break;
                case PrimaryOpCode.LD_E_L:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.VerifySet(x => x.E = It.Is<byte>(y => y == L), Times.Once);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Once);
                    break;
                case PrimaryOpCode.LD_H_L:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.VerifySet(x => x.H = It.Is<byte>(y => y == L), Times.Once);
                    this.gpRegisters.Verify(x => x.L, Times.Once);
                    break;
                case PrimaryOpCode.LD_L_L:
                    this.gpRegisters.Verify(x => x.A, Times.Never);
                    this.gpRegisters.Verify(x => x.B, Times.Never);
                    this.gpRegisters.Verify(x => x.C, Times.Never);
                    this.gpRegisters.Verify(x => x.D, Times.Never);
                    this.gpRegisters.Verify(x => x.E, Times.Never);
                    this.gpRegisters.Verify(x => x.H, Times.Never);
                    this.gpRegisters.Verify(x => x.L, Times.Never);
                    break;

                default:
                    throw new NotImplementedException(opcode.ToString());
            }

        }


    }
}
