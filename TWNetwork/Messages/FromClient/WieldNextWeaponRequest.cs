using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.Messages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class WieldNextWeaponRequest : GameNetworkMessage
    {
        public HandIndex WeaponIndex { get; private set; }
        public WeaponWieldActionType WieldActionType { get; private set; }

        public WieldNextWeaponRequest() { }
        public WieldNextWeaponRequest(HandIndex weaponIndex,WeaponWieldActionType wieldActionType) 
        {
            WeaponIndex = weaponIndex;
            WieldActionType = wieldActionType;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Mission;
        }

        protected override string OnGetLogFormat()
        {
            return "WieldNextWeaponRequest";
        }

        protected override bool OnRead()
        {
            bool result = true;
            WeaponIndex = (HandIndex)ReadIntFromPacket(new CompressionInfo.Integer(0, 1, true), ref result);
            WieldActionType = (WeaponWieldActionType)ReadIntFromPacket(new CompressionInfo.Integer(0, 3, true), ref result);
            return result;
        }

        protected override void OnWrite()
        {
            WriteIntToPacket((int)WeaponIndex,new CompressionInfo.Integer(0,1,true));
            WriteIntToPacket((int)WieldActionType,new CompressionInfo.Integer(0,3,true));
        }
    }
}