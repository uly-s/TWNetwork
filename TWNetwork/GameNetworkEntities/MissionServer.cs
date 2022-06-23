using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork
{
    internal enum ServerState
    {
        None, ReliableModuleEvent, UnreliableModuleEvent,BroadcastModuleEvent
    }

    public class MissionServer: MissionNetworkEntity
    {
        private List<NetworkCommunicator> NetworkPeers;
        private NetworkCommunicator Peer;
        private ServerState CurrentState = ServerState.None;
        public readonly MissionServerType ServerType;
        public Missions Missions { get; private set; }
        public MissionServer(MissionServerType serverType)
        {
            NetworkPeers = new List<NetworkCommunicator>();
            ServerType = serverType;
        }

        public void AddPeer(INetworkPeer peer)
        {
            lock (NetworkPeers)
            {
                NetworkPeers.Add(peer);
            }
        }

        public void BeginModuleEventAsServer(VirtualPlayer player)
        {
            Peer = PeerObserver.GetNetworkPeer(((NetworkCommunicator)player.Communicator));
            CurrentState = ServerState.ReliableModuleEvent;
        }

        public void BeginModuleEventAsServerUnreliable(VirtualPlayer player)
        {
            Peer = PeerObserver.GetNetworkPeer(((NetworkCommunicator)player.Communicator));
            CurrentState = ServerState.UnreliableModuleEvent;
        }

        public void BeginBroadcastModuleEvent()
        {
            CurrentState = ServerState.BroadcastModuleEvent;
        }

        public void EndModuleEventAsServer()
        {
            if (CurrentState != ServerState.ReliableModuleEvent || MessagesToSend is null)
                throw new InvalidOperationException();
            var memstream = new MemoryStream();
            Serializer.Serialize(memstream, MessagesToSend);
            memstream.TryGetBuffer(out var buffer);
            Peer.SendRaw(buffer, DeliveryMethodType.Reliable);
            MessagesToSend = null;
        }

        public void EndModuleEventAsServerUnreliable()
        {
            if (CurrentState != ServerState.UnreliableModuleEvent || MessagesToSend is null)
                throw new InvalidOperationException();
            var memstream = new MemoryStream();
            Serializer.Serialize(memstream, MessagesToSend);
            memstream.TryGetBuffer(out var buffer);
            Peer.SendRaw(buffer, DeliveryMethodType.Reliable);
            MessagesToSend = null;
        }

        private IEnumerable<INetworkPeer> GetNetworkPeersByFlags(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer)
        {
            List<INetworkPeer> peers = new List<INetworkPeer>(NetworkPeers);
            foreach (var BroadcastFlag in Enum.GetValues(typeof(GameNetwork.EventBroadcastFlags)))
            {
                var flag = broadcastFlags & (GameNetwork.EventBroadcastFlags)BroadcastFlag;
                switch (flag)
                {
                    case GameNetwork.EventBroadcastFlags.DontSendToPeers:
                        peers.Clear();
                        return peers;
                    case GameNetwork.EventBroadcastFlags.ExcludePeerTeamPlayers:

                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer:
                        peers.Remove(PeerObserver.GetNetworkPeer(targetPlayer));
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers:
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeNoBloodStainsOption:
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeNoParticlesOption:
                        break;
                    case GameNetwork.EventBroadcastFlags.AddToMissionRecord:
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeNoSoundOption:
                        break;
                    case GameNetwork.EventBroadcastFlags.IncludeUnsynchronizedClients:
                        break;
                }
            }
            return peers;
        }


        public void EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer)
        {
            
        }
        public void EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer)
        {
            //TODO: Handle EventBroadcastFlags
        }

        public bool HandleNetworkPacketAsServer(NetworkCommunicator networkPeer) { }

        public void AddNewPlayerOnServer(PlayerConnectionInfo playerConnectionInfo, bool serverPeer, bool isAdmin) { }

        public GameNetwork.AddPlayersResult AddNewPlayersOnServer(PlayerConnectionInfo[] playerConnectionInfos, bool serverPeer) { }

        public void AddPeerToDisconnect(NetworkCommunicator networkPeer) { }
    }
}