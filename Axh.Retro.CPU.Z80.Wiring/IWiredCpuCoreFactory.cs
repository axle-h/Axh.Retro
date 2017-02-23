using System;
using Axh.Retro.CPU.Z80.Contracts.Core;

namespace Axh.Retro.CPU.Z80.Wiring
{
    public interface IWiredCpuCoreFactory : IDisposable
    {
        /// <summary>
        /// Gets a new core.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        ICpuCore GetNewCore();
    }
}