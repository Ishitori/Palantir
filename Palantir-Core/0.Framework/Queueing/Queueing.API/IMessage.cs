namespace Ix.Palantir.Queueing.API
{
    using System;
    using System.Collections.Generic;

    public interface IMessage
    {
        string Id { get; }
        string Type { get; set; }
        string Text { get; set; }
        int TryIndex { get; set; }
        DateTime SentDate { get; set; }

        string QueueName { get; }
        int DeliveryTimeout { get; set; }
        int TtlInMinutes { get; set; }
        IDictionary<string, string> ExtraProperties { get; set; }

        IMessage Copy();
        void MarkAsProcessed();
        void MarkAsFailedToProcess();
    }
}
