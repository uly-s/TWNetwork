using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork
{
    public abstract class GameNetworkServerBase: IGameNetworkEntity
    {
        private static List<INetworkPeer> NetworkPeers;
        private GameNetworkMessage MessageToSend { get; set; }
        public void BeginModuleEventAsServer(NetworkCommunicator networkPeer) { }
        public void BeginModuleEventAsServer(VirtualPlayer player) { }
        public void BeginModuleEventAsServerUnreliable(NetworkCommunicator networkPeer) { }
        public void BeginModuleEventAsServerUnreliable(VirtualPlayer player) { }
        public void BeginBroadcastModuleEvent() { }
        public void EndModuleEventAsServer() { }
        public void EndModuleEventAsServerUnreliable() { }
        public void EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer = null) { }
        public void EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer = null) { }
        /// <summary>
        /// This method should be called, when the Server is constructed/initialized.
        /// </summary>
        public void InitializeServerSide() 
        {
            NetworkPeers = new List<INetworkPeer>();
        }
        /// <summary>
        /// This method should be called, when a new Peer is added to the Server.
        /// </summary>
        /// <param name="peer"></param>
        public void OnAddNewPeer(INetworkPeer peer)
        {
            lock (NetworkPeers)
            {
                NetworkPeers.Add(peer);
            }
        }

        /// <summary>
        /// This method should be called, when a client sent a GameNetworkMessage.
        /// </summary>
        /// <param name="buffer">The buffer that has been sent to us only containing the GameNetworkMessage object.</param>
        public void OnReceive(INetworkPeer peer,ArraySegment<byte> buffer)
        {
            object obj = Serializer.Deserialize<object>(new MemoryStream(buffer.Array));
            if (obj is JoinMissionMessage)
            {
                //Adding references to the networkpeer
                var con = new PlayerConnectionInfo();
                con.NetworkPeer = new NetworkCommunicator();
                GameNetwork.AddNewPlayerOnServer(, false, false);
                //Handle join (Stop mission, etc.)
            }
            else if (obj is DisconnectMissionMessage)
            {
                //Removing references from the networkpeer.
                //HandleDisconnect
            }
            else if (obj is GameNetworkMessage)
            {
                MessageToSend = (GameNetworkMessage)obj;
                HandleNetworkPacketAsServer(GameNetworkExtensions.GetTWNetworkPeer(peer));
            }
            else
            {
                throw new NotValidMessageException();
            }
        }
        public void AddNewPlayerOnServer(PlayerConnectionInfo playerConnectionInfo, bool serverPeer, bool isAdmin) { }
        public GameNetwork.AddPlayersResult AddNewPlayersOnServer(PlayerConnectionInfo[] playerConnectionInfos, bool serverPeer) { return new GameNetwork.AddPlayersResult(); }
        public void AddPeerToDisconnect(NetworkCommunicator networkPeer) { }
        /// <summary>
        /// This method should be called, when the Server is deleted/terminated.
        /// </summary>
        public void TerminateServerSide() 
        {
            NetworkPeers.Clear();
            NetworkPeers = null;
        }
        public abstract void HandleNetworkPacketAsServer(NetworkCommunicator networkPeer);
    }
}
