using ProtoBuf;
using TaleWorlds.MountAndBlade;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class BatteringRamSerializer
    {
        [ProtoMember(1)]
        public MissionObjectSerializer BatteringRamRef { get; set; }
        public BatteringRamSerializer() { }
        public BatteringRamSerializer(BatteringRam batteringRam)
        {
            BatteringRamRef = batteringRam;
        }

        public static implicit operator BatteringRamSerializer(BatteringRam batteringRam)
        {
            return new BatteringRamSerializer(batteringRam);
        }

        public static implicit operator BatteringRam(BatteringRamSerializer serializer)
        {
            return (BatteringRam)(MissionObject)serializer.BatteringRamRef;
        }
    }
}
