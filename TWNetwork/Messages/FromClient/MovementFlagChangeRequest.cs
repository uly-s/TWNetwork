using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.Messages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class MovementFlagChangeRequest: GameNetworkMessage
    {
        public MovementControlFlag MovementFlag { get; private set; }

        public MovementFlagChangeRequest()
        { }

        public MovementFlagChangeRequest(MovementControlFlag flag)
        {
            MovementFlag = flag;
        }

        protected override void OnWrite()
        {
            WriteUintToPacket((uint)MovementFlag,CompressionGenericExtended.MovementFlagCompressionInfo);
        }

        protected override bool OnRead()
        {
            bool result = true;
            MovementFlag = (MovementControlFlag)ReadUintFromPacket(CompressionGenericExtended.MovementFlagCompressionInfo,ref result);
            return result;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.General;
        }

        protected override string OnGetLogFormat()
        {
            return "MovementFlagChangedRequest";
        }
    }
}