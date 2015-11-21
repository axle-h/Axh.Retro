namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
{
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Registers.Interfaces;

    /// <summary>
    /// JOYPAD 0xff00 Register for reading joy pad info. (R/W)
    /// </summary>
    public interface ICoreJoyPad : IJoyPad, IRegister
    {
    }
}
