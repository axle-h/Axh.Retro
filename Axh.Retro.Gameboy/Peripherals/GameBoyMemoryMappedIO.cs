using System;
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
        private readonly IInterruptEnableRegister _interruptRegister;

        private readonly IMemoryBankController _memoryBankController;

        public GameBoyMemoryMappedIO(IHardwareRegisters hardwareRegisters,
            IInterruptEnableRegister interruptRegister,
            IGpu gpu,
            IMemoryBankController memoryBankController)
        {
            HardwareRegisters = hardwareRegisters;
            _interruptRegister = interruptRegister;
            Gpu = gpu;
            _memoryBankController = memoryBankController;
        }

        public void Signal(ControlSignal signal)
        {
            switch (signal)
            {
                case ControlSignal.Halt:
                    Gpu.Halt();
                    break;
                case ControlSignal.Resume:
                    Gpu.Resume();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(signal), signal, null);
            }
        }

        public IEnumerable<IAddressSegment> AddressSegments
            =>
                new IAddressSegment[] { HardwareRegisters, _interruptRegister, _memoryBankController }.Concat(Gpu.AddressSegments)
                                                                                                      .ToArray();

        public IHardwareRegisters HardwareRegisters { get; }

        public IGpu Gpu { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => Gpu.Dispose();
    }
}