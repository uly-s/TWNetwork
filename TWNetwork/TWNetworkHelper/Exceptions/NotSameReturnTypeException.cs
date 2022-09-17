using System;
using System.Runtime.Serialization;

namespace TWNetworkHelper
{
    [Serializable]
    public class NotSameReturnTypeException : Exception
    {
        public NotSameReturnTypeException()
        {
        }

        public NotSameReturnTypeException(string message) : base(message)
        {
        }

        public NotSameReturnTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotSameReturnTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}