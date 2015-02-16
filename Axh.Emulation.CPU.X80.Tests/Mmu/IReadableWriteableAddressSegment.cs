namespace Axh.Emulation.CPU.X80.Tests.Mmu
{
    using Axh.Emulation.CPU.X80.Contracts.Memory;

    public interface IReadableWriteableAddressSegment : IReadableAddressSegment, IWriteableAddressSegment
    {
    }
}
