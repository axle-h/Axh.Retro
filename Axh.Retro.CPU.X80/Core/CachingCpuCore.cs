namespace Axh.Retro.CPU.X80.Core
{
    using System;
    using System.Threading.Tasks;
    
    using Axh.Retro.CPU.X80.Contracts.Cache;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Core.Timing;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Peripherals;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public class CachingCpuCore<TRegisters> : ICpuCore where TRegisters : IRegisters
    {
        private readonly IRegisterFactory<TRegisters> registerFactory;

        private readonly IMmuFactory mmuFactory;

        private readonly IAluFactory aluFactory;

        private readonly IInstructionBlockDecoder<TRegisters> instructionBlockDecoder;

        private readonly IInstructionBlockCache<TRegisters> instructionBlockCache;

        private readonly IPeripheralManagerFactory peripheralManagerFactory;

        private readonly IInstructionTimer instructionTimer;

        private readonly IInterruptManagerFactory interruptManagerFactory;

        public CachingCpuCore(
            IRegisterFactory<TRegisters> registerFactory,
            IMmuFactory mmuFactory,
            IInstructionBlockDecoder<TRegisters> instructionBlockDecoder,
            IAluFactory aluFactory,
            IInstructionBlockCache<TRegisters> instructionBlockCache,
            IPeripheralManagerFactory peripheralManagerFactory,
            IInstructionTimer instructionTimer,
            IInterruptManagerFactory interruptManagerFactory)
        {
            this.registerFactory = registerFactory;
            this.mmuFactory = mmuFactory;
            this.instructionBlockDecoder = instructionBlockDecoder;
            this.aluFactory = aluFactory;
            this.instructionBlockCache = instructionBlockCache;
            this.peripheralManagerFactory = peripheralManagerFactory;
            this.instructionTimer = instructionTimer;
            this.interruptManagerFactory = interruptManagerFactory;

            if (!this.instructionBlockDecoder.SupportsInstructionBlockCaching)
            {
                throw new Exception("Instruction block decoder must support caching");
            }
            
        }

        public async Task StartCoreProcessAsync()
        {
            // Build components
            var registers = registerFactory.GetInitialRegisters();
            var interruptManager = this.interruptManagerFactory.GetInterruptManager();
            var peripheralManager = this.peripheralManagerFactory.GetPeripheralsManager(interruptManager);
            var mmu = mmuFactory.GetMmu(peripheralManager);
            var alu = this.aluFactory.GetArithmeticLogicUnit(registers.AccumulatorAndFlagsRegisters.Flags);
            

            // Register the invalidate cache event with mmu AddressWrite event
            mmu.AddressWrite += (sender, args) => this.instructionBlockCache.InvalidateCache(args.Address, args.Length);
            ushort? interruptAddress = null;

            while (true)
            {
                var address = interruptAddress ?? registers.ProgramCounter;
                var instructionBlock = this.instructionBlockCache.GetOrSet(address, () => this.instructionBlockDecoder.DecodeNextBlock(address, mmu));
                var timings = instructionBlock.ExecuteInstructionBlock(registers, mmu, alu, peripheralManager);

                if (instructionBlock.HaltCpu)
                {
                    interruptManager.Halt();
                    if (instructionBlock.HaltPeripherals)
                    {
                        peripheralManager.Halt();
                        interruptManager.AddResumeTask(() => peripheralManager.Resume());
                    }
                }

                if (interruptManager.IsHalted)
                {
                    if (registers.InterruptFlipFlop1)
                    {
                        // Notify halt success before halting
                        interruptManager.NotifyHalt();
                        interruptAddress = await interruptManager.WaitForNextInterrupt();

                        // Push the program counter onto the stack
                        registers.StackPointer = unchecked((ushort)(registers.StackPointer - 2));
                        mmu.WriteWord(registers.StackPointer, registers.ProgramCounter);
                    }
                    else
                    {
                        // Dummy halt so we don't block threads trigerring interrupts when disabled.
                        interruptManager.NotifyHalt();
                    }
                    interruptManager.NotifyResume();
                }
                else
                {
                    interruptAddress = null;
                    await this.instructionTimer.SyncToTimings(timings);
                }
            }
        }
        
        

        
    }
}
