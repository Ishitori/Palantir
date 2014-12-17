namespace Ix.Palantir.Querying.Common
{
    public class RankedValue<T>
    {
        public RankedValue(T value, int rank)
        {
            this.Value = value;
            this.Rank = rank;
        }

        public RankedValue(T value)
        {
            this.Value = value;
        }

        public T Value { get; set; }
        public int Rank { get; set; }
    }
}