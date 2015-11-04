namespace Axh.Retro.CPU.Z80.Contracts.Core.Timing
{
    public struct InstructionTimings
    {
        public InstructionTimings(int machineCycles, int throttlingStates) : this()
        {
            this.MachineCycles = machineCycles;
            this.ThrottlingStates = throttlingStates;
        }

        public int MachineCycles { get; set; }

        public int ThrottlingStates { get; set; }

        public static InstructionTimings operator +(InstructionTimings t0, InstructionTimings t1)
        {
            return new InstructionTimings(t0.MachineCycles + t1.MachineCycles, t0.ThrottlingStates + t1.ThrottlingStates);
        }
    }
}
