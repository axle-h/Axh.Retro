namespace Axh.Retro.GameBoy.Console
{
    using System.Threading;

    using Axh.Retro.CPU.Z80.Binding;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;
    using Axh.Retro.GameBoy.Binding;
    using Axh.Retro.GameBoy.Console.Config;
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Contracts.Peripherals;
    using Axh.Retro.GameBoy.Tests.Util;

    using Ninject;

    class Program
    {
        private const string ScopeName = "GameBoy-Console";

        static void Main(string[] args)
        {
            using (var kernel = new StandardKernel(new GameBoyConsoleModule(ScopeName, Resources.Tetris_W_Gb_Zip.UnZip()), new GameBoyModule(ScopeName), new Z80Module<IIntel8080Registers, Intel8080RegisterState>(ScopeName)))
            {
                var core = kernel.Get<ICpuCore<IIntel8080Registers, Intel8080RegisterState>>();

                var io = core.Context.PeripheralManager.GetMemoryMappedPeripherals<IGameBoyMemoryMappedIO>();
                io.HardwareRegisters.SerialPort.Connect(new ConsoleSerialPort());
                core.StartCoreProcessAsync(CancellationToken.None).Wait();
            }
        }
    }
}
