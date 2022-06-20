using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork
{
    public abstract class GameNetworkServerBase: IGameNetworkEntity
    {
        public GameNetworkMessage MessageToSend { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void BeginModuleEventAsServer(NetworkCommunicator networkPeer) { }
        public void BeginModuleEventAsServer(VirtualPlayer player) { }
        public void BeginModuleEventAsServerUnreliable(NetworkCommunicator networkPeer) { }
        public void BeginModuleEventAsServerUnreliable(VirtualPlayer player) { }
        public void BeginBroadcastModuleEvent() { }
        public void EndModuleEventAsServer() { }
        public void EndModuleEventAsServerUnreliable() { }
        public void EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer = null) { }
        public void EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer = null) { }
        public void InitializeServerSide() { }
        public void AddNewPlayerOnServer(PlayerConnectionInfo playerConnectionInfo, bool serverPeer, bool isAdmin) { }
        public GameNetwork.AddPlayersResult AddNewPlayersOnServer(PlayerConnectionInfo[] playerConnectionInfos, bool serverPeer) { return new GameNetwork.AddPlayersResult(); }
        public void AddPeerToDisconnect(NetworkCommunicator networkPeer) { }
        public void TerminateServerSide() { }
        public void HandleNetworkPacketAsServer(NetworkCommunicator networkPeer) { }
    }
}
