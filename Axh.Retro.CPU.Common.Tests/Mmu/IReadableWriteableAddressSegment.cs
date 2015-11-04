namespace Axh.Retro.CPU.Common.Tests.Mmu
{
    using Axh.Retro.CPU.Common.Contracts.Memory;

    public interface IReadableWriteableAddressSegment : IReadableAddressSegment, IWriteableAddressSegment
    {
    }
}
