using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TWNetwork.Extensions;

namespace TWNetwork
{
    internal enum ServerState
    {
        None, ReliableModuleEvent, UnreliableModuleEvent,BroadcastModuleEvent
    }

    public class MissionServer: MissionNetworkEntity
    {
        private static List<Type> _gameNetworkMessageIdsFromClient = ((List<Type>)typeof(GameNetwork).GetField("_gameNetworkMessageIdsFromClient", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null));
        private static Dictionary<int, List<object>> _fromClientMessageHandlers = ((Dictionary<int, List<object>>)typeof(GameNetwork).GetField("_fromClientMessageHandlers", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null));
        private List<NetworkCommunicator> NetworkPeers;
        private NetworkCommunicator Peer;
        private ServerState CurrentState = ServerState.None;
        public readonly MissionServerType ServerType;
        private static FieldInfo TeamAgents = typeof(Agent).GetField("_teamAgents", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        public Missions Missions { get; private set; }
        public MissionServer(MissionServerType serverType)
        {
            NetworkPeers = new List<NetworkCommunicator>();
            ServerType = serverType;
        }

        public void AddPeer(NetworkCommunicator peer)
        {
            lock (NetworkPeers)
            {
                NetworkPeers.Add(peer);
            }
        }

        public void BeginModuleEventAsServer(VirtualPlayer player)
        {
            Peer = (NetworkCommunicator)player.Communicator;
            CurrentState = ServerState.ReliableModuleEvent;
        }

        public void BeginModuleEventAsServerUnreliable(VirtualPlayer player)
        {
            Peer = (NetworkCommunicator)player.Communicator;
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
            Peer.Send(MessagesToSend, DeliveryMethodType.Reliable);
            MessagesToSend = null;
        }

        public void EndModuleEventAsServerUnreliable()
        {
            if (CurrentState != ServerState.UnreliableModuleEvent || MessagesToSend is null)
                throw new InvalidOperationException();
            Peer.Send(MessagesToSend, DeliveryMethodType.Unreliable);
            MessagesToSend = null;
        }

        private IEnumerable<NetworkCommunicator> GetNetworkPeersByFlags(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer)
        {
            List<NetworkCommunicator> peers = new List<NetworkCommunicator>(NetworkPeers);
            bool ShouldRemoveUnsynchronizedClients = true;
            foreach (var BroadcastFlag in Enum.GetValues(typeof(GameNetwork.EventBroadcastFlags)))
            {
                var flag = broadcastFlags & (GameNetwork.EventBroadcastFlags)BroadcastFlag;
                switch (flag)
                {

                    case GameNetwork.EventBroadcastFlags.DontSendToPeers:
                        peers.Clear();
                        return peers;
                    case GameNetwork.EventBroadcastFlags.ExcludePeerTeamPlayers:
                        if (targetPlayer is null || targetPlayer.ControlledAgent is null || targetPlayer.ControlledAgent.Team is null)
                            break;
                        foreach (var player in GameNetwork.NetworkPeers.Where(peer => peer != targetPlayer && peer.ControlledAgent != null && peer.ControlledAgent.Team != null && peer.ControlledAgent.Team.MBTeam == targetPlayer.ControlledAgent.Team.MBTeam))
                        {
                            peers.Remove(player);
                        }
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer:
                        if (targetPlayer is null)
                            break;
                        peers.Remove(targetPlayer);
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers:
                        if (targetPlayer is null || targetPlayer.ControlledAgent is null || targetPlayer.ControlledAgent.Team is null)
                            break;
                        foreach (var player in GameNetwork.NetworkPeers.Where(peer => peer.ControlledAgent != null && peer.ControlledAgent.Team != null && peer.ControlledAgent.Team.MBTeam != targetPlayer.ControlledAgent.Team.MBTeam))
                        {
                            peers.Remove(player);
                        }
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeNoBloodStainsOption:
                        //IDK yet, needs investigation
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeNoParticlesOption:
                        //IDK yet, needs investigation
                        break;
                    case GameNetwork.EventBroadcastFlags.AddToMissionRecord:
                        //IDK yet, needs investigation
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeNoSoundOption:
                        //IDK yet, needs investigation
                        break;
                    case GameNetwork.EventBroadcastFlags.IncludeUnsynchronizedClients:
                        ShouldRemoveUnsynchronizedClients = false;
                        break;
                }
            }
            if (ShouldRemoveUnsynchronizedClients)
            {
                foreach (var p in peers.Where(pe => !pe.IsSynchronized))
                {
                    peers.Remove(p);
                }
            }
            return peers;
        }


        public void EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer)
        {
            if (CurrentState != ServerState.BroadcastModuleEvent || MessagesToSend is null)
                throw new InvalidOperationException();
            var PeersToSendMessageTo = GetNetworkPeersByFlags(broadcastFlags,targetPlayer);
            foreach (var peer in PeersToSendMessageTo)
            {
                peer.Send(MessagesToSend, DeliveryMethodType.Reliable);
            }
            MessagesToSend = null;
        }
        public void EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer)
        {
            if (CurrentState != ServerState.BroadcastModuleEvent || MessagesToSend is null)
                throw new InvalidOperationException();
            var PeersToSendMessageTo = GetNetworkPeersByFlags(broadcastFlags, targetPlayer);
            foreach (var peer in PeersToSendMessageTo)
            {
                peer.Send(MessagesToSend, DeliveryMethodType.Unreliable);
            }
            MessagesToSend = null;
        }

        public bool HandleNetworkPacketAsServer(NetworkCommunicator networkPeer) 
        {
            bool Result;
            if (networkPeer == null)
            {
                MBDebug.Print("networkPeer == null", 0, Debug.DebugColor.White, 17592186044416UL);
                Result = false;
            }
            else
            {
                var msg = MessageToProcess;
                bool flag = true;
                if (msg.MessageId >= 0 && msg.MessageId < _gameNetworkMessageIdsFromClient.Count)
                {
                    List<object> list;
                    if (_fromClientMessageHandlers.TryGetValue(msg.MessageId, out list))
                    {
                        foreach (object obj in list)
                        {
                            Delegate method = obj as Delegate;
                            flag = flag && ((bool)method.DynamicInvokeWithLog(new object[]
                            {
                            networkPeer,
                            msg
                            }));
                            if (!flag)
                            {
                                break;
                            }
                        }
                        if (list.Count == 0)
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                }
                Result = flag;
            }
            return Result;
        }

        public void AddNewPlayerOnServer(PlayerConnectionInfo playerConnectionInfo, bool serverPeer, bool isAdmin) 
        {
            //TODO: Investigate PlayerConnectionInfo class and implement
        }

        public GameNetwork.AddPlayersResult AddNewPlayersOnServer(PlayerConnectionInfo[] playerConnectionInfos, bool serverPeer) 
        {
            //TODO: Investigate PlayerConnectionInfo class and implement
        }

        public void AddPeerToDisconnect(NetworkCommunicator networkPeer) 
        {
            //TODO: Investigate what this method is used for and implement
        }
    }
}