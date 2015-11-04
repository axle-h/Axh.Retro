namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    public enum DecodeResult
    {
        Continue = 0,
        Finalize = 1,
        FinalizeAndSync = 2,
        Halt = 3,

        /// <summary>
        /// GB specific - halts all peripherals as well as CPU
        /// </summary>
        Stop = 4
    }
}
