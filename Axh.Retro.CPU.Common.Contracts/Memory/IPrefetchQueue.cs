namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    public interface IPrefetchQueue
    {
        int TotalBytesRead { get; }
        byte NextByte();

        byte[] NextBytes(int length);

        ushort NextWord();

        void ReBuildCache(ushort newAddress);
    }
}