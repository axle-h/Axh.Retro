namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System;

    internal enum Opcode
    {
        NoOperation,
        Halt,

        // 8-Bit Load
        Load,

        // 16-Bit Load
        Load16,
        Push,
        Pop,

        // 8-Bit arithmetic
        Add,
        AddCarry,
        Subtract,
        SubtractCarry,
        And,
        Or,
        Xor,
        Compare,
        Increment,
        Decrement,

        // 16-Bit arithmetic
        Add16,
        AddCarry16,
        SubtractCarry16,
        Increment16,
        Decrement16,

        // Exchange
        Exchange,
        ExchangeAccumulatorAndFlags,
        ExchangeGeneralPurpose,
        
        // Jump
        Jump,
        JumpRelative,
        DecrementJumpRelativeIfNonZero,

        // Call/Return/Reset
        Call,
        Return,
        ReturnFromInterrupt,
        ReturnFromNonmaskableInterrupt,
        Reset,

        // IO
        Input,
        Output,

        // Rotate
        RotateLeftWithCarry,
        RotateLeft,
        RotateRightWithCarry,
        RotateRight,
        RotateLeftDigit,
        RotateRightDigit,

        // Shift
        ShiftLeft,
        ShiftLeftSet,
        ShiftRight,
        ShiftRightLogical,
        
        // Bit test, set & reset
        BitTest,
        BitSet,
        BitReset,

        // Block search & transfer
        TransferIncrement,
        TransferIncrementRepeat,
        TransferDecrement,
        TransferDecrementRepeat,
        SearchIncrement,
        SearchIncrementRepeat,
        SearchDecrement,
        SearchDecrementRepeat,
        InputTransferIncrement,
        InputTransferIncrementRepeat,
        InputTransferDecrement,
        InputTransferDecrementRepeat,
        OutputTransferIncrement,
        OutputTransferIncrementRepeat,
        OutputTransferDecrement,
        OutputTransferDecrementRepeat,

        // Gernal purpose arithmetic
        DecimalArithmeticAdjust,
        NegateOnesComplement,
        NegateTwosComplement,
        InvertCarryFlag,
        SetCarryFlag,

        DisableInterrupts,
        EnableInterrupts,
        
        InterruptMode0,
        InterruptMode1,
        InterruptMode2,

        // GB Specific
        Swap,
        LoadIncrement,
        LoadDecrement,
        Stop,
    }

    internal static class OpcodeExtensions
    {
        public static bool Is16Bit(this Opcode code)
        {
            switch (code)
            {
                case Opcode.Load16:
                case Opcode.Add16:
                case Opcode.AddCarry16:
                case Opcode.SubtractCarry16:
                case Opcode.Increment16:
                case Opcode.Decrement16:
                    return true;
                default:
                    return false;
            }
        }

        public static string GetMnemonic(this Opcode code)
        {
            switch (code)
            {
                case Opcode.NoOperation: return "NOP";
                case Opcode.Halt: return "HALT";
                case Opcode.Load: return "LD";
                case Opcode.Load16: return "LD";
                case Opcode.Push: return "PUSH";
                case Opcode.Pop: return "POP";
                case Opcode.Add: return "ADD";
                case Opcode.AddCarry: return "ADC";
                case Opcode.Subtract: return "SUB";
                case Opcode.SubtractCarry: return "SBC";
                case Opcode.And: return "AND";
                case Opcode.Or: return "OR";
                case Opcode.Xor: return "XOR";
                case Opcode.Compare: return "CP";
                case Opcode.Increment: return "INC";
                case Opcode.Decrement: return "DEC";
                case Opcode.Add16: return "ADD";
                case Opcode.AddCarry16: return "ADC";
                case Opcode.SubtractCarry16: return "SBC";
                case Opcode.Increment16: return "INC";
                case Opcode.Decrement16: return "DEC";
                case Opcode.Exchange: return "EX";
                case Opcode.ExchangeAccumulatorAndFlags: return "EX AF, AF'";
                case Opcode.ExchangeGeneralPurpose: return "EXX";
                case Opcode.Jump: return "JP";
                case Opcode.JumpRelative: return "JR";
                case Opcode.DecrementJumpRelativeIfNonZero: return "DJNZ";
                case Opcode.Call: return "CALL";
                case Opcode.ReturnFromInterrupt: return "RETI";
                case Opcode.ReturnFromNonmaskableInterrupt: return "RETN";
                case Opcode.Return: return "RET";
                case Opcode.Reset: return "RST";
                case Opcode.Input: return "IN";
                case Opcode.Output: return "OUT";
                case Opcode.RotateLeftWithCarry: return "RLC";
                case Opcode.RotateLeft: return "RL";
                case Opcode.RotateRightWithCarry: return "RRC";
                case Opcode.RotateRight: return "RR";
                case Opcode.RotateLeftDigit: return "RLD";
                case Opcode.RotateRightDigit: return "RRD";
                case Opcode.ShiftLeft: return "SLA";
                case Opcode.ShiftLeftSet: return "SLS";
                case Opcode.ShiftRightLogical: return "SRL";
                case Opcode.ShiftRight: return "SRA";
                case Opcode.BitTest: return "BIT";
                case Opcode.BitSet: return "SET";
                case Opcode.BitReset: return "RES";
                case Opcode.TransferIncrement: return "LDI";
                case Opcode.TransferIncrementRepeat: return "LDIR";
                case Opcode.TransferDecrement: return "LDD";
                case Opcode.TransferDecrementRepeat: return "LDDR";
                case Opcode.SearchIncrement: return "CPI";
                case Opcode.SearchIncrementRepeat: return "CPIR";
                case Opcode.SearchDecrement: return "CPD";
                case Opcode.SearchDecrementRepeat: return "CPDR";
                case Opcode.InputTransferIncrement: return "INI";
                case Opcode.InputTransferIncrementRepeat: return "INIR";
                case Opcode.InputTransferDecrement: return "IND";
                case Opcode.InputTransferDecrementRepeat: return "INDR";
                case Opcode.OutputTransferIncrement: return "OUTI";
                case Opcode.OutputTransferIncrementRepeat: return "OUTIR";
                case Opcode.OutputTransferDecrement: return "OUTD";
                case Opcode.OutputTransferDecrementRepeat: return "OUTDR";
                case Opcode.DecimalArithmeticAdjust: return "DAA";
                case Opcode.NegateOnesComplement: return "CPL";
                case Opcode.InvertCarryFlag: return "CCF";
                case Opcode.SetCarryFlag: return "SCF";
                case Opcode.DisableInterrupts: return "DI";
                case Opcode.EnableInterrupts: return "EI";
                case Opcode.NegateTwosComplement: return "NEG";
                case Opcode.InterruptMode0: return "IM0";
                case Opcode.InterruptMode1: return "IM1";
                case Opcode.InterruptMode2: return "IM2";
                case Opcode.Swap: return "SWAP";
                case Opcode.LoadIncrement: return "LDI";
                case Opcode.LoadDecrement: return "LDD";
                case Opcode.Stop: return "STOP";
                default:
                    throw new ArgumentOutOfRangeException(nameof(code), code, null);
            }
        }
    }
}
