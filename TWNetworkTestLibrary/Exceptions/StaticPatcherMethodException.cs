using System;
using System.Runtime.Serialization;

namespace TWNetworkPatcher
{
    [Serializable]
    public class StaticPatcherMethodException : Exception
    {
        public StaticPatcherMethodException()
        {
        }

        public StaticPatcherMethodException(string message) : base(message)
        {
        }

        public StaticPatcherMethodException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StaticPatcherMethodException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}