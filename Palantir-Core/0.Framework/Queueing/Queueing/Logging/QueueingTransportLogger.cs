namespace Ix.Palantir.Queueing.Logging
{
    using System;
    using Configuration.Queueing;
    using Framework.ObjectFactory;

    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.Queueing.API;

    using Palantir.Logging;

    public class QueueingTransportLogger : IQueueingTransportLogger, IDisposable
    {
        private const string CONST_QueueingLogItemFormat = "{0:yyyy-MM-dd H:mm:ss} {1} {2} {3} {4} {5:yy-MM-dd H:mm:ss} {6} {7}\r\n";
        private readonly AsyncBuffer<QueueingLogItem> asyncBuffer;
        private readonly ILog logger;

        public QueueingTransportLogger()
        {
            this.asyncBuffer = new AsyncBuffer<QueueingLogItem>(this.GetConfiguration().TransportLoggerBufferSize, this.ReadLogBuffer);
            this.logger = LogManager.GetLogger(ActivityType.Queueing.ToString());
        }

        public void LogMessageWasSent(string id, string type, int tryIndex, DateTime sentDate, int deliveryDelay, string text)
        {
            this.WriteLogBuffer(QueueingLogActionType.MessageSent, id, type, tryIndex, sentDate, deliveryDelay, text);
        }
        public void LogMessageWasReceived(string id, string type, int tryIndex, DateTime sentDate, int deliveryDelay, string text)
        {
            this.WriteLogBuffer(QueueingLogActionType.MessageReceived, id, type, tryIndex, sentDate, deliveryDelay, text);
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.asyncBuffer.Dispose();
            }
        }

        private void WriteLogBuffer(QueueingLogActionType actionType, string id, string type, int tryIndex, DateTime sentDate, int deliveryDelay, string text)
        {
            if (!this.GetConfiguration().EnableTransportLogging)
            {
                return;
            }

            if (!this.asyncBuffer.ReadStarted)
            {
                this.asyncBuffer.StartRead();
            }

            var logItem = new QueueingLogItem(actionType, id, type, tryIndex, sentDate, deliveryDelay, text);

            this.asyncBuffer.WriteAsync(logItem);
        }
        private void ReadLogBuffer(QueueingLogItem queueingLogItem)
        {
            this.logger.InfoFormat(
                CONST_QueueingLogItemFormat,
                queueingLogItem.LoggingDate,
                (int)queueingLogItem.Type,
                queueingLogItem.MessageId,
                queueingLogItem.MessageType,
                queueingLogItem.MessageTryIndex,
                queueingLogItem.MessageSentDate,
                queueingLogItem.DeliveryDelay,
                this.FormatMessageText(queueingLogItem.MessageText));
        }
        private string FormatMessageText(string text)
        {
            text = text
                    .Replace("=" + Environment.NewLine, string.Empty)
                    .Replace(Environment.NewLine, "|")
                    .Replace(" ", "+")
                    .Replace("\t", "++++");

            int maxLength = this.GetConfiguration().TransportLoggerMessageMaxLength;
            return text.Length > maxLength
                           ? text.Substring(0, maxLength)
                           : text;
        }

        private QueueingConfig GetConfiguration()
        {
            return Factory.GetInstance<IConfigurationProvider>().GetConfigurationSection<QueueingConfig>();
        }
    }
}