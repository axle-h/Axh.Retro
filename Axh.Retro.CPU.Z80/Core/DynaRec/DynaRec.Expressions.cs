using System.Linq.Expressions;
using System.Reflection;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Core.Timing;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Util;

namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    public partial class DynaRec<TRegisters> where TRegisters : IRegisters
    {
        // Register expressions
        private readonly Expression A;

        /// <summary>
        ///     AccumulatorAndResult parameter 'result'
        /// </summary>
        private readonly ParameterExpression AccumulatorAndResult;

        private readonly Expression AccumulatorAndResult_Accumulator;
        private readonly Expression AccumulatorAndResult_Result;
        private readonly Expression AF;
        private readonly ParameterExpression Alu;
        private readonly MethodInfo AluAdd;
        private readonly MethodInfo AluAdd16;
        private readonly MethodInfo AluAdd16WithCarry;

        private readonly MethodInfo AluAddDisplacement;
        private readonly MethodInfo AluAddWithCarry;
        private readonly MethodInfo AluAnd;
        private readonly MethodInfo AluBitReset;
        private readonly MethodInfo AluBitSet;

        private readonly MethodInfo AluBitTest;
        private readonly MethodInfo AluCompare;
        private readonly MethodInfo AluDecimalAdjust;
        private readonly MethodInfo AluDecrement;

        // ALU methods
        private readonly MethodInfo AluIncrement;
        private readonly MethodInfo AluOr;
        private readonly MethodInfo AluRotateLeft;
        private readonly MethodInfo AluRotateLeftDigit;

        private readonly MethodInfo AluRotateLeftWithCarry;
        private readonly MethodInfo AluRotateRight;

        private readonly MethodInfo AluRotateRightDigit;
        private readonly MethodInfo AluRotateRightWithCarry;

        private readonly MethodInfo AluShiftLeft;
        private readonly MethodInfo AluShiftLeftSet;
        private readonly MethodInfo AluShiftRight;
        private readonly MethodInfo AluShiftRightLogical;
        private readonly MethodInfo AluSubtract;
        private readonly MethodInfo AluSubtract16WithCarry;
        private readonly MethodInfo AluSubtractWithCarry;
        private readonly MethodInfo AluSwap;
        private readonly MethodInfo AluXor;
        private readonly Expression B;
        private readonly Expression BC;
        private readonly Expression C;
        private readonly Expression Carry;
        private readonly Expression D;
        private readonly Expression DE;

        /// <summary>
        ///     The dynamic instruction timer parameter 'timer'.
        ///     This is required for instructions that don't have compile time known timings e.g. LDIR.
        /// </summary>
        private readonly ParameterExpression DynamicTimer;

        private readonly MethodInfo DynamicTimerAdd;
        private readonly Expression E;
        private readonly Expression F;

        // Flags
        private readonly Expression Flags;
        private readonly Expression H;
        private readonly Expression HalfCarry;
        private readonly Expression HL;

        // Z80 specific register expressions
        private readonly Expression I;

        // Interrupt stuff
        private readonly Expression IFF1;
        private readonly Expression IFF2;
        private readonly Expression IM;
        private readonly ParameterExpression IO;

        // IO Methods
        private readonly MethodInfo IoReadByte;
        private readonly MethodInfo IoWriteByte;
        private readonly Expression IX;
        private readonly Expression IXh;

        private readonly Expression IXl;
        private readonly Expression IY;
        private readonly Expression IYh;
        private readonly Expression IYl;
        private readonly Expression L;

        /// <summary>
        ///     Word parameter 'w'
        /// </summary>
        private readonly ParameterExpression LocalWord;

        private readonly ParameterExpression Mmu;

        // MMU methods
        private readonly MethodInfo MmuReadByte;
        private readonly MethodInfo MmuReadWord;
        private readonly MethodInfo MmuTransferByte;
        private readonly MethodInfo MmuWriteByte;
        private readonly MethodInfo MmuWriteWord;
        private readonly Expression ParityOverflow;
        private readonly Expression PC;
        private readonly Expression PopSP;
        private readonly Expression PushSP;
        private readonly Expression R;

        /// <summary>
        ///     Reads a byte from the mmu at the address in HL
        /// </summary>
        private readonly Expression ReadByteAtHL;

        /// <summary>
        ///     Reads a word from the mmu at the address in SP and assigns it to PC
        /// </summary>
        private readonly Expression ReadPCFromStack;

        private readonly ParameterExpression Registers;
        private readonly MethodInfo SetResultFlags;
        private readonly MethodInfo SetUndocumentedFlags;
        private readonly Expression Sign;

        // Stack pointer stuff
        private readonly Expression SP;
        private readonly Expression Subtract;
        private readonly Expression SwitchToAlternativeAccumulatorAndFlagsRegisters;

        // Z80 specific register methods
        private readonly Expression SwitchToAlternativeGeneralPurposeRegisters;

        /// <summary>
        ///     Writes the PC to the mmu at the address in SP
        /// </summary>
        private readonly Expression WritePCToStack;

        private readonly Expression Zero;

        private DynaRec()
        {
            Registers = Expression.Parameter(typeof (TRegisters), "registers");
            Mmu = Expression.Parameter(typeof (IMmu), "mmu");
            Alu = Expression.Parameter(typeof (IAlu), "alu");
            IO = Expression.Parameter(typeof (IPeripheralManager), "io");
            LocalWord = Expression.Parameter(typeof (ushort), "w");
            DynamicTimer = Expression.Parameter(typeof (IInstructionTimingsBuilder), "timer");
            DynamicTimerAdd =
                ExpressionHelpers.GetMethodInfo<IInstructionTimingsBuilder, int, int>((dt, m, t) => dt.Add(m, t));

            AccumulatorAndResult = Expression.Parameter(typeof (AccumulatorAndResult), "result");
            AccumulatorAndResult_Accumulator =
                AccumulatorAndResult.GetPropertyExpression<AccumulatorAndResult, byte>(r => r.Accumulator);
            AccumulatorAndResult_Result =
                AccumulatorAndResult.GetPropertyExpression<AccumulatorAndResult, byte>(r => r.Result);

            // General purpose register expressions
            var generalPurposeRegisters =
                Registers.GetPropertyExpression<IRegisters, IGeneralPurposeRegisterSet>(r => r.GeneralPurposeRegisters);
            B = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.B);
            C = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.C);
            D = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.D);
            E = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.E);
            H = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.H);
            L = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, byte>(r => r.L);
            BC = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, ushort>(r => r.BC);
            DE = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, ushort>(r => r.DE);
            HL = generalPurposeRegisters.GetPropertyExpression<IGeneralPurposeRegisterSet, ushort>(r => r.HL);
            PC = Registers.GetPropertyExpression<IRegisters, ushort>(r => r.ProgramCounter);

            // Stack pointer stuff
            SP = Registers.GetPropertyExpression<IRegisters, ushort>(r => r.StackPointer);
            PushSP = Expression.SubtractAssign(SP, Expression.Constant((ushort) 2));
            PopSP = Expression.AddAssign(SP, Expression.Constant((ushort) 2));

            // Interrupt stuff
            IFF1 = Registers.GetPropertyExpression<IRegisters, bool>(r => r.InterruptFlipFlop1);
            IFF2 = Registers.GetPropertyExpression<IRegisters, bool>(r => r.InterruptFlipFlop2);
            IM = Registers.GetPropertyExpression<IRegisters, InterruptMode>(r => r.InterruptMode);

            // Accumulator & Flags register expressions
            var accumulatorAndFlagsRegisters =
                Registers.GetPropertyExpression<IRegisters, IAccumulatorAndFlagsRegisterSet>(
                    r =>
                        r
                            .AccumulatorAndFlagsRegisters);
            A = accumulatorAndFlagsRegisters.GetPropertyExpression<IAccumulatorAndFlagsRegisterSet, byte>(r => r.A);
            Flags =
                accumulatorAndFlagsRegisters.GetPropertyExpression<IAccumulatorAndFlagsRegisterSet, IFlagsRegister>(
                    r =>
                        r
                            .Flags);
            F = Flags.GetPropertyExpression<IFlagsRegister, byte>(r => r.Register);
            AF = accumulatorAndFlagsRegisters.GetPropertyExpression<IAccumulatorAndFlagsRegisterSet, ushort>(r => r.AF);
            Sign = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Sign);
            Zero = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Zero);
            Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Flag5);
            HalfCarry = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.HalfCarry);
            Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Flag3);
            ParityOverflow = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.ParityOverflow);
            Subtract = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Subtract);
            Carry = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Carry);
            SetResultFlags =
                ExpressionHelpers.GetMethodInfo<IFlagsRegister, byte>((flags, result) => flags.SetResultFlags(result));
            SetUndocumentedFlags =
                ExpressionHelpers.GetMethodInfo<IFlagsRegister, byte>(
                    (flags, result) =>
                        flags.SetUndocumentedFlags(result));

            // MMU expressions
            MmuReadByte = ExpressionHelpers.GetMethodInfo<IMmu, ushort, byte>((mmu, address) => mmu.ReadByte(address));
            MmuReadWord = ExpressionHelpers.GetMethodInfo<IMmu, ushort, ushort>((mmu, address) => mmu.ReadWord(address));
            MmuWriteByte =
                ExpressionHelpers.GetMethodInfo<IMmu, ushort, byte>(
                    (mmu, address, value) =>
                        mmu.WriteByte(address, value));
            MmuWriteWord =
                ExpressionHelpers.GetMethodInfo<IMmu, ushort, ushort>(
                    (mmu, address, value) =>
                        mmu.WriteWord(address, value));
            MmuTransferByte =
                ExpressionHelpers.GetMethodInfo<IMmu, ushort, ushort>(
                    (mmu, addressFrom, addressTo) =>
                        mmu.TransferByte(addressFrom, addressTo));

            ReadByteAtHL = Expression.Call(Mmu, MmuReadByte, HL);

            ReadPCFromStack = Expression.Assign(PC, Expression.Call(Mmu, MmuReadWord, SP));
            WritePCToStack = Expression.Call(Mmu, MmuWriteWord, SP, PC);

            // ALU expressions
            AluIncrement = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, b) => alu.Increment(b));
            AluDecrement = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, b) => alu.Decrement(b));
            AluAdd = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte, byte>((alu, a, b) => alu.Add(a, b));
            AluAddWithCarry =
                ExpressionHelpers.GetMethodInfo<IAlu, byte, byte, byte>((alu, a, b) => alu.AddWithCarry(a, b));
            AluAdd16 = ExpressionHelpers.GetMethodInfo<IAlu, ushort, ushort, ushort>((alu, a, b) => alu.Add(a, b));
            AluAdd16WithCarry =
                ExpressionHelpers.GetMethodInfo<IAlu, ushort, ushort, ushort>((alu, a, b) => alu.AddWithCarry(a, b));
            AluSubtract = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte, byte>((alu, a, b) => alu.Subtract(a, b));
            AluSubtractWithCarry =
                ExpressionHelpers.GetMethodInfo<IAlu, byte, byte, byte>((alu, a, b) => alu.SubtractWithCarry(a, b));
            AluSubtract16WithCarry =
                ExpressionHelpers.GetMethodInfo<IAlu, ushort, ushort, ushort>((alu, a, b) => alu.SubtractWithCarry(a, b));
            AluCompare = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a, b) => alu.Compare(a, b));
            AluAnd = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a, b) => alu.And(a, b));
            AluOr = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a, b) => alu.Or(a, b));
            AluXor = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a, b) => alu.Xor(a, b));
            AluDecimalAdjust =
                ExpressionHelpers.GetMethodInfo<IAlu, byte, bool, byte>((alu, a, b) => alu.DecimalAdjust(a, b));
            AluRotateLeftWithCarry =
                ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a) => alu.RotateLeftWithCarry(a));
            AluRotateLeft = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a) => alu.RotateLeft(a));
            AluRotateRightWithCarry =
                ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a) => alu.RotateRightWithCarry(a));
            AluRotateRight = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a) => alu.RotateRight(a));
            AluShiftLeft = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a) => alu.ShiftLeft(a));
            AluShiftLeftSet = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a) => alu.ShiftLeftSet(a));
            AluShiftRight = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a) => alu.ShiftRight(a));
            AluShiftRightLogical =
                ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a) => alu.ShiftRightLogical(a));

            AluRotateRightDigit =
                ExpressionHelpers.GetMethodInfo<IAlu, byte, byte, AccumulatorAndResult>(
                    (alu, a, b) =>
                        alu.RotateRightDigit(a, b));
            AluRotateLeftDigit =
                ExpressionHelpers.GetMethodInfo<IAlu, byte, byte, AccumulatorAndResult>(
                    (alu, a, b) =>
                        alu.RotateLeftDigit(a, b));

            AluBitTest = ExpressionHelpers.GetMethodInfo<IAlu, byte, int>((alu, a, bit) => alu.BitTest(a, bit));
            AluBitSet = ExpressionHelpers.GetMethodInfo<IAlu, byte, int, byte>((alu, a, bit) => alu.BitSet(a, bit));
            AluBitReset = ExpressionHelpers.GetMethodInfo<IAlu, byte, int, byte>((alu, a, bit) => alu.BitReset(a, bit));
            AluAddDisplacement =
                ExpressionHelpers.GetMethodInfo<IAlu, ushort, sbyte, ushort>((alu, a, d) => alu.AddDisplacement(a, d));
            AluSwap = ExpressionHelpers.GetMethodInfo<IAlu, byte, byte>((alu, a) => alu.Swap(a));

            // IO Expressions
            IoReadByte =
                ExpressionHelpers.GetMethodInfo<IPeripheralManager, byte, byte, byte>(
                    (io, port, addressMsb) =>
                        io.ReadByteFromPort(port,
                            addressMsb));
            IoWriteByte =
                ExpressionHelpers.GetMethodInfo<IPeripheralManager, byte, byte, byte>(
                    (io, port, addressMsb, value) =>
                        io.WriteByteToPort(port,
                            addressMsb,
                            value));

            if (typeof (TRegisters) == typeof (IZ80Registers))
            {
                // Z80 specific register expressions
                I = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.I);
                R = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.R);
                IX = Registers.GetPropertyExpression<IZ80Registers, ushort>(r => r.IX);
                IXl = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.IXl);
                IXh = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.IXh);
                IY = Registers.GetPropertyExpression<IZ80Registers, ushort>(r => r.IY);
                IYl = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.IYl);
                IYh = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.IYh);

                // Z80 specific register methods
                SwitchToAlternativeGeneralPurposeRegisters = Expression.Call(Registers,
                    ExpressionHelpers
                        .GetMethodInfo<IZ80Registers>(
                            registers
                                =>
                                registers
                                    .SwitchToAlternativeGeneralPurposeRegisters
                                    ()));
                SwitchToAlternativeAccumulatorAndFlagsRegisters = Expression.Call(Registers,
                    ExpressionHelpers
                        .GetMethodInfo<IZ80Registers>(
                            registers
                                =>
                                registers
                                    .SwitchToAlternativeAccumulatorAndFlagsRegisters
                                    ()));
            }
        }
    }
}