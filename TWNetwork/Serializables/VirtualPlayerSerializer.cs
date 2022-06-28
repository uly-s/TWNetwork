using ProtoBuf;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class VirtualPlayerSerializer
    {
        [ProtoMember(1)]
        public int Index { get; set; }
        public VirtualPlayerSerializer() { }
        public VirtualPlayerSerializer(VirtualPlayer virtualPlayer)
        {
            Index = (virtualPlayer != null) ? virtualPlayer.Index : -1;
        }

        public static implicit operator VirtualPlayer(VirtualPlayerSerializer serializer)
        {
            if ((serializer.Index >= 0 && !GameNetworkMessage.IsClientMissionOver))
            {
                return MBNetwork.VirtualPlayers[serializer.Index];
            }
            return null;
        }

        public static implicit operator VirtualPlayerSerializer(VirtualPlayer virtualPlayer)
        {
            return new VirtualPlayerSerializer(virtualPlayer);
        }
    }
}
