namespace Axh.Emulation.CPU.Z80.Contracts.Memory
{
    public interface IAddressSegment
    {
        ushort Address { get; }
        ushort Length { get; }
        bool IsWriteable { get; }
        byte ReadByte(ushort address);
        ushort ReadWord(ushort address);
        byte[] ReadBytes(ushort address, int length);
        ushort[] ReadWords(ushort address, int length);
        void WriteByte(ushort address, byte value);
        void WriteWord(ushort address, ushort word);
        void WriteBytes(ushort address, byte[] values);
        void WriteWords(ushort address, ushort[] values);
    }
}
