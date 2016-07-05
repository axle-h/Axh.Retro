using System;
using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.CPU.Common.Memory
{
    /// <summary>
    ///     Should speed up sequential access to the MMU e.g. reading op-codes
    /// </summary>
    public class PrefetchQueue : IPrefetchQueue
    {
        private const int CacheSize = 64;

        private readonly IMmu _mmu;

        private ushort _address;

        private byte[] _cache;

        private int _cachePointer;

        public PrefetchQueue(IMmu mmu)
        {
            _mmu = mmu;
            _address = 0x0000;
            Init();
        }

        public virtual byte NextByte()
        {
            var value = _cache[_cachePointer];

            if (++_cachePointer == CacheSize)
            {
                NudgeCache();
            }

            TotalBytesRead++;
            return value;
        }

        public virtual byte[] NextBytes(int length)
        {
            var bytes = new byte[length];
            var bytesRead = 0;

            while (bytesRead < length)
            {
                var bytesToRead = Math.Min(length - bytesRead, CacheSize - _cachePointer);
                Array.Copy(_cache, _cachePointer, bytes, bytesRead, bytesToRead);
                bytesRead += bytesToRead;

                _cachePointer += bytesToRead;
                if (_cachePointer == CacheSize)
                {
                    NudgeCache();
                }
            }

            TotalBytesRead += length;
            return bytes;
        }

        public ushort NextWord()
        {
            if (CacheSize - _cachePointer == 1)
            {
                // If there's only one byte left then we need to read it then the next byte from the next cache
                var lsb = NextByte();
                var msb = NextByte();

                return BitConverter.ToUInt16(new[] {lsb, msb}, 0);
            }

            var value = BitConverter.ToUInt16(_cache, _cachePointer);

            _cachePointer += 2;
            if (_cachePointer == CacheSize)
            {
                NudgeCache();
            }

            TotalBytesRead += 2;
            return value;
        }

        public virtual void ReBuildCache(ushort newAddress)
        {
            // TODO: check if we can re-use this cache, make sure TotalBytesRead is still reset

            _address = newAddress;
            Init();
        }

        public int TotalBytesRead { get; private set; }

        private void Init()
        {
            _cachePointer = 0;
            _cache = _mmu.ReadBytes(_address, CacheSize);
            TotalBytesRead = 0;
        }

        private void NudgeCache()
        {
            _address = unchecked((ushort) (_address + CacheSize));
            _cachePointer = 0;
            _cache = _mmu.ReadBytes(_address, CacheSize);
        }
    }
}