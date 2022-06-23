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

    public abstract class MissionServerBase: MissionNetworkEntity
    {
        private Dictionary<NetworkCommunicator,Guid> NetworkPeers;
        private NetworkCommunicator Peer;
        private ServerState CurrentState = ServerState.None;
        protected MissionServerBase()
        {
            NetworkPeers = new List<INetworkPeer>();
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
            if (CurrentState != ServerState.ReliableModuleEvent || MessageToSend is null)
                throw new InvalidOperationException();
            var memstream = new MemoryStream();
            Serializer.Serialize(memstream, MessageToSend);
            memstream.TryGetBuffer(out var buffer);
            Peer.SendRaw(buffer, DeliveryMethodType.Reliable);
            MessageToSend = null;
        }

        public void EndModuleEventAsServerUnreliable()
        {
            if (CurrentState != ServerState.UnreliableModuleEvent || MessageToSend is null)
                throw new InvalidOperationException();
            var memstream = new MemoryStream();
            Serializer.Serialize(memstream, MessageToSend);
            memstream.TryGetBuffer(out var buffer);
            Peer.SendRaw(buffer, DeliveryMethodType.Reliable);
            MessageToSend = null;
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

        public abstract bool HandleNetworkPacketAsServer(NetworkCommunicator networkPeer);

        public abstract void AddNewPlayerOnServer(PlayerConnectionInfo playerConnectionInfo, bool serverPeer, bool isAdmin);

        public abstract GameNetwork.AddPlayersResult AddNewPlayersOnServer(PlayerConnectionInfo[] playerConnectionInfos, bool serverPeer);

        public abstract void AddPeerToDisconnect(NetworkCommunicator networkPeer);
    }
}