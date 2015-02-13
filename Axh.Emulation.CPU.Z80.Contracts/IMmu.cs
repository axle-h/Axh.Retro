﻿namespace Axh.Emulation.CPU.Z80.Contracts
{
    public interface IMmu
    {
        byte ReadByte(ushort address);

        ushort ReadWord(ushort address);

        void WriteByte(ushort address, byte value);

        void WriteWord(ushort address, ushort word);

        void WriteBytes(ushort address, byte[] values);
    }
}