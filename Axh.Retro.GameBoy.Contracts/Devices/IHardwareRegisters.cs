namespace Axh.Retro.GameBoy.Contracts.Devices
{
    public interface IHardwareRegisters
    {
        IJoyPad JoyPad { get; }

        ISerialPort SerialPort { get; }
    }
}