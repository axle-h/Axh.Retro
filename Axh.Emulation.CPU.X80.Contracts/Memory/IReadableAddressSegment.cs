namespace Axh.Emulation.CPU.X80.Contracts.Memory
{
    public interface IReadableAddressSegment : IAddressSegment
    {
        byte ReadByte(ushort address);
        ushort ReadWord(ushort address);
        byte[] ReadBytes(ushort address, int length);
    }
}
