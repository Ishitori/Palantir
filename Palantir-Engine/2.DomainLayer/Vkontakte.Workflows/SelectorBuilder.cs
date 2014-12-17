namespace Ix.Palantir.Vkontakte.Workflows
{
    using System.Collections.Generic;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Configuration;
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Utilities;

    public class SelectorBuilder
    {
        public string BuildSelector(int vkGroupId = 0, string feedType = null)
        {
            Dictionary<string, string> selectorParts = new Dictionary<string, string>();

            if (vkGroupId != 0)
            {
                selectorParts.Add(DataFeed.VkGroupKey, vkGroupId.ToString());
            }

            string selector = selectorParts.ToFormattedString();
            var processingConfig = Factory.GetInstance<IConfigurationProvider>().GetConfigurationSection<FeedProcessingConfig>();

            if (!string.IsNullOrWhiteSpace(feedType))
            {
                selectorParts.Add(DataFeed.DataFeedTypeKey, feedType);
                return selectorParts.ToFormattedString();
            }

            if (!string.IsNullOrWhiteSpace(processingConfig.FeedFilter))
            {
                selector = string.Format("{0}{1}", selector, string.IsNullOrWhiteSpace(selector) ? processingConfig.FeedFilter : " AND " + processingConfig.FeedFilter);
            }

            return !string.IsNullOrWhiteSpace(selector) ? selector : null;
        }
    }
}