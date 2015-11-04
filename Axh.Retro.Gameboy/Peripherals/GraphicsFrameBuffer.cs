namespace Axh.Retro.GameBoy.Peripherals
{
    using System.Collections.Generic;

    using Axh.Retro.CPU.Common.Config;
    using Axh.Retro.CPU.Common.Contracts.Config;
    using Axh.Retro.CPU.Common.Contracts.Memory;
    using Axh.Retro.CPU.Common.Memory;
    using Axh.Retro.CPU.X80.Contracts.Config;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;
    using Axh.Retro.GameBoy.Contracts.Peripherals;

    public class GraphicsFrameBuffer : IMemoryMappedPeripheral
    {
        private static readonly IMemoryBankConfig SpriteRamConfig = new SimpleMemoryBankConfig(MemoryBankType.Peripheral, null, 0xfe00, 0x9f);
        private static readonly IMemoryBankConfig MapRamConfig = new SimpleMemoryBankConfig(MemoryBankType.Peripheral, null, 0x8000, 0x1fff);
        
        /// <summary>
        /// $FE00-$FE9F	OAM - Object Attribute Memory
        /// </summary>
        private readonly ArrayBackedMemoryBank spriteRam;
        
        /// <summary>
        /// $9C00-$9FFF	BG Map Data 2
        /// $9800-$9BFF BG Map Data 1
        /// $8000-$97FF	Character RAM
        /// </summary>
        private readonly ArrayBackedMemoryBank mapRam;

        private readonly IInterruptManager interruptManager;

        private readonly IHardwareRegisters hardwareRegisters;

        private readonly IRenderHandler renderhandler;

        public GraphicsFrameBuffer(IInterruptManager interruptManager, IHardwareRegisters hardwareRegisters, IRenderHandler renderhandler)
        {
            this.interruptManager = interruptManager;
            this.hardwareRegisters = hardwareRegisters;
            this.renderhandler = renderhandler;
            this.spriteRam = new ArrayBackedMemoryBank(SpriteRamConfig);
            this.mapRam = new ArrayBackedMemoryBank(MapRamConfig);
        }

        public void Halt()
        {
            throw new System.NotImplementedException();
        }

        public void Resume()
        {
            throw new System.NotImplementedException();
        }
        
        public IEnumerable<IAddressSegment> AddressSegments => new[] { mapRam, spriteRam };
    }
}
