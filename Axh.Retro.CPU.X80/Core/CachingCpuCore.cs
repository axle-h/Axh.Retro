namespace Axh.Retro.CPU.X80.Core
{
    using System;
    using System.Runtime.Caching;
    using System.Threading.Tasks;

    using Axh.Retro.CPU.X80.Contracts;
    using Axh.Retro.CPU.X80.Contracts.Cache;
    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    public class CachingCpuCore<TRegisters> : ICpuCore where TRegisters : IRegisters
    {
        private readonly TRegisters registers;

        private readonly IMmu mmu;

        private readonly IArithmeticLogicUnit arithmeticLogicUnit;

        private readonly IInstructionBlockDecoder<TRegisters> instructionBlockDecoder;

        private readonly IInstructionBlockCache<TRegisters> instructionBlockCache;

        public CachingCpuCore(IRegisterFactory<TRegisters> registerFactory, IMmuFactory mmuFactory, IInstructionBlockDecoder<TRegisters> instructionBlockDecoder, IArithmeticLogicUnit arithmeticLogicUnit, IInstructionBlockCache<TRegisters> instructionBlockCache)
        {
            this.instructionBlockDecoder = instructionBlockDecoder;
            this.arithmeticLogicUnit = arithmeticLogicUnit;
            this.instructionBlockCache = instructionBlockCache;
            this.registers = registerFactory.GetInitialRegisters();
            this.mmu = mmuFactory.GetMmu();

            if (!this.instructionBlockDecoder.SupportsInstructionBlockCaching)
            {
                throw new Exception("Instruction block decoder must support caching");
            }

            // Register the invalidate cache event with mmu AddressWrite event
            this.mmu.AddressWrite += (sender, args) => this.instructionBlockCache.InvalidateCache(args.Address, args.Length);
        }

        public Task StartCoreProcessAsync()
        {
            return Task.Factory.StartNew(StartCoreProcess, TaskCreationOptions.LongRunning);
        }

        public void StartCoreProcess()
        {
            while (true)
            {
                var address = this.registers.ProgramCounter;
                var instructionBlock = this.instructionBlockCache.GetOrSet(address, () => this.instructionBlockDecoder.DecodeNextBlock(this.mmu, address));
                instructionBlock.ExecuteInstructionBlock(this.registers, this.mmu, this.arithmeticLogicUnit);
            }
        }
    }
}
