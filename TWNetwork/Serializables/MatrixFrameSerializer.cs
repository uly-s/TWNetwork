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
    public class MatrixFrameSerializer
    {
        [ProtoMember(1)]
        public Mat3Serializer Rotation { get; set; }
        [ProtoMember(2)]
        public Vec3Serializer Origin { get; set; }

        public MatrixFrameSerializer()
        { }

        public MatrixFrameSerializer(MatrixFrame matrixFrame)
        {
            Origin = matrixFrame.origin;
            Rotation = matrixFrame.rotation;
        }

        public static implicit operator MatrixFrame(MatrixFrameSerializer serializer)
        {
            return new MatrixFrame(serializer.Rotation,serializer.Origin);
        }

        public static implicit operator MatrixFrameSerializer(MatrixFrame matrixFrame)
        {
            return new MatrixFrameSerializer(matrixFrame);
        }
    }
}
