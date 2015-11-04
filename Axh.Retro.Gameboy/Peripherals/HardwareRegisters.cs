namespace Axh.Retro.GameBoy.Peripherals
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.GameBoy.Contracts.Peripherals;

    public class HardwareRegisters : IHardwareRegisters, IReadableAddressSegment, IWriteableAddressSegment
    {
        private const ushort Address = 0xff00;
        private const ushort Length = 0x7f;
        
        public MemoryBankType Type => MemoryBankType.Peripheral;

        ushort IAddressSegment.Address => Address;

        ushort IAddressSegment.Length => Length;

        public byte ReadByte(ushort address)
        {
            throw new System.NotImplementedException();
        }

        public ushort ReadWord(ushort address)
        {
            throw new System.NotImplementedException();
        }

        public byte[] ReadBytes(ushort address, int length)
        {
            throw new System.NotImplementedException();
        }

        public void WriteByte(ushort address, byte value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteWord(ushort address, ushort word)
        {
            throw new System.NotImplementedException();
        }

        public void WriteBytes(ushort address, byte[] values)
        {
            throw new System.NotImplementedException();
        }
    }
}
