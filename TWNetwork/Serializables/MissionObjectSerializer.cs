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
    public class MissionObjectSerializer
    {
        [ProtoMember(1)]
        public MissionObjectIdSerializer MissionObjectId;
        

        public MissionObjectSerializer() { }

        public MissionObjectSerializer(MissionObject missionObject)
        {
            MissionObjectId = (missionObject != null) ? missionObject.Id : new MissionObjectId(-1, false);
        }

        public static implicit operator MissionObject(MissionObjectSerializer serializer)
        {
            MissionObjectId ID = serializer.MissionObjectId;
            if (ID.Id == -1 || GameNetworkMessage.IsClientMissionOver)
            {
                return null; 
            }
            return Mission.Current.MissionObjects.FirstOrDefault((MissionObject mo) => mo.Id == ID);
        }

        public static implicit operator MissionObjectSerializer(MissionObject missionObject)
        {
            return new MissionObjectSerializer(missionObject);
        }
    }
}
