using System;
using System.Collections.Generic;

namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    /// <summary>
    ///     A normal address range.
    ///     I.e. min > max due to mod ushort
    /// </summary>
    public struct AddressRange
    {
        private readonly ushort _address;
        private readonly ushort _maxAddress;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddressRange" /> struct.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="maxAddress">The maximum address.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public AddressRange(ushort address, ushort maxAddress) : this()
        {
            if (address > maxAddress)
            {
                throw new ArgumentException($"Cannot create normal range: {maxAddress} > {address}");
            }
            _address = address;
            _maxAddress = maxAddress;
        }

        /// <summary>
        ///     Determines whether the specified address range intersects with this one.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns></returns>
        public bool Intersects(AddressRange range)
            => Math.Max(range._address, _address) <= Math.Min(range._maxAddress, _maxAddress);

        /// <summary>
        ///     Gets all address ranges required to satisfy the specified address, length pair.
        ///     If the range overflows an unsigned 16-bit number then two ranges will be returned.
        /// </summary>
        /// <param name="address">The start address of the range.</param>
        /// <param name="length">The address range length.</param>
        /// <returns></returns>
        public static IEnumerable<AddressRange> GetRanges(ushort address, ushort length)
        {
            var maxAddress = unchecked((ushort) (address + length - 1));
            if (maxAddress >= address)
            {
                yield return new AddressRange(address, maxAddress);
            }
            else
            {
                yield return new AddressRange(ushort.MinValue, maxAddress);
                yield return new AddressRange(address, ushort.MaxValue);
            }
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => $"({_address}, {_maxAddress})";
    }
}