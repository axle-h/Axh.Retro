namespace Axh.Emulation.CPU.X80.Contracts.Core
{
    using System;

    using Axh.Emulation.CPU.X80.Contracts.Memory;
    using Axh.Emulation.CPU.X80.Contracts.Registers;

    public class Z80DynamicallyRecompiledBlock
    {
        public ushort Address { get; set; }

        public ushort Length { get; set; }

        public Action<IZ80Registers, IMmu> Action { get; set; }
    }
}
