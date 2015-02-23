namespace Axh.Emulation.CPU.X80.Z80
{
    using System;
    using System.Threading.Tasks;

    using Axh.Emulation.CPU.X80.Contracts;
    using Axh.Emulation.CPU.X80.Contracts.Factories;
    using Axh.Emulation.CPU.X80.Contracts.Memory;
    using Axh.Emulation.CPU.X80.Contracts.OpCodes;
    using Axh.Emulation.CPU.X80.Contracts.Registers;
    using Axh.Emulation.CPU.X80.Util;

    public class Z80Core : ICpuCore
    {
        private readonly IZ80Registers registers;

        private readonly IMmu mmu;
        
        public Z80Core(IRegisterFactory registerFactory, IMmuFactory mmuFactory)
        {
            this.registers = registerFactory.GetInitialZ80Registers();
            this.mmu = mmuFactory.GetMmu();
        }

        public async Task StartCoreProcess()
        {
            while (true)
            {
                // Get next instruction
                var instruction = this.GetNextInstruction();

                // Try to parse the opcode
                if (!Enum.IsDefined(typeof(PrimaryOpCode), instruction))
                {
                    var ex = new NotSupportedException("Primary Op Code: " + instruction.ToHex());
                    throw ex;
                }

                var opcode = (PrimaryOpCode)instruction;
            }
        }

        private byte GetNextInstruction()
        {
            // Get next instruction
            var opcode = this.mmu.ReadByte(this.registers.ProgramCounter);

            // Increment LS 7 bits of memory refresh register
            this.registers.R = unchecked((byte)((this.registers.R + 1) & 0x7f));

            // Increment program counter register
            this.registers.ProgramCounter = unchecked((ushort)(this.registers.ProgramCounter + 1));

            return opcode;
        }


    }
}
