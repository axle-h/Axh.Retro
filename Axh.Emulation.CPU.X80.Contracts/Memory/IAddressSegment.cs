namespace Axh.Emulation.CPU.X80.Contracts.Memory
{
    public interface IAddressSegment
    {
        ushort Address { get; }
        ushort Length { get; }
    }
}
