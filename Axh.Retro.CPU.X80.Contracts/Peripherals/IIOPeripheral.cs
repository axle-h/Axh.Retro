namespace Axh.Retro.CPU.X80.Contracts.Peripherals
{
    public interface IIOPeripheral : IPeripheral
    {
        /// <summary>
        /// The IO port of this peripheral
        /// </summary>
        byte Port { get; }

        /// <summary>
        /// Read the next byte from this IO device
        /// </summary>
        /// <param name="addressMsb">The most significant byte of the address bus (the LSB is used as the IO port)</param>
        /// <returns></returns>
        byte ReadByte(byte addressMsb);

        /// <summary>
        /// Write a byte to this device
        /// </summary>
        /// <param name="addressMsb">The most significant byte of the address bus (the LSB is used as the IO port)</param>
        /// <param name="value">The byte to write</param>
        void WriteByte(byte addressMsb, byte value);
    }
}
