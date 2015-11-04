namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    public interface IReadableAddressSegment : IAddressSegment
    {
        byte ReadByte(ushort address);
        ushort ReadWord(ushort address);
        byte[] ReadBytes(ushort address, int length);
    }
}
