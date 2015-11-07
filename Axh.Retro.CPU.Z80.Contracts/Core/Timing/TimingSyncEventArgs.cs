namespace Axh.Retro.CPU.Z80.Contracts.Core.Timing
{
    using System;

    public class TimingSyncEventArgs : EventArgs
    {
        public TimingSyncEventArgs(InstructionTimings instructionTimings)
        {
            InstructionTimings = instructionTimings;
        }

        public InstructionTimings InstructionTimings { get; }
    }
}
