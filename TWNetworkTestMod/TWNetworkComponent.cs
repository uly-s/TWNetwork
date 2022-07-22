using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TWNetworkTestMod.Messages.FromServer;

namespace TWNetworkTestMod
{
    public class TWNetworkComponent : UdpNetworkComponent
    {
        protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
        {
            if (GameNetwork.IsClient)
            {
                registerer.Register(new GameNetworkMessage.ServerMessageHandlerDelegate<LoadCustomBattle>(HandleServerEventLoadCustomBattle));
                return;
            }
        }

        private void HandleServerEventLoadCustomBattle(LoadCustomBattle message)
        {
            BannerlordMissions.OpenCustomBattleMission(message.SceneID,null,null,null,false,null,message.SceneLevels,message.SeasonString,message.TimeOfDay);
        }
    }
}
