namespace Ix.Palantir.UI.Formatters
{
    using System;
    using Ix.Palantir.Querying.Common;

    public class ComparativeValueFormatter
    {
        private readonly DoubleComparativeValue doubleValue;
        private readonly Int32ComparativeValue intValue;

        public ComparativeValueFormatter(DoubleComparativeValue doubleValue)
        {
            this.doubleValue = doubleValue;
        }

        public ComparativeValueFormatter(Int32ComparativeValue intValue)
        {
            this.intValue = intValue;
        }

        protected double AbsoluteValue
        {
            get
            {
                return this.doubleValue != null
                           ? this.doubleValue.Value
                           : this.intValue.Value;
            }
        }

        protected double AbsoluteDifference
        {
            get
            {
                return this.doubleValue != null
                           ? this.doubleValue.AbsouluteDifference
                           : this.intValue.AbsouluteDifference;
            }
        }

        protected double RelativeDifference
        {
            get
            {
                return this.doubleValue != null
                           ? this.doubleValue.RelativeDifference
                           : this.intValue.RelativeDifference;
            }
        }

        protected bool CanCalculateRelativeDifference
        {
            get
            {
                return this.doubleValue != null
                           ? this.doubleValue.CanCalculateRelativeDifference
                           : this.intValue.CanCalculateRelativeDifference;
            }
        }

        public override string ToString()
        {
            string absoluteValue = Math.Round(this.AbsoluteValue, 2).ToString();
            var cssClass = string.Empty;

            if (this.AbsoluteDifference != 0)
            {
                cssClass = this.AbsoluteDifference < 0 ? "negValue" : "posValue";
            }

            return string.Format("<div class=\"comparativeValue clearfix {1}\"><div class=\"absoluteValue\">{0}</div></div>", absoluteValue, cssClass);
        }
    }
}