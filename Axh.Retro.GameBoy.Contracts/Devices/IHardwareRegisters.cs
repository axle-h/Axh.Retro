namespace Axh.Retro.GameBoy.Contracts.Devices
{
    using Axh.Retro.CPU.Common.Contracts.Memory;

    public interface IHardwareRegisters
    {
        IJoyPad JoyPad { get; }

        ISerialPort SerialPort { get; }
    }
}