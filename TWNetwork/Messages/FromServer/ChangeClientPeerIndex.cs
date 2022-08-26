using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork.Messages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class ChangeClientPeerIndex : GameNetworkMessage
    {
        public int PeerIndex { get; private set; }
        public ChangeClientPeerIndex() { }
        public ChangeClientPeerIndex(int index) 
        {
            PeerIndex = index;
        }
        protected override void OnWrite()
        {
            WriteIntToPacket(PeerIndex, new CompressionInfo.Integer(0,32));
        }

        protected override bool OnRead()
        {
            bool result = true;
            PeerIndex = ReadIntFromPacket(new CompressionInfo.Integer(0,32),ref result);
            return result;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.General;
        }

        protected override string OnGetLogFormat()
        {
            return "ChangeClientPeerIndex";
        }
    }
}
