using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace TWNetwork.Extensions
{
    public static class NetworkCommunicatorExtensions
    {
        private static ConcurrentDictionary<NetworkCommunicator, INetworkPeer> Peers = new ConcurrentDictionary<NetworkCommunicator, INetworkPeer>();
        private static ConcurrentDictionary<NetworkCommunicator, Guid> Missions = new ConcurrentDictionary<NetworkCommunicator, Guid>();
        public static void Send(this NetworkCommunicator communicator, byte[] buffer,DeliveryMethodType methodType)
        {
            Peers[communicator].SendRaw(buffer,methodType);
        }

        public static Mission GetMission(this NetworkCommunicator communicator)
        {
            if (!GameNetwork.IsServer)
                throw new InvalidOperationException();
            return ((MissionServer)GameNetworkServerPatches.Entity).Missions.GetMission(Missions[communicator]);
        }
        public static NetworkCommunicator GetNetworkCommunicator(this INetworkPeer peer)
        {
            if (!GameNetwork.IsServer)
                throw new InvalidOperationException();
            foreach (var key in Peers.Keys)
            {
                if (Peers[key] == peer)
                {
                    return key;
                }
            }
            return null;
        }
        public static void OnJoinMission(NetworkCommunicator communicator, Guid missionID)
        {
            if (!GameNetwork.IsServer)
                throw new InvalidOperationException();
            //TODO: Implement
        }

        public static void OnNewClientConnects(NetworkCommunicator communicator, INetworkPeer peer)
        {
            if(!GameNetwork.IsServer || !Peers.TryAdd(communicator, peer))
                throw new InvalidOperationException();
        }
    }
}
