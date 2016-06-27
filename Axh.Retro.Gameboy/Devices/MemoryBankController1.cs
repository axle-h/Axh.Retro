using System;
using Axh.Retro.CPU.Common.Contracts.Memory;

namespace Axh.Retro.GameBoy.Devices
{
    public class MemoryBankController1 : IMemoryBankController
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
            ramBankingMode = false;
            RomBankNumber = 1;
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
                OnEvent(MemoryBankControllerEventTarget.RamEnable);
                return;
            }

            if (address < RamBankNumberAddress)
            {
                // ROM Bank Number
                RomBankNumber &= 0xe0; // Clear 0x17
                RomBankNumber |= GetRomBankNumber(value);

                OnEvent(MemoryBankControllerEventTarget.RomBankSwitch);
                return;
            }

            if (address < ModeSelectAddress)
            {
                // RAM Bank Number
                // TODO this should select the low 2 bits of the ROM bank number when romBankingMode is false
                if (ramBankingMode)
                {
                    RamBankNumber = (byte) (value & 0x3);
                    OnEvent(MemoryBankControllerEventTarget.RamBankSwitch);
                }
                return;
            }

            // ROM / RAM Mode Select
            ramBankingMode = (value & 0x1) == 0x1;
        }

        public void WriteWord(ushort address, ushort word)
        {
            WriteByte(address, (byte) (word & 0xff));
            WriteByte(address, (byte) ((word & 0xff00) >> 8));
        }

        public void WriteBytes(ushort address, byte[] values)
        {
            for (var i = 0; i < values.Length; i++, address++)
            {
                WriteByte(address, values[i]);
            }
        }

        public bool RamEnable { get; private set; }

        public byte RomBankNumber { get; private set; }

        public byte RamBankNumber { get; private set; }

        public event EventHandler<MemoryBankControllerEventArgs> MemoryBankSwitch;

        private static byte GetRomBankNumber(byte value)
        {
            value = (byte) (value & 0x1f);
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

        protected void OnEvent(MemoryBankControllerEventTarget eventTarget)
        {
            MemoryBankSwitch?.Invoke(this, new MemoryBankControllerEventArgs(eventTarget));
        }
    }
}