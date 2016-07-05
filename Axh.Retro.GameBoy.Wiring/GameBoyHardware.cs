using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.State;
using Axh.Retro.CPU.Z80.Wiring;
using Axh.Retro.GameBoy.Config;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Contracts.Factories;
using Axh.Retro.GameBoy.Contracts.Graphics;
using Axh.Retro.GameBoy.Devices;
using Axh.Retro.GameBoy.Factories;
using Axh.Retro.GameBoy.Peripherals;
using Axh.Retro.GameBoy.Registers;
using Axh.Retro.GameBoy.Registers.Interfaces;
using DryIoc;

namespace Axh.Retro.GameBoy.Wiring
{
    public class GameBoyHardware : IZ80Module
    {
        /// <summary>
        /// Registers all hardware in this module.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="reuse">The reuse scope.</param>
        public void Register(IContainer container, IReuse reuse)
        {
            container.Register<IInterruptFlagsRegister, InterruptFlagsRegister>(reuse);
            container.Register<IHardwareRegisters, HardwareRegisters>(reuse);

            // Named registers
            container.Register<IInterruptEnableRegister, InterruptEnableRegister>(reuse);
            container.Register<IJoyPadRegister, JoyPad>(reuse);
            container.RegisterMany(new[] { typeof(ISerialPort), typeof(ISerialPortRegister) }, typeof(SyncSerialPort), reuse);
            container.Register<IMemoryBankController, MemoryBankController1>(reuse);
            
            // GPU registers.
            container.Register<IGpuRegisters, GpuRegisters>(reuse);
            container.Register<ILcdControlRegister, LcdControlRegister>(reuse);
            container.Register<ICurrentScanlineRegister, CurrentScanlineRegister>(reuse);
            container.Register<ILcdMonochromePaletteRegister, LcdMonochromePaletteRegister>(reuse);
            container.Register<ILcdStatusRegister, LcdStatusRegister>(reuse);

            // Un-named registers
            container.Register<IRegister, LazyDividerRegister>(reuse);
            container.Register<IRegister, LcdOamDmaTransferRegister>(reuse);

            // Peripherals, no IO mapped peripherals on GB, only memory mapped
            container.Register<IPeripheral, GameBoyMemoryMappedIO>(reuse);

            // GPU
            container.Register<IGpu, Gpu>(reuse);

            container.Register<IInitialStateFactory<Intel8080RegisterState>, GameBoyInitialStateFactory>(Reuse.Singleton);
            container.Register<IPlatformConfig, GameBoyPlatformConfig>(Reuse.Singleton);
            container.Register<IRuntimeConfig, GameBoyRuntimeConfig>(Reuse.Singleton);
            container.Register<ICartridgeFactory, CartridgeFactory>(Reuse.Singleton);
        }
    }
}
