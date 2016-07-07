namespace Axh.Retro.CPU.Common.Contracts.Exceptions
{
    /// <summary>
    /// MMU address segment overlap exception.
    /// Thrown when two address segment overlap.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Common.Contracts.Exceptions.PlatformConfigurationException" />
    public class MmuAddressSegmentOverlapException : PlatformConfigurationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MmuAddressSegmentOverlapException"/> class.
        /// </summary>
        /// <param name="addressFrom">The address from.</param>
        /// <param name="addressTo">The address to.</param>
        public MmuAddressSegmentOverlapException(ushort addressFrom, ushort addressTo)
            : base($"Overlapping configured address segments from 0x{addressFrom:x4} to 0x{addressTo:x4}")
        {
            AddressFrom = addressFrom;
            AddressTo = addressTo;
        }

        /// <summary>
        /// Gets the address of the first byte of overlaping address ranges.
        /// </summary>
        /// <value>
        /// The address of the first byte of overlaping address ranges.
        /// </value>
        public ushort AddressFrom { get; private set; }

        /// <summary>
        /// Gets the address of the last byte of overlaping address ranges.
        /// </summary>
        /// <value>
        /// The address of the last byte of overlaping address ranges.
        /// </value>
        public ushort AddressTo { get; private set; }
    }
}