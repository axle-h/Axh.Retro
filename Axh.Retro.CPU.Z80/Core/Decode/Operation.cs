namespace Axh.Retro.CPU.Z80.Core.Decode
{
    using System;
    using System.Text;

    internal class Operation
    {
        public Operation(OpCode opCode)
        {
            OpCode = opCode;
        }

        public Operation(OpCode opCode, Operand operand1)
        {
            OpCode = opCode;
            Operand1 = operand1;
        }
        
        public Operation(OpCode opCode, Operand operand1, Operand operand2)
        {
            OpCode = opCode;
            Operand1 = operand1;
            Operand2 = operand2;
        }

        public Operation WithFlag(FlagTest flagTest)
        {
            this.FlagTest = flagTest;
            return this;
        }

        public Operation AddLiteral(byte literal)
        {
            this.ByteLiteral = literal;
            return this;
        }

        public Operation WithByteLiteral()
        {
            this.OpCodeMeta |= OpCodeMeta.ByteLiteral;
            return this;
        }

        public Operation AddLiteral(ushort literal)
        {
            this.WordLiteral = literal;
            return this;
        }

        public Operation WithWordLiteral()
        {
            this.OpCodeMeta |= OpCodeMeta.WordLiteral;
            return this;
        }

        public Operation AddDisplacement(byte displacement)
        {
            this.Displacement = (sbyte)displacement;
            return this;
        }

        public Operation WithDisplacement()
        {
            this.OpCodeMeta |= OpCodeMeta.Displacement;
            return this;
        }

        public Operation EndBlock()
        {
            this.OpCodeMeta |= OpCodeMeta.EndBlock;
            return this;
        }

        public void AutoCopy()
        {
            this.OpCodeMeta |= OpCodeMeta.AutoCopy;
        }

        public Operation UseAlternativeFlagAffection()
        {
            this.OpCodeMeta |= OpCodeMeta.UseAlternativeFlagAffection;
            return this;
        }

        public ushort Address { get; set; }
        
        public OpCode OpCode { get; }

        public Operand Operand1 { get; set; }

        public Operand Operand2 { get; set; }

        public FlagTest FlagTest { get; private set; }

        public byte ByteLiteral { get; private set; }

        public ushort WordLiteral { get; private set; }

        public sbyte Displacement { get; private set; }

        public OpCodeMeta OpCodeMeta { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder().AppendFormat("0x{0}   {1}", this.Address.ToString("x4"), OpCode.GetMnemonic());
            if (this.FlagTest != FlagTest.None)
            {
                sb.AppendFormat(" {0}", GetFlagTestString(this.FlagTest));
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
                    return $"(0x{this.WordLiteral.ToString("x4")})";
                case Operand.nn:
                    return $"0x{this.WordLiteral.ToString("x4")}";
                case Operand.n:
                    return $"0x{this.ByteLiteral.ToString("x2")}";
                case Operand.d:
                    return ((sbyte)this.ByteLiteral).ToString();
                case Operand.mIXd:
                    return this.Displacement > 0 ? $"(IX+{this.Displacement})" : $"(IX{this.Displacement})";
                case Operand.mIYd:
                    return this.Displacement > 0 ? $"(IY+{this.Displacement})" : $"(IY{this.Displacement})";
                case Operand.mCl:
                    return "(0xff00 + C)";
                case Operand.mnl:
                    return $"(0xff00 + 0x{this.ByteLiteral:x2})";
                case Operand.SPd:
                    var d = (sbyte)this.ByteLiteral;
                    return d > 0 ? $"SP+{d}" : $"SP{d}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(operand), operand, null);
            }
        }
    }
}
