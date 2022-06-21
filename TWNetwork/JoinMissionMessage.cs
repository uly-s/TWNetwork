using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNetwork
{
    [ProtoContract]
    public class JoinMissionMessage
    {
        [ProtoMember(1)]
        public Guid MissionId { get; set; }

        public JoinMissionMessage() { }
        public JoinMissionMessage(Guid id) { MissionId = id; }
    }
}
