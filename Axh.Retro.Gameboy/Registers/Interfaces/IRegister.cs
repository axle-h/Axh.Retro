namespace Axh.Retro.GameBoy.Registers.Interfaces
{
    /// <summary>
    /// Gameboy memory mapped register.
    /// </summary>
    public interface IRegister
    {
        ushort Address { get; }

        string Name { get; }

        byte Register { get; set; }

        string DebugView { get; }
    }

    internal class SimpleRegister : IRegister
    {
        public SimpleRegister(ushort address, string name)
        {
            Address = address;
            Name = name;
        }

        public ushort Address { get; }

        public string Name { get; }

        public byte Register { get; set; }

        public string DebugView => this.ToString();

        public override string ToString()
        {
            return $"{Name} ({Address}) = {Register}"; ;
        }
    }
}
