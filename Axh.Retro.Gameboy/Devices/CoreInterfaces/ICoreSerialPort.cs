using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
{
    public interface ICoreSerialPort : ISerialPort, IRegister
    {
        /// <summary>
        ///     8 Bits of data to be read/written
        ///     Transmitting and receiving serial data is done simultaneously.
        /// </summary>
        IRegister SerialData { get; }
    }
}