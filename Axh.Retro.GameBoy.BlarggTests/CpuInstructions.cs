using NUnit.Framework;

namespace Axh.Retro.GameBoy.BlarggTests
{
    public class CpuInstructions : BlarggTestBase
    {
        [Test]
        public void cpu_instrs_01_special()
        {
            Run(Resources.cpu_instrs_01_special);
        }

        [Test, Ignore]
        public void cpu_instrs_02_interrupts()
        {
            Run(Resources.cpu_instrs_02_interrupts);
        }

        [Test]
        public void cpu_instrs_03_op_sp_hl()
        {
            Run(Resources.cpu_instrs_03_op_sp_hl);
        }

        [Test]
        public void cpu_instrs_04_op_r_imm()
        {
            Run(Resources.cpu_instrs_04_op_r_imm);
        }

        [Test]
        public void cpu_instrs_05_op_rp()
        {
            Run(Resources.cpu_instrs_05_op_rp);
        }

        [Test]
        public void cpu_instrs_06_ld_r_r()
        {
            Run(Resources.cpu_instrs_06_ld_r_r);
        }

        [Test]
        public void cpu_instrs_07_jr_jp_call_ret_rst()
        {
            Run(Resources.cpu_instrs_07_jr_jp_call_ret_rst);
        }

        [Test]
        public void cpu_instrs_08_misc_instrs()
        {
            Run(Resources.cpu_instrs_08_misc_instrs);
        }

        [Test]
        public void cpu_instrs_09_op_r_r()
        {
            Run(Resources.cpu_instrs_09_op_r_r);
        }

        [Test]
        public void cpu_instrs_10_bit_ops()
        {
            Run(Resources.cpu_instrs_10_bit_ops);
        }

        [Test]
        public void cpu_instrs_11_op_a_mhl()
        {
            Run(Resources.cpu_instrs_11_op_a_mhl);
        }
    }
}