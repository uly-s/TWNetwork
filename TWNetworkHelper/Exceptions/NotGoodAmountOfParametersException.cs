using System;
using System.Runtime.Serialization;

namespace TWNetworkHelper
{
    [Serializable]
    public class NotGoodAmountOfParametersException : Exception
    {
        public NotGoodAmountOfParametersException()
        {
        }

        public NotGoodAmountOfParametersException(string message) : base(message)
        {
        }

        public NotGoodAmountOfParametersException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotGoodAmountOfParametersException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}