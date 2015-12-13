namespace Axh.Retro.GameBoy.BlarggTests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Axh.Retro.CPU.Z80.Binding;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.CPU.Z80.Contracts.Registers;
    using Axh.Retro.CPU.Z80.Contracts.State;
    using Axh.Retro.GameBoy.Binding;
    using Axh.Retro.GameBoy.BlarggTests.Config;
    using Axh.Retro.GameBoy.Contracts.Config;
    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Contracts.Peripherals;

    using Ninject;

    using NUnit.Framework;

    [TestFixture]
    public class BlarggTestBase : IDisposable
    {
        private const string ScopeName = "GameBoy-Blargg";

        private readonly IKernel kernel;

        private readonly BlarggTestGameBoyConfig config;

        public BlarggTestBase()
        {
            kernel = new StandardKernel(new BlarggTestModule(ScopeName), new GameBoyModule(ScopeName), new Z80Module<IIntel8080Registers, Intel8080RegisterState>(ScopeName));
            config = kernel.Get<IGameBoyConfig>() as BlarggTestGameBoyConfig;
        }

        [Test]
        public void cpu_instrs_01_special()
        {
            var result = Run(Resources.cpu_instrs_01_special, port => port.Words.Count > 2 && port.Words.Reverse().Skip(1).First() == "Failed", port => false);
            Assert.IsTrue(result);

        }

        protected bool Run(byte[] cartridge, Func<BlarggTestSerialPort, bool> terminate, Func<BlarggTestSerialPort, bool> pass)
        {
            config.CartridgeData = cartridge;
            var core = kernel.Get<ICpuCore<IIntel8080Registers, Intel8080RegisterState>>();

            var io = core.Context.PeripheralManager.GetMemoryMappedPeripherals<IGameBoyMemoryMappedIO>();
            var serialPort = new BlarggTestSerialPort();
            io.HardwareRegisters.SerialPort.Connect(serialPort);

            using (var cancellation = new CancellationTokenSource())
            {
                var cpuProcess = core.StartCoreProcessAsync(cancellation.Token);
                while (true)
                {
                    if (!serialPort.WaitForNextWord())
                    {
                        throw new Exception("Couldn't get next word");
                    }

                    if (terminate(serialPort))
                    {
                        cancellation.Cancel();
                        return pass(serialPort);
                    }
                }
            }
        }

        public void Dispose()
        {
            kernel.Dispose();
        }
    }
}
