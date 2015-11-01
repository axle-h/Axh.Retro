namespace Axh.Retro.CPU.X80.Core
{
    using System;
    using System.Threading.Tasks;
    
    using Axh.Retro.CPU.X80.Contracts.Cache;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Core.Timing;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.IO;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public class CachingCpuCore<TRegisters> : ICpuCore where TRegisters : IRegisters
    {
        private readonly IRegisterFactory<TRegisters> registerFactory;

        private readonly IMmuFactory mmuFactory;

        private readonly IAluFactory aluFactory;

        private readonly IInstructionBlockDecoder<TRegisters> instructionBlockDecoder;

        private readonly IInstructionBlockCache<TRegisters> instructionBlockCache;

        private readonly IInputOutputManager inputOutputManager;

        private readonly IInstructionTimer instructionTimer;

        public CachingCpuCore(
            IRegisterFactory<TRegisters> registerFactory,
            IMmuFactory mmuFactory,
            IInstructionBlockDecoder<TRegisters> instructionBlockDecoder,
            IAluFactory aluFactory,
            IInstructionBlockCache<TRegisters> instructionBlockCache,
            IInputOutputManager inputOutputManager,
            IInstructionTimer instructionTimer)
        {
            this.registerFactory = registerFactory;
            this.mmuFactory = mmuFactory;
            this.instructionBlockDecoder = instructionBlockDecoder;
            this.aluFactory = aluFactory;
            this.instructionBlockCache = instructionBlockCache;
            this.inputOutputManager = inputOutputManager;
            this.instructionTimer = instructionTimer;

            if (!this.instructionBlockDecoder.SupportsInstructionBlockCaching)
            {
                throw new Exception("Instruction block decoder must support caching");
            }
        }

        public async Task StartCoreProcessAsync()
        {
            // Build components
            var registers = registerFactory.GetInitialRegisters();
            var mmu = mmuFactory.GetMmu();
            var alu = this.aluFactory.GetArithmeticLogicUnit(registers.AccumulatorAndFlagsRegisters.Flags);

            // Register the invalidate cache event with mmu AddressWrite event
            mmu.AddressWrite += (sender, args) => this.instructionBlockCache.InvalidateCache(args.Address, args.Length);

            while (true)
            {
                var address = registers.ProgramCounter;
                var instructionBlock = this.instructionBlockCache.GetOrSet(address, () => this.instructionBlockDecoder.DecodeNextBlock(address, mmu));
                var timings = instructionBlock.ExecuteInstructionBlock(registers, mmu, alu, this.inputOutputManager);

                await this.instructionTimer.SyncToTimings(timings);
            }
        }
    }
}
