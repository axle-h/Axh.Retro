namespace Axh.Retro.CPU.Z80.Contracts.Config
{
    public enum InstructionTimingSyncMode
    {
        /// <summary>
        ///     Do no timing sync, simulation will run as fast as possible
        /// </summary>
        Null,

        /// <summary>
        ///     Sync to machine cycles
        /// </summary>
        MachineCycles,

        /// <summary>
        ///     Sync to throttling states
        /// </summary>
        ThrottlingStates
    }
}