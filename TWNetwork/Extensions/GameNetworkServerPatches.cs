using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TWNetwork.Extensions;
using TWNetworkPatcher;

namespace TWNetwork
{
    public enum MissionServerType
    {
        MultipleMissionServer,SingleMissionServer
    }
    public abstract class GameNetworkServerPatches : HarmonyPatches
    {
        public static bool IsInitialized { get; private set; } = false;
        private static MissionServerType _missionServerType = MissionServerType.SingleMissionServer;
        private static MissionServer _missionServer = null;
        public static MissionNetworkEntity Entity => _missionServer;
        public static MissionServerType MissionServerType 
        {
            get
            {
                return _missionServerType;
            } 
            set
            {
                if (IsInitialized)
                {
                    throw new MissionEntityAlreadyInitializedException();
                }
                _missionServerType = value;
            }
        }

        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.BeginModuleEventAsServer), new Type[] { typeof(VirtualPlayer) }, true)]
        private void BeginModuleEventAsServer(VirtualPlayer player) { _missionServer.BeginModuleEventAsServer(player); }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.BeginModuleEventAsServerUnreliable), new Type[] { typeof(VirtualPlayer) }, true)]
        private void BeginModuleEventAsServerUnreliable(VirtualPlayer player) { _missionServer.BeginModuleEventAsServerUnreliable(player); }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.BeginBroadcastModuleEvent), true)]
        private void BeginBroadcastModuleEvent() { _missionServer.BeginBroadcastModuleEvent(); }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.EndModuleEventAsServer), true)]
        private void EndModuleEventAsServer() { _missionServer.EndModuleEventAsServer(); }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.EndModuleEventAsServerUnreliable), true)]
        private void EndModuleEventAsServerUnreliable() { _missionServer.EndModuleEventAsServerUnreliable(); }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.EndBroadcastModuleEvent), new Type[] { typeof(GameNetwork.EventBroadcastFlags),typeof(NetworkCommunicator) }, true)]
        private void EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer = null) { _missionServer.EndBroadcastModuleEvent(broadcastFlags,targetPlayer); }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.EndBroadcastModuleEventUnreliable), new Type[] { typeof(GameNetwork.EventBroadcastFlags), typeof(NetworkCommunicator) }, true)]
        private void EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer = null) { _missionServer.EndBroadcastModuleEventUnreliable(broadcastFlags, targetPlayer); }
        /// <summary>
        /// This method should be called, when the Server is constructed/initialized.
        /// </summary>
        [PatchedMethod(typeof(GameNetwork),"InitializeServerSide",true)]
        private void InitializeServerSide() 
        {
            if (IsInitialized || GameNetworkClientPatches.IsInitialized)
                throw new InvalidOperationException();
            _missionServer = new MissionServer(_missionServerType);
        }
        /// <summary>
        /// This method should be called, when a new Peer is added to the Server.
        /// </summary>
        /// <param name="peer"></param>
        public static void OnAddNewPeer(INetworkPeer peer)
        {
            if (!IsInitialized)
                return;
            _missionServer.AddPeer(peer);
        }

        /// <summary>
        /// This method should be called, when a client sent a GameNetworkMessage.
        /// </summary>
        /// <param name="buffer">The buffer that has been sent to us only containing the GameNetworkMessage object.</param>
        public static void OnReceive(INetworkPeer peer,ArraySegment<byte> buffer)
        {
            NetworkCommunicator communicator = peer.GetNetworkCommunicator();
            object obj = Serializer.Deserialize<object>(new MemoryStream(buffer.Array));
            if (obj is JoinMissionMessage)
            {
                //Adding references to the networkpeer
                //var con = new PlayerConnectionInfo();
                //con.NetworkPeer = new NetworkCommunicator();
                //GameNetwork.AddNewPlayerOnServer(, false, false);
                //Handle join (Stop mission, etc.)
            }
            else if (obj is DisconnectMissionMessage)
            {
                //Removing references from the networkpeer.
                //HandleDisconnect
            }
            else if (obj is GameNetworkMessage)
            {
                //MessageToSend = (GameNetworkMessage)obj;
                //HandleNetworkPacketAsServer(GameNetworkExtensions.GetTWNetworkPeer(peer));
            }
            else
            {
                throw new NotValidMessageException();
            }
        }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.AddNewPlayerOnServer), new Type[] { typeof(PlayerConnectionInfo),typeof(bool),typeof(bool) }, true)]
        private void AddNewPlayerOnServer(PlayerConnectionInfo playerConnectionInfo, bool serverPeer, bool isAdmin) { _missionServer.AddNewPlayerOnServer(playerConnectionInfo, serverPeer, isAdmin); }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.AddNewPlayersOnServer), new Type[] { typeof(PlayerConnectionInfo[]), typeof(bool) }, true)]
        private GameNetwork.AddPlayersResult AddNewPlayersOnServer(PlayerConnectionInfo[] playerConnectionInfos, bool serverPeer) { return _missionServer.AddNewPlayersOnServer(playerConnectionInfos,serverPeer); }
        [PatchedMethod(typeof(GameNetwork), "AddPeerToDisconnect", new Type[] { typeof(NetworkCommunicator) }, true)]
        private void AddPeerToDisconnect(NetworkCommunicator networkPeer) { _missionServer.AddPeerToDisconnect(networkPeer);  }
        /// <summary>
        /// This method should be called, when the Server is deleted/terminated.
        /// </summary>
        [PatchedMethod(typeof(GameNetwork), "TerminateServerSide)", new Type[] { typeof(NetworkCommunicator) }, true)]
        private void TerminateServerSide() 
        {
            _missionServer = null;
            IsInitialized = false;
        }
        [PatchedMethod(typeof(GameNetwork),"HandleNetworkPacketAsServer",true)]
        private void HandleNetworkPacketAsServer(NetworkCommunicator networkPeer)
        {
            _missionServer.HandleNetworkPacketAsServer(networkPeer);
        }
    }
}
