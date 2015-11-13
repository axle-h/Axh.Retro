namespace Axh.Retro.CPU.Z80.Core.Decode
{
    internal class DecodeResult
    {
        public DecodeResult(Opcode opcode)
        {
            Opcode = opcode;
        }

        public DecodeResult(Opcode opcode, Operand operand1)
        {
            Opcode = opcode;
            Operand1 = operand1;
        }
        
        public DecodeResult(Opcode opcode, Operand operand1, Operand operand2)
        {
            Opcode = opcode;
            Operand1 = operand1;
            Operand2 = operand2;
        }

        public DecodeResult AddFlagTest(FlagTest flagTest)
        {
            this.FlagTest = flagTest;
            return this;
        }

        public DecodeResult AddLiteral(byte literal)
        {
            this.ByteLiteral = literal;
            return this;
        }

        public DecodeResult AddLiteral(ushort literal)
        {
            this.WordLiteral = literal;
            return this;
        }

        public DecodeResult AddDisplacement(byte displacement)
        {
            this.Displacement = (sbyte)displacement;
            return this;
        }

        public Opcode Opcode { get; }

        public Operand Operand1 { get; }

        public Operand Operand2 { get; }

        public FlagTest FlagTest { get; private set; }

        public byte ByteLiteral { get; private set; }

        public ushort WordLiteral { get; private set; }

        public sbyte Displacement { get; private set; }
    }
}
