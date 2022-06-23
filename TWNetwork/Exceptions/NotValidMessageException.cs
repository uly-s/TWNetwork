using System;
using System.Runtime.Serialization;

namespace TWNetwork
{
    [Serializable]
    internal class NotValidMessageException : Exception
    {
        public NotValidMessageException()
        {
        }

        public NotValidMessageException(string message) : base(message)
        {
        }

        public NotValidMessageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotValidMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}