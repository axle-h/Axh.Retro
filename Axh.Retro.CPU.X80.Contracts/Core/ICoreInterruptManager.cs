namespace Axh.Retro.CPU.X80.Contracts.Core
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
    }
}