using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TWNetwork
{
    public class MultipleMissionServer : MissionServerBase
    {
        public Missions Missions { get; private set; }

        public override void AddNewPlayerOnServer(PlayerConnectionInfo playerConnectionInfo, bool serverPeer, bool isAdmin)
        {
            throw new NotImplementedException();
        }

        public override GameNetwork.AddPlayersResult AddNewPlayersOnServer(PlayerConnectionInfo[] playerConnectionInfos, bool serverPeer)
        {
            throw new NotImplementedException();
        }

        public override void AddPeerToDisconnect(NetworkCommunicator networkPeer)
        {
            throw new NotImplementedException();
        }

        public override bool HandleNetworkPacketAsServer(NetworkCommunicator networkPeer)
        {
            throw new NotImplementedException();
        }
    }
}
