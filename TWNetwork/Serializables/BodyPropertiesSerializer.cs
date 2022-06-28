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
    public class BodyPropertiesSerializer
    {
        [ProtoMember(1)]
        public float Age { get; set; }
        [ProtoMember(2)]
        public float Weight { get; set; }
        [ProtoMember(3)]
        public float Build { get; set; }
        [ProtoMember(4)]
        public ulong KeyPart1 { get; set; }
        [ProtoMember(5)]
        public ulong KeyPart2 { get; set; }
        [ProtoMember(6)]
        public ulong KeyPart3 { get; set; }
        [ProtoMember(7)]
        public ulong KeyPart4 { get; set; }
        [ProtoMember(8)]
        public ulong KeyPart5 { get; set; }
        [ProtoMember(9)]
        public ulong KeyPart6 { get; set; }
        [ProtoMember(10)]
        public ulong KeyPart7 { get; set; }
        [ProtoMember(11)]
        public ulong KeyPart8 { get; set; }

        public BodyPropertiesSerializer(BodyProperties properties)
        { 
            this.Age = properties.Age;
            this.Weight = properties.Weight;
            this.Build = properties.Build;
            KeyPart1 = properties.KeyPart1;
            KeyPart2 = properties.KeyPart2;
            KeyPart3 = properties.KeyPart3;
            KeyPart4 = properties.KeyPart4;
            KeyPart5 = properties.KeyPart5;
            KeyPart6 = properties.KeyPart6;
            KeyPart7 = properties.KeyPart7;
            KeyPart8 = properties.KeyPart8;
        }

        public BodyPropertiesSerializer()
        { }

        public static implicit operator BodyProperties(BodyPropertiesSerializer serializer)
        {
            return new BodyProperties(new DynamicBodyProperties(serializer.Age, serializer.Weight, serializer.Build), new StaticBodyProperties(serializer.KeyPart1, serializer.KeyPart2, serializer.KeyPart3, serializer.KeyPart4, serializer.KeyPart5, serializer.KeyPart6, serializer.KeyPart7, serializer.KeyPart8));
        }

        public static implicit operator BodyPropertiesSerializer(BodyProperties bodyProperties)
        {
            return new BodyPropertiesSerializer(bodyProperties);
        }
    }
}
