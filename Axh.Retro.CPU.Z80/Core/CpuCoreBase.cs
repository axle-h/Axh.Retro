using System;
using System.Threading;
using System.Threading.Tasks;
using Axh.Retro.CPU.Common.Contracts.Memory;
using Axh.Retro.CPU.Common.Contracts.Memory.Dma;
using Axh.Retro.CPU.Common.Contracts.Timing;
using Axh.Retro.CPU.Z80.Contracts.Cache;
using Axh.Retro.CPU.Z80.Contracts.Core;
using Axh.Retro.CPU.Z80.Contracts.Peripherals;
using Axh.Retro.CPU.Z80.Contracts.Registers;

namespace Axh.Retro.CPU.Z80.Core
{
    /// <summary>
    /// Base class for CPU cores with external api implemented and dispose wired up.
    /// </summary>
    public abstract class CpuCoreBase : ICpuCore
    {
        protected IRegisters Registers;
        protected readonly IInterruptManager InterruptManager;
        protected readonly IPeripheralManager PeripheralManager;
        protected readonly IMmu Mmu;
        protected readonly IInstructionTimer InstructionTimer;
        protected readonly IAlu Alu;
        protected readonly IInstructionBlockDecoder InstructionBlockDecoder;
        protected readonly IDmaController DmaController;

        private ushort? _interruptAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="CpuCoreBase"/> class.
        /// </summary>
        /// <param name="registers">The registers.</param>
        /// <param name="interruptManager">The interrupt manager.</param>
        /// <param name="peripheralManager">The peripheral manager.</param>
        /// <param name="mmu">The mmu.</param>
        /// <param name="instructionTimer">The instruction timer.</param>
        /// <param name="alu">The alu.</param>
        /// <param name="instructionBlockDecoder">The instruction block decoder.</param>
        /// <param name="dmaController">The dma controller.</param>
        protected CpuCoreBase(IRegisters registers,
            IInterruptManager interruptManager,
            IPeripheralManager peripheralManager,
            IMmu mmu,
            IInstructionTimer instructionTimer,
            IAlu alu,
            IInstructionBlockDecoder instructionBlockDecoder,
            IDmaController dmaController)
        {
            CoreId = Guid.NewGuid();
            Registers = registers;
            InterruptManager = interruptManager;
            PeripheralManager = peripheralManager;
            Mmu = mmu;
            InstructionTimer = instructionTimer;
            Alu = alu;
            InstructionBlockDecoder = instructionBlockDecoder;
            DmaController = dmaController;
        }

        /// <summary>
        /// Gets the core identifier.
        /// </summary>
        /// <value>
        /// The core identifier.
        /// </value>
        public Guid CoreId { get; }

        /// <summary>
        /// Starts the core process.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public abstract Task StartCoreProcessAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieve peripheral of specified type.
        /// </summary>
        /// <typeparam name="TPeripheral"></typeparam>
        /// <returns></returns>
        public TPeripheral GetPeripheralOfType<TPeripheral>() where TPeripheral : IPeripheral => PeripheralManager.PeripheralOfType<TPeripheral>();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            InterruptManager.Dispose();
            PeripheralManager.Dispose();
            Mmu.Dispose();
            DmaController.Dispose();
        }

        /// <summary>
        /// Gets the next address.
        /// </summary>
        /// <returns></returns>
        protected ushort GetNextAddress() => _interruptAddress ?? Registers.ProgramCounter;
        
        /// <summary>
        /// Executes the specified instruction block.
        /// </summary>
        /// <param name="instructionBlock">The instruction block.</param>
        /// <returns></returns>
        protected async Task ExecuteInstructionBlockAsync(IInstructionBlock instructionBlock)
        {
            var timings = instructionBlock.ExecuteInstructionBlock(Registers, Mmu, Alu, PeripheralManager);

            if (instructionBlock.HaltCpu)
            {
                InterruptManager.Halt();
                if (instructionBlock.HaltPeripherals)
                {
                    PeripheralManager.Signal(ControlSignal.Halt);
                    InterruptManager.AddResumeTask(() => PeripheralManager.Signal(ControlSignal.Resume));
                }
            }

            if (InterruptManager.IsHalted)
            {
                // Did we request an interrupt or run a HALT opcode.
                if (InterruptManager.IsInterrupted || instructionBlock.HaltCpu)
                {
                    // Notify halt success before halting
                    InterruptManager.NotifyHalt();
                    _interruptAddress = await InterruptManager.WaitForNextInterrupt().ConfigureAwait(false);

                    // Push the program counter onto the stack
                    Registers.StackPointer = (ushort) (Registers.StackPointer - 2);
                    Mmu.WriteWord(Registers.StackPointer, Registers.ProgramCounter);
                }
                else
                {
                    // Dummy halt so we don't block threads trigerring interrupts when disabled.
                    InterruptManager.NotifyHalt();
                }

                InterruptManager.NotifyResume();
            }
            else
            {
                _interruptAddress = null;
            }

            InstructionTimer.SyncToTimings(timings);
        }
    }
}
