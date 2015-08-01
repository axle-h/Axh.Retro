namespace Axh.Retro.CPU.X80.Contracts.Core
{
    using Retro.CPU.X80.Contracts.OpCodes;

    public interface IZ80InstructionDecoder
    {
        Z80DynamicallyRecompiledBlock DecodeNextBlock(ushort address);
    }
}