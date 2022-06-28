using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class UsableMissionObjectSerializer
    {
        [ProtoMember(1)]
        MissionObjectSerializer MissionObject { get; set; }
        public UsableMissionObjectSerializer() { }
        public UsableMissionObjectSerializer(UsableMissionObject usableMissionObject)
        {
            MissionObject = usableMissionObject;
        }

        public static implicit operator UsableMissionObjectSerializer(UsableMissionObject usableMissionObject)
        {
            return new UsableMissionObjectSerializer(usableMissionObject);
        }

        public static implicit operator UsableMissionObject(UsableMissionObjectSerializer serializer)
        {
            return (UsableMissionObject)(MissionObject)serializer.MissionObject;
        }
    }
}
