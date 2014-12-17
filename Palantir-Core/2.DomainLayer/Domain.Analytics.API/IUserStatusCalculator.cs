namespace Ix.Palantir.Domain.Analytics.API
{
    using System.Collections.Generic;

    public interface IUserStatusCalculator
    {
        IList<long> GetInactiveUsers(IList<MemberWithStatus> userIds, IList<long> postCreatorIds, IList<long> postCommentCreatorIds, IList<long> topicCreatorIds, IList<long> topicCommentCreatorIds);
        IList<long> GetBots(IList<MemberWithStatus> members);
        IList<long> GetDeletedUsers(IList<MemberWithStatus> members);
        IList<long> GetBlockedUsers(IList<MemberWithStatus> members);
        IList<long> GetActiveUsers(IList<long> allUsers, IList<long> inactiveUsers, IList<long> bots, IList<long> deletedUsers, IList<long> blockedUsers);
        
        IDictionary<int, IList<long>> FindUsersIntersection(IList<long> initialUsers, Dictionary<int, IList<long>> otherProjectUsers);
    }
}