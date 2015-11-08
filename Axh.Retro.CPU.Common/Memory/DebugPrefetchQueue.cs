namespace Axh.Retro.CPU.Common.Memory
{
    using System.IO;

    using Axh.Retro.CPU.Common.Contracts.Memory;

    public class DebugPrefetchQueue : PrefetchQueue
    {
        private MemoryStream debugStream;

        public DebugPrefetchQueue(IMmu mmu, ushort address)
            : base(mmu, address)
        {
            this.debugStream = new MemoryStream();
        }

        public override byte NextByte()
        {
            var value = base.NextByte();
            this.debugStream.WriteByte(value);
            return value;
        }

        public override byte[] NextBytes(int length)
        {
            var bytes = base.NextBytes(length);
            this.debugStream.Write(bytes, 0, bytes.Length);
            return bytes;
        }
        
        public override void ReBuildCache(ushort newAddress)
        {
            this.debugStream.Close();
            this.debugStream = new MemoryStream();
            base.ReBuildCache(newAddress);
        }

        public override Stream GetDebugStream()
        {
            this.debugStream.Seek(0, SeekOrigin.Begin);
            return this.debugStream;
        }
    }
}
