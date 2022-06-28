using ProtoBuf;
using TaleWorlds.MountAndBlade;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class StonePileSerializer
    {
        [ProtoMember(1)]
        public MissionObjectSerializer StonePileRef { get; set; }
        public StonePileSerializer() { }
        public StonePileSerializer(StonePile stonePile)
        {
            StonePileRef = stonePile;
        }

        public static implicit operator StonePileSerializer(StonePile stonePile)
        {
            return new StonePileSerializer(stonePile);
        }

        public static implicit operator StonePile(StonePileSerializer serializer)
        {
            return (StonePile)(MissionObject)serializer.StonePileRef;
        }
    }
}
