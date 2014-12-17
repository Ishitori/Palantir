namespace Ix.Palantir.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class PalantirException : Exception
    {
        public PalantirException()
        {
        }

        public PalantirException(string message)
            : base(message)
        {
        }
        
        public PalantirException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PalantirException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}