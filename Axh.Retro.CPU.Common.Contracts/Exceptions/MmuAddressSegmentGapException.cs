namespace Axh.Retro.CPU.Common.Contracts.Exceptions
{
    public class MmuAddressSegmentGapException : PlatformConfigurationException
    {
        public MmuAddressSegmentGapException(ushort addressFrom, ushort addressTo)
            : base($"Gap in configured address segments from 0x{addressFrom:x4} to 0x{addressTo:x4}")
        {
            AddressFrom = addressFrom;
            AddressTo = addressTo;
        }

        public ushort AddressFrom { get; private set; }

        public ushort AddressTo { get; private set; }
    }
}
