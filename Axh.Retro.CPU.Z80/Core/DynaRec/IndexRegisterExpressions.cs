namespace Axh.Retro.CPU.Z80.Core.DynaRec
{
    using System.Linq.Expressions;

    internal class IndexRegisterExpressions
    {
        public IndexRegister IndexRegister { get; set; }
        public Expression Register { get; set; }
        public Expression RegisterLowOrder { get; set; }
        public Expression RegisterHighOrder { get; set; }
        public Expression IndexedAddress { get; set; }
        public Expression ReadIndexedValue { get; set; }
        public bool UsesDisplacedIndexTimings { get; set; }
    }
}
