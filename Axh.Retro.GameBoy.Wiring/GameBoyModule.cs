using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Z80.Contracts.Config;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.State;
using Axh.Retro.GameBoy.Config;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Contracts.Factories;
using Axh.Retro.GameBoy.Devices;
using Axh.Retro.GameBoy.Devices.CoreInterfaces;
using Axh.Retro.GameBoy.Factories;
using Axh.Retro.GameBoy.Peripherals;
using Axh.Retro.GameBoy.Registers;
using Axh.Retro.GameBoy.Registers.Interfaces;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;

namespace Axh.Retro.GameBoy.Wiring
{
    public class GameBoyModule : NinjectModule
    {
        private readonly string cpuContextScope;

        public GameBoyModule(string cpuContextScope)
        {
            this.cpuContextScope = cpuContextScope;
        }

        public override void Load()
        {
            Bind<IInterruptFlagsRegister>().To<InterruptFlagsRegister>().InNamedScope(cpuContextScope);
            Bind<ICoreHardwareRegisters>().To<HardwareRegisters>().InNamedScope(cpuContextScope);

            // Named registers
            Bind<IInterruptEnableRegister>().To<InterruptEnableRegister>().InNamedScope(cpuContextScope);
            Bind<ICoreJoyPad>().To<JoyPad>().InNamedScope(cpuContextScope);
            Bind<ISerialPort, ICoreSerialPort>().To<SyncSerialPort>().InNamedScope(cpuContextScope);
            Bind<IMemoryBankController>().To<MemoryBankController1>().InNamedScope(cpuContextScope);


            // GPU registers.
            Bind<IGpuRegisters>().To<GpuRegisters>().InNamedScope(cpuContextScope);
            Bind<ILcdControlRegister>().To<LcdControlRegister>().InNamedScope(cpuContextScope);
            Bind<ICurrentScanlineRegister>().To<CurrentScanlineRegister>().InNamedScope(cpuContextScope);
            Bind<ILcdMonochromePaletteRegister>().To<LcdMonochromePaletteRegister>().InNamedScope(cpuContextScope);
            Bind<ILcdStatusRegister>().To<LcdStatusRegister>().InNamedScope(cpuContextScope);

            // Un-named registers
            Bind<IRegister>().To<LazyDividerRegister>().InNamedScope(cpuContextScope);
            Bind<IRegister>().To<LcdOamDmaTransferRegister>().InNamedScope(cpuContextScope);

            // Peripherals, no IO mapped peripherals on GB, only memory mapped
            Bind<IMemoryMappedPeripheral>().To<GameBoyMemoryMappedIO>().InNamedScope(cpuContextScope);

            // GPU
            Bind<ICoreGpu>().To<Gpu>().InNamedScope(cpuContextScope);

            Bind<IInitialStateFactory<Intel8080RegisterState>>().To<GameBoyInitialStateFactory>().InSingletonScope();
            Bind<IPlatformConfig>().To<GameBoyPlatformConfig>().InSingletonScope();
            Bind<IRuntimeConfig>().To<GameBoyRuntimeConfig>().InSingletonScope();
            Bind<ICartridgeFactory>().To<CartridgeFactory>().InSingletonScope();


            // Remaining
            // IGameBoyConfig Singleton coupled to filesystem
            // IRenderHandler NamedScope coupled to view
        }
    }
}