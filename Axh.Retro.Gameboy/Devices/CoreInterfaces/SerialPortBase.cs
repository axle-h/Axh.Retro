namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
{
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Registers.Interfaces;

    /// <summary>
    /// FF01 - SB - Serial transfer data (R/W)
    /// </summary>
    public abstract class SerialPortBase : ISerialPort, IRegister
    {
        /// <summary>
        /// 8 Bits of data to be read/written
        /// Transmitting and receiving serial data is done simultaneously.
        /// </summary>
        protected IRegister SerialData;

        protected SerialPortBase()
        {
            SerialData = new SimpleRegister(0xff01, "Serial data (SIODATA R/W)");
        }

        public abstract void Connect(ISerialPort serialPort);

        public abstract void Disconnect();

        public abstract byte Transfer(byte value);

        /// <summary>
        /// Address of control register
        /// </summary>
        public ushort Address => 0xff02;

        public string Name => "Serial Control (SIOCONT R/W)";

        public abstract byte Register { get; set; }

        public string DebugView => this.ToString();

        public override string ToString()
        {
            return $"{this.SerialData}\n{Name} ({Address}) = {Register}";
        }
    }
}
