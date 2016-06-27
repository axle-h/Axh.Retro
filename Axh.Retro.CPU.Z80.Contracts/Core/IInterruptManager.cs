using System;
using System.Threading.Tasks;

namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    public interface ICoreInterruptManager : IInterruptManager, IDisposable
    {
        bool IsHalted { get; }

        bool IsInterrupted { get; }
        void AddResumeTask(Action task);

        void NotifyHalt();

        void NotifyResume();

        Task<ushort> WaitForNextInterrupt();
    }

    public interface IInterruptManager
    {
        bool InterruptsEnabled { get; }
        void Interrupt(ushort address);

        void Halt();
    }
}