using System;
using System.Threading;
using System.Threading.Tasks;
using Axh.Retro.CPU.Z80.Contracts.Registers;
using Axh.Retro.CPU.Z80.Wiring;
using Axh.Retro.GameBoy.BlarggTests.Config;
using Axh.Retro.GameBoy.Contracts.Peripherals;
using Axh.Retro.GameBoy.Wiring;
using NUnit.Framework;

namespace Axh.Retro.GameBoy.BlarggTests
{
    [TestFixture]
    public class BlarggTestBase : IDisposable
    {
        private readonly Intel8080WiringBase<IRegisters> _coreWiring;

        private readonly BlarggTestGameBoyConfig _config;

        public BlarggTestBase()
        {
            var blarggTest = new BlarggTest();
            _coreWiring = new GameBoyWiring().With<GameBoyHardware>().With(blarggTest).Init();
            _config = blarggTest.Config;
        }

        protected void Run(byte[] cartridge)
        {
            _config.CartridgeData = cartridge;
            var core = _coreWiring.GetNewCore();

            var io = core.GetPeripheralOfType<IGameBoyMemoryMappedIO>();
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
            _coreWiring.Dispose();
        }
    }
}