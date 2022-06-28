using ProtoBuf;
using TaleWorlds.Core;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class MonsterSerializer
    {
        [ProtoMember(1)]
        public uint Reference { get; set; }

        public MonsterSerializer(Monster monster)
        {
            Reference = SerializerHelper.GetReferenceFromObject(monster);
        }

        public MonsterSerializer() { }

        public static implicit operator MonsterSerializer(Monster monster)
        {
            return new MonsterSerializer(monster);
        }

        public static implicit operator Monster(MonsterSerializer serializer)
        {
            return (Monster)SerializerHelper.GetObjectFromRef(serializer.Reference);
        }
    }
}
