namespace Axh.Retro.GameBoy.Contracts.Devices
{
    public interface IDividerRegister
    {
        /// <summary>
        /// This register is incremented 16384 (~16779 on SGB) times a second.
        /// It is also affected by CGB double speed.
        /// Writing any value sets it to $00.
        /// </summary>
        byte Register { get; set; }
    }
}