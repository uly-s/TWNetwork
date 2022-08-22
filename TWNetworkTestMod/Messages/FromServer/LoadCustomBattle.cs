using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetworkTestMod.Messages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class LoadCustomBattle : GameNetworkMessage
    {
        public string SceneID { get; private set; }
        public LoadCustomBattle() { }
        public LoadCustomBattle(string sceneID) 
        {
            SceneID = sceneID;
        }
        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Mission;
        }

        protected override string OnGetLogFormat()
        {
            return "LoadCustomBattle";
        }

        protected override bool OnRead()
        {
            bool result = true;
            SceneID = ReadStringFromPacket(ref result);
            return result;
        }

        protected override void OnWrite()
        {
            WriteStringToPacket(SceneID);
        }
    }
}
