namespace Axh.Emulation.CPU.X80.Contracts.Z80
{
    using System;
    using System.Linq.Expressions;

    using Axh.Emulation.CPU.X80.Contracts.Memory;
    using Axh.Emulation.CPU.X80.Contracts.OpCodes;
    using Axh.Emulation.CPU.X80.Contracts.Registers;

    public interface IZ80InstructionDecoder
    {
        Expression<Action<IZ80Registers, IMmu>> DecodeSingleOperation(PrimaryOpCode opCode);
    }
}