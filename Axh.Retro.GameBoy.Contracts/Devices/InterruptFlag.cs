using System;

namespace Axh.Retro.GameBoy.Contracts.Devices
{
    [Flags]
    public enum InterruptFlag : byte
    {
        None = 0,
        VerticalBlank = 0x01,
        LcdStatusTriggers = 0x02,
        TimerOverflow = 0x04,
        SerialLink = 0x08,
        JoyPadPress = 0x10
    }
}