namespace Axh.Retro.GameBoy.Devices
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Axh.Retro.CPU.Common.Config;
    using Axh.Retro.CPU.Common.Contracts.Config;
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Common.Memory;
    using Axh.Retro.CPU.Z80.Contracts.Core;
    using Axh.Retro.GameBoy.Contracts.Graphics;
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;

    internal class Gpu : ICoreGpu
    {
        private static readonly IMemoryBankConfig SpriteRamConfig = new SimpleMemoryBankConfig(MemoryBankType.Peripheral, null, 0xfe00, 0xa0);
        private static readonly IMemoryBankConfig MapRamConfig = new SimpleMemoryBankConfig(MemoryBankType.Peripheral, null, 0x8000, 0x2000);

        /// <summary>
        /// $FE00-$FE9F	OAM - Object Attribute Memory
        /// </summary>
        private readonly ArrayBackedMemoryBank spriteRam;

        /// <summary>
        /// $9C00-$9FFF	Tile map #1
        /// $9800-$9BFF Tile map #0 32*32
        /// $9000-$97FF Tile set #0: tiles 0-127
        /// $8800-$8FFF Tile set #1: tiles 128-255 & Tile set #0: tiles -128 to -1
        /// $8000-$87FF Tile set #1: tiles 0-127
        /// </summary>
        private readonly ArrayBackedMemoryBank tileRam;

        private readonly IInterruptManager interruptManager;

        private readonly ICoreHardwareRegisters hardwareRegisters;

        private readonly IRenderHandler renderhandler;

        /// <summary>
        /// Tile set #0: tiles -128 to 128 (tiles -128 to -1 are references to tile set #1 tiles 128 to 255)
        /// </summary>
        private readonly Tile[] tileSet0;

        /// <summary>
        /// Tile set #1: tiles 0 to 255
        /// </summary>
        private readonly Tile[] tileSet1;

        /// <summary>
        /// We expect the GPU to reuse tiles often. Better to cache the result of parsing tiles than process them every frame.
        /// TODO: How big will this grow?
        /// </summary>
        private readonly IDictionary<Guid, Tile> tileCache;

        private readonly Bitmap frame;

        public Gpu(IInterruptManager interruptManager, ICoreHardwareRegisters hardwareRegisters, IRenderHandler renderhandler)
        {
            this.interruptManager = interruptManager;
            this.hardwareRegisters = hardwareRegisters;
            this.renderhandler = renderhandler;
            this.spriteRam = new ArrayBackedMemoryBank(SpriteRamConfig);
            this.tileRam = new ArrayBackedMemoryBank(MapRamConfig);

            this.tileSet0 = new Tile[256];
            this.tileSet1 = new Tile[256];
            this.tileCache = new Dictionary<Guid, Tile>();
            this.frame = new Bitmap(160, 144);
        }

        public void Halt()
        {
            
        }

        public void Resume()
        {
            
        }

        private void UpdateTileSets()
        {
            // $9000-$97FF Tile set #0: tiles 0-127
            GetTiles(this.tileSet0, 0x9000, 0);

            // $8800-$8FFF Tile set #1: tiles 128-255 & Tile set #0: tiles -128 (0) to -1 (127)
            GetTiles(this.tileSet0, 0x8800, 128);
            Array.Copy(this.tileSet0, 0, this.tileSet1, 0, 128);

            // $8000-$87FF Tile set #1: tiles 0 (128) to 127 (255)
            GetTiles(this.tileSet1, 0x8000, 128);
        }

        private void GetTiles(Tile[] tileSet, ushort address, int tileId)
        {
            var buffer = new byte[16];
            for (var i = 0; i < 128; address += 16, i++, tileId++)
            {
                tileRam.ReadBytes(address, buffer);
                var id = new Guid(buffer); // So happnes that a Guid stores 16 bytes nicely

                if (this.tileCache.ContainsKey(id))
                {
                    tileSet[tileId] = this.tileCache[id];
                }
                else
                {
                    tileSet[tileId] = GetTile(id, buffer);
                    this.tileCache[id] = tileSet[tileId];
                }
            }
        }

        private static Tile GetTile(Guid id, byte[] buffer)
        {
            var palette = new Palette[8][];
            for (var row = 0; row < 8; row++)
            {
                palette[row] = new Palette[8];
                var address = row * 2;
                var low = buffer[address];
                var high = buffer[address + 1];

                for (var col = 0; col < 8; col++)
                {
                    // Each value is a 2bit number stored in matching positions across low and high bytes
                    var mask = 0x1 << (8 - col - 1);
                    palette[row][col] = (Palette)(((low & mask) > 0 ? 1 : 0) + ((high & mask) > 0 ? 2 : 0));
                }
            }

            var tile = new Tile(id, palette);
            return tile;
        }

        public IEnumerable<IAddressSegment> AddressSegments => new[] { spriteRam, tileRam };
    }
}
