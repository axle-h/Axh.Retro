namespace Axh.Retro.GameBoy.Devices.CoreInterfaces
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