namespace Ix.Palantir.Configuration.Queueing
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ConfigExtension
    {
        private const string CONST_QueueNamePrefix = "queue://";

        public static Queue GetQueueById(this IList<Queue> queues, string queueId)
        {
            return queues.Where(q => string.Compare(q.Id, queueId, false) == 0).FirstOrDefault();
        }
        public static Queue GetQueueByName(this IList<Queue> queues, string queueName)
        {
            if (!queueName.StartsWith(CONST_QueueNamePrefix))
            {
                queueName = CONST_QueueNamePrefix + queueName;
            }

            return queues.Where(q => q.Name.ToLower().StartsWith(queueName.ToLower())).FirstOrDefault();
        }
    }
}
