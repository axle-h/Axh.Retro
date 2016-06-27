using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Registers
{
    public class SimpleRegister : IRegister
    {
        public SimpleRegister(ushort address, string name)
        {
            Address = address;
            Name = name;
        }

        public ushort Address { get; }

        public string Name { get; }

        public byte Register { get; set; }

        public string DebugView => ToString();

        public override string ToString()
        {
            return $"{Name} ({Address}) = {Register}";
        }
    }
}