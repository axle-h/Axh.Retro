using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.GameBoy.Contracts.Devices;

namespace Axh.Retro.GameBoy.Registers.Interfaces
{
    /// <summary>
    /// FFFF - IE - Interrupt Enable (R/W)
    /// </summary>
    public interface IInterruptEnableRegister : IRegister, IReadableAddressSegment, IWriteableAddressSegment
    {
        /// <summary>
        /// Bit 0: V-Blank  Interrupt Enable  (INT 40h)  (1=Enable)
        /// </summary>
        bool VerticalBlank { get; }

        /// <summary>
        /// Bit 1: LCD STAT Interrupt Enable  (INT 48h)  (1=Enable)
        /// </summary>
        bool LcdStatusTriggers { get; }

        /// <summary>
        /// Bit 2: Timer    Interrupt Enable  (INT 50h)  (1=Enable)
        /// </summary>
        bool TimerOverflow { get; }

        /// <summary>
        /// Bit 3: Serial   Interrupt Enable  (INT 58h)  (1=Enable)
        /// </summary>
        bool SerialLink { get; }

        /// <summary>
        /// Bit 4: Joypad   Interrupt Enable  (INT 60h)  (1=Enable)
        /// </summary>
        bool JoyPadPress { get; }

        bool InterruptEnabled(InterruptFlag flag);
    }
}