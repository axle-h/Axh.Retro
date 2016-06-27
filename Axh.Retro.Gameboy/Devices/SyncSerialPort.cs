using System;
using System.Threading.Tasks;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Registers.Interfaces;

namespace Axh.Retro.GameBoy.Devices
{
    /// <summary>
    ///     Serial port that will transfer bytes synchronously.
    ///     Wanring! This will lock up the CPU if the Transfer funciton on the other SerialPort do any 'work'.
    /// </summary>
    public class SyncSerialPort : SerialPortBase
    {
        private const byte TransferStartFlagMask = 0x80;
        private const byte IsFastModeMask = 0x02;
        private const byte IsInternalClockMask = 0x01;

        private static readonly TimeSpan ExternalTransferTimeout = TimeSpan.FromSeconds(1);
                                         // Real GB doesn't have timeout but we can't lock this thread forever.

        private readonly IInterruptFlagsRegister interruptFlagsRegister;

        private bool isFastMode;
        private bool isInternalClock;

        private TaskCompletionSource<bool> transferredTaskSource;
        private bool transferStartFlag;

        public SyncSerialPort(IInterruptFlagsRegister interruptFlagsRegister)
        {
            this.interruptFlagsRegister = interruptFlagsRegister;
        }

        /// <summary>
        ///     Bit 7 - Transfer Start Flag
        ///     0: Non transfer
        ///     1: Start transfer
        ///     Bit 0 - Shift Clock
        ///     0: External Clock(500KHz Max.)
        ///     1: Internal Clock(8192Hz)
        ///     Transfer is initiated by setting the
        ///     Transfer Start Flag.This bit may be read
        ///     and is automatically set to 0 at the end
        ///     of Transfer.
        /// </summary>
        public override byte Register
        {
            get
            {
                var value = 0x00;
                if (transferStartFlag)
                {
                    value |= TransferStartFlagMask;
                }

                if (isFastMode)
                {
                    value |= IsFastModeMask;
                }


                if (isInternalClock)
                {
                    value |= IsInternalClockMask;
                }

                return (byte) value;
            }
            set
            {
                transferStartFlag = (value & TransferStartFlagMask) > 0;
                if (!transferStartFlag)
                {
                    // No transfer, nothing to transfer to.
                    Reset();
                    return;
                }

                isFastMode = (value & IsFastModeMask) > 0;
                isInternalClock = (value & IsInternalClockMask) > 0;

                if (isInternalClock)
                {
                    SerialData.Register = ConnectedSerialPort?.Transfer(SerialData.Register) ?? 0x00;
                }
                else
                {
                    // Wait... Except we also need to be listening for interrupts some how...
                    // transferredTaskSource = new TaskCompletionSource<bool>();
                    // transferredTaskSource.Task.Wait(ExternalTransferTimeout);

                    // Not implemented.
                    SerialData.Register = 0x00;
                }

                Reset();
            }
        }

        private void Reset()
        {
            transferStartFlag = false;
            isFastMode = false;
            isInternalClock = false;
        }

        public override byte Transfer(byte value)
        {
            var tmp = SerialData.Register;
            SerialData.Register = value;

            transferredTaskSource?.TrySetResult(true);

            interruptFlagsRegister.UpdateInterrupts(InterruptFlag.SerialLink);
            return tmp;
        }
    }
}