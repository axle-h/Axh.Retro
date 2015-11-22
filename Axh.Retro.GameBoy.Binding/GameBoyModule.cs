namespace Axh.Retro.GameBoy.Binding
{
    using Axh.Retro.CPU.Z80.Contracts.Config;
    using Axh.Retro.CPU.Z80.Contracts.Peripherals;
    using Axh.Retro.CPU.Z80.Contracts.State;
    using Axh.Retro.GameBoy.Config;
    using Axh.Retro.GameBoy.Contracts.Config;
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Contracts.Factories;
    using Axh.Retro.GameBoy.Contracts.Graphics;
    using Axh.Retro.GameBoy.Devices;
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;
    using Axh.Retro.GameBoy.Factories;
    using Axh.Retro.GameBoy.Peripherals;
    using Axh.Retro.GameBoy.Registers;
    using Axh.Retro.GameBoy.Registers.Interfaces;

    using Ninject.Extensions.NamedScope;
    using Ninject.Modules;

    public class GameBoyModule : NinjectModule
    {
        private readonly string cpuContextScope;

        public GameBoyModule(string cpuContextScope)
        {
            this.cpuContextScope = cpuContextScope;
        }

        public override void Load()
        {
            this.Bind<IGameBoyInterruptManager>().To<GameBoyInterruptManager>().InNamedScope(cpuContextScope);
            this.Bind<ICoreHardwareRegisters>().To<HardwareRegisters>().InNamedScope(cpuContextScope);

            // Named registers
            this.Bind<IInterruptEnableRegister>().To<InterruptEnableRegister>().InNamedScope(cpuContextScope);
            this.Bind<ICoreJoyPad>().To<JoyPad>().InNamedScope(cpuContextScope);
            this.Bind<ICoreSerialPort>().To<SyncSerialPort>().InNamedScope(cpuContextScope);
            this.Bind<ILcdControlRegister>().To<LcdControlRegister>().InNamedScope(cpuContextScope);
            this.Bind<IMemoryBankController1>().To<MemoryBankController1>().InNamedScope(cpuContextScope);
            // Un-named registers
            this.Bind<IRegister>().To<LazyDividerRegister>().InNamedScope(cpuContextScope);

            // Peripherals, no IO mapped peripherals on GB, only memory mapped
            this.Bind<IMemoryMappedPeripheral>().To<GameBoyMemoryMappedIO>().InNamedScope(cpuContextScope);

            // GPU
            this.Bind<ICoreGpu>().To<Gpu>().InNamedScope(cpuContextScope);

            this.Bind<IInitialStateFactory<Intel8080RegisterState>>().To<GameBoyInitialStateFactory>().InSingletonScope();
            this.Bind<IPlatformConfig>().To<GameBoyPlatformConfig>().InSingletonScope();
            this.Bind<IRuntimeConfig>().To<GameBoyRuntimeConfig>().InSingletonScope();
            this.Bind<ICartridgeFactory>().To<CartridgeFactory>().InSingletonScope();


            // Remaining
            // IGameBoyConfig Singleton copuled to filesystem
            // IRenderHandler NamedScope coupled to view
        }
    }
}
