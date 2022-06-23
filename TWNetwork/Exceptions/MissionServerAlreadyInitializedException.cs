using System;
using System.Runtime.Serialization;

namespace TWNetwork
{
    [Serializable]
    internal class MissionEntityAlreadyInitializedException : Exception
    {
        public MissionEntityAlreadyInitializedException()
        {
        }

        public MissionEntityAlreadyInitializedException(string message) : base(message)
        {
        }

        public MissionEntityAlreadyInitializedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissionEntityAlreadyInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}