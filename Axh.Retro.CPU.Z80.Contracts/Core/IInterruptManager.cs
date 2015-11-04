namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    using System.Threading.Tasks;

    public interface IInterruptManager
    {
        Task Interrupt(ushort address);

        void Halt();
    }
}
