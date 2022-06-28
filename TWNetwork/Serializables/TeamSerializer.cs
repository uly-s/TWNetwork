using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class TeamSerializer
    {
        [ProtoMember(1)]
        public MBTeamSerializer MBTeamRef { get; set; }

        public TeamSerializer()
        { }
        public TeamSerializer(Team team)
        {
            MBTeam t = (team == null) ? MBTeam.InvalidTeam : team.MBTeam;
            MBTeamRef = t;
        }

        public static implicit operator Team(TeamSerializer serializer)
        {
            MBTeam mbTeam = serializer.MBTeamRef;
            if (mbTeam.IsValid)
                return Mission.Current.Teams.Find(mbTeam);
            return Team.Invalid;
        }

        public static implicit operator TeamSerializer(Team team)
        {
            return new TeamSerializer(team);
        }
    }
}
