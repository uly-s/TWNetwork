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
    public class Vec2Serializer
    {
        [ProtoMember(1)]
        public float X { get; set; }
        [ProtoMember(2)]
        public float Y { get; set; }

        public Vec2Serializer()
        { }

        public Vec2Serializer(Vec2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public static implicit operator Vec2(Vec2Serializer serializer)
        { 
            return new Vec2(serializer.X, serializer.Y); 
        }

        public static implicit operator Vec2Serializer(Vec2 vec2)
        {
           return new Vec2Serializer(vec2);
        }    
    }
}
