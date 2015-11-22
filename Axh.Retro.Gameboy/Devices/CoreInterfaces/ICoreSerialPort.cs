namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
{
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Registers.Interfaces;

    public interface ICoreSerialPort : ISerialPort, IRegister
    {
        /// <summary>
        /// 8 Bits of data to be read/written
        /// Transmitting and receiving serial data is done simultaneously.
        /// </summary>
        IRegister SerialData { get; }
    }
}