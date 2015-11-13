namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    using System.IO;

    public interface IPrefetchQueue
    {
        byte NextByte();

        byte[] NextBytes(int length);

        ushort NextWord();

        void ReBuildCache(ushort newAddress);

        int TotalBytesRead { get; }
    }
}