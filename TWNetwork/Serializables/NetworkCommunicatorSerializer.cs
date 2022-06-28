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
    public class NetworkCommunicatorSerializer
    {
        [ProtoMember(1)]
        public int Index { get; set; }
        public NetworkCommunicatorSerializer() { }
        public NetworkCommunicatorSerializer(NetworkCommunicator networkCommunicator) 
        {
            Index = (networkCommunicator != null) ? networkCommunicator.Index : -1;
        }

        public static implicit operator NetworkCommunicatorSerializer(NetworkCommunicator networkCommunicator)
        {
            return new NetworkCommunicatorSerializer(networkCommunicator);
        }

        public static implicit operator NetworkCommunicator(NetworkCommunicatorSerializer serializer)
        {
            if ((serializer.Index >= 0 && !GameNetworkMessage.IsClientMissionOver))
            {
                return GameNetwork.FindNetworkPeer(serializer.Index);
            }
            return null;
        }
    }
}
