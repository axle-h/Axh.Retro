﻿namespace Axh.Retro.CPU.Z80.Contracts.Peripherals
{
    /// <summary>
    /// A peripheral control signal.
    /// </summary>
    public enum ControlSignal
    {
        /// <summary>
        /// Halt the peripheral.
        /// </summary>
        Halt,

        /// <summary>
        /// Resume the peripheral.
        /// </summary>
        Resume
    }
}