using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class Vec3Serializer
    {
        [ProtoMember(1)]
        public float X { get; set; }
        [ProtoMember(2)]
        public float Y { get; set; }
        [ProtoMember(3)]
        public float Z { get; set; }

        public Vec3Serializer()
        { }

        public Vec3Serializer(Vec3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public static implicit operator Vec3(Vec3Serializer serializer)
        { 
            return new Vec3(serializer.X, serializer.Y, serializer.Z); 
        }

        public static implicit operator Vec3Serializer(Vec3 vec3)
        {
            return new Vec3Serializer(vec3);
        }
    }
}
