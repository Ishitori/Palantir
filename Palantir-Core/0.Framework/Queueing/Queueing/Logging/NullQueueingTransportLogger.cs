namespace Ix.Palantir.Queueing.Logging
{
    using System;

    using Ix.Palantir.Queueing.API;

    public class NullQueueingTransportLogger : IQueueingTransportLogger
    {
        public void LogMessageWasReceived(string id, string type, int tryIndex, DateTime sentDate, int deliveryDelay, string text)
        {
        }
        public void LogMessageWasSent(string id, string type, int tryIndex, DateTime sentDate, int deliveryDelay, string text)
        {
        }
    }
}