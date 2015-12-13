namespace Axh.Retro.GameBoy.Devices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Axh.Retro.GameBoy.Contracts.Devices;
    using Axh.Retro.GameBoy.Devices.CoreInterfaces;
    using Axh.Retro.GameBoy.Registers.Interfaces;

    /// <summary>
    /// Serial port that will transfer bytes synchronously.
    /// Wanring! This will lock up the CPU if the Transfer funciton on the other SerialPort do any 'work'.
    /// </summary>
    public class SyncSerialPort : SerialPortBase, IDisposable
    {
        private const byte TransferStartFlagMask = 0x80;
        private const byte IsFastModeMask = 0x02;
        private const byte IsInternalClockMask = 0x01;
        private static readonly TimeSpan ExternalTransferTimeout = TimeSpan.FromSeconds(30); // Real GB doesn't have timeout but we can't lock this thread forever.

        private readonly IGameBoyInterruptManager gameBoyInterruptManager;

        private bool isFastMode;
        private bool transferStartFlag;
        private bool isInternalClock;

        private TaskCompletionSource<bool> transferredTaskSource;
        
        private CancellationTokenSource transferredCancellationSource;
        
        public SyncSerialPort(IGameBoyInterruptManager gameBoyInterruptManager)
        {
            this.gameBoyInterruptManager = gameBoyInterruptManager;
        }

        /// <summary>
        /// Bit 7 - Transfer Start Flag
        /// 0: Non transfer
        /// 1: Start transfer
        /// Bit 0 - Shift Clock
        /// 0: External Clock(500KHz Max.)
        /// 1: Internal Clock(8192Hz)
        /// Transfer is initiated by setting the
        /// Transfer Start Flag.This bit may be read
        /// and is automatically set to 0 at the end
        /// of Transfer.
        /// </summary>
        public override byte Register {
            get
            {
                var value = 0x00;
                if (this.transferStartFlag)
                {
                    value |= TransferStartFlagMask;
                }

                if (this.isFastMode)
                {
                    value |= IsFastModeMask;
                }


                if (this.isInternalClock)
                {
                    value |= IsInternalClockMask;
                }

                return (byte)value;
            }
            set
            {
                this.transferStartFlag = (value & TransferStartFlagMask) > 0;
                if (!this.transferStartFlag || this.ConnectedSerialPort == null)
                {
                    // No transfer, nothing to transfer to.
                    Reset();
                    return;
                }

                this.isFastMode = (value & IsFastModeMask) > 0;
                this.isInternalClock = (value & IsInternalClockMask) > 0;

                if (this.isInternalClock)
                {
                    this.SerialData.Register = this.ConnectedSerialPort.Transfer(this.SerialData.Register);
                }
                else
                {
                    // Wait...
                    transferredCancellationSource = new CancellationTokenSource();
                    transferredCancellationSource.CancelAfter(ExternalTransferTimeout);
                    transferredTaskSource = new TaskCompletionSource<bool>();
                    transferredTaskSource.Task.Wait(transferredCancellationSource.Token);
                }

                this.gameBoyInterruptManager.SerialLink();
                Reset();
            }
        }

        private void Reset()
        {
            this.transferStartFlag = false;
            this.isFastMode = false;
            this.isInternalClock = false;
        }

        public override byte Transfer(byte value)
        {
            var tmp = this.SerialData.Register;
            this.SerialData.Register = value;

            this.transferredTaskSource?.TrySetResult(true);

            return tmp;
        }

        public void Dispose()
        {
            if (this.transferredCancellationSource == null)
            {
                return;
            }

            this.transferredCancellationSource.Cancel();
            this.transferredCancellationSource.Dispose();
        }
    }
}
