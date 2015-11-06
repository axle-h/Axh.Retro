namespace Axh.Retro.GameBoy.Contracts.Devices
{
    public interface IGameBoyInterruptManager
    {
        void VerticalBlank();

        void LcdStatusTriggers();

        void TimerOverflow();

        void SerialLink();

        void JoyPadPress();
    }
}