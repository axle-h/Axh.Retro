using System;
using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.CPU.Z80.Contracts.Peripherals
{
    /// <summary>
    /// A peripheral called through the 8080's IO interface.
    /// IO ports on the 8080 are produced form the LSB of the address bus. There are 256 possible IO ports.
    /// The data bus on the 8080 is 8 bits wide so all communication is done byte-wise
    /// </summary>
    public interface IPeripheral : IDisposable
    {
        void Signal(ControlSignal signal);
        
    }
}
