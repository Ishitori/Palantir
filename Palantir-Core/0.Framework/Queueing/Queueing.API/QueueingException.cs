namespace Ix.Palantir.Queueing.API
{
    using System;
    using System.Runtime.Serialization;
    using Ix.Palantir.Exceptions;

    [Serializable]
    public class QueueingException : PalantirException
    {
        public QueueingException()
        {
        }

        public QueueingException(string message) : base(message)
        {
        }

        public QueueingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected QueueingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}