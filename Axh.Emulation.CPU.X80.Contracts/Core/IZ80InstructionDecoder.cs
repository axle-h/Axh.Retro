namespace Axh.Emulation.CPU.X80.Contracts.Core
{
    using Axh.Emulation.CPU.X80.Contracts.OpCodes;

    public interface IZ80InstructionDecoder
    {
        Z80DynamicallyRecompiledBlock DecodeNextBlock(ushort address);
    }
}