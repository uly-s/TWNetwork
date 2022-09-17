using System;
using System.Runtime.Serialization;

namespace TWNetworkHelper
{
    [Serializable]
    public class NotInstanceMethodException : Exception
    {
        public NotInstanceMethodException()
        {
        }

        public NotInstanceMethodException(string message) : base(message)
        {
        }

        public NotInstanceMethodException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotInstanceMethodException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}