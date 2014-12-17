namespace Ix.Palantir.Querying.Common
{
    public class DoubleComparativeValue : ComparativeValue<double>
    {
        public DoubleComparativeValue(double anotherValue, double baseValue) : base(anotherValue, baseValue)
        {
        }

        protected override double Minus(double a, double b)
        {
            return a - b;
        }

        protected override double Divide(double a, double b)
        {
            return a / b;
        }

        protected override bool IsZero(double a)
        {
            return a == 0;
        }
    }
}