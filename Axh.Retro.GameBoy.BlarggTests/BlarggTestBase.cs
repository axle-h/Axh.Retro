using System;
using System.Threading;
using System.Threading.Tasks;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Contracts.State;
using Axh.Retro.CPU.Z80.Wiring;
using Axh.Retro.GameBoy.BlarggTests.Config;
using Axh.Retro.GameBoy.Contracts.Config;
using Axh.Retro.GameBoy.Contracts.Peripherals;
using Axh.Retro.GameBoy.Wiring;
using Ninject;
using NUnit.Framework;

namespace Axh.Retro.GameBoy.BlarggTests
{
    [TestFixture]
    public class BlarggTestBase : IDisposable
    {
        private const string ScopeName = "GameBoy-Blargg";

        private readonly IKernel kernel;

        private readonly BlarggTestGameBoyConfig config;

        public BlarggTestBase()
        {
            kernel = new StandardKernel(new BlarggTestModule(ScopeName),
                                        new GameBoyModule(ScopeName),
                                        new Z80Module<IIntel8080Registers, Intel8080RegisterState>(ScopeName));
            config = kernel.Get<IGameBoyConfig>() as BlarggTestGameBoyConfig;
        }

        protected void Run(byte[] cartridge)
        {
            config.CartridgeData = cartridge;
            var core = kernel.Get<ICpuCore<IIntel8080Registers, Intel8080RegisterState>>();

            var io = core.Context.PeripheralManager.GetMemoryMappedPeripherals<IGameBoyMemoryMappedIO>();
            var serialPort = new BlarggTestSerialPort();
            io.HardwareRegisters.SerialPort.Connect(serialPort);

            using (var cancellation = new CancellationTokenSource())
            {
                var token = cancellation.Token;
                Task.Run(() => core.StartCoreProcessAsync(token), token);
                while (true)
                {
                    var word = serialPort.WaitForNextWord();
                    if (word == null)
                    {
                        throw new Exception("Couldn't get next word");
                    }

                    if (word == "Failed" || word == "Passed")
                    {
                        Task.Delay(TimeSpan.FromSeconds(1), token).Wait(token);
                        cancellation.Cancel();
                        Assert.IsTrue(word == "Passed");
                        return;
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