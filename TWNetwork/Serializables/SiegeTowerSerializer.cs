using ProtoBuf;
using TaleWorlds.MountAndBlade;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class SiegeTowerSerializer
    {
        [ProtoMember(1)]
        public MissionObjectSerializer SiegeTowerRef { get; set; }
        public SiegeTowerSerializer() { }
        public SiegeTowerSerializer(SiegeTower siegeTower)
        {
            SiegeTowerRef = siegeTower;
        }

        public static implicit operator SiegeTowerSerializer(SiegeTower siegeTower)
        {
            return new SiegeTowerSerializer(siegeTower);
        }

        public static implicit operator SiegeTower(SiegeTowerSerializer serializer)
        {
            return (SiegeTower)(MissionObject)serializer.SiegeTowerRef;
        }
    }
}
