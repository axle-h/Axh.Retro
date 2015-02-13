namespace Axh.Emulation.CPU.Z80.Contracts.Memory
{
    public interface IAddressSegment
    {
        ushort Address { get; }

        ushort Length { get; }

        bool IsWriteable { get; }

        byte ReadByte(ushort address);

        ushort ReadWord(ushort address);

        void WriteByte(ushort address, byte value);

        void WriteWord(ushort address, ushort word);

        void WriteBytes(ushort address, byte[] values);
    }
}
