using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TWNetwork.InterfacePatches;
using TWNetwork.NetworkFiles;

namespace TWNetworkTestMod
{
    public class TWNetworkClient : IUpdatable,INetEventListener
    {
        private NetManager Client = null;
        private TWNetworkConnection ServerPeer = null;

        public TWNetworkClient() { }

        public void Start(string serverAddress,int port)
        {
            Client = new NetManager(this);
            Client.Start();
            Client.Connect(serverAddress,port,"");
        }

        public void Stop()
        {
            Client.Stop();
            Client = null;
            ServerPeer = null;
        }

        public void Update()
        {
            Client?.PollEvents();
        }
        public void OnConnectionRequest(ConnectionRequest request)
        {
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            IMBNetworkClient.Client.HandleNetworkPacketAsClient(reader.GetRemainingBytes());
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public void OnPeerConnected(NetPeer peer)
        {
            ServerPeer = new TWNetworkConnection(peer);
            IMBNetwork.ServerPeer = ServerPeer;
            GameNetwork.StartMultiplayerOnClient("", 0, 1, 1);
            MBCommon.CurrentGameType = MBCommon.GameType.Single;
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
        }
    }
}
