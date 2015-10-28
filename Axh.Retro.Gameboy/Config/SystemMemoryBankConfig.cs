﻿namespace Axh.Retro.GameBoy.Config
{
    using Axh.Retro.CPU.X80.Contracts.Config;

    public class MemoryBankConfig : IMemoryBankConfig
    {
        public MemoryBankConfig(int bankId, ushort address, ushort length)
        {
            this.BankId = bankId;
            this.Address = address;
            this.Length = length;
            this.State = new byte[length];
        }

        public int BankId { get; }

        public ushort Address { get; }

        public ushort Length { get; }

        public byte[] State { get; }
    }
}
