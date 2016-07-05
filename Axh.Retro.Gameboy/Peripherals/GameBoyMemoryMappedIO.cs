﻿using System;
using System.Collections.Generic;
using System.Linq;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Contracts.Graphics;
using Axh.Retro.GameBoy.Contracts.Peripherals;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Peripherals
{
    public class GameBoyMemoryMappedIO : IGameBoyMemoryMappedIO
    {
        private readonly IGpu gpu;

        private readonly IHardwareRegisters hardwareRegisters;

        private readonly IInterruptEnableRegister interruptRegister;

        private readonly IMemoryBankController memoryBankController;

        public GameBoyMemoryMappedIO(IHardwareRegisters hardwareRegisters,
                                     IInterruptEnableRegister interruptRegister,
                                     IGpu gpu,
                                     IMemoryBankController memoryBankController)
        {
            this.hardwareRegisters = hardwareRegisters;
            this.interruptRegister = interruptRegister;
            this.gpu = gpu;
            this.memoryBankController = memoryBankController;
        }

        public void Signal(ControlSignal signal)
        {
            switch (signal)
            {
                case ControlSignal.Halt:
                    gpu.Halt();
                    break;
                case ControlSignal.Resume:
                    gpu.Resume();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(signal), signal, null);
            }
        }

        public IEnumerable<IAddressSegment> AddressSegments
            =>
                new IAddressSegment[] {hardwareRegisters, interruptRegister, memoryBankController}.Concat(
                                                                                                          gpu
                                                                                                              .AddressSegments)
                                                                                                  .ToArray();

        public IHardwareRegisters HardwareRegisters => hardwareRegisters;

        public IGpu Gpu => gpu;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => gpu.Dispose();
    }
}