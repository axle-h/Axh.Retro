namespace Axh.Retro.GameBoy.Contracts.Devices
{
    using System;

    public interface ISerialPort
    {
        /// <summary>
        /// 8 Bits of data to be read/written
        /// Transmitting and receiving serial data is done simultaneously.
        /// </summary>
        byte SerialData { get; set; }

        /// <summary>
        /// Bit 7 - Transfer Start Flag
        /// 0: Non transfer
        /// 1: Start transfer
        /// Bit 0 - Shift Clock
        /// 0: External Clock(500KHz Max.)
        /// 1: Internal Clock(8192Hz)
        /// Transfer is initiated by setting the
        /// Transfer Start Flag.This bit may be read
        /// and is automatically set to 0 at the end
        /// of Transfer.
        /// </summary>
        byte ControlRegister { get; set; }

        /// <summary>
        /// Connect to external serial port.
        /// </summary>
        /// <param name="serialPort"></param>
        void Connect(ISerialPort serialPort);

        /// <summary>
        /// Disconnect from external serial port.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Transfer a byte to external serial port and read a byte from external serial port.
        /// Only the serial port running the GB internal clock should be calling this method.
        /// </summary>
        /// <param name="value">The value to write to this port.</param>
        /// <returns>The value to transfer to the connected port.</returns>
        byte Transfer(byte value);
    }
    
}