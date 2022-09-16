using LiteNetLib;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.PlayerServices;
using TWNetwork.Extensions;
using TWNetwork.InterfacePatches;
using TWNetwork.NetworkFiles;
using TWNetworkTestMod.Messages.FromServer;

namespace TWNetworkTestMod
{
    public class TWNetworkServer : IUpdatable,INetEventListener
    {
        static TWNetworkServer()
        {
            IMBNetworkServer.AddOnClientConnectedEvent(
                (communicator) =>
                {
                    GameNetwork.BeginModuleEventAsServer(communicator);
                    GameNetwork.WriteMessage(new LoadCustomBattle(TWNetworkCustomBattlePatches.SceneID));
                    GameNetwork.EndModuleEventAsServer();
                }
                );
        }
        private NetManager Server= null;
        private List<TWNetworkConnection> Clients = null;
        private int Capacity;

        public TWNetworkServer() { }

        public void Start(int port,int capacity)
        {
            Server = new NetManager(this);
            Clients = new List<TWNetworkConnection>();
            Capacity = capacity;
            Server.Start(port);
            IMBNetwork.Capacity = Capacity;
            GameNetwork.StartMultiplayerOnServer(port);
            MBCommon.CurrentGameType = MBCommon.GameType.Single;
            GameNetwork.AddNewPlayerOnServer(new PlayerConnectionInfo(new PlayerId(Guid.NewGuid())),true,true);
        }

        public void Stop()
        {
            Server.Stop();
            Server = null;
            Clients = null;
            Capacity = 0;
        }

        public void Update() 
        {
            Server?.PollEvents();
        }
        public void OnConnectionRequest(ConnectionRequest request)
        {
            if (Clients.Count < Capacity)
                request.Accept();
            else
                request.Reject();
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            IMBNetworkServer.Server.HandleNetworkPacketAsServer(peer.GetConnection(), reader.GetRemainingBytes());
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public void OnPeerConnected(NetPeer peer)
        {
            InformationManager.DisplayMessage(new InformationMessage("Client connected,waiting for client to load mission."));
            TWNetworkConnection con = new TWNetworkConnection(peer);
            Clients.Add(con);
            PlayerConnectionInfo info = new PlayerConnectionInfo(new PlayerId(Guid.NewGuid()));
            IMBNetworkServer.Server.HandleNewClientConnect(con,info);
            IMBNetworkServer.InvokeOnClientConnectedEvents(con.GetNetworkCommunicator());
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Clients.Remove(peer.GetConnection());
        }
    }
}
