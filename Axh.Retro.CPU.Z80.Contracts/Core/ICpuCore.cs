﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Contracts.Core
{
    /// <summary>
    /// A Z80 CPU core.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ICpuCore : IDisposable
    {
        /// <summary>
        /// Gets the core identifier.
        /// </summary>
        /// <value>
        /// The core identifier.
        /// </value>
        Guid CoreId { get; }

        /// <summary>
        /// Starts the core process.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task StartCoreProcessAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieve peripheral of specified type.
        /// </summary>
        /// <typeparam name="TPeripheral"></typeparam>
        /// <returns></returns>
        TPeripheral GetPeripheralOfType<TPeripheral>() where TPeripheral : IPeripheral;
    }
}