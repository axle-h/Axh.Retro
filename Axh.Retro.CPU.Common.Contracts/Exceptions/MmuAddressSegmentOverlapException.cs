namespace Axh.Retro.CPU.Common.Contracts.Exceptions
{
    public class MmuAddressSegmentOverlapException : PlatformConfigurationException
    {
        public MmuAddressSegmentOverlapException(ushort addressFrom, ushort addressTo)
            : base($"Overlapping configured address segments from 0x{addressFrom:x4} to 0x{addressTo:x4}")
        {
            AddressFrom = addressFrom;
            AddressTo = addressTo;
        }

        public ushort AddressFrom { get; private set; }

        public ushort AddressTo { get; private set; }
    }
}