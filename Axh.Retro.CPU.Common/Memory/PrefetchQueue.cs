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

        private readonly IMmu mmu;

        private ushort address;

        private byte[] cache;

        private int cachePointer;

        public PrefetchQueue(IMmu mmu)
        {
            this.mmu = mmu;
            address = 0x0000;
            Init();
        }

        public virtual byte NextByte()
        {
            var value = cache[cachePointer];

            if (++cachePointer == CacheSize)
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
                var bytesToRead = Math.Min(length - bytesRead, CacheSize - cachePointer);
                Array.Copy(cache, cachePointer, bytes, bytesRead, bytesToRead);
                bytesRead += bytesToRead;

                cachePointer += bytesToRead;
                if (cachePointer == CacheSize)
                {
                    NudgeCache();
                }
            }

            TotalBytesRead += length;
            return bytes;
        }

        public ushort NextWord()
        {
            if (CacheSize - cachePointer == 1)
            {
                // If there's only one byte left then we need to read it then the next byte from the next cache
                var lsb = NextByte();
                var msb = NextByte();

                return BitConverter.ToUInt16(new[] {lsb, msb}, 0);
            }

            var value = BitConverter.ToUInt16(cache, cachePointer);

            cachePointer += 2;
            if (cachePointer == CacheSize)
            {
                NudgeCache();
            }

            TotalBytesRead += 2;
            return value;
        }

        public virtual void ReBuildCache(ushort newAddress)
        {
            // TODO: check if we can re-use this cache, make sure TotalBytesRead is still reset

            address = newAddress;
            Init();
        }

        public int TotalBytesRead { get; private set; }

        private void Init()
        {
            cachePointer = 0;
            cache = mmu.ReadBytes(address, CacheSize);
            TotalBytesRead = 0;
        }

        private void NudgeCache()
        {
            address = unchecked((ushort) (address + CacheSize));
            cachePointer = 0;
            cache = mmu.ReadBytes(address, CacheSize);
        }
    }
}