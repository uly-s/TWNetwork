using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.Messages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class MainAgentTick : GameNetworkMessage
    {
        public EventControlFlag EventFlag { get; private set; }
        public Vec3 LookDirection { get; private set; }
        public MovementControlFlag MovementFlag { get; private set; }
        public Vec2 MovementInputVector { get; private set; }

        public MainAgentTick() { }
        public MainAgentTick(EventControlFlag eventFlag, Vec3 lookDirection, MovementControlFlag movementFlag, Vec2 movementInputVector)
        {
            EventFlag = eventFlag;
            LookDirection = lookDirection;
            MovementFlag = movementFlag;
            MovementInputVector = movementInputVector;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Mission;
        }

        protected override string OnGetLogFormat()
        {
            return "MainAgentTick";
        }

        protected override bool OnRead()
        {
            bool result = true;
            MovementFlag = (MovementControlFlag)ReadUintFromPacket(CompressionGenericExtended.MovementFlagCompressionInfo, ref result);
            EventFlag = (EventControlFlag)ReadUintFromPacket(CompressionGenericExtended.EventControlFlagCompressionInfo, ref result);
            LookDirection = ReadVec3FromPacket(CompressionInfo.Float.FullPrecision, ref result);
            MovementInputVector = ReadVec2FromPacket(CompressionInfo.Float.FullPrecision, ref result);
            return result;
        }

        protected override void OnWrite()
        {
            WriteUintToPacket((uint)MovementFlag, CompressionGenericExtended.MovementFlagCompressionInfo);
            WriteUintToPacket((uint)EventFlag, CompressionGenericExtended.EventControlFlagCompressionInfo);
            WriteVec3ToPacket(LookDirection, CompressionInfo.Float.FullPrecision);
            WriteVec2ToPacket(MovementInputVector, CompressionInfo.Float.FullPrecision);
        }
    }
}
