namespace Axh.Retro.CPU.X80.Core
{
    using System.Threading.Tasks;

    using Axh.Retro.CPU.X80.Contracts.Core;
    using Axh.Retro.CPU.X80.Contracts.Core.Timing;
    using Axh.Retro.CPU.X80.Contracts.Factories;
    using Axh.Retro.CPU.X80.Contracts.IO;
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

        private readonly IInputOutputManager inputOutputManager;

        private readonly IInstructionTimer instructionTimer;

        public StaticCpuCore(
            IRegisterFactory<TRegisters> registerFactory,
            IMmuFactory mmuFactory,
            IInstructionBlockDecoder<TRegisters> instructionBlockDecoder,
            IArithmeticLogicUnit arithmeticLogicUnit,
            IInputOutputManager inputOutputManager,
            IInstructionTimer instructionTimer)
        {
            this.instructionBlockDecoder = instructionBlockDecoder;
            this.arithmeticLogicUnit = arithmeticLogicUnit;
            this.inputOutputManager = inputOutputManager;
            this.instructionTimer = instructionTimer;
            this.registers = registerFactory.GetInitialRegisters();
            this.mmu = mmuFactory.GetMmu();
        }

        public async Task StartCoreProcessAsync()
        {
            while (true)
            {
                var instructionBlock = this.instructionBlockDecoder.DecodeNextBlock(this.registers.ProgramCounter, this.mmu);
                var timings = instructionBlock.ExecuteInstructionBlock(this.registers, this.mmu, this.arithmeticLogicUnit, this.inputOutputManager);
                await this.instructionTimer.SyncToTimings(timings);
            }
        }

        public Task Interrupt(ushort address)
        {
            throw new System.NotImplementedException();
        }
    }
}
