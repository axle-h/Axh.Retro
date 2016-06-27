using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.CPU.Common.Tests.Mmu
{
    public interface IReadableWriteableAddressSegment : IReadableAddressSegment, IWriteableAddressSegment
    {
    }
}