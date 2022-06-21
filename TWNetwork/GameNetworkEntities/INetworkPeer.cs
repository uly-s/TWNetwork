using System;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork
{
    public enum DeliveryMethodType
    {
        Reliable,
        Unreliable,
    }

    /// <summary>
    /// This class should be implemented by the Connection class, which represents a connection between a server and a client.
    /// </summary>
    public interface INetworkPeer
    {
        /// <summary>
        /// This method should send the buffer to the other side of the connection. 
        /// Probably a header can be beneficial to add to the buffer in this method to inform the other side, that it is a GameNetworkMessage.
        /// </summary>
        /// <param name="buffer">The buffer to send.</param>
        /// <param name="deliveryMethodType">The way the message should be sent.</param>
        void SendRaw(ArraySegment<byte> buffer,DeliveryMethodType deliveryMethodType);
    }
}