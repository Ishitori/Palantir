namespace Ix.Palantir.UI.Models.Metrics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Script.Serialization;
    using Ix.Palantir.Services.API;

    public class InterestsViewModel
    {
        public InterestsViewModel(List<MemberInterestsObject> interests)
        {
            this.Object = new
                {
                    name = "interests",
                    children = interests.GroupBy(x => x.Type).Select(x => new
                        {
                            name = x.Key,
                            children = x.Select(y => new
                                {
                                    name = y.Title,
                                    size = y.Count,

                                    groupPercent = CalculatePercent(y.Count, x.Sum(z => z.Count)),
                                    totalPercent = CalculatePercent(y.Count, interests.Sum(z => z.Count)),
                                })
                        })
                };
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            this.Interests = serializer.Serialize(this.Object);
        }

        public string Interests { get; set; }
        public dynamic Object { get; set; }

        private static string CalculatePercent(int membersCount, int totalMembersCount)
        {
            var percent = ((double)membersCount / totalMembersCount).ToString("P");
            return percent;
        }
    }
}