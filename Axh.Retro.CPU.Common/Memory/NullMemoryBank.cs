namespace Axh.Retro.CPU.Common.Memory
{
    using Axh.Retro.CPU.Common.Contracts.Config;
    using Axh.Retro.CPU.Common.Contracts.Memory;

    public class NullMemoryBank : IReadableAddressSegment, IWriteableAddressSegment
    {
        public NullMemoryBank(IMemoryBankConfig memoryBankConfig)
        {
            this.Type = memoryBankConfig.Type;
            this.Address = memoryBankConfig.Address;
            this.Length = memoryBankConfig.Length;
        }

        public MemoryBankType Type { get; }

        public ushort Address { get; }

        public ushort Length { get; }

        public byte ReadByte(ushort address)
        {
            return 0x00;
        }

        public ushort ReadWord(ushort address)
        {
            return 0x0000;
        }

        public byte[] ReadBytes(ushort address, int length)
        {
            return new byte[length];
        }

        public void ReadBytes(ushort address, byte[] buffer)
        {
        }

        public void WriteByte(ushort address, byte value)
        {
        }

        public void WriteWord(ushort address, ushort word)
        {
        }

        public void WriteBytes(ushort address, byte[] values)
        {
        }
        public override string ToString()
        {
            return $"{Type}: 0x{Address:x4} - 0x{Address + Length - 1:x4}";
        }
    }
}
