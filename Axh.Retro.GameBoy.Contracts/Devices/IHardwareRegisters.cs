using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.GameBoy.Contracts.Devices
{
    public interface IHardwareRegisters : IReadableAddressSegment, IWriteableAddressSegment
    {
        IJoyPad JoyPad { get; }

        ISerialPort SerialPort { get; }
    }
}