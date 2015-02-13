namespace Axh.Emulation.GameBoy.Z80.Config
{
    using Axh.Emulation.CPU.Z80.Contracts.Config;

    public class MemoryBankConfig : IMemoryBankConfig
    {
        public MemoryBankConfig(int bankId, ushort address, ushort length)
        {
            this.BankId = bankId;
            this.Address = address;
            this.Length = length;
        }

        public int BankId { get; private set; }

        public ushort Address { get; private set; }

        public ushort Length { get; private set; }
    }
}
