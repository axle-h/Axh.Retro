namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    using System;
    using System.Threading.Tasks;

    public interface ICoreInterruptManager : IInterruptManager
    {
        void AddResumeTask(Action task);

        void NotifyHalt();

        void NotifyResume();

        Task<ushort> WaitForNextInterrupt();

        bool IsHalted { get; }

        bool IsInterrupted { get; }
        
    }

    public interface IInterruptManager
    {
        Task Interrupt(ushort address);

        void Halt();

        bool InterruptsEnabled { get; }
    }
}
