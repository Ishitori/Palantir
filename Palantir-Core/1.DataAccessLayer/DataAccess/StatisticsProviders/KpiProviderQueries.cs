namespace Ix.Palantir.DataAccess.StatisticsProviders
{
    using Ix.Palantir.Querying.Common;

    public class KpiProviderQueries
    {
        private const string CONST_DateRangeSuffix = " and posteddate >= @from and posteddate <= @to";

        public KpiProviderQueries(DateRange dateRange)
        {
            this.PostDataQuery = @"select count(*)::int as postscount, sum(likescount)::int as likescount, sum(commentscount)::int as commentscount from post where vkgroupid = @vkgroupid"
                + (dateRange.IsSpecified ? CONST_DateRangeSuffix : string.Empty);

            this.GenericMemberCountQuery = @"select count(*)::int as memberscount from member where vkgroupid = @vkgroupid";
            this.MemberCountQuery = this.GenericMemberCountQuery;
            const string PostsWithAdminCommentsCountQueryTemplate = @"
            select count(distinct pc.vkpostid)::int as postswithadmincommentscount 
            from postcomment pc
            where 
	        pc.vkgroupid = @vkgroupid
	        AND pc.creatorid in @adminids
	        AND pc.vkpostid in (select p.vkid from post p where p.vkgroupid = @vkgroupid AND p.creatorid not in @adminids {0})";

            this.PostsWithAdminCommentsCountQuery = string.Format(PostsWithAdminCommentsCountQueryTemplate, dateRange.IsSpecified ? CONST_DateRangeSuffix : string.Empty);

            this.AdminPostsCountQuery = @"select count(p.vkid)::integer as adminpostscount from post p where p.vkgroupid = @vkgroupid AND p.creatorid in @adminids"
                + (dateRange.IsSpecified ? CONST_DateRangeSuffix : string.Empty);
        }

        public string PostDataQuery { get; set; }
        public string GenericMemberCountQuery { get; set; }
        public string MemberCountQuery { get; set; }
        public string PostsWithAdminCommentsCountQuery { get; set; }
        public string AdminPostsCountQuery { get; set; }
    }
}