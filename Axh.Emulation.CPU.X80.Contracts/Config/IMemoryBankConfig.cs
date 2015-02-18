namespace Axh.Emulation.CPU.X80.Contracts.Config
{
    public interface IMemoryBankConfig
    {
        int BankId { get; }

        ushort Address { get; }

        ushort Length { get; }

        /// <summary>
        /// Initial state of the memory bank.
        /// </summary>
        byte[] State { get; }
    }
}