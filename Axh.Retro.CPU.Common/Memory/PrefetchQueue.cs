namespace Axh.Retro.CPU.Common.Memory
{
    using System;
    using System.IO;

    using Axh.Retro.CPU.Common.Contracts.Memory;

    /// <summary>
    /// Should speed up sequential access to the MMU e.g. reading op-codes
    /// </summary>
    public class PrefetchQueue : IPrefetchQueue
    {
        private const int CacheSize = 64;

        private readonly IMmu mmu;

        private byte[] cache;

        private int cachePointer;

        private ushort address;

        public PrefetchQueue(IMmu mmu)
        {
            this.mmu = mmu;
            this.address = 0x0000;
            this.Init();
        }

        public virtual byte NextByte()
        {
            var value = this.cache[this.cachePointer];
            
            if (++this.cachePointer == CacheSize)
            {
                this.NudgeCache();
            }

            this.TotalBytesRead++;
            return value;
        }

        public virtual byte[] NextBytes(int length)
        {
            var bytes = new byte[length];
            var bytesRead = 0;

            while (bytesRead < length)
            {
                var bytesToRead = Math.Min(length - bytesRead, CacheSize - this.cachePointer);
                Array.Copy(this.cache, this.cachePointer, bytes, bytesRead, bytesToRead);
                bytesRead += bytesToRead;

                this.cachePointer += bytesToRead;
                if (this.cachePointer == CacheSize)
                {
                    this.NudgeCache();
                }
            }

            this.TotalBytesRead += length;
            return bytes;
        }

        public ushort NextWord()
        {
            if (CacheSize - this.cachePointer == 1)
            {
                // If there's only one byte left then we need to read it then the next byte from the next cache
                var lsb = this.NextByte();
                var msb = this.NextByte();

                return BitConverter.ToUInt16(new[] { lsb, msb }, 0);
            }

            var value = BitConverter.ToUInt16(this.cache, this.cachePointer);

            this.cachePointer += 2;
            if (this.cachePointer == CacheSize)
            {
                this.NudgeCache();
            }

            this.TotalBytesRead += 2;
            return value;
        }

        public virtual void ReBuildCache(ushort newAddress)
        {
            // TODO: check if we can re-use this cache, make sure TotalBytesRead is still reset

            this.address = newAddress;
            Init();
        }

        public int TotalBytesRead { get; private set; }
        
        private void Init()
        {
            this.cachePointer = 0;
            this.cache = this.mmu.ReadBytes(this.address, CacheSize);
            this.TotalBytesRead = 0;
        }

        private void NudgeCache()
        {
            this.address = unchecked((ushort)(this.address + CacheSize));
            this.cachePointer = 0;
            this.cache = this.mmu.ReadBytes(this.address, CacheSize);
        }

    }
}
