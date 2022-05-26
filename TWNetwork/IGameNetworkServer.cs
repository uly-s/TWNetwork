using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TWNetwork
{
    public interface IGameNetworkServer: IGameNetworkEntity
    {
        void BeginModuleEventAsServer(NetworkCommunicator networkPeer);
        void BeginModuleEventAsServer(VirtualPlayer player);
        void BeginModuleEventAsServerUnreliable(NetworkCommunicator networkPeer);
        void BeginModuleEventAsServerUnreliable(VirtualPlayer player);
        void BeginBroadcastModuleEvent();
        void EndModuleEventAsServer();
        void EndModuleEventAsServerUnreliable();
        void EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer = null);
        void EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer = null);
    }
}
