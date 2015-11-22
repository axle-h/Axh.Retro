namespace Axh.Retro.GameBoy.Devices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;
    using Axh.Retro.GameBoy.Registers.Interfaces;

    public class HardwareRegisters : ICoreHardwareRegisters
    {
        private readonly IDictionary<ushort, IRegister> registers;

        public HardwareRegisters(IEnumerable<IRegister> registers, ICoreJoyPad joyPad, ICoreSerialPort serialPort)
        {
            JoyPad = joyPad;
            SerialPort = serialPort;
            this.registers = registers.Concat(new[] { joyPad, serialPort, serialPort.SerialData }).ToDictionary(x => x.Address);
        }

        private const ushort Address = 0xff00;
        private const ushort Length = 0x80;
        
        public MemoryBankType Type => MemoryBankType.Peripheral;

        ushort IAddressSegment.Address => Address;

        ushort IAddressSegment.Length => Length;

        public byte ReadByte(ushort address)
        {
            return this.registers.ContainsKey(address) ? this.registers[address].Register : (byte)0x0000;
        }

        public ushort ReadWord(ushort address)
        {
            var bytes = this.ReadBytes(address, 2);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public byte[] ReadBytes(ushort address, int length)
        {
            var bytes = new byte[length];
            ReadBytes(address, bytes);
            return bytes;
        }

        public void ReadBytes(ushort address, byte[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = this.ReadByte(unchecked((ushort)(address + i)));
            }
        }

        public void WriteByte(ushort address, byte value)
        {
            if (this.registers.ContainsKey(address))
            {
                this.registers[address].Register = value;
            }
        }

        public void WriteWord(ushort address, ushort word)
        {
            var bytes = BitConverter.GetBytes(word);
            this.WriteBytes(address, bytes);
        }

        public void WriteBytes(ushort address, byte[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                this.WriteByte(unchecked((ushort)(address + i)), values[i]);
            }
        }

        public IJoyPad JoyPad { get; }

        public ISerialPort SerialPort { get; }

        public override string ToString()
        {
            return $"{Type}: 0x{Address:x4} - 0x{Address + Length - 1:x4}";
        }
    }
}
