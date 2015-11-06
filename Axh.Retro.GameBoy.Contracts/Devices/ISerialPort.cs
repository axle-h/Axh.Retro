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

        void Connect(ISerialPort serialPort);

        void Disconnect();

        byte Transfer(byte value);
    }
    
}