using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork
{
    internal enum ClientState
    {
        None,ReliableModuleEvent,UnreliableModuleEvent
    }
    public class MissionClient : MissionNetworkEntity
    {
        private readonly INetworkPeer Peer;
        private ClientState CurrentState;
        public MissionClient(INetworkPeer peer)
        {
            Peer = peer;
            CurrentState = ClientState.None;
        }

        public void BeginModuleEventAsClient()
        {
            CurrentState = ClientState.ReliableModuleEvent;
        }

        public void BeginModuleEventAsClientUnreliable()
        {
            CurrentState = ClientState.UnreliableModuleEvent;
        }

        public void EndModuleEventAsClient()
        {
            if (CurrentState != ClientState.ReliableModuleEvent || MessagesToSend is null)
                throw new InvalidOperationException();
            var memstream = new MemoryStream();
            Serializer.Serialize(memstream, MessagesToSend);
            memstream.TryGetBuffer(out var buffer);
            Peer.SendRaw(buffer,DeliveryMethodType.Reliable);
            MessagesToSend = null;
        }

        public void EndModuleEventAsClientUnreliable()
        {
            if (CurrentState != ClientState.UnreliableModuleEvent || MessagesToSend is null)
                throw new InvalidOperationException();
            var memstream = new MemoryStream();
            Serializer.Serialize(memstream, MessagesToSend);
            memstream.TryGetBuffer(out var buffer);
            Peer.SendRaw(buffer, DeliveryMethodType.Unreliable);
            MessagesToSend = null;
        }

        public bool HandleNetworkPacketAsClient()
        {
            //TODO: implement
        }
    }
}
