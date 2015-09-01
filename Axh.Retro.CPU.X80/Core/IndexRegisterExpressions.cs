namespace Axh.Retro.CPU.X80.Core
{
    using System.Linq.Expressions;

    internal class IndexRegisterExpressions
    {
        public Expression Register { get; set; }
        public Expression RegisterLowOrder { get; set; }
        public Expression RegisterHighOrder { get; set; }
        public Expression IndexedAddress { get; set; }
        public Expression IndexedValue { get; set; }
        public bool UsesDisplacedIndexTimings { get; set; }
    }
}
