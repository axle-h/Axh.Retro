namespace Axh.Retro.CPU.X80.Core
{
    using System.Threading.Tasks;

    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.Memory;
    using Axh.Retro.CPU.X80.Contracts.Registers;

    /// <summary>
    /// Not implemented
    /// </summary>
    public class StaticCpuCore<TRegisters> : ICpuCore where TRegisters : IRegisters
    {
        private readonly TRegisters registers;

        private readonly IMmu mmu;

        private readonly IArithmeticLogicUnit arithmeticLogicUnit;

        private readonly IInstructionBlockDecoder<TRegisters> instructionBlockDecoder;

        public StaticCpuCore(IRegisterFactory<TRegisters> registerFactory, IMmuFactory mmuFactory, IInstructionBlockDecoder<TRegisters> instructionBlockDecoder, IArithmeticLogicUnit arithmeticLogicUnit)
        {
            this.instructionBlockDecoder = instructionBlockDecoder;
            this.arithmeticLogicUnit = arithmeticLogicUnit;
            this.registers = registerFactory.GetInitialRegisters();
            this.mmu = mmuFactory.GetMmu();
        }

        public Task StartCoreProcessAsync()
        {
            return Task.Factory.StartNew(StartCoreProcess, TaskCreationOptions.LongRunning);
        }

        public void StartCoreProcess()
        {
            while (true)
            {
                var instructionBlock = this.instructionBlockDecoder.DecodeNextBlock(this.mmu, this.registers.ProgramCounter);
                instructionBlock.ExecuteInstructionBlock(this.registers, this.mmu, this.arithmeticLogicUnit);
            }
        }
    }
}
