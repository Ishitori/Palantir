namespace Ix.Palantir.UI.Models.Metrics
{
    using System.Collections.Generic;
    using System.Linq;

    using Ix.Palantir.Services.API;

    using Services.API.Metrics;

    public class SocialViewModel : MetricsViewModel
    {
        public SocialViewModel(SocialMetrics metrics)
            : base(metrics)
        {
            this.Locations = GetLocations(metrics.MostPopularCities);
            this.MembersWithoutCity = metrics.MembersWithoutCity;
        }

        public dynamic Locations
        {
            get;
            private set;
        }
        public int MembersWithoutCity { get; set; }

        public static dynamic GetLocations(IList<PopularCityInfo> citiesData)
        {
            var result = new
            {
                name = "countries",
                children =
                    citiesData.GroupBy(x => x.Country)
                           .Select(
                               x =>
                               new
                               {
                                   name = x.Key,
                                   children = x.Select(c => new
                                   {
                                       name = c.City,
                                       size = c.MembersCount,

                                       groupPercent =
                                   CalculatePercent(
                                       c.MembersCount,
                                       x.Sum(mc => mc.MembersCount)),

                                       totalPercent =
                                   CalculatePercent(
                                       c.MembersCount,
                                       citiesData.Sum(
                                           mc => mc.MembersCount)),
                                   })
                               })
                           .ToList()
            };

            return result;
        }

        private static string CalculatePercent(int membersCount, int totalMembersCount)
        {
            var percent = ((double)membersCount / totalMembersCount).ToString("P");
            return percent;
        }
    }
}