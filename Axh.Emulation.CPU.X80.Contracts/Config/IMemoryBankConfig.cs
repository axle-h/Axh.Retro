namespace Axh.Emulation.CPU.X80.Contracts.Config
{
    public interface IMemoryBankConfig
    {
        int BankId { get; }

        ushort Address { get; }

        ushort Length { get; }
    }
}