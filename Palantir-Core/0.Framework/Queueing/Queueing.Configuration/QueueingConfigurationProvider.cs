namespace Ix.Palantir.Queueing.Configuration
{
    using System;
    using Ix.Palantir.Framework.Configuration;

    public class QueueingConfigurationProvider : ConfigurationProvider<QueueingConfig>, IQueueingConfigurationProvider
    {
        protected override string CacheKey
        {
            get
            {
                return "22428936-FBAD-4C03-8591-E628408E3C2B";
            }
        }

        protected override string ConfigFileName
        {
            get 
            {
                return "QueueingConfig";
            }
        }
        public override Type ConfigurationSectionType
        {
            get
            {
                return typeof(QueueingConfig);
            }
        }
    }
}
