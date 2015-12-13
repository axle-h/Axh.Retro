﻿namespace Axh.Retro.GameBoy.Registers.Interfaces
{
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;

    /// <summary>
    /// DIV 0xff04 Divider Register (R/W)
    /// </summary>
    public abstract class DividerRegisterBase : IRegister
    {
        public ushort Address => 0xff04;

        public string Name => "Divider (DIV R/W)";

        public abstract byte Register { get; set; }

        public string DebugView => this.ToString();

        public override string ToString()
        {
            return $"{Name} ({Address}) = {Register}"; ;
        }
    }
}