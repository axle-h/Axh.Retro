namespace Axh.Retro.CPU.X80.Registers
{
    using Axh.Retro.CPU.X80.Contracts.Registers;
    using Axh.Retro.CPU.X80.Contracts.State;
    using Axh.Retro.CPU.X80.Util;

    public class AccumulatorAndFlagsRegisterSet : IAccumulatorAndFlagsRegisterSet
    {
        public AccumulatorAndFlagsRegisterSet(IFlagsRegister flagsRegister)
        {
            this.Flags = flagsRegister;
        }

        public byte A { get; set; }
        
        public IFlagsRegister Flags { get; }

        public ushort AF
        {
            get
            {
                return BitConverterHelpers.To16Bit(this.A, this.Flags.Register);
            }
            set
            {
                var bytes = BitConverterHelpers.To8Bit(value);
                this.A = bytes[1];
                this.Flags.Register = bytes[0];
            }
        }

        public void Reset()
        {
            A = 0;
            this.Flags.ResetFlags();
        }

        public void ResetToState(AccumulatorAndFlagsRegisterState state)
        {
            A = state.A;
            Flags.Register = state.F;
        }

        public AccumulatorAndFlagsRegisterState GetRegisterState()
        {
            return new AccumulatorAndFlagsRegisterState { A = A, F = Flags.Register };
        }
    }
}
