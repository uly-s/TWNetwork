using System;
using System.Runtime.Serialization;

namespace TWNetwork
{
    [Serializable]
    internal class MissionAlreadyAddedException : Exception
    {
        public MissionAlreadyAddedException()
        {
        }

        public MissionAlreadyAddedException(string message) : base(message)
        {
        }

        public MissionAlreadyAddedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissionAlreadyAddedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}