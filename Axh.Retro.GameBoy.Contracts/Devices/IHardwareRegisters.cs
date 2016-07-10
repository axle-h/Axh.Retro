using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.GameBoy.Contracts.Devices
{
    /// <summary>
    /// GameBoy hardware registers.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Common.Contracts.Memory.IReadableAddressSegment" />
    /// <seealso cref="Axh.Retro.CPU.Common.Contracts.Memory.IWriteableAddressSegment" />
    public interface IHardwareRegisters : IReadableAddressSegment, IWriteableAddressSegment
    {
        /// <summary>
        /// Gets the joy pad.
        /// </summary>
        /// <value>
        /// The joy pad.
        /// </value>
        IJoyPad JoyPad { get; }

        /// <summary>
        /// Gets the serial port.
        /// </summary>
        /// <value>
        /// The serial port.
        /// </value>
        ISerialPort SerialPort { get; }
    }
}