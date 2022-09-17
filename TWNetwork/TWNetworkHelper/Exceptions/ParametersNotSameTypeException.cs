using System;
using System.Runtime.Serialization;

namespace TWNetworkHelper
{
    [Serializable]
    public class ParametersNotSameTypeException : Exception
    {
        public ParametersNotSameTypeException()
        {
        }

        public ParametersNotSameTypeException(string message) : base(message)
        {
        }

        public ParametersNotSameTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParametersNotSameTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}