using Axh.Retro.GameBoy.Contracts.Devices;

namespace Axh.Retro.GameBoy.Registers.Interfaces
{
    public interface ISerialPortRegister : ISerialPort, IRegister
    {
        /// <summary>
        ///     8 Bits of data to be read/written
        ///     Transmitting and receiving serial data is done simultaneously.
        /// </summary>
        IRegister SerialData { get; }
    }
}