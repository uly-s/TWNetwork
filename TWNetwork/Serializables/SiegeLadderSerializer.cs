using ProtoBuf;
using TaleWorlds.MountAndBlade;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class SiegeLadderSerializer
    {
        [ProtoMember(1)]
        public MissionObjectSerializer SiegeLadderRef { get; set; }
        public SiegeLadderSerializer() { }
        public SiegeLadderSerializer(SiegeLadder siegeLadder)
        {
            SiegeLadderRef = siegeLadder;
        }

        public static implicit operator SiegeLadderSerializer(SiegeLadder siegeLadder)
        {
            return new SiegeLadderSerializer(siegeLadder);
        }

        public static implicit operator SiegeLadder(SiegeLadderSerializer serializer)
        {
            return (SiegeLadder)(MissionObject)serializer.SiegeLadderRef;
        }
    }
}
