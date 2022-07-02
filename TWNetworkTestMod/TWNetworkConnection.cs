using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNetwork.NetworkFiles;

namespace TWNetworkTestMod
{
    public class TWNetworkConnection : TWNetworkPeer
    {
        public readonly NetPeer Peer;

        public TWNetworkConnection(NetPeer peer)
        {
            Peer = peer;
            Peer.Tag = this;
        }
        public void SendRaw(byte[] buffer, DeliveryMethodType deliveryMethodType)
        {
            Peer.Send(buffer, (deliveryMethodType == DeliveryMethodType.Reliable) ? DeliveryMethod.ReliableOrdered : DeliveryMethod.Unreliable);
        }
    }

    public static class NetPeerExtensions 
    {
        public static TWNetworkConnection GetConnection(this NetPeer peer)
        {
            return (TWNetworkConnection)peer.Tag;
        }
    }
}
