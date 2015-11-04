namespace Axh.Retro.GameBoy.Factories
{
    using System;
    using System.Linq;

    using Axh.Retro.GameBoy.Contracts.Factories;
    using Axh.Retro.GameBoy.Contracts.Media;
    using Axh.Retro.GameBoy.Media;

    public class CartridgeFactory : ICartridgeFactory
    {
        private const int RomBankLength = 0x4000;
        
        public ICartridge GetCartridge(byte[] cartridge)
        {
            var header = new CartridgeHeader(cartridge);
            var numberOfBanks = cartridge.Length / RomBankLength;

            if (cartridge.Length % RomBankLength != 0)
            {
                throw new Exception("Invalid cartridge length: " + cartridge.Length);
            }

            if (numberOfBanks % 2 != 0)
            {
                throw new Exception("Invalid number of banks: " + numberOfBanks);
            }

            // EntryPoint should not be all 0's or ff's
            if (header.EntryPoint.All(x => x == 0 || x == 0xff))
            {
                throw new Exception("No valid entrypoint in cartridge");
            }

            if (!header.NintendoLogoValid)
            {
                // Should we do this?!
                throw new Exception("Nintendo logo is not valid");
            }

            if (string.IsNullOrEmpty(header.Title))
            {
                // Using as a sanity check. Will there ever not be a title?
                throw new Exception("No title in cartridge");
            }

            if (header.RomSize.NumberOfBanks() != numberOfBanks)
            {
                throw new Exception($"Cartridge type {header.RomSize} does not match ROM banks: " + numberOfBanks);
            }

            var banks = new byte[numberOfBanks][];
            for(var n = 0; n < numberOfBanks; n++)
            {
                var bank = new byte[RomBankLength];
                Array.Copy(cartridge, n * RomBankLength, bank, 0, RomBankLength);
                banks[n] = bank;
            }
            
            var ramBanks = Enumerable.Range(0, header.RamSize.NumberOfBanks()).Select(i => header.RamSize.BankSize()).ToArray();

            return new Cartridge(banks, ramBanks, header);
        }
    }
}
