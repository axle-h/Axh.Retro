namespace Axh.Emulation.CPU.Z80.Contracts.Registers
{
    public interface IFlagsRegister
    {
        byte Register { get; set; }
        bool Sign { get; set; }
        bool Zero { get; set; }
        bool Flag5 { get; set; }
        bool HalfCarry { get; set; }
        bool Flag3 { get; set; }
        bool ParityOverflow { get; set; }
        bool Subtract { get; set; }
        bool Carry { get; set; }
        void SetUndocumentedFlags(byte res);
        void SetResultFlags(byte res);
        void SetParityFlags(byte res);
        void ResetFlags();
        void SetFlags();
    }
}