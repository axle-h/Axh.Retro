namespace Axh.Emulation.CPU.Z80.Contracts.Registers
{
    public interface IRegisterSet
    {
        byte A { get; set; }
        byte B { get; set; }
        byte C { get; set; }
        byte D { get; set; }
        byte E { get; set; }
        byte H { get; set; }
        byte L { get; set; }
        IFlagsRegister Flags { get; }
        ushort AF { get; set; }
        ushort BC { get; set; }
        ushort DE { get; set; }
        ushort HL { get; set; }
    }
}