namespace Axh.Retro.CPU.Common.Contracts.Exceptions
{
    /// <summary>
    /// MMU address segment gap exception.
    /// Thrown when the address range has not been completely filled by address segments.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Common.Contracts.Exceptions.PlatformConfigurationException" />
    public class MmuAddressSegmentGapException : PlatformConfigurationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MmuAddressSegmentGapException"/> class.
        /// </summary>
        /// <param name="addressFrom">The address from.</param>
        /// <param name="addressTo">The address to.</param>
        public MmuAddressSegmentGapException(ushort addressFrom, ushort addressTo)
            : base($"Gap in configured address segments from 0x{addressFrom:x4} to 0x{addressTo:x4}")
        {
            AddressFrom = addressFrom;
            AddressTo = addressTo;
        }

        /// <summary>
        /// Gets the address of the first byte of the gap.
        /// </summary>
        /// <value>
        /// The address of the first byte of the gap.
        /// </value>
        public ushort AddressFrom { get; private set; }

        /// <summary>
        /// Gets the address of the last byte of the gap.
        /// </summary>
        /// <value>
        /// The address of the last byte of the gap.
        /// </value>
        public ushort AddressTo { get; private set; }
    }
}