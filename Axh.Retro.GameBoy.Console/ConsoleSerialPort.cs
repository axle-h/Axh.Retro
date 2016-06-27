using Axh.Retro.GameBoy.Contracts.Devices;

namespace Axh.Retro.GameBoy.Console
{
    public class ConsoleSerialPort : ISerialPort
    {
        public void Connect(ISerialPort serialPort)
        {
        }

        public void Disconnect()
        {
        }

        public byte Transfer(byte value)
        {
            var c = (char) value;
            System.Console.Write(c);
            return 0x00;
        }
    }
}