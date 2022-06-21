using System;
using System.Runtime.Serialization;

namespace TWNetworkPatcher
{
    [Serializable]
    internal class NotSameReturnTypeException : Exception
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