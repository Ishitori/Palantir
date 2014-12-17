namespace Ix.Palantir.Queueing
{
    using System;
    using System.Linq;

    using Ix.Palantir.Queueing.API;
    using Ix.Palantir.Queueing.API.Command;

    public static class MessageExtension
    {
        private const string CONST_MessageTypeKey = "IxMessageType";
        private const string CONST_SentDateKey = "IxSentDate";
        private const string CONST_RedeliveryIndexKey = "IxRedeliveryIndex";
        private const string CONST_DeliveryDelayKey = "AMQ_SCHEDULED_DELAY";

        public static void SetMessageType(this Apache.NMS.IMessage message, string type)
        {
            message.Properties[CONST_MessageTypeKey] = type;
        }
        public static string GetMessageType(this Apache.NMS.IMessage message)
        {
            if (message.Properties.Contains(CONST_MessageTypeKey))
            {
                object messageType = message.Properties[CONST_MessageTypeKey];
                return messageType != null ? messageType.ToString() : string.Empty;
            }

            return string.Empty;
        }

        public static string GetId(this Apache.NMS.IMessage message)
        {
            var activeMQMessage = message as Apache.NMS.ActiveMQ.Commands.ActiveMQMessage;

            if (activeMQMessage == null)
            {
                return string.Empty;
            }

            return activeMQMessage.NMSMessageId;
        }
        public static DateTime GetSentDate(this Apache.NMS.IMessage message)
        {
            if (message.Properties.Contains(CONST_SentDateKey))
            {
                DateTime sentDate;
                object sentDateObject = message.Properties[CONST_SentDateKey];

                if (sentDateObject != null && DateTime.TryParse(sentDateObject.ToString(), out sentDate))
                {
                    return sentDate;
                }
            }

            return DateTime.MinValue;
        }
        public static void SetSentDate(this Apache.NMS.IMessage message, DateTime sendDate)
        {
            message.Properties[CONST_SentDateKey] = sendDate.ToString();
        }

        public static int GetTryIndex(this Apache.NMS.IMessage message)
        {
            var activeMQMessage = message as Apache.NMS.ActiveMQ.Commands.ActiveMQMessage;

            if (activeMQMessage == null)
            {
                return -1;
            }

            int redeliveryCounter = activeMQMessage.Properties.Contains(CONST_RedeliveryIndexKey)
                ? Convert.ToInt32(activeMQMessage.Properties[CONST_RedeliveryIndexKey])
                : activeMQMessage.RedeliveryCounter;

            return redeliveryCounter + 1;
        }
        public static void SetTryIndex(this Apache.NMS.IMessage message, int tryIndex)
        {
            var activeMQMessage = message as Apache.NMS.ActiveMQ.Commands.ActiveMQMessage;

            if (activeMQMessage == null)
            {
                return;
            }

            if (tryIndex <= 0)
            {
                return;
            }

            activeMQMessage.Properties[CONST_RedeliveryIndexKey] = tryIndex - 1;
        }
        public static int GetDeliveryDelay(this Apache.NMS.IMessage message)
        {
            var activeMQMessage = message as Apache.NMS.ActiveMQ.Commands.ActiveMQMessage;

            if (activeMQMessage == null)
            {
                return 0;
            }

            object deliveryTimeout = message.Properties[CONST_DeliveryDelayKey];

            if (deliveryTimeout == null)
            {
                return 0;
            }

            return Convert.ToInt32(deliveryTimeout);
        }
        public static void SetDeliveryDelay(this Apache.NMS.IMessage message, int delayInMs)
        {
            var activeMQMessage = message as Apache.NMS.ActiveMQ.Commands.ActiveMQMessage;

            if (activeMQMessage == null)
            {
                return;
            }

            if (delayInMs <= 0)
            {
                return;
            }

            message.Properties[CONST_DeliveryDelayKey] = delayInMs;
        }

        public static bool IsUnsupported(this IMessage message, ICommandRepository commandRepository)
        {
            return !commandRepository.GetSupportedCommandNames().Any(cn => string.Compare(cn, message.Type, false) == 0);
        }
    }
}
