using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.GameBoy.Devices
{
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;

    public class MemoryBankController1 : IMemoryBankController1
    {
        private const ushort Address = 0x0000;
        private const ushort Length = 0x8000;

        private const ushort RamEnableAddress = 0x0000;
        private const ushort RomBankNumberAddress = 0x2000;
        private const ushort RamBankNumberAddress = 0x4000;
        private const ushort ModeSelectAddress = 0x6000;

        private bool ramBankingMode;

        public MemoryBankController1()
        {
            this.ramBankingMode = false;
        }

        public MemoryBankType Type => MemoryBankType.Peripheral;

        ushort IAddressSegment.Address => Address;

        ushort IAddressSegment.Length => Length;

        public void WriteByte(ushort address, byte value)
        {
            if (address < RomBankNumberAddress)
            {
                // RAM Enable
                RamEnable = (value & 0xf) == 0xa;
                return;
            }

            if(address < RamBankNumberAddress)
            {
                // ROM Bank Number
                RomBankNumber &= 0xe0; // Clear 0x17
                RomBankNumber |= GetRomBankNumber(value);
                return;
            }

            if (address < ModeSelectAddress)
            {
                // RAM Bank Number
                // TODO this should select the low 2 bits of the ROM bank number when romBankingMode is false
                if (this.ramBankingMode)
                {
                    RamBankNumber = (byte)(value & 0x3);
                }
                else
                {
                    
                }
                return;
            }

            // ROM / RAM Mode Select
            this.ramBankingMode = (value & 0x1) == 0x1;
        }

        private static byte GetRomBankNumber(byte value)
        {
            value = (byte)(value & 0x1f);
            switch (value)
            {
                case 0x00:
                    return 0x01;
                case 0x20:
                    return 0x21;
                case 0x40:
                    return 0x41;
                case 0x60:
                    return 0x61;
            }

            return value;
        }

        public void WriteWord(ushort address, ushort word)
        {
            this.WriteByte(address, (byte)(word & 0xff));
            this.WriteByte(address, (byte)((word & 0xff00) >> 8));
        }

        public void WriteBytes(ushort address, byte[] values)
        {
            for (var i = 0; i < values.Length; i++, address++)
            {
                this.WriteByte(address, values[i]);
            }
        }

        public bool RamEnable { get; private set; }

        public byte RomBankNumber { get; private set; }

        public byte RamBankNumber { get; private set; }
    }
}
