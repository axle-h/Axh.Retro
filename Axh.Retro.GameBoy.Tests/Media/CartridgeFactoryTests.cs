﻿using System.Linq;
using Axh.Retro.GameBoy.Contracts.Media;
using Axh.Retro.GameBoy.Media;
using NUnit.Framework;

namespace Axh.Retro.GameBoy.Tests.Media
{
    [TestFixture]
    public class CartridgeFactoryTests
    {
        private ICartridgeFactory _cartridgeFactory;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _cartridgeFactory = new CartridgeFactory();
        }

        private static void AssertCartridgeBanks(ICartridge cartridge)
        {
            Assert.AreEqual(cartridge.Header.RamSize.NumberOfBanks(), cartridge.RamBankLengths.Length);
            foreach (var rambankLength in cartridge.RamBankLengths)
            {
                Assert.AreEqual(cartridge.Header.RamSize.BankSize(), rambankLength);
            }

            Assert.AreEqual(cartridge.Header.RomSize.NumberOfBanks(), cartridge.RomBanks.Length);
            foreach (var rombankLength in cartridge.RomBanks.Select(x => x.Length))
            {
                Assert.AreEqual(0x4000, rombankLength);
            }
        }

        [Test]
        public void CanReadPokemon()
        {
            var pokemon = Resources.Pokemon_Red_UE_Gb_Zip.UnZip();
            var cartridge = _cartridgeFactory.GetCartridge(pokemon);

            Assert.IsNotNull(cartridge);
            Assert.AreEqual("POKEMON RED", cartridge.Header.Title);
            Assert.AreEqual(DestinationCode.NonJapanese, cartridge.Header.DestinationCode);
            Assert.IsFalse(cartridge.Header.IsGameBoyColour);
            Assert.IsTrue(cartridge.Header.IsSuperGameBoy);
            Assert.AreEqual(CartridgeType.MBC3_RAM_BATTERY, cartridge.Header.CartridgeType);

            AssertCartridgeBanks(cartridge);
        }

        [Test]
        public void CanReadTetris()
        {
            var tetris = Resources.Tetris_W_Gb_Zip.UnZip();
            var cartridge = _cartridgeFactory.GetCartridge(tetris);

            Assert.IsNotNull(cartridge);
            Assert.AreEqual("TETRIS", cartridge.Header.Title);
            Assert.AreEqual(DestinationCode.Japanese, cartridge.Header.DestinationCode);
            Assert.IsFalse(cartridge.Header.IsGameBoyColour);
            Assert.IsFalse(cartridge.Header.IsSuperGameBoy);
            Assert.AreEqual(CartridgeType.ROM, cartridge.Header.CartridgeType);

            AssertCartridgeBanks(cartridge);
        }
    }
}