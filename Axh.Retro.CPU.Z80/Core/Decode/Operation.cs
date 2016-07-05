using System;
using System.Text;

namespace Axh.Retro.CPU.Z80.Core.Decode
{
    internal struct Operation
    {
        public Operation(ushort address,
            OpCode opCode,
            Operand operand1,
            Operand operand2,
            FlagTest flagTest,
            OpCodeMeta opCodeMeta,
            byte byteLiteral,
            ushort wordLiteral,
            sbyte displacement) : this()
        {
            Address = address;
            OpCode = opCode;
            Operand1 = operand1;
            Operand2 = operand2;
            FlagTest = flagTest;
            OpCodeMeta = opCodeMeta;
            ByteLiteral = byteLiteral;
            WordLiteral = wordLiteral;
            Displacement = displacement;
        }

        public ushort Address { get; }

        public OpCode OpCode { get; }

        public Operand Operand1 { get; }

        public Operand Operand2 { get; }

        public FlagTest FlagTest { get; }

        public OpCodeMeta OpCodeMeta { get; }

        public byte ByteLiteral { get; }

        public ushort WordLiteral { get; }

        public sbyte Displacement { get; }

        public override string ToString()
        {
            var sb = new StringBuilder().AppendFormat("0x{0}   {1}", Address.ToString("x4"), OpCode.GetMnemonic());
            if (FlagTest != FlagTest.None)
            {
                sb.AppendFormat(" {0}", GetFlagTestString(FlagTest));
            }

            if (Operand1 != Operand.None)
            {
                sb.AppendFormat(" {0}", GetOperandString(Operand1));
            }

            if (Operand2 == Operand.None)
            {
                return sb.ToString();
            }

            return sb.AppendFormat(", {0}", GetOperandString(Operand2)).ToString();
        }

        private static string GetFlagTestString(FlagTest flagTest)
        {
            switch (flagTest)
            {
                case FlagTest.NotZero:
                    return "NZ";
                case FlagTest.Zero:
                    return "Z";
                case FlagTest.NotCarry:
                    return "NC";
                case FlagTest.Carry:
                    return "C";
                case FlagTest.ParityOdd:
                    return "PO";
                case FlagTest.ParityEven:
                    return "PE";
                case FlagTest.Possitive:
                    return "P";
                case FlagTest.Negative:
                    return "M";
                default:
                    throw new ArgumentOutOfRangeException(nameof(flagTest), flagTest, null);
            }
        }

        private string GetOperandString(Operand operand)
        {
            switch (operand)
            {
                case Operand.A:
                case Operand.B:
                case Operand.C:
                case Operand.D:
                case Operand.E:
                case Operand.F:
                case Operand.H:
                case Operand.L:
                case Operand.HL:
                case Operand.BC:
                case Operand.DE:
                case Operand.AF:
                case Operand.SP:
                case Operand.IX:
                case Operand.IXl:
                case Operand.IXh:
                case Operand.IY:
                case Operand.IYl:
                case Operand.IYh:
                case Operand.I:
                case Operand.R:
                    return operand.ToString();
                case Operand.mHL:
                    return "(HL)";
                case Operand.mBC:
                    return "(BC)";
                case Operand.mDE:
                    return "(DE)";
                case Operand.mSP:
                    return "(SP)";
                case Operand.mnn:
                    return $"(0x{WordLiteral.ToString("x4")})";
                case Operand.nn:
                    return $"0x{WordLiteral.ToString("x4")}";
                case Operand.n:
                    return $"0x{ByteLiteral.ToString("x2")}";
                case Operand.d:
                    return ((sbyte) ByteLiteral).ToString();
                case Operand.mIXd:
                    return Displacement > 0 ? $"(IX+{Displacement})" : $"(IX{Displacement})";
                case Operand.mIYd:
                    return Displacement > 0 ? $"(IY+{Displacement})" : $"(IY{Displacement})";
                case Operand.mCl:
                    return "(0xff00 + C)";
                case Operand.mnl:
                    return $"(0xff00 + 0x{ByteLiteral:x2})";
                case Operand.SPd:
                    var d = (sbyte) ByteLiteral;
                    return d > 0 ? $"SP+{d}" : $"SP{d}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(operand), operand, null);
            }
        }
    }
}