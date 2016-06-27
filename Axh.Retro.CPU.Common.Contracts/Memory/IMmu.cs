using System;

namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    public interface IMmu : IDisposable
    {
        byte ReadByte(ushort address);

        ushort ReadWord(ushort address);

        byte[] ReadBytes(ushort address, int length);

        void WriteByte(ushort address, byte value);

        void WriteWord(ushort address, ushort word);

        void WriteBytes(ushort address, byte[] bytes);

        void TransferByte(ushort addressFrom, ushort addressTo);

        void TransferBytes(ushort addressFrom, ushort addressTo, int length);
    }
}