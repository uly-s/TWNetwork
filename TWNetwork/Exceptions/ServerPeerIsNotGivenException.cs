using System;
using System.Runtime.Serialization;

namespace TWNetwork
{
    [Serializable]
    internal class ServerPeerIsNotGivenException : Exception
    {
        public ServerPeerIsNotGivenException()
        {
        }

        public ServerPeerIsNotGivenException(string message) : base(message)
        {
        }

        public ServerPeerIsNotGivenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ServerPeerIsNotGivenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}