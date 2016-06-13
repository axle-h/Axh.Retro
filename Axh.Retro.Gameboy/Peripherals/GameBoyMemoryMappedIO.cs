namespace Axh.Retro.GameBoy.Peripherals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Contracts.Graphics;
    using Axh.Retro.GameBoy.Contracts.Peripherals;
    using Axh.Retro.GameBoy.Devices;
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;
    using Axh.Retro.GameBoy.Registers.Interfaces;

    public class GameBoyMemoryMappedIO : IGameBoyMemoryMappedIO
    {
        private readonly ICoreGpu gpu;

        private readonly IMemoryBankController memoryBankController;

        private readonly IInterruptEnableRegister interruptRegister;

        private readonly ICoreHardwareRegisters hardwareRegisters;

        public GameBoyMemoryMappedIO(ICoreHardwareRegisters hardwareRegisters, IInterruptEnableRegister interruptRegister, ICoreGpu gpu, IMemoryBankController memoryBankController)
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
        
        public IEnumerable<IAddressSegment> AddressSegments => new IAddressSegment[] { hardwareRegisters, interruptRegister, memoryBankController }.Concat(gpu.AddressSegments).ToArray();

        public IHardwareRegisters HardwareRegisters => hardwareRegisters;

        public IGpu Gpu => this.gpu;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => gpu.Dispose();
    }
}
