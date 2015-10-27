namespace Axh.Retro.CPU.X80.Core.DynaRec
{
    using System.Linq.Expressions;
    using System.Reflection;

    using Axh.Retro.CPU.X80.Contracts;

    using Xpr = Z80Expressions;

    internal partial class Z80BlockBuilder
    {
        private bool CpuSupportsDynamicTimings()
        {
            // Z80 supports some opcodes that generate dynamic timings.
            return this.cpuMode == CpuMode.Z80;
        }

        private bool CpuSupportsAccummulatorAndResultOperations()
        {
            // Z80 supports some opcodes that manipulate the accumulator and a result in memory at the same time.
            return this.cpuMode == CpuMode.Z80;
        }

        private Expression GetAluCallAssign(int mCycles, int tStates, MethodInfo aluMethod, Expression registerExpression)
        {
            timer.Add(mCycles, tStates);
            return Expression.Assign(registerExpression, Expression.Call(Xpr.Alu, aluMethod, registerExpression));
        }

        private Expression GetAluCallWrite(int mCycles, int tStates, MethodInfo aluMethod)
        {
            timer.Add(mCycles, tStates);
            return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, aluMethod, index.ReadIndexedValue));
        }
    }
}
