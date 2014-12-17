namespace Ix.Palantir.App.Tests
{
    using System;
    using System.Collections.Generic;
    using Ix.Palantir.Domain.Analytics;
    using Ix.Palantir.Domain.Analytics.API;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Querying.Common;

    using NUnit.Framework;

    [TestFixture]
    public class PostDensityCalculatorTest
    {
        [Test]
        public void Check_ForSinglePost()
        {
            Post post1 = new Post() { PostedDate = new DateTime(2013, 04, 14, 11, 0, 0) };
            Post post2 = new Post() { PostedDate = new DateTime(2013, 04, 14, 12, 17, 0) };
            Post post3 = new Post() { PostedDate = new DateTime(2013, 04, 15, 12, 17, 0) };
            IList<Post> posts = new List<Post> { post1, post2, post3 };

            PostDensityCalculator calculator = new PostDensityCalculator();
            IList<PostDensity> mostCrowdedTime = calculator.GetMostCrowdedTime(posts);

            Assert.That(mostCrowdedTime.Count > 0);
            Assert.That(mostCrowdedTime[0].AbsoluteValue == 2);
            Assert.That(mostCrowdedTime[0].TimeFrame.Equals(new TimeFrame(DayOfWeek.Sunday, 11, 13)));
        }
    }
}