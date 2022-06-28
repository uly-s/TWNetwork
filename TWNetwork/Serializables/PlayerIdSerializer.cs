using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.PlayerServices;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class PlayerIdSerializer
    {
        [ProtoMember(1)]
        public ulong Part1 { get; set; }
        [ProtoMember(2)]
        public ulong Part2 { get; set; }
        [ProtoMember(3)]
        public ulong Part3 { get; set; }
        [ProtoMember(4)]
        public ulong Part4 { get; set; }

        public PlayerIdSerializer(PlayerId playerId)
        { 
        }
        public PlayerIdSerializer() { }
        public static implicit operator PlayerIdSerializer(PlayerId playerId)
        {
            return new PlayerIdSerializer(playerId);
        }
        public static implicit operator PlayerId(PlayerIdSerializer serializer)
        {
            return new PlayerId(serializer.Part1, serializer.Part2, serializer.Part3, serializer.Part4);
        }
    }
}
