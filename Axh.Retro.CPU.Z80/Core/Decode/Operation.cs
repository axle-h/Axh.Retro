namespace Axh.Retro.CPU.Z80.Core.Decode
{
    internal class Operation
    {
        public Operation(Opcode opcode)
        {
            Opcode = opcode;
        }

        public Operation(Opcode opcode, Operand operand1)
        {
            Opcode = opcode;
            Operand1 = operand1;
        }
        
        public Operation(Opcode opcode, Operand operand1, Operand operand2)
        {
            Opcode = opcode;
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
        
        public Opcode Opcode { get; }

        public Operand Operand1 { get; set; }

        public Operand Operand2 { get; set; }

        public FlagTest FlagTest { get; private set; }

        public byte ByteLiteral { get; private set; }

        public ushort WordLiteral { get; private set; }

        public sbyte Displacement { get; private set; }

        public OpCodeMeta OpCodeMeta { get; private set; }
    }
}
