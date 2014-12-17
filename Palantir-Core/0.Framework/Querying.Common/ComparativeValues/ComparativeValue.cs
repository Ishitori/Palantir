namespace Ix.Palantir.Querying.Common
{
    public abstract class ComparativeValue<T>
    {
        private readonly T anotherValue;
        private readonly T baseValue;

        protected ComparativeValue(T anotherValue, T baseValue)
        {
            this.baseValue = baseValue;
            this.anotherValue = anotherValue;
        }

        public T Value
        {
            get { return this.anotherValue; }
        }

        public T AbsouluteDifference
        {
            get { return this.Minus(this.anotherValue, this.baseValue); }
        }

        public double RelativeDifference
        {
            get { return (1 - this.Divide(this.anotherValue, this.baseValue)) * 100; }
        }

        public bool CanCalculateRelativeDifference
        {
            get
            {
                return !this.IsZero(this.baseValue);
            }
        }

        protected abstract T Minus(T a, T b);
        protected abstract double Divide(T a, T b);
        protected abstract bool IsZero(T a);
    }
}