using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class MBTeamSerializer
    {
        private static ConstructorInfo MBTeamCtr = typeof(MBTeam).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,null,new Type[] { typeof(Mission), typeof(int) },null);

        [ProtoMember(1)]
        public int Index { get; set; }
        public MBTeamSerializer()
        { }

        public MBTeamSerializer(MBTeam mBTeam)
        {
            Index = mBTeam.Index;
        }

        public static implicit operator MBTeam(MBTeamSerializer serializer)
        {
            if (!GameNetworkMessage.IsClientMissionOver)
            {
                return (MBTeam)MBTeamCtr.Invoke(new object[] { Mission.Current, serializer.Index });
            }
            return MBTeam.InvalidTeam;
        }

        public static implicit operator MBTeamSerializer(MBTeam mBTeam)
        {
            return new MBTeamSerializer(mBTeam);
        }
    }
}
