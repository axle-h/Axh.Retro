namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
{
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Registers.Interfaces;

    public interface IGameBoyInterruptManager
    {
        IRegister InterruptFlagsRegister { get; }

        void UpdateInterrupts(InterruptFlag interrupts);
    }
}