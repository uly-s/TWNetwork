using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TWNetworkTestMod.Messages.FromServer;

namespace TWNetworkTestMod
{
    public class TWNetworkComponent : UdpNetworkComponent
    {
        public TWNetworkComponent() : base() 
        {
        }
        protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
        {
            if (!TWNetworkGameManager.CurrentManager.IsServer)
            {
                registerer.Register(new GameNetworkMessage.ServerMessageHandlerDelegate<LoadCustomBattle>(HandleServerEventLoadCustomBattle));
                registerer.Register(new GameNetworkMessage.ServerMessageHandlerDelegate<AddPeerComponent>(HandleServerEventAddPeerComponent));
                registerer.Register(new GameNetworkMessage.ServerMessageHandlerDelegate<RemovePeerComponent>(HandleServerEventRemovePeerComponent));
                registerer.Register(new GameNetworkMessage.ServerMessageHandlerDelegate<SynchronizingDone>(HandleServerEventSynchronizingDone));
            }
            else
            {
                registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<FinishedLoading>(HandleClientEventFinishedLoading));
            }
        }

        private void HandleServerEventAddPeerComponent(AddPeerComponent message)
        {
            NetworkCommunicator peer = message.Peer;
            uint componentId = message.ComponentId;
            if (peer.GetComponent(componentId) == null)
            {
                peer.AddComponent(componentId);
            }
        }

        private void HandleServerEventRemovePeerComponent(RemovePeerComponent message)
        {
            NetworkCommunicator peer = message.Peer;
            uint componentId = message.ComponentId;
            PeerComponent component = peer.GetComponent(componentId);
            peer.RemoveComponent(component);
        }

        private void HandleServerEventSynchronizingDone(SynchronizingDone message)
        {
            NetworkCommunicator peer = message.Peer;
            peer.IsSynchronized = message.Synchronized;
            Mission mission = Mission.Current;
            MissionNetworkComponent missionNetworkComponent = (mission != null) ? mission.GetMissionBehavior<MissionNetworkComponent>() : null;
            if (missionNetworkComponent != null && message.Synchronized && peer.GetComponent<MissionPeer>() != null)
            {
                missionNetworkComponent.OnClientSynchronized(peer);
            }
        }

        private bool HandleClientEventFinishedLoading(NetworkCommunicator peer, FinishedLoading message)
        {
            this.HandleClientEventFinishedLoadingAux(peer, message);
            return true;
        }

        private async void HandleClientEventFinishedLoadingAux(NetworkCommunicator networkPeer, FinishedLoading message)
        {
            while (Mission.Current != null && Mission.Current.CurrentState != Mission.State.Continuing)
            {
                await Task.Delay(1);
            }
            if (!networkPeer.IsServerPeer)
            {
                MBDebug.Print("Server: " + networkPeer.UserName + " has finished loading. From now on, I will include him in the broadcasted messages", 0, Debug.DebugColor.White, 17179869184UL);
                GameNetwork.ClientFinishedLoading(networkPeer);
            }
        }

        private void HandleServerEventLoadCustomBattle(LoadCustomBattle message)
        {
            BannerlordMissions.OpenCustomBattleMission(message.SceneID,null,null,null,false,null);
        }
    }
}
