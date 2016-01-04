namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
{
    using System.Threading.Tasks;
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Registers.Interfaces;

    public interface IGameBoyInterruptManager
    {
        IRegister InterruptFlagsRegister { get; }

        bool UpdateInterrupts(InterruptFlag interrupts);
    }
}