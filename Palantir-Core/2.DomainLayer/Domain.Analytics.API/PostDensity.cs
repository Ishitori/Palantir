namespace Ix.Palantir.Domain.Analytics.API
{
    using Ix.Palantir.Localization.API;

    public class PostDensity
    {
        public PostDensity()
        {
            this.TimeFrame = new TimeFrame();
        }

        public int AbsoluteValue { get; set; }
        public double RelativeValue { get; set; }
        public TimeFrame TimeFrame { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.TimeFrame, this.AbsoluteValue);
        }
    }
}