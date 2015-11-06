namespace Axh.Retro.GameBoy.Devices
{
    using System;

    using Axh.Retro.GameBoy.Contracts.Devices;

    /// <summary>
    /// TODO: implemented SGB & CGB increment speeds.
    /// </summary>
    public class DividerRegister : IDividerRegister
    {
        private const int RegisterIncrementRate = 16384;

        private byte registerValue;

        private DateTime dateLastSet;

        /// <summary>
        /// This register is incremented 16384 (~16779 on SGB) times a second.
        /// Writing any value sets it to $00.
        /// It is also affected by CGB double speed.
        /// </summary>
        public byte Register
        {
            get
            {
                var timeSinceLastSet = DateTime.UtcNow - dateLastSet;
                var totalIncrements = (timeSinceLastSet.TotalSeconds * RegisterIncrementRate) % RegisterIncrementRate;

                registerValue = unchecked((byte)(registerValue + totalIncrements));
                dateLastSet = DateTime.UtcNow;

                return registerValue;
            }
            set
            {
                registerValue = 0x00;
                dateLastSet = DateTime.UtcNow;
            }
        }
    }
}
