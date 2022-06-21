using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNetwork
{
    [ProtoContract]
    public class DisconnectMissionMessage
    {
        [ProtoMember(1)]
        public Guid MissionId { get; set; }

        public DisconnectMissionMessage()
        { }

        public DisconnectMissionMessage(Guid id)
        {
            MissionId = id;
        }
    }
}
