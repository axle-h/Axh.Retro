using System;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Registers
{
    /// <summary>
    ///     Lazy i.e. will calcualte the value of the register on read. As opposed to tracking teh timings in real time.
    ///     It also assumes the CPU is running real time.
    ///     TODO: implement SGB & CGB increment speeds.
    /// </summary>
    public class LazyDividerRegister : DividerRegisterBase
    {
        private const int RegisterIncrementRate = 16384;

        private DateTime dateLastSet;

        private byte registerValue;

        /// <summary>
        ///     This register is incremented 16384 (~16779 on SGB) times a second.
        ///     Writing any value sets it to $00.
        ///     It is also affected by CGB double speed.
        /// </summary>
        public override byte Register
        {
            get
            {
                var timeSinceLastSet = DateTime.UtcNow - dateLastSet;
                var totalIncrements = timeSinceLastSet.TotalSeconds * RegisterIncrementRate % RegisterIncrementRate;

                registerValue = unchecked((byte) (registerValue + totalIncrements));
                dateLastSet = DateTime.UtcNow;

                return registerValue;
            }
            set
            {
                // Writing anything to this register resets it
                registerValue = 0x00;
                dateLastSet = DateTime.UtcNow;
            }
        }
    }
}