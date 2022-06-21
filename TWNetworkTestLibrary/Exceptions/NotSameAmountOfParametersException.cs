using System;
using System.Runtime.Serialization;

namespace TWNetworkPatcher
{
    [Serializable]
    internal class NotSameAmountOfParametersException : Exception
    {
        public NotSameAmountOfParametersException()
        {
        }

        public NotSameAmountOfParametersException(string message) : base(message)
        {
        }

        public NotSameAmountOfParametersException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotSameAmountOfParametersException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}