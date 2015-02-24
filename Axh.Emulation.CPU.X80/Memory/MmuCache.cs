namespace Axh.Emulation.CPU.X80.Memory
{
    using System;

    using Axh.Emulation.CPU.X80.Contracts.Memory;

    public class MmuCache : IMmuCache
    {
        private const int CacheSize = 64;

        private readonly IMmu mmu;

        private byte[] cache;

        private int cachePointer;

        private ushort address;

        public MmuCache(IMmu mmu, ushort address)
        {
            this.mmu = mmu;

            this.ReBuildCache(address);
        }

        public byte GetNextByte()
        {
            var value = this.cache[this.cachePointer];
            
            if (++this.cachePointer == CacheSize)
            {
                this.NudgeCache();
            }

            this.TotalBytesRead++;
            return value;
        }

        public byte[] GetNextBytes(int length)
        {
            var bytes = new byte[length];
            var bytesRead = 0;

            while (bytesRead < length)
            {
                var bytesToRead = Math.Min(length, CacheSize - this.cachePointer);
                Array.Copy(this.cache, this.cachePointer, bytes, bytesRead, bytesToRead);
                bytesRead += bytesToRead;

                this.cachePointer += bytesRead;
                if (this.cachePointer == CacheSize)
                {
                    this.NudgeCache();
                }
            }

            this.TotalBytesRead += length;
            return bytes;
        }

        public ushort GetNextWord()
        {
            if (CacheSize - this.cachePointer == 1)
            {
                // If there's only one byte left then we need to read it then the next byte from the next cache
                var lsb = this.GetNextByte();
                var msb = this.GetNextByte();

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

        public void ReBuildCache(ushort newAddress)
        {
            this.TotalBytesRead = 0;
            this.cachePointer = 0;
            this.address = newAddress;
            this.cache = this.mmu.ReadBytes(this.address, CacheSize);
        }

        public int TotalBytesRead { get; private set; }

        private void NudgeCache()
        {
            this.address = unchecked((ushort)(this.address + CacheSize));
            this.cachePointer = 0;
            this.cache = this.mmu.ReadBytes(this.address, CacheSize);
        }
    }
}
