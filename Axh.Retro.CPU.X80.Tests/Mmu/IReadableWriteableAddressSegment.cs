using Axh.Retro.CPU.X80.Contracts.Memory;

namespace Axh.Retro.CPU.X80.Tests.Mmu
{
    using Retro.CPU.X80.Contracts.Memory;

    public interface IReadableWriteableAddressSegment : IReadableAddressSegment, IWriteableAddressSegment
    {
    }
}
