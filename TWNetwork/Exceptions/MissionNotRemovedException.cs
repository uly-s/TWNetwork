using System;
using System.Runtime.Serialization;

namespace TWNetwork
{
    [Serializable]
    internal class MissionNotRemovedException : Exception
    {
        public MissionNotRemovedException()
        {
        }

        public MissionNotRemovedException(string message) : base(message)
        {
        }

        public MissionNotRemovedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissionNotRemovedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}