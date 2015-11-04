namespace Axh.Retro.CPU.Z80.Core
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public class CoreContext<TRegisters> : ICoreContext<TRegisters> where TRegisters : IRegisters
    {
        public TRegisters Registers { get; set; }

        public IInterruptManager InterruptManager { get; set; }

        public IPeripheralManager PeripheralManager { get; set; }

        public IMmu Mmu { get; set; }
    }
}
