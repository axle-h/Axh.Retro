namespace Axh.Retro.GameBoy.Registers.Interfaces
{
    public interface ICurrentScanlineRegister : IRegister
    {
        void IncrementScanline();
    }
}