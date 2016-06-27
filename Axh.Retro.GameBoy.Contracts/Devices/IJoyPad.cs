namespace Axh.Retro.GameBoy.Contracts.Devices
{
    public interface IJoyPad
    {
        bool Up { get; set; }

        bool Down { get; set; }

        bool Left { get; set; }

        bool Right { get; set; }

        bool A { get; set; }

        bool B { get; set; }

        bool Select { get; set; }

        bool Start { get; set; }
    }
}