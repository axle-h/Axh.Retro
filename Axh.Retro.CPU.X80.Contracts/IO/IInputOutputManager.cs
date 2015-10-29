namespace Axh.Retro.CPU.X80.Contracts.IO
{
    public interface IInputOutputManager
    {
        /// <summary>
        /// Read the next byte from this a peripheral at port
        /// </summary>
        /// <param name="port">The port of the device to read from</param>
        /// <param name="addressMsb">The most significant byte of the address bus (the LSB is used as the IO port)</param>
        /// <returns></returns>
        byte ReadByte(byte port, byte addressMsb);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port">The port of teh device to write to</param>
        /// <param name="addressMsb">The most significant byte of the address bus (the LSB is used as the IO port)</param>
        /// <param name="value">The byte to write</param>
        void WriteByte(byte port, byte addressMsb, byte value);
    }
}
