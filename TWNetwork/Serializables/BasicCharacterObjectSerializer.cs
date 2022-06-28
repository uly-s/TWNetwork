using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class BasicCharacterObjectSerializer
    {
        [ProtoMember(1)]
        public uint Reference { get; set; }

        public BasicCharacterObjectSerializer(BasicCharacterObject basicCultureObject)
        {
            Reference = SerializerHelper.GetReferenceFromObject(basicCultureObject);
        }

        public BasicCharacterObjectSerializer() { }

        public static implicit operator BasicCharacterObjectSerializer(BasicCharacterObject basicCultureObject)
        {
            return new BasicCharacterObjectSerializer(basicCultureObject);
        }

        public static implicit operator BasicCharacterObject(BasicCharacterObjectSerializer serializer)
        {
            return (BasicCharacterObject)SerializerHelper.GetObjectFromRef(serializer.Reference);
        }
    }
}
