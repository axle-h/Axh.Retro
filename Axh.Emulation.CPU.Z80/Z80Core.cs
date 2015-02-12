namespace Axh.Emulation.CPU.Z80
{
    using System.Threading.Tasks;
    using Axh.Emulation.CPU.Z80.Contracts.Factories;
    using Axh.Emulation.CPU.Z80.Contracts.Registers;

    public class Z80Core
    {
        private readonly IRegisterSet primaryRegisterSet;
        private readonly IRegisterSet alternativeRegisterSet;
        private IRegisterSet currentRegisterSet;
        private bool isAlternative;

        public Z80Core(IRegisterSetFactory registerSetFactory)
        {
            this.primaryRegisterSet = registerSetFactory.GetRegisterSet();
            this.alternativeRegisterSet = registerSetFactory.GetRegisterSet();
            this.currentRegisterSet = this.primaryRegisterSet;
            this.isAlternative = false;
        }

        public async Task StartCoreProcess()
        {
            
        }


    }
}
