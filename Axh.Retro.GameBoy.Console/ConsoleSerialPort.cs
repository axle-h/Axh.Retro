namespace Axh.Retro.GameBoy.Console
{
    using System;

    using Axh.Retro.GameBoy.Contracts.Devices;

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
            var c = (char)value;
            Console.Write(c);
            return 0x00;
        }
    }
}
