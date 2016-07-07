using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Registers
{
    /// <summary>
    /// FF01 - SB - Serial transfer data (R/W)
    /// </summary>
    public abstract class SerialPortBase : ISerialPortRegister
    {
        protected ISerialPort ConnectedSerialPort;

        protected SerialPortBase()
        {
            SerialData = new Interfaces.SimpleRegister(0xff01, "Serial data (SIODATA R/W)");
        }

        /// <summary>
        /// 8 Bits of data to be read/written
        /// Transmitting and receiving serial data is done simultaneously.
        /// </summary>
        public IRegister SerialData { get; }

        public abstract byte Transfer(byte value);

        /// <summary>
        /// Address of control register
        /// </summary>
        public ushort Address => 0xff02;

        public string Name => "Serial Control (SIOCONT R/W)";

        public abstract byte Register { get; set; }

        public string DebugView => ToString();

        public virtual void Connect(ISerialPort serialPort)
        {
            ConnectedSerialPort = serialPort;
        }

        public virtual void Disconnect()
        {
            ConnectedSerialPort = null;
        }

        public override string ToString()
        {
            return $"{SerialData}\n{Name} ({Address}) = {Register}";
        }
    }
}