namespace Axh.Retro.CPU.Z80.Core.Decode
{
    internal struct DecodeResult
    {
        public DecodeResult(Opcode opcode) : this()
        {
            Opcode = opcode;
        }

        public DecodeResult(Opcode opcode, Operand operand1) : this()
        {
            Opcode = opcode;
            Operand1 = operand1;
        }

        public DecodeResult(Opcode opcode, Operand operand1, ushort literal) : this()
        {
            Opcode = opcode;
            Operand1 = operand1;
            WordLiteral = literal;
        }

        public DecodeResult(Opcode opcode, FlagTest flagTest) : this()
        {
            Opcode = opcode;
            FlagTest = flagTest;
        }

        public DecodeResult(Opcode opcode, Operand operand1, FlagTest flagTest) : this()
        {
            Opcode = opcode;
            Operand1 = operand1;
            FlagTest = flagTest;
        }

        public DecodeResult(Opcode opcode, Operand operand1, FlagTest flagTest, ushort literal) : this()
        {
            Opcode = opcode;
            Operand1 = operand1;
            FlagTest = flagTest;
            WordLiteral = literal;
        }

        public DecodeResult(Opcode opcode, Operand operand1, Operand operand2) : this()
        {
            Opcode = opcode;
            Operand1 = operand1;
            Operand2 = operand2;
        }

        public DecodeResult(Opcode opcode, Operand operand1, Operand operand2, byte literal) : this()
        {
            Opcode = opcode;
            Operand1 = operand1;
            Operand2 = operand2;
            ByteLiteral = literal;
        }

        public DecodeResult(Opcode opcode, Operand operand1, Operand operand2, ushort literal) : this()
        {
            Opcode = opcode;
            Operand1 = operand1;
            Operand2 = operand2;
            WordLiteral = literal;
        }

        public Opcode Opcode { get; }

        public Operand Operand1 { get; }

        public Operand Operand2 { get; }

        public FlagTest FlagTest { get; }

        public byte ByteLiteral { get; }

        public ushort WordLiteral { get; }
    }
}
