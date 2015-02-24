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
            SetupRegisters();

            mmu = new Mock<IMmu>();

            cache = new Mock<IMmuCache>();

            var mmuFactory = new Mock<IMmuFactory>();
            mmuFactory.Setup(x => x.GetMmuCache(mmu.Object, Address)).Returns(cache.Object);

            decoder = new Z80InstructionDecoder(mmuFactory.Object, mmu.Object);
        }

        private void SetupRegisters()
        {
            gpRegisters.SetupProperty(x => x.A, A);
            gpRegisters.SetupProperty(x => x.B, B);
            gpRegisters.SetupProperty(x => x.C, C);
            gpRegisters.SetupProperty(x => x.D, D);
            gpRegisters.SetupProperty(x => x.E, E);
            gpRegisters.SetupProperty(x => x.H, H);
            gpRegisters.SetupProperty(x => x.L, L);
        }

        private void ResetMocks()
        {
            registers.ResetCalls();
            cache.ResetCalls();
            mmu.ResetCalls();
            gpRegisters.ResetCalls();
        }


        [Test]
        public void HaltIncrentsProgramCounterAndMemoryRefreshRegisters()
        {
            ResetMocks();

            cache.SetupSequence(x => x.GetNextByte()).Returns((byte)PrimaryOpCode.NOP).Returns((byte)PrimaryOpCode.NOP).Returns((byte)PrimaryOpCode.HALT);
            var block = decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);
            
            registers.SetupProperty(x => x.R, (byte)0x5f);
            registers.SetupProperty(x => x.ProgramCounter, (ushort)0x1234);

            block.Action(registers.Object, mmu.Object);
            
            cache.Verify(x => x.GetNextByte(), Times.Exactly(3));
            Assert.AreEqual(0x62, registers.Object.R);
            Assert.AreEqual(0x1237, registers.Object.ProgramCounter);
        }

        [Test]
        public void NopIncrentsProgramCounterAndMemoryRefreshRegistersWithCorrectOverflow()
        {
            ResetMocks();

            cache.Setup(x => x.GetNextByte()).Returns((byte)PrimaryOpCode.HALT);
            var block = decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);

            registers.SetupProperty(x => x.R, (byte)0x7f);
            registers.SetupProperty(x => x.ProgramCounter, (ushort)0xffff);
            
            block.Action(registers.Object, mmu.Object);

            cache.Verify(x => x.GetNextByte(), Times.Once);
            Assert.AreEqual(0x00, registers.Object.R);
            Assert.AreEqual(0x0000, registers.Object.ProgramCounter);
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
            SetupRegisters();
            ResetMocks();

            cache.SetupSequence(x => x.GetNextByte()).Returns((byte)opcode).Returns((byte)PrimaryOpCode.HALT);

            var block = decoder.DecodeNextBlock(Address);
            Assert.IsNotNull(block);

            block.Action(registers.Object, mmu.Object);

            cache.Verify(x => x.GetNextByte(), Times.Exactly(2));

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
