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
    public class Mat3Serializer
    {
        [ProtoMember(1)]
        public Vec3Serializer S { get; set; }
        [ProtoMember(2)]
        public Vec3Serializer F { get; set; }
        [ProtoMember(3)]
        public Vec3Serializer U { get; set; }

        public Mat3Serializer() { }
        public Mat3Serializer(Mat3 mat)
        {
            S = new Vec3Serializer(mat.s);
            F = new Vec3Serializer(mat.f);
            U = new Vec3Serializer(mat.u);
        }

        public static implicit operator Mat3(Mat3Serializer serializer)
        {
            return new Mat3(serializer.S, serializer.F, serializer.U);
        }

        public static implicit operator Mat3Serializer(Mat3 mat3)
        {
            return new Mat3Serializer(mat3);
        }


    }
}
