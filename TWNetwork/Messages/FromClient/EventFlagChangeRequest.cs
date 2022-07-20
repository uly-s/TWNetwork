using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.Messages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class EventFlagChangeRequest : GameNetworkMessage
    {

        public EventControlFlag EventFlag { get; private set; }

        public EventFlagChangeRequest()
        { }

        public EventFlagChangeRequest(EventControlFlag flag)
        {
            EventFlag = flag;
        }

        protected override void OnWrite()
        {
            WriteUintToPacket((uint)EventFlag, CompressionGenericExtended.EventControlFlagCompressionInfo);
        }

        protected override bool OnRead()
        {
            bool result = true;
            EventFlag = (EventControlFlag)ReadUintFromPacket(CompressionGenericExtended.EventControlFlagCompressionInfo, ref result);
            return result;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.General;
        }

        protected override string OnGetLogFormat()
        {
            return "EventControlFlagChangedRequest";
        }
    }
}