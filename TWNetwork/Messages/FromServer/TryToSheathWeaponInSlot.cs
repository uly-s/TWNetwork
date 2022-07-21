using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.Messages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class TryToSheathWeaponInSlot : GameNetworkMessage
    {
        public HandIndex HandIndex { get; private set; }
        public WeaponWieldActionType Type { get; private set; }
        public Agent AgentRef { get; private set; }

        public TryToSheathWeaponInSlot(Agent agent,HandIndex handIndex, WeaponWieldActionType type)
        {
            HandIndex = handIndex;
            Type = type;
            AgentRef = agent;
        }

        public TryToSheathWeaponInSlot()
        {}
        protected override bool OnRead()
        {
            bool result = true;
            Type = (WeaponWieldActionType)ReadIntFromPacket(new CompressionInfo.Integer(0, 3, true), ref result);
            HandIndex = (HandIndex)ReadIntFromPacket(new CompressionInfo.Integer(0, 1, true), ref result);
            AgentRef = ReadAgentReferenceFromPacket(ref result);
            return result;
        }

        protected override void OnWrite()
        {
            WriteIntToPacket((int)Type, new CompressionInfo.Integer(0, 3, true));
            WriteIntToPacket((int)HandIndex, new CompressionInfo.Integer(0, 1, true));
            WriteAgentReferenceToPacket(AgentRef);
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Mission;
        }

        protected override string OnGetLogFormat()
        {
            return "TryToSheathWeaponInSlot";
        }
    }
}