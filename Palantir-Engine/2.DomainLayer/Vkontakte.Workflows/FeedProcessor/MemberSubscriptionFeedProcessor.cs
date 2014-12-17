namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.DomainModel.Comparers;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Utilities;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Responses.MemberSubscription;
    using response = Ix.Palantir.Vkontakte.API.Responses.MemberSubscription.response;

    public class MemberSubscriptionFeedProcessor : IFeedProcessor
    {
        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;
        private readonly IMemberSubscriptionRepository memberRepository;

        public MemberSubscriptionFeedProcessor(ILog log, IVkResponseMapper responseMapper, IMemberSubscriptionRepository memberRepository)
        {
            this.log = log;
            this.responseMapper = responseMapper;
            this.memberRepository = memberRepository;
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);
            this.ProcessSubscriptions(feed, dataFeed.RelatedObjectId, group);
        }

        private void ProcessSubscriptions(response memberSubscriptions, string vkMemberId, VkGroup group)
        {
            IList<MemberSubscription> newSubscriptions = new List<MemberSubscription>();
            long memberId = long.Parse(vkMemberId);

            MemberSubscriptionCollection oldSubscriptions = this.memberRepository.GetSubscriptions(group.Id, memberId);

            if (memberSubscriptions.group != null && memberSubscriptions.group.Length > 0)
            {
                newSubscriptions = this.GetNewSubscriptionList(group.Id, memberId, memberSubscriptions.group);
            }

            this.log.DebugFormat("Old subscriptions: {0}", this.GetDebugString(oldSubscriptions.Subscriptions));
            this.log.DebugFormat("New subscriptions: {0}", this.GetDebugString(newSubscriptions));
            IDictionary<int, MemberSubscription> oldSubscriptionHash = oldSubscriptions.Subscriptions.Distinct(new MemberSubscriptionEqualityComparer()).ToDictionary(x => x.SubscribedVkGroupId);
            IDictionary<int, MemberSubscription> newSubscriptionHash = newSubscriptions.Distinct(new MemberSubscriptionEqualityComparer()).ToDictionary(x => x.SubscribedVkGroupId);

            IList<MemberSubscription> toInsert = newSubscriptions.Where(newS => !oldSubscriptionHash.ContainsKey(newS.SubscribedVkGroupId)).ToList();
            IList<MemberSubscription> toDelete = oldSubscriptions.Subscriptions.Where(old => !newSubscriptionHash.ContainsKey(old.SubscribedVkGroupId)).ToList();
            IList<MemberSubscription> same = newSubscriptions.Where(newS => oldSubscriptionHash.ContainsKey(newS.SubscribedVkGroupId)).ToList();
            this.log.DebugFormat("Records to insert: {0}", this.GetDebugString(toInsert));
            this.log.DebugFormat("Records to delete: {0}", this.GetDebugString(toDelete));
            this.log.DebugFormat("Same records: {0}", this.GetDebugString(same));

            // here need to update titles of the groups
            this.memberRepository.Delete(new MemberSubscriptionCollection(group.Id, memberId, toDelete));
            this.memberRepository.Save(new MemberSubscriptionCollection(group.Id, memberId, toInsert), same);

            this.log.DebugFormat("Subscription for MemberId={0} are processed. {1} subscription(s) inserted and {2} subscription(s) deleted", vkMemberId, toInsert.Count, toDelete.Count);
        }

        private string GetDebugString(IEnumerable<MemberSubscription> subscriptions)
        {
            var builder = new SeparatedStringBuilder("; ");

            foreach (var subscription in subscriptions)
            {
                builder.AppendFormatWithSeparator("<{0}, {1}, {2}>", subscription.VkGroupId, subscription.VkMemberId, subscription.SubscribedVkGroupId);
            }

            return builder.ToString();
        }

        private IList<MemberSubscription> GetNewSubscriptionList(int groupId, long vkMemberId, IEnumerable<responseGroup> responseGroups)
        {
            IList<MemberSubscription> newSubscriptions = new List<MemberSubscription>();

            foreach (var responseGroup in responseGroups)
            {
                var memberSubscription = new MemberSubscription
                {
                    VkGroupId = groupId,
                    SubscribedVkGroupId = int.Parse(responseGroup.gid),
                    VkMemberId = vkMemberId,
                    SubscribedVkGroup = new VkGroupReference()
                    {
                        VkGroupId = int.Parse(responseGroup.gid),
                        NameGroup = responseGroup.name,
                        Photo = responseGroup.photo_medium,
                        ScreenName = responseGroup.screen_name
                    }
                };

                newSubscriptions.Add(memberSubscription);
            }

            return newSubscriptions;
        }
    }
}