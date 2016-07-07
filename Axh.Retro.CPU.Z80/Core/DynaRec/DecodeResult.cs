namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    /// <summary>
    /// The post-result action of decoding a single operation.
    /// </summary>
    public enum DecodeResult
    {
        /// <summary>
        /// Continue decoding.
        /// </summary>
        Continue = 0,

        /// <summary>
        /// Finalize the block.
        /// </summary>
        Finalize = 1,

        /// <summary>
        /// Finalize the block and synchronize registers and timings.
        /// </summary>
        FinalizeAndSync = 2,

        /// <summary>
        /// Halts the CPU.
        /// </summary>
        Halt = 3,

        /// <summary>
        /// GB specific - halts all peripherals as well as CPU
        /// </summary>
        Stop = 4
    }
}