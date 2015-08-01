namespace Axh.Retro.CPU.X80.Contracts.State
{
    public struct RegisterState
    {
        public GeneralPurposeRegisterState GeneralPurposeRegisterState { get; set; }

        public ushort StackPointer { get; set; }
        public ushort ProgramCounter { get; set; }
    }
}
