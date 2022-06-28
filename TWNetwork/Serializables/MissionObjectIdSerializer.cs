using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class MissionObjectIdSerializer
    {
        [ProtoMember(1)]
        public bool CreatedAtRuntime;
        [ProtoMember(2)]
        public int Id;

        public MissionObjectIdSerializer() { }

        public MissionObjectIdSerializer(MissionObjectId ID)
        {
            CreatedAtRuntime = ID.CreatedAtRuntime;
            Id = ID.Id;
        }

        public static implicit operator MissionObjectId(MissionObjectIdSerializer serializer)
        {
            return new MissionObjectId(serializer.Id,serializer.CreatedAtRuntime);
        }

        public static implicit operator MissionObjectIdSerializer(MissionObjectId missionObjectId)
        {
            return new MissionObjectIdSerializer(missionObjectId);
        }
    }
}
