using System;
using System.Collections.Concurrent;
using System.Linq;
using TaleWorlds.MountAndBlade;
using TWNetwork.InterfacePatches;
using TWNetwork.NetworkFiles;

namespace TWNetwork.Extensions
{
    public static class NetworkCommunicatorExtensions
    {
        private static ConcurrentDictionary<TWNetworkPeer, NativeMBPeer> PeerToCommunicator = new ConcurrentDictionary<TWNetworkPeer, NativeMBPeer>();
        private static ConcurrentDictionary<NativeMBPeer, TWNetworkPeer> CommunicatorToPeer = new ConcurrentDictionary<NativeMBPeer, TWNetworkPeer>();
        public static void Send(this NetworkCommunicator communicator, byte[] buffer, DeliveryMethodType methodType)
        {
            if (GameNetwork.IsServer)
            {
                if (communicator == GameNetwork.MyPeer)
                    return;
                NativeMBPeer peer = IMBNetworkServer.Server.FindPeerByCommunicator(communicator);
                if (!CommunicatorToPeer.ContainsKey(peer))
                    throw new InvalidOperationException();
                CommunicatorToPeer[peer].SendRaw(buffer, methodType); 
            }
            else
                throw new InvalidOperationException();
        }
        public static NetworkCommunicator GetNetworkCommunicator(this TWNetworkPeer peer)
        {
            if (!GameNetwork.IsServer || !PeerToCommunicator.ContainsKey(peer))
                throw new InvalidOperationException();
            return PeerToCommunicator[peer].Communicator;
        }
        internal static void AddTWNetworkPeer(TWNetworkPeer peer)
        {
            if (!GameNetwork.IsServer)
                throw new InvalidOperationException();
            PeerToCommunicator.TryAdd(peer, null);
        }
        internal static void AddNativeMBPeerToLastPeer(NativeMBPeer communicator)
        {
            if (!GameNetwork.IsServer)
                throw new InvalidOperationException();
            TWNetworkPeer peer = PeerToCommunicator.Keys.Last();
            PeerToCommunicator[peer] = communicator;
            CommunicatorToPeer.TryAdd(PeerToCommunicator[peer], peer);
        }
    }
}
