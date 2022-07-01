using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.Messages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class ClientTick : GameNetworkMessage
    {
        public EventControlFlag EventFlags { get; private set; }
        public MovementControlFlag MovementFlags { get; private set; }
        public Vec2 MovementInputVector { get; private set; }
        public Vec3 LookDirection { get; private set; }

        public ClientTick(Agent agent)
        {
            EventFlags = agent.EventControlFlags;
            MovementFlags = agent.MovementFlags;
            MovementInputVector = agent.MovementInputVector;
            LookDirection = agent.LookDirection;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Mission;
        }

        protected override string OnGetLogFormat()
        {
            return "ClientTick"; 
        }

        protected override bool OnRead()
        {
            bool result = true;
            EventFlags = (EventControlFlag)ReadUintFromPacket(CompressionGenericExtended.EventControlFlagCompressionInfo, ref result);
            MovementFlags = (MovementControlFlag)ReadUintFromPacket(CompressionGenericExtended.MovementFlagCompressionInfo, ref result);
            MovementInputVector = ReadVec2FromPacket(CompressionInfo.Float.FullPrecision, ref result);
            LookDirection = ReadVec3FromPacket(CompressionInfo.Float.FullPrecision,ref result);
            return result;
        }

        protected override void OnWrite()
        {
            WriteUintToPacket((uint)EventFlags,CompressionGenericExtended.EventControlFlagCompressionInfo);
            WriteUintToPacket((uint)MovementFlags, CompressionGenericExtended.MovementFlagCompressionInfo);
            WriteVec2ToPacket(MovementInputVector,CompressionInfo.Float.FullPrecision);
            WriteVec3ToPacket(LookDirection, CompressionInfo.Float.FullPrecision);
        }
    }
}