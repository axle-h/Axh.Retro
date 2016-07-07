using System;
using System.Collections.Generic;
using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.CPU.Z80.Contracts.Peripherals
{
    public interface IPeripheralManager : IDisposable
    {
        /// <summary>
        /// Gets all memory mapped peripherals.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAddressSegment> MemoryMap { get; }

        /// <summary>
        /// Read the next byte from the peripheral at IO port
        /// </summary>
        /// <param name="port">The port of the device to read from</param>
        /// <param name="addressMsb">The most significant byte of the address bus (the LSB is used as the IO port)</param>
        /// <returns></returns>
        byte ReadByteFromPort(byte port, byte addressMsb);

        /// <summary>
        /// Write a byte to the peripheral at IO port
        /// </summary>
        /// <param name="port">The port of the device to write to</param>
        /// <param name="addressMsb">The most significant byte of the address bus (the LSB is used as the IO port)</param>
        /// <param name="value">The byte to write</param>
        void WriteByteToPort(byte port, byte addressMsb, byte value);

        /// <summary>
        /// Signal all peripherals
        /// </summary>
        void Signal(ControlSignal signal);

        /// <summary>
        /// Retrieve peripheral of specified type.
        /// </summary>
        /// <typeparam name="TPeripheral"></typeparam>
        /// <returns></returns>
        TPeripheral PeripheralOfType<TPeripheral>() where TPeripheral : IPeripheral;
    }
}