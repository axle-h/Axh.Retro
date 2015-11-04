namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    public interface IAddressSegment
    {
        MemoryBankType Type { get; }
        ushort Address { get; }
        ushort Length { get; }
    }
}
