namespace Axh.Retro.CPU.X80.Contracts.Core
{
    public interface IZ80InstructionDecoder
    {
        Z80DynamicallyRecompiledBlock DecodeNextBlock(ushort address);
    }
}