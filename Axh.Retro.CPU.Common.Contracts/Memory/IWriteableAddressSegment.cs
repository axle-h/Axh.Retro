namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    public interface IWriteableAddressSegment : IAddressSegment
    {
        void WriteByte(ushort address, byte value);
        void WriteWord(ushort address, ushort word);
        void WriteBytes(ushort address, byte[] values);
    }
}