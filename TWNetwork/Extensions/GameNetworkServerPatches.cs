using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        private static MethodInfo HandlePacketAsServer = typeof(GameNetwork).GetMethod("HandleNetworkPacketAsServer", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,null,new Type[] { typeof(NetworkCommunicator)},null);
        public static MissionNetworkEntity Entity => _missionServer;
        public static int Capacity { get; set; } = 100;
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
            _missionServer = new MissionServer(_missionServerType,Capacity);
        }
        /// <summary>
        /// This method should be called, when a new Peer is added to the Server.
        /// </summary>
        /// <param name="peer"></param>
        public static void OnAddNewPeer(INetworkPeer peer)
        {
            if (!IsInitialized)
                return;
            //Create NetworkCommunicator on join.
            _missionServer.AddPeer(peer.GetNetworkCommunicator());
        }

        /// <summary>
        /// This method should be called, when a client sent a GameNetworkMessages class object.
        /// </summary>
        /// <param name="buffer">The buffer that has been sent to us only containing the GameNetworkMessage object.</param>
        public static void OnReceive(INetworkPeer peer,ArraySegment<byte> buffer)
        {
            NetworkCommunicator communicator = peer.GetNetworkCommunicator();
            object obj = Serializer.Deserialize<object>(new MemoryStream(buffer.Array));
            if (obj is GameNetworkMessages)
            {
                var messages = (GameNetworkMessages)obj;
                while (messages.PopMessage(out GameNetworkMessage message))
                {
                    _missionServer.MessageToProcess = message;
                    HandlePacketAsServer.Invoke(null,new object[] { communicator });
                }
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
            _missionServerType = MissionServerType.SingleMissionServer;
            IsInitialized = false;
        }
        [PatchedMethod(typeof(GameNetwork),"HandleNetworkPacketAsServer",true)]
        private void HandleNetworkPacketAsServer(NetworkCommunicator networkPeer)
        {
            _missionServer.HandleNetworkPacketAsServer(networkPeer);
        }
    }
}
