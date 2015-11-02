namespace Axh.Retro.CPU.X80.Contracts.Peripherals
{
    using Axh.Retro.CPU.X80.Contracts.Memory;

    public interface IMemoryMappedPeripheral : IPeripheral, IAddressSegment
    {
    }
}
