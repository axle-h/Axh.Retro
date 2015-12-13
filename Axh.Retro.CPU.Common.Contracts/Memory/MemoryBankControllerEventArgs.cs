namespace Axh.Retro.CPU.Common.Contracts.Memory
{
    using System;

    public class MemoryBankControllerEventArgs : EventArgs
    {
        public MemoryBankControllerEventArgs(MemoryBankControllerEventTarget target)
        {
            Target = target;
        }
        
        public MemoryBankControllerEventTarget Target { get; }
    }

    public enum MemoryBankControllerEventTarget
    {
        /// <summary>
        /// A RAM bank switch has occurred.
        /// </summary>
        RamBankSwitch,

        /// <summary>
        /// A ROM bank switch has occurred.
        /// </summary>
        RomBankSwitch,

        /// <summary>
        /// RAM has been enabled.
        /// </summary>
        RamEnable
    }
}
