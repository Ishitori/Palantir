namespace Ix.Palantir.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;

    public class MemberSubscriptionRepository : IMemberSubscriptionRepository
    {
        private readonly IDataGatewayProvider dataGatewayProvider;
        private readonly ILog log;

        public MemberSubscriptionRepository(IDataGatewayProvider dataGatewayProvider, ILog log)
        {
            this.dataGatewayProvider = dataGatewayProvider;
            this.log = log;
        }

        public MemberSubscriptionCollection GetSubscriptions(int vkGroupId, long vkMemberId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var query = @"select ms.vkmemberid, ms.vkgroupid, ms.subscribedvkgroupid, vkr.vkgroupid, vkr.namegroup, vkr.screenname, vkr.photo from membersubscriptions ms inner join vkgroupreference vkr on (ms.subscribedvkgroupid = vkr.vkgroupid) where ms.vkgroupid = @VkGroupId and ms.vkmemberid = @VkMemberId";
                var subscriptions = dataGateway.Connection.Query<MemberSubscription, VkGroupReference, MemberSubscription>(
                    query,
                    (subscription, groupReference) =>
                    {
                        subscription.SubscribedVkGroup = groupReference;
                        return subscription;
                    },
                    new { vkGroupId, vkMemberId },
                    splitOn: "vkr.vkgroupid").ToList();
                var collection = new MemberSubscriptionCollection(vkGroupId, vkMemberId, subscriptions);
                
                return collection;
            }
        }

        public void Save(MemberSubscription subscription)
        {
            if (!subscription.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                try
                {
                    subscription.Id = dataGateway.Connection.Query<int>(@"insert into membersubscriptions(vkmemberid, vkgroupid, subscribedvkgroupid) values (@VkMemberId, @VkGroupId, @SubscribedVkGroupId) RETURNING id", subscription).First();
                }
                catch (Exception exc)
                {
                    this.log.ErrorFormat("Unable to save subscription {0} due to: {1}", subscription.ToString(), exc.ToString());
                    throw;
                }
            }
        }

        public void Save(MemberSubscriptionCollection subscriptions, IList<MemberSubscription> existingSubscriptions)
        {
            foreach (var memberSubscription in subscriptions.Subscriptions)
            {
                this.Save(memberSubscription);
            }
        }

        public void Delete(MemberSubscription subscription)
        {
            Contract.Requires(subscription != null);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.Connection.Execute(@"delete from membersubscriptions where id = @Id", new { subscription.Id });
            }
        }

        public void Delete(MemberSubscriptionCollection subscriptions)
        {
            foreach (var subscription in subscriptions.Subscriptions)
            {
                this.Delete(subscription);
            }
        }
    }
}