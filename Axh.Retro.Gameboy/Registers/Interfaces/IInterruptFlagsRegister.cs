using Axh.Retro.GameBoy.Contracts.Devices;

namespace Axh.Retro.GameBoy.Registers.Interfaces
{
    public interface IInterruptFlagsRegister : IRegister
    {
        void UpdateInterrupts(InterruptFlag interrupts);
    }
}