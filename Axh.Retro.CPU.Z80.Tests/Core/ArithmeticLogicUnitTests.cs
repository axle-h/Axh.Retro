using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Core;
using Moq;
using NUnit.Framework;

namespace Axh.Retro.CPU.Z80.Tests.Core
{
    [TestFixture]
    public class ArithmeticLogicUnitTests
    {
        private Mock<IFlagsRegister> _flags;

        private Alu<IZ80Registers> _alu;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _flags = new Mock<IFlagsRegister>();
            var registers = new Mock<IZ80Registers>();
            var accumulatorAndFlagsRegisterSet = new Mock<IAccumulatorAndFlagsRegisterSet>();

            registers.Setup(x => x.AccumulatorAndFlagsRegisters).Returns(accumulatorAndFlagsRegisterSet.Object);
            accumulatorAndFlagsRegisterSet.Setup(x => x.Flags).Returns(_flags.Object);

            _alu = new Alu<IZ80Registers>(registers.Object);
        }

        private void Reset(bool carry = false)
        {
            _flags.ResetCalls();
            _flags.SetupProperty(x => x.Sign, false);
            _flags.SetupProperty(x => x.Zero, false);
            _flags.SetupProperty(x => x.Flag5, false);
            _flags.SetupProperty(x => x.HalfCarry, false);
            _flags.SetupProperty(x => x.ParityOverflow, false);
            _flags.SetupProperty(x => x.Flag3, false);
            _flags.SetupProperty(x => x.Subtract, false);
            _flags.SetupProperty(x => x.Carry, carry);
        }

        [TestCase(0x4e, 0x4f, false, false)]
        [TestCase(0x0f, 0x10, true, false)]
        [TestCase(0x7f, 0x80, true, true)]
        public void Increment(byte a, byte expected, bool halfCarry, bool overflow)
        {
            Reset();
            var result = _alu.Increment(a);

            Assert.AreEqual(expected, result);

            AssertFlags(result, null, null, halfCarry, overflow, false, null);
        }

        [TestCase(0x4e, 0x4d, false, false)]
        [TestCase(0xf0, 0xef, true, false)]
        [TestCase(0x80, 0x7f, true, true)]
        public void Decrement(byte a, byte expected, bool halfCarry, bool overflow)
        {
            Reset();

            var result = _alu.Decrement(a);

            Assert.AreEqual(expected, result);

            AssertFlags(result, null, null, halfCarry, overflow, true, null);
        }

        [TestCase(0, 0, 0, false, false, false)]
        [TestCase(0, 1, 1, false, false, false)]
        [TestCase(0, 127, 127, false, false, false)]
        [TestCase(0, 128, 128, false, false, false)]
        [TestCase(0, 129, 129, false, false, false)]
        [TestCase(0, 255, 255, false, false, false)]
        [TestCase(1, 0, 1, false, false, false)]
        [TestCase(1, 1, 2, false, false, false)]
        [TestCase(1, 127, 128, true, false, true)]
        [TestCase(1, 128, 129, false, false, false)]
        [TestCase(1, 129, 130, false, false, false)]
        [TestCase(1, 255, 0, true, true, false)]
        [TestCase(127, 0, 127, false, false, false)]
        [TestCase(127, 1, 128, true, false, true)]
        [TestCase(127, 127, 254, true, false, true)]
        [TestCase(127, 128, 255, false, false, false)]
        [TestCase(127, 129, 0, true, true, false)]
        [TestCase(127, 255, 126, true, true, false)]
        [TestCase(128, 0, 128, false, false, false)]
        [TestCase(128, 1, 129, false, false, false)]
        [TestCase(128, 127, 255, false, false, false)]
        [TestCase(128, 128, 0, false, true, true)]
        [TestCase(128, 129, 1, false, true, true)]
        [TestCase(128, 255, 127, false, true, true)]
        [TestCase(129, 0, 129, false, false, false)]
        [TestCase(129, 1, 130, false, false, false)]
        [TestCase(129, 127, 0, true, true, false)]
        [TestCase(129, 128, 1, false, true, true)]
        [TestCase(129, 129, 2, false, true, true)]
        [TestCase(129, 255, 128, true, true, false)]
        [TestCase(255, 0, 255, false, false, false)]
        [TestCase(255, 1, 0, true, true, false)]
        [TestCase(255, 127, 126, true, true, false)]
        [TestCase(255, 128, 127, false, true, true)]
        [TestCase(255, 129, 128, true, true, false)]
        [TestCase(255, 255, 254, true, true, false)]
        public void Add(byte a, byte b, byte expected, bool halfCarry, bool carry, bool overflow)
        {
            Reset();

            var result = _alu.Add(a, b);

            Assert.AreEqual(expected, result);

            AssertFlags(result, null, null, halfCarry, overflow, false, carry);
        }

        [TestCase(0, 0, 1, false, false, false)]
        [TestCase(0, 1, 2, false, false, false)]
        [TestCase(0, 127, 128, true, false, true)]
        [TestCase(0, 128, 129, false, false, false)]
        [TestCase(0, 129, 130, false, false, false)]
        [TestCase(0, 255, 0, true, true, false)]
        [TestCase(1, 0, 2, false, false, false)]
        [TestCase(1, 1, 3, false, false, false)]
        [TestCase(1, 127, 129, true, false, true)]
        [TestCase(1, 128, 130, false, false, false)]
        [TestCase(1, 129, 131, false, false, false)]
        [TestCase(1, 255, 1, true, true, false)]
        [TestCase(127, 0, 128, true, false, true)]
        [TestCase(127, 1, 129, true, false, true)]
        [TestCase(127, 127, 255, true, false, true)]
        [TestCase(127, 128, 0, true, true, false)]
        [TestCase(127, 129, 1, true, true, false)]
        [TestCase(127, 255, 127, true, true, false)]
        [TestCase(128, 0, 129, false, false, false)]
        [TestCase(128, 1, 130, false, false, false)]
        [TestCase(128, 127, 0, true, true, false)]
        [TestCase(128, 128, 1, false, true, true)]
        [TestCase(128, 129, 2, false, true, true)]
        [TestCase(128, 255, 128, true, true, false)]
        [TestCase(129, 0, 130, false, false, false)]
        [TestCase(129, 1, 131, false, false, false)]
        [TestCase(129, 127, 1, true, true, false)]
        [TestCase(129, 128, 2, false, true, true)]
        [TestCase(129, 129, 3, false, true, true)]
        [TestCase(129, 255, 129, true, true, false)]
        [TestCase(255, 0, 0, true, true, false)]
        [TestCase(255, 1, 1, true, true, false)]
        [TestCase(255, 127, 127, true, true, false)]
        [TestCase(255, 128, 128, true, true, false)]
        [TestCase(255, 129, 129, true, true, false)]
        [TestCase(255, 255, 255, true, true, false)]
        public void AddWithCarry(byte a, byte b, byte expected, bool halfCarry, bool carry, bool overflow)
        {
            Reset();

            _flags.SetupProperty(x => x.Carry, true);

            var result = _alu.AddWithCarry(a, b);

            Assert.AreEqual(expected, result);

            AssertFlags(result, null, null, halfCarry, overflow, false, carry);
        }

        [TestCase(0, 0, 0, false, false, false)]
        [TestCase(0, 1, 255, true, true, false)]
        [TestCase(0, 127, 129, true, true, false)]
        [TestCase(0, 128, 128, false, true, true)]
        [TestCase(0, 129, 127, true, true, false)]
        [TestCase(0, 255, 1, true, true, false)]
        [TestCase(1, 0, 1, false, false, false)]
        [TestCase(1, 1, 0, false, false, false)]
        [TestCase(1, 127, 130, true, true, false)]
        [TestCase(1, 128, 129, false, true, true)]
        [TestCase(1, 129, 128, false, true, true)]
        [TestCase(1, 255, 2, true, true, false)]
        [TestCase(127, 0, 127, false, false, false)]
        [TestCase(127, 1, 126, false, false, false)]
        [TestCase(127, 127, 0, false, false, false)]
        [TestCase(127, 128, 255, false, true, true)]
        [TestCase(127, 129, 254, false, true, true)]
        [TestCase(127, 255, 128, false, true, true)]
        [TestCase(128, 0, 128, false, false, false)]
        [TestCase(128, 1, 127, true, false, true)]
        [TestCase(128, 127, 1, true, false, true)]
        [TestCase(128, 128, 0, false, false, false)]
        [TestCase(128, 129, 255, true, true, false)]
        [TestCase(128, 255, 129, true, true, false)]
        [TestCase(129, 0, 129, false, false, false)]
        [TestCase(129, 1, 128, false, false, false)]
        [TestCase(129, 127, 2, true, false, true)]
        [TestCase(129, 128, 1, false, false, false)]
        [TestCase(129, 129, 0, false, false, false)]
        [TestCase(129, 255, 130, true, true, false)]
        [TestCase(255, 0, 255, false, false, false)]
        [TestCase(255, 1, 254, false, false, false)]
        [TestCase(255, 127, 128, false, false, false)]
        [TestCase(255, 128, 127, false, false, false)]
        [TestCase(255, 129, 126, false, false, false)]
        [TestCase(255, 255, 0, false, false, false)]
        public void Subtract(byte a, byte b, byte expected, bool halfCarry, bool carry, bool overflow)
        {
            Reset();

            var result = _alu.Subtract(a, b);

            Assert.AreEqual(expected, result);

            AssertFlags(result, null, null, halfCarry, overflow, true, carry);
        }

        [TestCase(0, 0, 255, true, true, false)]
        [TestCase(0, 1, 254, true, true, false)]
        [TestCase(0, 127, 128, true, true, false)]
        [TestCase(0, 128, 127, true, true, false)]
        [TestCase(0, 129, 126, true, true, false)]
        [TestCase(0, 255, 0, true, true, false)]
        [TestCase(1, 0, 0, false, false, false)]
        [TestCase(1, 1, 255, true, true, false)]
        [TestCase(1, 127, 129, true, true, false)]
        [TestCase(1, 128, 128, false, true, true)]
        [TestCase(1, 129, 127, true, true, false)]
        [TestCase(1, 255, 1, true, true, false)]
        [TestCase(127, 0, 126, false, false, false)]
        [TestCase(127, 1, 125, false, false, false)]
        [TestCase(127, 127, 255, true, true, false)]
        [TestCase(127, 128, 254, false, true, true)]
        [TestCase(127, 129, 253, false, true, true)]
        [TestCase(127, 255, 127, true, true, false)]
        [TestCase(128, 0, 127, true, false, true)]
        [TestCase(128, 1, 126, true, false, true)]
        [TestCase(128, 127, 0, true, false, true)]
        [TestCase(128, 128, 255, true, true, false)]
        [TestCase(128, 129, 254, true, true, false)]
        [TestCase(128, 255, 128, true, true, false)]
        [TestCase(129, 0, 128, false, false, false)]
        [TestCase(129, 1, 127, true, false, true)]
        [TestCase(129, 127, 1, true, false, true)]
        [TestCase(129, 128, 0, false, false, false)]
        [TestCase(129, 129, 255, true, true, false)]
        [TestCase(129, 255, 129, true, true, false)]
        [TestCase(255, 0, 254, false, false, false)]
        [TestCase(255, 1, 253, false, false, false)]
        [TestCase(255, 127, 127, true, false, true)]
        [TestCase(255, 128, 126, false, false, false)]
        [TestCase(255, 129, 125, false, false, false)]
        [TestCase(255, 255, 255, true, true, false)]
        public void SubtractWithCarry(byte a, byte b, byte expected, bool halfCarry, bool carry, bool overflow)
        {
            Reset();

            _flags.SetupProperty(x => x.Carry, true);

            var result = _alu.SubtractWithCarry(a, b);

            Assert.AreEqual(expected, result);

            AssertFlags(result, null, null, halfCarry, overflow, true, carry);
        }

        [TestCase(0, 0, 0, false, false)]
        [TestCase(0, 1, 255, true, false)]
        [TestCase(0, 127, 129, true, false)]
        [TestCase(0, 128, 128, false, true)]
        [TestCase(0, 129, 127, true, false)]
        [TestCase(0, 255, 1, true, false)]
        [TestCase(1, 0, 1, false, false)]
        [TestCase(1, 1, 0, false, false)]
        [TestCase(1, 127, 130, true, false)]
        [TestCase(1, 128, 129, false, true)]
        [TestCase(1, 129, 128, false, true)]
        [TestCase(1, 255, 2, true, false)]
        [TestCase(127, 0, 127, false, false)]
        [TestCase(127, 1, 126, false, false)]
        [TestCase(127, 127, 0, false, false)]
        [TestCase(127, 128, 255, false, true)]
        [TestCase(127, 129, 254, false, true)]
        [TestCase(127, 255, 128, false, true)]
        [TestCase(128, 0, 128, false, false)]
        [TestCase(128, 1, 127, true, true)]
        [TestCase(128, 127, 1, true, true)]
        [TestCase(128, 128, 0, false, false)]
        [TestCase(128, 129, 255, true, false)]
        [TestCase(128, 255, 129, true, false)]
        [TestCase(129, 0, 129, false, false)]
        [TestCase(129, 1, 128, false, false)]
        [TestCase(129, 127, 2, true, true)]
        [TestCase(129, 128, 1, false, false)]
        [TestCase(129, 129, 0, false, false)]
        [TestCase(129, 255, 130, true, false)]
        [TestCase(255, 0, 255, false, false)]
        [TestCase(255, 1, 254, false, false)]
        [TestCase(255, 127, 128, false, false)]
        [TestCase(255, 128, 127, false, false)]
        [TestCase(255, 129, 126, false, false)]
        [TestCase(255, 255, 0, false, false)]
        public void Compare(byte a, byte b, byte expected, bool halfCarry, bool overflow)
        {
            Reset();

            _alu.Compare(a, b);

            AssertFlags(expected, null, null, halfCarry, overflow, true, null);
        }

        [TestCase(0x7b, 0xc3, 0x43)]
        public void And(byte a, byte b, byte expected)
        {
            Reset();

            var result = _alu.And(a, b);

            Assert.AreEqual(expected, result);
            _flags.Verify(x => x.SetParityFlags(expected), Times.Once);
            _flags.VerifySet(x => x.HalfCarry = true, Times.Once);
            _flags.VerifySet(x => x.Carry = false, Times.Once);
        }

        [TestCase(0x48, 0x12, 0x5a)]
        public void Or(byte a, byte b, byte expected)
        {
            Reset();

            var result = _alu.Or(a, b);

            Assert.AreEqual(expected, result);
            _flags.Verify(x => x.SetParityFlags(expected), Times.Once);
            _flags.VerifySet(x => x.HalfCarry = false, Times.Once);
            _flags.VerifySet(x => x.Carry = false, Times.Once);
        }

        [TestCase(0x96, 0x5d, 0xcb)]
        public void Xor(byte a, byte b, byte expected)
        {
            Reset();

            var result = _alu.Xor(a, b);

            Assert.AreEqual(expected, result);
            _flags.Verify(x => x.SetParityFlags(expected), Times.Once);
            _flags.VerifySet(x => x.HalfCarry = false, Times.Once);
            _flags.VerifySet(x => x.Carry = false, Times.Once);
        }

        [TestCase(0x15, 0x27, 0x42)]
        [TestCase(0x50, 0x18, 0x68)]
        public void DecimalAdjustAddition(byte a, byte b, byte expected)
        {
            Reset();

            var result = _alu.Add(a, b);
            var daa = _alu.DecimalAdjust(result, true);

            Assert.AreEqual(expected, daa);
            _flags.Verify(x => x.SetResultFlags(expected), Times.AtLeastOnce);
        }

        [TestCase(0x15, 0x27, 0x88)]
        [TestCase(0x50, 0x18, 0x32)]
        public void DecimalAdjustSubraction(byte a, byte b, byte expected)
        {
            Reset();

            var result = _alu.Subtract(a, b);
            var daa = _alu.DecimalAdjust(result, true);

            Assert.AreEqual(expected, daa);
            _flags.Verify(x => x.SetResultFlags(expected), Times.AtLeastOnce);
        }

        [TestCase((ushort) 0x4242, (ushort) 0x1111, (ushort) 0x5353, false, false)]
        [TestCase((ushort) 0x0100, (ushort) 0x7f00, (ushort) 0x8000, true, false)]
        [TestCase((ushort) 0xffff, (ushort) 0x0001, (ushort) 0x0000, true, true)]
        [TestCase((ushort) 0xaaaa, (ushort) 0xbbbb, (ushort) 0x6665, true, true)]
        public void Add16(ushort a, ushort b, ushort expected, bool halfCarry, bool carry)
        {
            Reset();

            var result = _alu.Add(a, b);

            Assert.AreEqual(expected, result);

            AssertFlags(null, null, null, halfCarry, null, false, carry);
        }

        [TestCase((ushort) 0x4242, (ushort) 0x1111, (ushort) 0x5354, false, false, false)]
        [TestCase((ushort) 0x0100, (ushort) 0x7f00, (ushort) 0x8001, true, true, false)]
        [TestCase((ushort) 0xffff, (ushort) 0x0001, (ushort) 0x0001, true, false, true)]
        [TestCase((ushort) 0xaaaa, (ushort) 0xbbbb, (ushort) 0x6666, true, true, true)]
        public void Add16WithCarry(ushort a, ushort b, ushort expected, bool halfCarry, bool overflow, bool carry)
        {
            Reset();

            _flags.SetupProperty(x => x.Carry, true);

            var result = _alu.AddWithCarry(a, b);

            Assert.AreEqual(expected, result);

            AssertFlags(null, null, null, halfCarry, overflow, false, carry);
            _flags.Verify(x => x.SetResultFlags(result), Times.Once);
        }

        [TestCase((ushort) 0x9999, (ushort) 0x1111, (ushort) 0x8887, false, false, false)]
        [TestCase((ushort) 0x4242, (ushort) 0x1111, (ushort) 0x3130, false, false, false)]
        [TestCase((ushort) 0x0100, (ushort) 0x7f00, (ushort) 0x81ff, true, false, true)]
        [TestCase((ushort) 0xaaaa, (ushort) 0x4444, (ushort) 0x6665, false, true, false)]
        public void Subtract16WithCarry(ushort a, ushort b, ushort expected, bool halfCarry, bool overflow, bool carry)
        {
            Reset();

            _flags.SetupProperty(x => x.Carry, true);

            var result = _alu.SubtractWithCarry(a, b);

            Assert.AreEqual(expected, result);

            AssertFlags(null, null, null, halfCarry, overflow, true, carry);
            _flags.Verify(x => x.SetResultFlags(result), Times.Once);
        }

        [TestCase(0x88, 0x11, true)]
        [TestCase(0x11, 0x22, false)]
        public void RotateLeftWithCarry(byte a, byte expected, bool expectedCarry)
        {
            Reset();

            var result = _alu.RotateLeftWithCarry(a);

            Assert.AreEqual(expected, result);

            AssertFlags(null, null, null, false, null, false, expectedCarry);

            _flags.Verify(x => x.SetResultFlags(result), Times.Once);
        }

        [TestCase(false, 0x88, 0x10, true)]
        [TestCase(false, 0x11, 0x22, false)]
        [TestCase(true, 0x88, 0x11, true)]
        [TestCase(true, 0x11, 0x23, false)]
        public void RotateLeft(bool carry, byte a, byte expected, bool expectedCarry)
        {
            Reset(carry);

            var result = _alu.RotateLeft(a);

            Assert.AreEqual(expected, result);

            AssertFlags(null, null, null, false, null, false, expectedCarry);

            _flags.Verify(x => x.SetResultFlags(result), Times.Once);
        }

        [TestCase(0x11, 0x88, true)]
        [TestCase(0x22, 0x11, false)]
        public void RotateRightWithCarry(byte a, byte expected, bool expectedCarry)
        {
            Reset();

            var result = _alu.RotateRightWithCarry(a);

            Assert.AreEqual(expected, result);

            AssertFlags(null, null, null, false, null, false, expectedCarry);

            _flags.Verify(x => x.SetResultFlags(result), Times.Once);
        }

        [TestCase(false, 0x11, 0x08, true)]
        [TestCase(false, 0x22, 0x11, false)]
        [TestCase(true, 0x11, 0x88, true)]
        [TestCase(true, 0x22, 0x91, false)]
        public void RotateRight(bool carry, byte a, byte expected, bool expectedCarry)
        {
            Reset(carry);

            var result = _alu.RotateRight(a);

            Assert.AreEqual(expected, result);

            AssertFlags(null, null, null, false, null, false, expectedCarry);

            _flags.Verify(x => x.SetResultFlags(result), Times.Once);
        }

        [TestCase(0x88, 0x10, true)]
        [TestCase(0x11, 0x22, false)]
        public void ShiftLeft(byte a, byte expected, bool expectedCarry)
        {
            Reset();

            var result = _alu.ShiftLeft(a);

            Assert.AreEqual(expected, result);

            AssertFlags(null, null, null, false, null, null, expectedCarry);

            _flags.Verify(x => x.SetParityFlags(result), Times.Once);
        }

        [TestCase(0x88, 0x11, true)]
        [TestCase(0x11, 0x23, false)]
        public void ShiftLeftSet(byte a, byte expected, bool expectedCarry)
        {
            Reset();

            var result = _alu.ShiftLeftSet(a);

            Assert.AreEqual(expected, result);

            AssertFlags(null, null, null, false, null, null, expectedCarry);

            _flags.Verify(x => x.SetParityFlags(result), Times.Once);
        }

        [TestCase(0x88, 0xc4, false)]
        [TestCase(0x11, 0x08, true)]
        public void ShiftRight(byte a, byte expected, bool expectedCarry)
        {
            Reset();

            var result = _alu.ShiftRight(a);

            Assert.AreEqual(expected, result);

            AssertFlags(null, null, null, false, null, null, expectedCarry);

            _flags.Verify(x => x.SetParityFlags(result), Times.Once);
        }


        [TestCase(0x88, 0x44, false)]
        [TestCase(0x11, 0x08, true)]
        public void ShiftRightLogical(byte a, byte expected, bool expectedCarry)
        {
            Reset();

            var result = _alu.ShiftRightLogical(a);

            Assert.AreEqual(expected, result);

            AssertFlags(null, null, null, false, null, null, expectedCarry);

            _flags.Verify(x => x.SetParityFlags(result), Times.Once);
        }

        [TestCase(0x7a, 0x31, 0x73, 0x1a)]
        public void RotateLeftDigit(byte accumulator, byte b, byte expectedAccumulator, byte expected)
        {
            Reset();

            var result = _alu.RotateLeftDigit(accumulator, b);

            Assert.AreEqual(expectedAccumulator, result.Accumulator);
            Assert.AreEqual(expected, result.Result);

            AssertFlags(null, null, null, false, null, null, null);

            _flags.Verify(x => x.SetParityFlags(result.Accumulator), Times.Once);
        }

        [TestCase(0x84, 0x20, 0x80, 0x42)]
        public void RotateRightDigit(byte accumulator, byte b, byte expectedAccumulator, byte expected)
        {
            Reset();

            var result = _alu.RotateRightDigit(accumulator, b);

            Assert.AreEqual(expectedAccumulator, result.Accumulator);
            Assert.AreEqual(expected, result.Result);

            AssertFlags(null, null, null, false, null, null, null);

            _flags.Verify(x => x.SetParityFlags(result.Accumulator), Times.Once);
        }

        private void AssertFlags(byte? result,
            bool? sign,
            bool? zero,
            bool? halfCarry,
            bool? parityOverflow,
            bool? subtract,
            bool? carry)
        {
            if (result.HasValue)
            {
                _flags.Verify(x => x.SetResultFlags(result.Value), Times.Once);
            }

            FlagsHelpers.VerifyFlag(_flags, x => x.Sign, sign);
            FlagsHelpers.VerifyFlag(_flags, x => x.Zero, zero);
            FlagsHelpers.VerifyFlag(_flags, x => x.HalfCarry, halfCarry);
            FlagsHelpers.VerifyFlag(_flags, x => x.ParityOverflow, parityOverflow);
            FlagsHelpers.VerifyFlag(_flags, x => x.Subtract, subtract);
            FlagsHelpers.VerifyFlag(_flags, x => x.Carry, carry);
        }

        [Test]
        public void BitReset()
        {
            for (var i = 0; i < 8; i++)
            {
                Reset();
                var result = _alu.BitReset(0xff, i);
                Assert.AreEqual((1 << i) ^ 0xff, result);
            }
        }

        [Test]
        public void BitSet()
        {
            for (var i = 0; i < 8; i++)
            {
                Reset();
                var result = _alu.BitSet(0x00, i);
                Assert.AreEqual(1 << i, result);
            }
        }

        [Test]
        public void BitTest0()
        {
            for (var i = 0; i < 8; i++)
            {
                Reset();
                _alu.BitTest(0x00, i);
                AssertFlags(null, i == 7, true, true, true, false, null);
                _flags.Verify(x => x.SetUndocumentedFlags(0x00), Times.Once);
            }
        }

        [Test]
        public void BitTest1()
        {
            for (var i = 0; i < 8; i++)
            {
                Reset();
                _alu.BitTest(0xff, i);
                AssertFlags(null, false, false, true, false, false, null);
                _flags.Verify(x => x.SetUndocumentedFlags(0xff), Times.Once);
            }
        }
    }
}