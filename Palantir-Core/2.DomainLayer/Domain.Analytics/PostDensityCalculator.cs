namespace Ix.Palantir.Domain.Analytics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Ix.Palantir.Domain.Analytics.API;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;

    public class PostDensityCalculator : IPostDensityCalculator
    {
        private const int CONST_IntervalLengthHours = 3;

        public IList<PostDensity> GetMostCrowdedTime(IList<Post> posts)
        {
            Contract.Requires(posts != null);

            if (posts.Count == 0)
            {
                return new List<PostDensity>();
            }

            var first = posts.Min(p => p.PostedDate);
            var last = posts.Max(p => p.PostedDate);
            IDictionary<TimeFrame, PostDensity> intervaledDensities = this.CreateInterval(first, last, CONST_IntervalLengthHours);
            IDictionary<TimeFrame, PostDensity> perHourDensities = this.CreateInterval(first.AddHours(-CONST_IntervalLengthHours + 1), last, 1);

            foreach (var post in posts)
            {
                IList<TimeFrame> densitiesToIncrease = this.GetIntervalItems(post);

                foreach (var timeFrame in densitiesToIncrease)
                {
                    intervaledDensities[timeFrame].AbsoluteValue++;
                }

                TimeFrame perHourInterval = TimeFrame.Create(new DateTime(post.Year, post.Month, post.Day, post.Hour, 0, 0), new DateTime(post.Year, post.Month, post.Day, post.Hour, 0, 0).AddHours(1));
                perHourDensities[perHourInterval].AbsoluteValue++;
            }

            var result = intervaledDensities.Values.OrderByDescending(d => d.AbsoluteValue).ToList();
            this.ShrinkDensities(result, perHourDensities.Values);
            this.CalculateRelativeValues(posts, result);

            return result;
        }

        private IDictionary<TimeFrame, PostDensity> CreateInterval(DateTime first, DateTime last, int intervalLength)
        {
            DateTime start = new DateTime(first.Year, first.Month, first.Day, first.Hour, 0, 0).AddHours(-intervalLength + 1);
            DateTime end = new DateTime(last.Year, last.Month, last.Day, last.Hour, 0, 0).AddHours(1);
            IDictionary<TimeFrame, PostDensity> densities = new Dictionary<TimeFrame, PostDensity>();

            DateTime currentDate = start;

            while (currentDate < end)
            {
                DateTime nextDate = currentDate.AddHours(intervalLength);
                TimeFrame frame = TimeFrame.Create(currentDate, nextDate);

                if (densities.ContainsKey(frame))
                {
                    break;
                }

                densities.Add(frame, new PostDensity { TimeFrame = frame });
                currentDate = currentDate.AddHours(1);
            }

            return densities;
        }
        private IList<TimeFrame> GetIntervalItems(Post post)
        {
            IList<TimeFrame> frames = new List<TimeFrame>();
            DateTime start = new DateTime(post.Year, post.Month, post.Day, post.Hour, 0, 0).AddHours(-CONST_IntervalLengthHours + 1);
            DateTime end = new DateTime(post.Year, post.Month, post.Day, post.Hour, 0, 0).AddHours(1);

            DateTime currentDate = start;

            while (currentDate < end)
            {
                DateTime nextDate = currentDate.AddHours(CONST_IntervalLengthHours);
                var timeFrame = TimeFrame.Create(currentDate, nextDate);
                frames.Add(timeFrame);
                currentDate = currentDate.AddHours(1);
            }

            return frames;
        }
        private void CalculateRelativeValues(IList<Post> posts, List<PostDensity> result)
        {
            double postsCount = posts.Count;

            foreach (var postDensity in result)
            {
                postDensity.RelativeValue = Math.Round((postDensity.AbsoluteValue / postsCount) * 100, 2);
            }
        }
        private void ShrinkDensities(List<PostDensity> result, IEnumerable<PostDensity> perHourDensities)
        {
            IList<PostDensity> emptyItems = perHourDensities.Where(v => v.AbsoluteValue == 0).ToList();

            if (emptyItems.Count == 0)
            {
                return;
            }

            foreach (var emptyDensity in emptyItems)
            {
                foreach (var density in result)
                {
                    if (density.TimeFrame.DayOfWeek == emptyDensity.TimeFrame.DayOfWeek)
                    {
                        if (density.TimeFrame.BeginHour == emptyDensity.TimeFrame.BeginHour)
                        {
                            density.TimeFrame.BeginHour = emptyDensity.TimeFrame.EndHour;
                        }

                        if (density.TimeFrame.EndHour == emptyDensity.TimeFrame.EndHour)
                        {
                            density.TimeFrame.EndHour = emptyDensity.TimeFrame.BeginHour;
                        }
                    }
                }
            }
        }
    }
}