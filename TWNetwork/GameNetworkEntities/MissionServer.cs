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
using TaleWorlds.MountAndBlade.Diamond;
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
        private readonly int Capacity;
        private ServerState CurrentState = ServerState.None;
        public readonly MissionServerType ServerType;
        private static PropertyInfo MyPeer = typeof(GameNetwork).GetProperty("MyPeer");
        private static MethodInfo GetSessionKeyForPlayer = typeof(GameNetwork).GetMethod("GetSessionKeyForPlayer",BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
        private static MethodInfo AddNetworkPeer = typeof(GameNetwork).GetMethod("AddNetworkPeer", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(NetworkCommunicator) }, null);
        private static MethodInfo PrepareNewUdpSession = typeof(GameNetwork).GetMethod("PrepareNewUdpSession", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(int),typeof(int)}, null);
        private static PropertyInfo SessionKey = typeof(NetworkCommunicator).GetProperty("SessionKey");
        private static MethodInfo CreateAsServer = typeof(NetworkCommunicator).GetMethod("CreateAsServer", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic,null,new Type[] { typeof(PlayerConnectionInfo), typeof(int),typeof(bool)},null);
        private static MethodInfo SetServerPeer = typeof(NetworkCommunicator).GetMethod("SetServerPeer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { typeof(bool) }, null);
        private static IGameNetworkHandler Handler => (IGameNetworkHandler)typeof(GameNetwork).GetField("_handler", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        public Missions Missions { get; private set; }
        public MissionServer(MissionServerType serverType,int capacity)
        {
            NetworkPeers = new List<NetworkCommunicator>();
            ServerType = serverType;
            Capacity = capacity;
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

        public ICommunicator AddNewPlayerOnServer(PlayerConnectionInfo playerConnectionInfo, bool serverPeer, bool isAdmin) 
        {
            int num = NetworkPeers.Count + 1;
            Debug.Print(string.Concat(new object[]
            {
                ">>> AddNewPlayerOnServer: ",
                playerConnectionInfo.Name,
                " index: ",
                num
            }), 0, Debug.DebugColor.White, 17179869184UL);
            if (num >= 0)
            {
                int sessionKey = 0;
                if (!serverPeer)
                {
                    sessionKey = (int)GetSessionKeyForPlayer.Invoke(null,new object[] { });
                }
                int num2 = -1;
                ICommunicator communicator = null;
                for (int i = 0; i < MBNetwork.DisconnectedNetworkPeers.Count; i++)
                {
                    PlayerData parameter = playerConnectionInfo.GetParameter<PlayerData>("PlayerData");
                    if (parameter != null && MBNetwork.DisconnectedNetworkPeers[i].VirtualPlayer.Id == parameter.PlayerId)
                    {
                        num2 = i;
                        communicator = MBNetwork.DisconnectedNetworkPeers[i];
                        NetworkCommunicator networkCommunicator = communicator as NetworkCommunicator;
                        networkCommunicator.UpdateIndexForReconnectingPlayer(num);
                        networkCommunicator.UpdateConnectionInfoForReconnect(playerConnectionInfo, isAdmin);

                        IMBPeerPatches.m_SetUserData.Invoke(IMBPeerPatches.IMBPeer, new object[] { num, IMBPeerPatches.MBNetworkPeer_Ctr.Invoke(new object[] { networkCommunicator }) });
                        Debug.Print("> RemoveFromDisconnectedPeers: " + networkCommunicator.UserName, 0, Debug.DebugColor.White, 17179869184UL);
                        MBNetwork.DisconnectedNetworkPeers.RemoveAt(i);
                        break;
                    }
                }
                if (communicator == null)
                {
                    communicator = (NetworkCommunicator)CreateAsServer.Invoke(null,new object[] { playerConnectionInfo, num, isAdmin });
                }
                MBNetwork.VirtualPlayers[communicator.VirtualPlayer.Index] = communicator.VirtualPlayer;

                NetworkCommunicator networkCommunicator2 = communicator as NetworkCommunicator;
                if (serverPeer && GameNetwork.IsServer)
                {
                    GameNetwork.ClientPeerIndex = num;
                    MyPeer.SetValue(null, networkCommunicator2);
                }
                SessionKey.SetValue(networkCommunicator2, sessionKey);
                SetServerPeer.Invoke(networkCommunicator2, new object[] { serverPeer });
                AddNetworkPeer.Invoke(null,new object[] { networkCommunicator2 });
                playerConnectionInfo.NetworkPeer = networkCommunicator2;
                if (!serverPeer)
                {
                    PrepareNewUdpSession.Invoke(null,new object[] { num, sessionKey });
                }
                if (num2 < 0)
                {
                    GameNetwork.BeginBroadcastModuleEvent();
                    GameNetwork.WriteMessage(new CreatePlayer(networkCommunicator2.Index, playerConnectionInfo.Name, num2, false, false));
                    GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord | GameNetwork.EventBroadcastFlags.DontSendToPeers, null);
                }
                foreach (NetworkCommunicator networkCommunicator3 in GameNetwork.NetworkPeers)
                {
                    if (networkCommunicator3 != networkCommunicator2 && networkCommunicator3 != GameNetwork.MyPeer)
                    {
                        GameNetwork.BeginModuleEventAsServer(networkCommunicator3);
                        GameNetwork.WriteMessage(new CreatePlayer(networkCommunicator2.Index, playerConnectionInfo.Name, num2, false, false));
                        GameNetwork.EndModuleEventAsServer();
                    }
                    if (!serverPeer)
                    {
                        bool isReceiverPeer = networkCommunicator3 == networkCommunicator2;
                        GameNetwork.BeginModuleEventAsServer(networkCommunicator2);
                        GameNetwork.WriteMessage(new CreatePlayer(networkCommunicator3.Index, networkCommunicator3.UserName, -1, false, isReceiverPeer));
                        GameNetwork.EndModuleEventAsServer();
                    }
                }
                for (int j = 0; j < MBNetwork.DisconnectedNetworkPeers.Count; j++)
                {
                    NetworkCommunicator networkCommunicator4 = MBNetwork.DisconnectedNetworkPeers[j] as NetworkCommunicator;
                    GameNetwork.BeginModuleEventAsServer(networkCommunicator2);
                    GameNetwork.WriteMessage(new CreatePlayer(networkCommunicator4.Index, networkCommunicator4.UserName, j, true, false));
                    GameNetwork.EndModuleEventAsServer();
                }
                foreach (IUdpNetworkHandler udpNetworkHandler in GameNetwork.NetworkHandlers)
                {
                    udpNetworkHandler.HandleNewClientConnect(playerConnectionInfo);
                }
                Handler.OnPlayerConnectedToServer(networkCommunicator2);
                NetworkPeers.Add((NetworkCommunicator)communicator);
                return communicator;
            }
            return null;
        }

        public GameNetwork.AddPlayersResult AddNewPlayersOnServer(PlayerConnectionInfo[] playerConnectionInfos, bool serverPeer) 
        {
            bool flag = Capacity > playerConnectionInfos.Length;
            NetworkCommunicator[] array = new NetworkCommunicator[playerConnectionInfos.Length];
            if (flag)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    ICommunicator communicator = GameNetwork.AddNewPlayerOnServer(playerConnectionInfos[i], serverPeer, false);
                    array[i] = (communicator as NetworkCommunicator);
                }
            }
            return new GameNetwork.AddPlayersResult
            {
                NetworkPeers = array,
                Success = flag
            };
        }

        public void AddPeerToDisconnect(NetworkCommunicator networkPeer) 
        {
            //TODO: Investigate what this method is used for and implement
        }
    }
}