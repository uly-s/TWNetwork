using System;
using System.Runtime.Serialization;

namespace TWNetwork
{
    [Serializable]
    internal class MissionMissingException : Exception
    {
        public MissionMissingException()
        {
        }

        public MissionMissingException(string message) : base(message)
        {
        }

        public MissionMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissionMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}