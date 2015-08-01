namespace Axh.Retro.CPU.X80.Contracts.Exceptions
{
    public class MmuAddressSegmentGapException : PlatformConfigurationException
    {
        public MmuAddressSegmentGapException(ushort addressFrom, ushort addressTo)
            : base(string.Format("Gap in configured address segments from 0x{0:x4} to 0x{1:x4}", addressFrom, addressTo))
        {
            AddressFrom = addressFrom;
            AddressTo = addressTo;
        }

        public ushort AddressFrom { get; private set; }

        public ushort AddressTo { get; private set; }
    }
}
