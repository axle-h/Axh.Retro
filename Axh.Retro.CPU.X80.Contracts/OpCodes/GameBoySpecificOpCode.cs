namespace Axh.Retro.CPU.X80.Contracts.OpCodes
{
    public enum GameBoySpecificOpCode : byte
    {
        LD_mnn_SP = PrimaryOpCode.EX_AF,

        STOP = PrimaryOpCode.DJNZ,
    }
}
