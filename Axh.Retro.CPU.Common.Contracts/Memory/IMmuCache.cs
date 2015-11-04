namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    public interface IMmuCache
    {
        byte NextByte();

        byte[] NextBytes(int length);

        ushort NextWord();

        void ReBuildCache(ushort newAddress);

        int TotalBytesRead { get; }
    }
}