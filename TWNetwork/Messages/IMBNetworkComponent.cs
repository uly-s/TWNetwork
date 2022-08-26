using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TWNetwork.InterfacePatches;
using TWNetwork.Messages.FromServer;

namespace TWNetwork.Messages
{
    public class IMBNetworkComponent : UdpNetworkComponent
    {
        public IMBNetworkComponent() : base()
        {
        }
        protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
        {
            if (TWNetworkPatches.NetworkIdentifier == NetworkIdentifier.Server)
            {
            }
            else
            {
                registerer.Register(new GameNetworkMessage.ServerMessageHandlerDelegate<ChangeClientPeerIndex>(HandleServerEventChangeClientPeerIndex));
            }
        }

        private void HandleServerEventChangeClientPeerIndex(ChangeClientPeerIndex message)
        {
            GameNetwork.ClientPeerIndex = message.PeerIndex;
        }
    }
}
