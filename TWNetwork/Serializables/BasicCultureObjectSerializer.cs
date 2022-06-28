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
    public class BasicCultureObjectSerializer
    {
        [ProtoMember(1)]
        public uint Reference { get; set; }

        public BasicCultureObjectSerializer(BasicCultureObject basicCultureObject)
        {
            Reference = SerializerHelper.GetReferenceFromObject(basicCultureObject);
        }

        public BasicCultureObjectSerializer() { }

        public static implicit operator BasicCultureObjectSerializer(BasicCultureObject basicCultureObject)
        {
            return new BasicCultureObjectSerializer(basicCultureObject);
        }

        public static implicit operator BasicCultureObject(BasicCultureObjectSerializer serializer)
        {
           return (BasicCultureObject)SerializerHelper.GetObjectFromRef(serializer.Reference);
        }
    }
}
