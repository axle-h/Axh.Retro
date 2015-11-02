namespace Axh.Retro.CPU.X80.Contracts.Memory
{
    public interface IAddressSegment
    {
        MemoryBankType Type { get; }
        ushort Address { get; }
        ushort Length { get; }
    }
}
