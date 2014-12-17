namespace Ix.Palantir.Domain.Analytics
{
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.Domain.Analytics.API;
    using Ix.Palantir.DomainModel;

    public class UserStatusCalculator : IUserStatusCalculator
    {
        public IList<long> GetInactiveUsers(
            IList<MemberWithStatus> userIds,
            IList<long> postCreatorIds,
            IList<long> postCommentCreatorIds,
            IList<long> topicCreatorIds,
            IList<long> topicCommentCreatorIds)
        {
            var userDictionary = userIds.Where(x => x.Status == MemberStatus.Active).ToDictionary(x => x.MemberId);

            this.ExcludeItems(userDictionary, postCreatorIds);
            this.ExcludeItems(userDictionary, postCommentCreatorIds);
            this.ExcludeItems(userDictionary, topicCreatorIds);
            this.ExcludeItems(userDictionary, topicCommentCreatorIds);

            return userDictionary.Keys.ToList();
        }
        public IList<long> GetBots(IList<MemberWithStatus> members)
        {
            return members.Where(m => m.Status == MemberStatus.Bot).Select(m => m.MemberId).ToList();
        }
        public IList<long> GetDeletedUsers(IList<MemberWithStatus> members)
        {
            return members.Where(m => m.Status == MemberStatus.Deleted).Select(m => m.MemberId).ToList();
        }
        public IList<long> GetBlockedUsers(IList<MemberWithStatus> members)
        {
            return members.Where(m => m.Status == MemberStatus.Blocked).Select(m => m.MemberId).ToList();
        }
        public IList<long> GetActiveUsers(IList<long> allUsers, IList<long> inactiveUsers, IList<long> bots, IList<long> deletedUsers, IList<long> blockedUsers)
        {
            var userDictionary = allUsers.ToDictionary(x => x);

            this.ExcludeItems(userDictionary, inactiveUsers);
            this.ExcludeItems(userDictionary, bots);
            this.ExcludeItems(userDictionary, deletedUsers);
            this.ExcludeItems(userDictionary, blockedUsers);

            return userDictionary.Keys.ToList();
        }
        public IDictionary<int, IList<long>> FindUsersIntersection(IList<long> initialUsers, Dictionary<int, IList<long>> otherProjectUsers)
        {
            var userDictionary = initialUsers.ToDictionary(x => x);
            IDictionary<int, IList<long>> result = new Dictionary<int, IList<long>>();

            foreach (var projectUser in otherProjectUsers)
            {
                IList<long> intersection = this.GetIntersection(userDictionary, projectUser.Value);
                result.Add(projectUser.Key, intersection);
            }

            return result;
        }

        private void ExcludeItems<T>(Dictionary<long, T> userDictionary, IList<long> creatorIds)
        {
            for (int i = 0; i < creatorIds.Count; i++)
            {
                if (userDictionary.ContainsKey(creatorIds[i]))
                {
                    userDictionary.Remove(creatorIds[i]);
                }
            }
        }
        private IList<long> GetIntersection(IDictionary<long, long> initialUsers, IList<long> otherUsers)
        {
            IList<long> intersection = new List<long>();

            for (int i = 0; i < otherUsers.Count; i++)
            {
                if (initialUsers.ContainsKey(otherUsers[i]))
                {
                    intersection.Add(otherUsers[i]);
                }
            }

            return intersection;
        }
    }
}