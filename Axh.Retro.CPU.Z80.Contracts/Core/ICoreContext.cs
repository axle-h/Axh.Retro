namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.Registers;

    public interface ICoreContext<TRegisters> where TRegisters : IRegisters
    {
        TRegisters Registers { get; set; }

        IInterruptManager InterruptManager { get; set; }

        IPeripheralManager PeripheralManager { get; set; }

        IMmu Mmu { get; set; }
    }
}
