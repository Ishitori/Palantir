namespace Ix.Palantir.Querying.Common
{
    public class Int32ComparativeValue : ComparativeValue<int>
    {
        public Int32ComparativeValue(int anotherValue, int baseValue) : base(anotherValue, baseValue)
        {
        }

        protected override int Minus(int a, int b)
        {
            return a - b;
        }

        protected override double Divide(int a, int b)
        {
            return (double)a / b;
        }

        protected override bool IsZero(int a)
        {
            return a == 0;
        }
    }
}