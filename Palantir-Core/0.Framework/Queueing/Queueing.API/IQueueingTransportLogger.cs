namespace Ix.Palantir.Queueing.API
{
    using System;

    public interface IQueueingTransportLogger
    {
        void LogMessageWasSent(string id, string type, int tryIndex, DateTime sentDate, int deliveryDelay, string text);
        void LogMessageWasReceived(string id, string type, int tryIndex, DateTime sentDate, int deliveryDelay, string text);
    }
}
