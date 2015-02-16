namespace Axh.Emulation.CPU.X80.Contracts.State
{
    using Axh.Emulation.CPU.X80.Contracts.Registers;

    public struct Z80RegisterState
    {
        public GeneralPurposeRegisterState PrimaryRegisterState { get; set; }

        public GeneralPurposeRegisterState AlternativeRegisterState { get; set; }
        
        public bool IsAlternative;

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
