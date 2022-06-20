using System;
using System.Runtime.Serialization;

namespace TWNetwork
{
    [Serializable]
    internal class MissionIDNotAddedException : Exception
    {
        public MissionIDNotAddedException()
        {
        }

        public MissionIDNotAddedException(string message) : base(message)
        {
        }

        public MissionIDNotAddedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissionIDNotAddedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}