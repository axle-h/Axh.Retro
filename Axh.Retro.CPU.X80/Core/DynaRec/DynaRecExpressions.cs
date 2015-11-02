namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Core.Timing;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Util;

    internal static class DynaRecExpressions
    {
        public static readonly ParameterExpression Registers;
        public static readonly ParameterExpression Mmu;
        public static readonly ParameterExpression Alu;
        public static readonly ParameterExpression IO;

        /// <summary>
        /// Byte parameter 'b'
        /// </summary>
        public static readonly ParameterExpression LocalByte;

        /// <summary>
        /// Word parameter 'w'
        /// </summary>
        public static readonly ParameterExpression LocalWord;

        /// <summary>
        /// The dynamic instruction timer parameter 'timer'.
        /// This is required for instructions that don't have compile time known timings e.g. LDIR.
        /// </summary>
        public static readonly ParameterExpression DynamicTimer;
        public static readonly MethodInfo DynamicTimerAdd;

        /// <summary>
        /// AccumulatorAndResult parameter 'result'
        /// </summary>
        public static readonly ParameterExpression AccumulatorAndResult;
        public static readonly Expression AccumulatorAndResult_Accumulator;
        public static readonly Expression AccumulatorAndResult_Result;

        // Register expressions
        public static readonly Expression A;
        public static readonly Expression B;
        public static readonly Expression C;
        public static readonly Expression D;
        public static readonly Expression E;
        public static readonly Expression F;
        public static readonly Expression H;
        public static readonly Expression L;
        public static readonly Expression BC;
        public static readonly Expression DE;
        public static readonly Expression HL;
        public static readonly Expression PC;

        // Stack pointer stuff
        public static readonly Expression SP;
        public static readonly Expression PushSP;
        public static readonly Expression PushPushSP;
        public static readonly Expression PopSP;
        public static readonly Expression PopPopSP;

        // Z80 specific register expressions
        public static readonly Expression I;
        public static readonly Expression R;
        public static readonly Expression IX;
        public static readonly Expression IY;

        public static readonly Expression IXl;
        public static readonly Expression IXh;
        public static readonly Expression IYl;
        public static readonly Expression IYh;

        // Z80 specific helpers
        /// <summary>
        /// IX + d
        /// d is read from LocalByte
        /// </summary>
        public static readonly Expression IXd;

        /// <summary>
        /// IY + d
        /// d is read from LocalByte
        /// </summary>
        public static readonly Expression IYd;

        /// <summary>
        /// Reads a byte from the mmu at the address at IX + b (using 2's compliant addition)
        /// </summary>
        public static readonly Expression ReadByteAtIXd;

        /// <summary>
        /// Reads a byte from the mmu at the address at IY + b (using 2's compliant addition)
        /// </summary>
        public static readonly Expression ReadByteAtIYd;

        // Z80 specific register methods
        public static readonly Expression SwitchToAlternativeGeneralPurposeRegisters;
        public static readonly Expression SwitchToAlternativeAccumulatorAndFlagsRegisters;

        // Interrupt stuff
        public static readonly Expression IFF1;
        public static readonly Expression IFF2;
        public static readonly Expression IM;

        // Flags
        public static readonly Expression Flags;
        public static readonly Expression Sign;
        public static readonly Expression Zero;
        public static readonly Expression Flag5;
        public static readonly Expression HalfCarry;
        public static readonly Expression Flag3;
        public static readonly Expression ParityOverflow;
        public static readonly Expression Subtract;
        public static readonly Expression Carry;
        public static readonly MethodInfo SetResultFlags;
        public static readonly MethodInfo SetUndocumentedFlags;

        /// <summary>
        /// Reads a byte from the mmu at the address at LocalWord
        /// </summary>
        public static readonly Expression ReadByteAtLocalWord;

        /// <summary>
        /// Reads a byte from the mmu at the address in HL
        /// </summary>
        public static readonly Expression ReadByteAtHL;

        /// <summary>
        /// Reads a byte from the mmu at the address in BC
        /// </summary>
        public static readonly Expression ReadByteAtBC;

        /// <summary>
        /// Reads a byte from the mmu at the address in DE
        /// </summary>
        public static readonly Expression ReadByteAtDE;

        /// <summary>
        /// Writes the PC to the mmu at the address in SP
        /// </summary>
        public static readonly Expression WritePCToStack;

        /// <summary>
        /// Reads a word from the mmu at the address in SP and assigns it to PC
        /// </summary>
        public static readonly Expression ReadPCFromStack;

        // MMU methods
        public static readonly MethodInfo MmuReadByte;
        public static readonly MethodInfo MmuReadWord;
        public static readonly MethodInfo MmuWriteByte;
        public static readonly MethodInfo MmuWriteWord;
        public static readonly MethodInfo MmuTransferByte;

        // ALU methods
        public static readonly MethodInfo AluIncrement;
        public static readonly MethodInfo AluDecrement;
        public static readonly MethodInfo AluAdd;
        public static readonly MethodInfo AluAddWithCarry;
        public static readonly MethodInfo AluAdd16;
        public static readonly MethodInfo AluAdd16WithCarry;
        public static readonly MethodInfo AluSubtract;
        public static readonly MethodInfo AluSubtractWithCarry;
        public static readonly MethodInfo AluSubtract16WithCarry;
        public static readonly MethodInfo AluCompare;
        public static readonly MethodInfo AluAnd;
        public static readonly MethodInfo AluOr;
        public static readonly MethodInfo AluXor;
        public static readonly MethodInfo AluDecimalAdjust;

        public static readonly MethodInfo AluRotateLeftWithCarry;
        public static readonly MethodInfo AluRotateLeft;
        public static readonly MethodInfo AluRotateRightWithCarry;
        public static readonly MethodInfo AluRotateRight;
        
        public static readonly MethodInfo AluShiftLeft;
        public static readonly MethodInfo AluShiftLeftSet;
        public static readonly MethodInfo AluShiftRight;
        public static readonly MethodInfo AluShiftRightLogical;

        public static readonly MethodInfo AluRotateRightDigit;
        public static readonly MethodInfo AluRotateLeftDigit;

        public static readonly MethodInfo AluBitTest;
        public static readonly MethodInfo AluBitSet;
        public static readonly MethodInfo AluBitReset;

        // IO Methods
        public static readonly MethodInfo IoReadByte;
        public static readonly MethodInfo IoWriteByte;

        // General helpers
        /// <summary>
        /// Increment the program counter by a 2s complement displacement set in LocalByte
        /// </summary>
        public static readonly Expression JumpToDisplacement;

        public static readonly IDictionary<IndexRegister, IndexRegisterExpressions> IndexRegisterExpressions;

        static DynaRecExpressions()
        {
            Registers = Expression.Parameter(typeof(IZ80Registers), "registers");
            Mmu = Expression.Parameter(typeof(IMmu), "mmu");
            Alu = Expression.Parameter(typeof(IArithmeticLogicUnit), "alu");
            IO = Expression.Parameter(typeof(IPeripheralManager), "io");
            LocalByte = Expression.Parameter(typeof(byte), "b");
            LocalWord = Expression.Parameter(typeof(ushort), "w");
            DynamicTimer = Expression.Parameter(typeof(IInstructionTimingsBuilder), "timer");
            DynamicTimerAdd = ExpressionHelpers.GetMethodInfo<IInstructionTimingsBuilder, int, int>((dt, m, t) => dt.Add(m, t));

            AccumulatorAndResult = Expression.Parameter(typeof(AccumulatorAndResult), "result");
            AccumulatorAndResult_Accumulator = AccumulatorAndResult.GetPropertyExpression<AccumulatorAndResult, byte>(r => r.Accumulator);
            AccumulatorAndResult_Result = AccumulatorAndResult.GetPropertyExpression<AccumulatorAndResult, byte>(r => r.Result);

            // General purpose register expressions
            var generalPurposeRegisters = Registers.GetPropertyExpression<IRegisters, IGeneralPurposeRegisterSet>(r => r.GeneralPurposeRegisters);
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
            PushSP = Expression.PreDecrementAssign(SP);
            PushPushSP = Expression.SubtractAssign(SP, Expression.Constant((ushort)2));
            PopSP = Expression.PreIncrementAssign(SP);
            PopPopSP = Expression.AddAssign(SP, Expression.Constant((ushort)2));

            // Z80 specific register expressions
            I = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.I);
            R = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.R);
            IX = Registers.GetPropertyExpression<IZ80Registers, ushort>(r => r.IX);
            IXl = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.IXl);
            IXh = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.IXh);
            IY = Registers.GetPropertyExpression<IZ80Registers, ushort>(r => r.IY);
            IYl = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.IYl);
            IYh = Registers.GetPropertyExpression<IZ80Registers, byte>(r => r.IYh);

            // Z80 specific Index register expressions i.e. IX+d and IY+d where d is LocalByte (expression local value, which must be initialised before running these)
            IXd = Expression.Convert(Expression.Add(Expression.Convert(IX, typeof(int)), Expression.Convert(Expression.Convert(LocalByte, typeof(sbyte)), typeof(int))), typeof(ushort));
            IYd = Expression.Convert(Expression.Add(Expression.Convert(IY, typeof(int)), Expression.Convert(Expression.Convert(LocalByte, typeof(sbyte)), typeof(int))), typeof(ushort));
            
            // Z80 specific register methods
            SwitchToAlternativeGeneralPurposeRegisters = Expression.Call(Registers, ExpressionHelpers.GetMethodInfo<IZ80Registers>((registers) => registers.SwitchToAlternativeGeneralPurposeRegisters()));
            SwitchToAlternativeAccumulatorAndFlagsRegisters = Expression.Call(Registers, ExpressionHelpers.GetMethodInfo<IZ80Registers>((registers) => registers.SwitchToAlternativeAccumulatorAndFlagsRegisters()));


            // Interrupt stuff
            IFF1 = Registers.GetPropertyExpression<IRegisters, bool>(r => r.InterruptFlipFlop1);
            IFF2 = Registers.GetPropertyExpression<IRegisters, bool>(r => r.InterruptFlipFlop2);
            IM = Registers.GetPropertyExpression<IRegisters, InterruptMode>(r => r.InterruptMode);

            // Accumulator & Flags register expressions
            var accumulatorAndFlagsRegisters = Registers.GetPropertyExpression<IRegisters, IAccumulatorAndFlagsRegisterSet>(r => r.AccumulatorAndFlagsRegisters);
            A = accumulatorAndFlagsRegisters.GetPropertyExpression<IAccumulatorAndFlagsRegisterSet, byte>(r => r.A);
            Flags = accumulatorAndFlagsRegisters.GetPropertyExpression<IAccumulatorAndFlagsRegisterSet, IFlagsRegister>(r => r.Flags);
            F = Flags.GetPropertyExpression<IFlagsRegister, byte>(r => r.Register);
            Sign = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Sign);
            Zero = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Zero);
            Flag5 = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Flag5);
            HalfCarry = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.HalfCarry);
            Flag3 = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Flag3);
            ParityOverflow = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.ParityOverflow);
            Subtract = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Subtract);
            Carry = Flags.GetPropertyExpression<IFlagsRegister, bool>(r => r.Carry);
            SetResultFlags = ExpressionHelpers.GetMethodInfo<IFlagsRegister, byte>((flags, result) => flags.SetResultFlags(result));
            SetUndocumentedFlags = ExpressionHelpers.GetMethodInfo<IFlagsRegister, byte>((flags, result) => flags.SetUndocumentedFlags(result));
            
            // MMU expressions
            MmuReadByte = ExpressionHelpers.GetMethodInfo<IMmu, ushort, byte>((mmu, address) => mmu.ReadByte(address));
            MmuReadWord = ExpressionHelpers.GetMethodInfo<IMmu, ushort, ushort>((mmu, address) => mmu.ReadWord(address));
            MmuWriteByte = ExpressionHelpers.GetMethodInfo<IMmu, ushort, byte>((mmu, address, value) => mmu.WriteByte(address, value));
            MmuWriteWord = ExpressionHelpers.GetMethodInfo<IMmu, ushort, ushort>((mmu, address, value) => mmu.WriteWord(address, value));
            MmuTransferByte = ExpressionHelpers.GetMethodInfo<IMmu, ushort, ushort>((mmu, addressFrom, addressTo) => mmu.TransferByte(addressFrom, addressTo));

            ReadByteAtLocalWord = Expression.Call(Mmu, MmuReadByte, LocalWord);
            ReadByteAtHL = Expression.Call(Mmu, MmuReadByte, HL);
            ReadByteAtBC = Expression.Call(Mmu, MmuReadByte, BC);
            ReadByteAtDE = Expression.Call(Mmu, MmuReadByte, DE);

            ReadPCFromStack = Expression.Assign(PC, Expression.Call(Mmu, MmuReadWord, SP));
            WritePCToStack = Expression.Call(Mmu, MmuWriteWord, SP, PC);

            // Z80 specific
            ReadByteAtIXd = Expression.Call(Mmu, MmuReadByte, IXd);
            ReadByteAtIYd = Expression.Call(Mmu, MmuReadByte, IYd);

            // ALU expressions
            AluIncrement = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, b) => alu.Increment(b));
            AluDecrement = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, b) => alu.Decrement(b));
            AluAdd = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte, byte>((alu, a, b) => alu.Add(a, b));
            AluAddWithCarry = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte, byte>((alu, a, b) => alu.AddWithCarry(a, b));
            AluAdd16 = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, ushort, ushort, ushort>((alu, a, b) => alu.Add(a, b));
            AluAdd16WithCarry = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, ushort, ushort, ushort>((alu, a, b) => alu.AddWithCarry(a, b));
            AluSubtract = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte, byte>((alu, a, b) => alu.Subtract(a, b));
            AluSubtractWithCarry = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte, byte>((alu, a, b) => alu.SubtractWithCarry(a, b));
            AluSubtract16WithCarry = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, ushort, ushort, ushort>((alu, a, b) => alu.SubtractWithCarry(a, b));
            AluCompare = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a, b) => alu.Compare(a, b));
            AluAnd = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a, b) => alu.And(a, b));
            AluOr = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a, b) => alu.Or(a, b));
            AluXor = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a, b) => alu.Xor(a, b));
            AluDecimalAdjust = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a) => alu.DecimalAdjust(a));
            AluRotateLeftWithCarry = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a) => alu.RotateLeftWithCarry(a));
            AluRotateLeft = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a) => alu.RotateLeft(a));
            AluRotateRightWithCarry = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a) => alu.RotateRightWithCarry(a));
            AluRotateRight = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a) => alu.RotateRight(a));
            AluShiftLeft = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a) => alu.ShiftLeft(a));
            AluShiftLeftSet = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a) => alu.ShiftLeftSet(a));
            AluShiftRight = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a) => alu.ShiftRight(a));
            AluShiftRightLogical = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte>((alu, a) => alu.ShiftRightLogical(a));

            AluRotateRightDigit = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte, AccumulatorAndResult>((alu, a, b) => alu.RotateRightDigit(a, b));
            AluRotateLeftDigit = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, byte, AccumulatorAndResult>((alu, a, b) => alu.RotateLeftDigit(a, b));

            AluBitTest = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, int>((alu, a, bit) => alu.BitTest(a, bit));
            AluBitSet = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, int, byte>((alu, a, bit) => alu.BitSet(a, bit));
            AluBitReset = ExpressionHelpers.GetMethodInfo<IArithmeticLogicUnit, byte, int, byte>((alu, a, bit) => alu.BitReset(a, bit));

            // IO Expressions
            IoReadByte = ExpressionHelpers.GetMethodInfo<IPeripheralManager, byte, byte, byte>((io, port, addressMsb) => io.ReadByteFromPort(port, addressMsb));
            IoWriteByte = ExpressionHelpers.GetMethodInfo<IPeripheralManager, byte, byte, byte>((io, port, addressMsb, value) => io.WriteByteToPort(port, addressMsb, value));

            JumpToDisplacement = Expression.Assign(PC, Expression.Convert(Expression.Add(Expression.Convert(PC, typeof(int)), Expression.Convert(Expression.Convert(LocalByte, typeof(sbyte)), typeof(int))), typeof(ushort)));

            IndexRegisterExpressions = new Dictionary<IndexRegister, IndexRegisterExpressions>
                                       {
                                           {
                                               IndexRegister.HL,
                                               new IndexRegisterExpressions
                                               {
                                                   IndexRegister = IndexRegister.HL,
                                                   Register = HL,
                                                   RegisterLowOrder = L,
                                                   RegisterHighOrder = H,
                                                   IndexedAddress = HL,
                                                   // HL indexes don't have a displacement
                                                   ReadIndexedValue = ReadByteAtHL,
                                                   UsesDisplacedIndexTimings = false
                                               }
                                           },
                                           {
                                               IndexRegister.IX,
                                               new IndexRegisterExpressions
                                               {
                                                   IndexRegister = IndexRegister.IX,
                                                   Register = IX,
                                                   RegisterLowOrder = IXl,
                                                   RegisterHighOrder = IXh,
                                                   IndexedAddress = IXd,
                                                   ReadIndexedValue = ReadByteAtIXd,
                                                   UsesDisplacedIndexTimings = true
                                               }
                                           },
                                           {
                                               IndexRegister.IY,
                                               new IndexRegisterExpressions
                                               {
                                                   IndexRegister = IndexRegister.IY,
                                                   Register = IY,
                                                   RegisterLowOrder = IYl,
                                                   RegisterHighOrder = IYh,
                                                   IndexedAddress = IYd,
                                                   ReadIndexedValue = ReadByteAtIYd,
                                                   UsesDisplacedIndexTimings = true
                                               }
                                           }
                                       };
        }

        public static Expression GetDynamicTimings(int mCycles, int tStates)
        {
            return Expression.Call(DynamicTimer, DynamicTimerAdd, Expression.Constant(mCycles), Expression.Constant(tStates));
        }

        public static Expression GetMemoryRefreshDeltaExpression(Expression deltaExpression)
        {
            var increment7LsbR = Expression.And(Expression.Add(Expression.Convert(R, typeof(int)), deltaExpression), Expression.Constant(0x7f));
            return Expression.Assign(R, Expression.Convert(increment7LsbR, typeof(byte)));
        }

        public static IEnumerable<Expression> GetLdExpressions(bool decrement = false)
        {
            yield return Expression.Call(Mmu, MmuTransferByte, HL, DE);
            yield return decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL);
            yield return decrement ? Expression.PreDecrementAssign(DE) : Expression.PreIncrementAssign(DE);
            yield return Expression.PreDecrementAssign(BC);
            yield return Expression.Assign(HalfCarry, Expression.Constant(false));
            yield return Expression.Assign(ParityOverflow, Expression.NotEqual(BC, Expression.Constant((ushort)0)));
            yield return Expression.Assign(Subtract, Expression.Constant(false));
        }

        public static IEnumerable<Expression> GetLdrExpressions(bool decrement = false)
        {
            var breakLabel = Expression.Label();
            yield return
                Expression.Loop(
                    Expression.Block(
                        Expression.Call(Mmu, MmuTransferByte, HL, DE),
                        decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL),
                        decrement ? Expression.PreDecrementAssign(DE) : Expression.PreIncrementAssign(DE),
                        Expression.PreDecrementAssign(BC),
                        Expression.IfThen(Expression.Equal(BC, Expression.Constant((ushort)0)), Expression.Break(breakLabel)),
                        GetDynamicTimings(5, 21),
                        GetMemoryRefreshDeltaExpression(Expression.Constant(2))), // This function actually decreases the PC by two for each 'loop' hence need more refresh cycles.
                    breakLabel);

            yield return Expression.Assign(HalfCarry, Expression.Constant(false));
            yield return Expression.Assign(ParityOverflow, Expression.Constant(false));
            yield return Expression.Assign(Subtract, Expression.Constant(false));
        }

        public static IEnumerable<Expression> GetCpExpressions(bool decrement = false)
        {
            yield return Expression.Call(Alu, AluCompare, A, Expression.Call(Mmu, MmuReadByte, HL));
            yield return decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL);
            yield return Expression.PreDecrementAssign(BC);
        }

        public static Expression GetCprExpression(bool decrement = false)
        {
            var breakLabel = Expression.Label();
            var expressions = GetCpExpressions(decrement);
            var iterationExpressions = new[]
                                       {
                                           Expression.IfThen(Expression.OrElse(Expression.Equal(BC, Expression.Constant((ushort)0)), Zero), Expression.Break(breakLabel)), GetDynamicTimings(5, 21),
                                           GetMemoryRefreshDeltaExpression(Expression.Constant(2))
                                       };
            return Expression.Loop(Expression.Block(expressions.Concat(iterationExpressions).ToArray()), breakLabel);
        }

        public static IEnumerable<Expression> GetInExpressions(bool decrement = false)
        {
            yield return Expression.Call(Mmu, MmuWriteByte, HL, Expression.Call(IO, IoReadByte, C, B));
            yield return decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL);
            yield return Expression.Assign(B, Expression.Convert(Expression.Subtract(Expression.Convert(B, typeof(int)), Expression.Constant(1)), typeof(byte)));
            yield return Expression.Assign(Subtract, Expression.Constant(true));
            yield return Expression.Call(Flags, SetResultFlags, B);
        }
        
        public static Expression GetInrExpression(bool decrement = false)
        {
            var breakLabel = Expression.Label();

            var expressions = GetInExpressions(decrement);
            var iterationExpressions = new[]
                                       {
                                           Expression.IfThen(Expression.Equal(B, Expression.Constant((byte)0)), Expression.Break(breakLabel)), GetDynamicTimings(5, 21),
                                           GetMemoryRefreshDeltaExpression(Expression.Constant(2))
                                       };

            return Expression.Loop(Expression.Block(expressions.Concat(iterationExpressions).ToArray()), breakLabel);
        }

        public static IEnumerable<Expression> GetOutExpressions(bool decrement = false)
        {
            yield return Expression.Call(IO, IoWriteByte, C, B, ReadByteAtHL);
            yield return decrement ? Expression.PreDecrementAssign(HL) : Expression.PreIncrementAssign(HL);
            yield return Expression.Assign(B, Expression.Convert(Expression.Subtract(Expression.Convert(B, typeof(int)), Expression.Constant(1)), typeof(byte)));
            yield return Expression.Assign(Subtract, Expression.Constant(true));
            yield return Expression.Call(Flags, SetResultFlags, B);
        }

        public static Expression GetOutrExpression(bool decrement = false)
        {
            var breakLabel = Expression.Label();

            var expressions = GetOutExpressions(decrement);
            var iterationExpressions = new[]
                                       {
                                           Expression.IfThen(Expression.Equal(B, Expression.Constant((byte)0)), Expression.Break(breakLabel)), GetDynamicTimings(5, 21),
                                           GetMemoryRefreshDeltaExpression(Expression.Constant(2))
                                       };

            return Expression.Loop(Expression.Block(expressions.Concat(iterationExpressions).ToArray()), breakLabel);
        }
    }
}
