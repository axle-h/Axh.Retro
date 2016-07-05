﻿using System;
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
using NUnit.Framework;

namespace Axh.Retro.GameBoy.BlarggTests
{
    [TestFixture]
    public class BlarggTestBase : IDisposable
    {
        private const string ScopeName = "GameBoy-Blargg";

        private readonly Z80<IIntel8080Registers, Intel8080RegisterState> z80;

        private readonly BlarggTestGameBoyConfig config;

        public BlarggTestBase()
        {
            var blarggTest = new BlarggTest();
            z80 = new Z80<IIntel8080Registers, Intel8080RegisterState>().With<GameBoyHardware>().With(blarggTest).Init();
            config = blarggTest.Config;
        }

        protected void Run(byte[] cartridge)
        {
            config.CartridgeData = cartridge;
            var core = z80.GetNewCore();

            var io = core.Context.PeripheralManager.PeripheralOfType<IGameBoyMemoryMappedIO>();
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
            z80.Dispose();
        }
    }
}