using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.Messages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class TryToSheathWeaponInSlotRequest : GameNetworkMessage
    {
        public WeaponWieldActionType Type { get; private set; }
        public HandIndex HandIndex { get; private set; }

        public TryToSheathWeaponInSlotRequest() { }
        public TryToSheathWeaponInSlotRequest(WeaponWieldActionType type,HandIndex handIndex) 
        {
            Type = type;
            HandIndex = handIndex;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Mission;
        }

        protected override string OnGetLogFormat()
        {
            return "TryToSheathWeaponInSlotRequest";
        }

        protected override bool OnRead()
        {
            bool result = true;
            Type = (WeaponWieldActionType)ReadIntFromPacket(new CompressionInfo.Integer(0,3,true),ref result);
            HandIndex = (HandIndex)ReadIntFromPacket(new CompressionInfo.Integer(0, 1, true), ref result);
            return result;
        }

        protected override void OnWrite()
        {
            WriteIntToPacket((int)Type,new CompressionInfo.Integer(0,3,true));
            WriteIntToPacket((int)HandIndex,new CompressionInfo.Integer(0,1,true));
        }
    }
}