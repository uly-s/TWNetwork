using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetworkTestMod.Messages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class SpawnAgentRequest : GameNetworkMessage
    {
        public SpawnAgentRequest() { }
        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Mission;
        }

        protected override string OnGetLogFormat()
        {
            return "SpawnAgentRequest";
        }

        protected override bool OnRead()
        {
            return true;
        }

        protected override void OnWrite()
        {
        }
    }
}
