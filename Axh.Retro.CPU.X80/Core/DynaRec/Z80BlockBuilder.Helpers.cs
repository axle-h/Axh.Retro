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

        private Expression BitTest(Expression registerExpression, int bit)
        {
            timer.Add(2, 8);
            return Expression.Call(Xpr.Alu, Xpr.AluBitTest, registerExpression, Expression.Constant(bit));
        }

        private Expression BitTestFromIndex(int bit)
        {
            if (index.UsesDisplacedIndexTimings)
            {
                // Timings are DD/FD prefix NOP(1, 4) + (4, 16) = (5, 20)
                timer.Add(4, 16);
            }
            else
            {
                timer.Add(3, 12);
            }
            return Expression.Call(Xpr.Alu, Xpr.AluBitTest, index.ReadIndexedValue, Expression.Constant(bit));
        }

        private Expression BitSet(Expression registerExpression, int bit)
        {
            timer.Add(2, 8);
            return Expression.Assign(registerExpression, Expression.Call(Xpr.Alu, Xpr.AluBitSet, registerExpression, Expression.Constant(bit)));
        }

        private Expression BitSetFromIndex(int bit)
        {
            if (index.UsesDisplacedIndexTimings)
            {
                // Timings are DD/FD prefix NOP(1, 4) + (5, 19) = (6, 23)
                timer.Add(5, 19);
            }
            else
            {
                timer.Add(4, 15);
            }

            return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluBitSet, index.ReadIndexedValue, Expression.Constant(bit)));
        }

        private Expression BitReset(Expression registerExpression, int bit)
        {
            timer.Add(2, 8);
            return Expression.Assign(registerExpression, Expression.Call(Xpr.Alu, Xpr.AluBitReset, registerExpression, Expression.Constant(bit)));
        }

        private Expression BitResetFromIndex(int bit)
        {
            if (index.UsesDisplacedIndexTimings)
            {
                // Timings are DD/FD prefix NOP(1, 4) + (5, 19) = (6, 23)
                timer.Add(5, 19);
            }
            else
            {
                timer.Add(4, 15);
            }

            return Expression.Call(Xpr.Mmu, Xpr.MmuWriteByte, index.IndexedAddress, Expression.Call(Xpr.Alu, Xpr.AluBitReset, index.ReadIndexedValue, Expression.Constant(bit)));
        }
    }
}
