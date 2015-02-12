namespace Axh.Emulation.CPU.Z80
{
    using System.Threading.Tasks;
    using Axh.Emulation.CPU.Z80.Contracts.Factories;
    using Axh.Emulation.CPU.Z80.Contracts.Registers;

    public class Z80Core
    {
        private readonly IZ80Registers registers;
        
        public Z80Core(IRegisterFactory registerFactory)
        {
            this.registers = registerFactory.GetInitialZ80Registers();
        }

        public async Task StartCoreProcess()
        {
            
        }


    }
}
