using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.Messages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class LookDirectionChangeRequest : GameNetworkMessage
    {
        public Vec3 LookDirection { get; private set; }

        public LookDirectionChangeRequest(Vec3 lookDirection)
        {
            LookDirection = lookDirection;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Mission;
        }

        protected override string OnGetLogFormat()
        {
            return "LookDirectionChangeRequest"; 
        }

        protected override bool OnRead()
        {
            bool result = true;
            LookDirection = ReadVec3FromPacket(CompressionInfo.Float.FullPrecision,ref result);
            return result;
        }

        protected override void OnWrite()
        {
            WriteVec3ToPacket(LookDirection, CompressionInfo.Float.FullPrecision);
        }
    }
}