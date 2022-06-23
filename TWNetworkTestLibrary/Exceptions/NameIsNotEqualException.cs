using System;
using System.Runtime.Serialization;

namespace TWNetworkPatcher
{
    [Serializable]
    public class NameIsNotEqualException : Exception
    {
        public NameIsNotEqualException()
        {
        }

        public NameIsNotEqualException(string message) : base(message)
        {
        }

        public NameIsNotEqualException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NameIsNotEqualException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}