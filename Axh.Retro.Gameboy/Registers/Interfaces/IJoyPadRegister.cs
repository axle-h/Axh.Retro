using Axh.Retro.GameBoy.Contracts.Devices;

namespace Axh.Retro.GameBoy.Registers.Interfaces
{
    /// <summary>
    /// The GameBoy joypad register.
    /// </summary>
    /// <seealso cref="Axh.Retro.GameBoy.Contracts.Devices.IJoyPad" />
    /// <seealso cref="Axh.Retro.GameBoy.Registers.Interfaces.IRegister" />
    public interface IJoyPadRegister : IJoyPad, IRegister
    {
    }
}