namespace Axh.Retro.CPU.X80.Contracts.Memory
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