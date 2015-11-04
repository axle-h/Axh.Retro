namespace Axh.Retro.CPU.Z80.Contracts.State
{
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public struct Z80RegisterState
    {
        public GeneralPurposeRegisterState PrimaryGeneralPurposeRegisterState { get; set; }

        public GeneralPurposeRegisterState AlternativeGeneralPurposeRegisterState { get; set; }

        public AccumulatorAndFlagsRegisterState PrimaryAccumulatorAndFlagsRegisterState { get; set; }

        public AccumulatorAndFlagsRegisterState AlternativeAccumulatorAndFlagsRegisterState { get; set; }

        public bool IsGeneralPurposeAlternative;

        public bool IsAccumulatorAndFlagsAlternative;

        //Index Registers
        public ushort IX { get; set; }
        public ushort IY { get; set; }

        /// <summary>
        /// Interrupt vector
        /// </summary>
        public byte I { get; set; }

        /// <summary>
        /// Memory refresh
        /// </summary>
        public byte R { get; set; }

        public ushort StackPointer { get; set; }
        public ushort ProgramCounter { get; set; }

        //Interrupt Flip-Flops
        public bool InterruptFlipFlop1 { get; set; }
        public bool InterruptFlipFlop2 { get; set; }
        public InterruptMode InterruptMode { get; set; }
    }
}
