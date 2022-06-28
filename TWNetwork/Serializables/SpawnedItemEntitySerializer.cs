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
    public class SpawnedItemEntitySerializer
    {
        [ProtoMember(1)]
        MissionObjectSerializer MissionObject { get; set; }
        public SpawnedItemEntitySerializer() { }
        public SpawnedItemEntitySerializer(SpawnedItemEntity spawnedItemEntity)
        {
            MissionObject = spawnedItemEntity;
        }

        public static implicit operator SpawnedItemEntitySerializer(SpawnedItemEntity spawnedItemEntity)
        { 
            return new SpawnedItemEntitySerializer(spawnedItemEntity);
        }

        public static implicit operator SpawnedItemEntity(SpawnedItemEntitySerializer serializer)
        {
            return (SpawnedItemEntity)(MissionObject)serializer.MissionObject;
        }
    }
}
