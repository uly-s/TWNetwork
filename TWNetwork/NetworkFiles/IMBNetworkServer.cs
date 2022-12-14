using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.MountAndBlade;
using TWNetwork.Messages.FromServer;
using TWNetwork.Patches;

namespace TWNetwork.NetworkFiles
{
    public class IMBNetworkServer: IMBNetworkEntity
    {
        private Dictionary<int,NativeMBPeer> Peers;
        private readonly List<int> AvailableIndexes;
        private readonly IServer NetworkServer;
        private NativeMBPeer CurrentPeer = null;
        private Random rnd;
        public delegate void OnClientConnectedHandler(NetworkCommunicator communicator);
        private static event OnClientConnectedHandler OnClientConnected;

        static IMBNetworkServer()
        {
            OnClientConnected += (communicator) =>
            {
                GameNetwork.BeginModuleEventAsServer(communicator);
                GameNetwork.WriteMessage(new ChangeClientPeerIndex(communicator.Index));
                GameNetwork.EndModuleEventAsServer();
            };
        }

        private IMBNetworkServer(int port, IServer s)
        {
            NetworkServer = s;
            Peers = new Dictionary<int,NativeMBPeer>();
            AvailableIndexes = Enumerable.Range(1,10).ToList(); //TODO: Capacity of server should be instead of 10.
            rnd = new Random();
            HandleNetworkPacket = typeof(GameNetwork).GetMethod("HandleNetworkPacketAsServer", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { typeof(NetworkCommunicator) }, null);
            NetworkServer.Start(port);
        }

        /// <summary>
		/// This method should be called from the server, when a GameNetworkMessage is received.
		/// </summary>
		/// <param name="peer">The peer, who sent the packet.</param>
		/// <param name="packet">The packet in a byte array.</param>
		public void HandleNetworkPacketAsServer(TWNetworkPeer peer, byte[] packet)
        {
            HandleNetworkPacketAsEntity(packet, new object[] { peer.Communicator });
        }
        public void HandleNewClientConnect(TWNetworkPeer peer,PlayerConnectionInfo info )
        {
            NetworkCommunicator communicator = GameNetwork.HandleNewClientConnect(info,false);
            Peers[communicator.Index].SetPeer(peer);
            peer.Communicator = communicator;

        }

        internal NativeMBPeer FindPeerByCommunicator(NetworkCommunicator communicator)
        {
            foreach (var peer in Peers.Values)
            {
                if (peer.Communicator == communicator)
                    return peer;
            }
            return null;
        }
        internal int AddNewPlayer(bool serverPlayer)
        {
            int index;
            var MBPeer = new NativeMBPeer();
            if (serverPlayer)
            {
                index = 0;
            }
            else
            {
                int idx = rnd.Next(0, AvailableIndexes.Count);
                index = AvailableIndexes[idx];
                AvailableIndexes.RemoveAt(idx);
            }
            Peers.Add(index,MBPeer);
            return index;
        }
        
        public static void AddOnClientConnectedEvent(OnClientConnectedHandler EventHandler)
        {
            OnClientConnected += EventHandler;
        }

        public static void RemoveOnClientConnectedEvent(OnClientConnectedHandler EventHandler)
        {
            OnClientConnected -= EventHandler;
        }

        public static void ClearOnClientConnectedEvents()
        {
            foreach (var Event in OnClientConnected.GetInvocationList())
            {
                OnClientConnected -= Event as OnClientConnectedHandler;
            }
        }

        public static void InvokeOnClientConnectedEvents(NetworkCommunicator communicator) 
        {
            OnClientConnected?.Invoke(communicator);
        }

        internal NativeMBPeer GetPeer(int index)
        {
            return Peers[index];
        }

        internal void BeginSingleModuleEvent(int index,bool isReliable)
        {
            BeginModuleEvent();
            CurrentPeer = Peers[index];
        }
        internal bool CanAddNewPlayers(int numPlayers)
        {
            //TODO: Adding capacity in MultiplayerOptions class.
            return true;
        }
        internal void EndSingleModuleEvent(bool isReliable)
        {
            CurrentPeer.Peer.SendRaw(GetBuffer(), (isReliable) ? DeliveryMethodType.Reliable : DeliveryMethodType.Unreliable);
            CurrentPeer = null;
            EndModuleEvent();
        }

        internal void OnPeerDisconnect(int peer)
        {
            //Questionable based on TW code.
            Peers.Remove(peer);
            AvailableIndexes.Add(peer);
        }

        internal void BeginBroadcastModuleEvent()
        {
            BeginModuleEvent();
        }

        private IReadOnlyList<NativeMBPeer> FilterPeers(GameNetwork.EventBroadcastFlags Flags,NativeMBPeer targetPlayer)
        {
            List<NativeMBPeer> peers = new List<NativeMBPeer>(Peers.Values);
            bool ShouldRemoveUnsynchronizedClients = true;
            foreach (var BroadcastFlag in Enum.GetValues(typeof(GameNetwork.EventBroadcastFlags)))
            {
                var flag = Flags & (GameNetwork.EventBroadcastFlags)BroadcastFlag;
                switch (flag)
                {
                    case GameNetwork.EventBroadcastFlags.DontSendToPeers:
                        peers.Clear();
                        return peers;
                    case GameNetwork.EventBroadcastFlags.ExcludePeerTeamPlayers:

                        if (targetPlayer is null || targetPlayer.Communicator.ControlledAgent is null || targetPlayer.Communicator.ControlledAgent.Team is null)
                            break;
                        foreach (var player in GameNetwork.NetworkPeers.Where(peer => peer != targetPlayer.Communicator && peer.ControlledAgent != null && peer.ControlledAgent.Team != null && peer.ControlledAgent.Team.MBTeam == targetPlayer.Communicator.ControlledAgent.Team.MBTeam))
                        {
                            peers.Remove(peers.Find(p => p.Communicator == player));
                        }
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer:
                        if (targetPlayer is null)
                            break;
                        peers.Remove(targetPlayer);
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers:
                        if (targetPlayer is null || targetPlayer.ControlledAgent is null || targetPlayer.ControlledAgent.Team is null)
                            break;
                        foreach (var player in GameNetwork.NetworkPeers.Where(peer => peer.ControlledAgent != null && peer.ControlledAgent.Team != null && peer.ControlledAgent.Team.MBTeam != targetPlayer.ControlledAgent.Team.MBTeam))
                        {
                            peers.Remove(peers.Find(p => p.Communicator == player));
                        }
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeNoBloodStainsOption:
                        //IDK yet, needs investigation
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeNoParticlesOption:
                        //IDK yet, needs investigation
                        break;
                    case GameNetwork.EventBroadcastFlags.AddToMissionRecord:
                        //IDK yet, needs investigation
                        break;
                    case GameNetwork.EventBroadcastFlags.ExcludeNoSoundOption:
                        //IDK yet, needs investigation
                        break;
                    case GameNetwork.EventBroadcastFlags.IncludeUnsynchronizedClients:
                        ShouldRemoveUnsynchronizedClients = false;
                        break;
                }
            }
            if (ShouldRemoveUnsynchronizedClients)
            {
                var p = peers.Where(pe => !pe.IsSynchronized).ToList();
                for(int i=0;i< p.Count;i++)
                {
                    peers.Remove(p[i]);
                }
            }
            return peers;
        }

        internal void EndBroadcastModuleEvent(int broadcastFlags, int targetPlayer, bool isReliable)
        {
            foreach (NativeMBPeer peer in FilterPeers((GameNetwork.EventBroadcastFlags)broadcastFlags,(targetPlayer == -1)?null:Peers[targetPlayer]))
            {
                peer.Peer.SendRaw(GetBuffer(), (isReliable) ? DeliveryMethodType.Reliable : DeliveryMethodType.Unreliable);
            }
            EndModuleEvent();
        }

        #region Static Members

        private static IMBNetworkServer server = null;
        public static IMBNetworkServer Server => server;
        /// <summary>
        /// Should be called, when the server is initialized, but before the GameNetwork.StartMultiplayerOnServer is called.
        /// </summary>
        public static void InitializeServer(int port,IServer s)
        {
            server = new IMBNetworkServer(port,s);
            Entity = server;
        }
        /// <summary>
        /// Should be called, when the server is stopped, but after the GameNetwork.TerminateServerSide method is called.
        /// </summary>
        public static void TerminateServer()
        {
            server.NetworkServer.Stop();
            server = null;
            Entity = null;
            GameNetworkPatches.NetworkIdentifier = NetworkIdentifier.None;
        }
        #endregion
    }
}
