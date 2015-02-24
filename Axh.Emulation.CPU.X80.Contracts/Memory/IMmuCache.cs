namespace Axh.Emulation.CPU.X80.Contracts.Memory
{
    public interface IMmuCache
    {
        byte GetNextByte();

        byte[] GetNextBytes(int length);

        ushort GetNextWord();

        void ReBuildCache(ushort newAddress);

        int TotalBytesRead { get; }
    }
}