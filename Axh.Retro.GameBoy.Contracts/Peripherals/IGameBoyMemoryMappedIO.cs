using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Contracts.Graphics;

namespace Axh.Retro.GameBoy.Contracts.Peripherals
{
    /// <summary>
    /// The GameBoy IO interface peripheral.
    /// </summary>
    /// <seealso cref="Axh.Retro.CPU.Z80.Contracts.Peripherals.IMemoryMappedPeripheral" />
    public interface IGameBoyMemoryMappedIo : IMemoryMappedPeripheral
    {
        /// <summary>
        /// Gets the hardware registers.
        /// </summary>
        /// <value>
        /// The hardware registers.
        /// </value>
        IHardwareRegisters HardwareRegisters { get; }

        /// <summary>
        /// Gets the GPU.
        /// </summary>
        /// <value>
        /// The GPU.
        /// </value>
        IGpu Gpu { get; }
    }
}