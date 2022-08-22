using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.MountAndBlade;
using TWNetwork.Extensions;

namespace TWNetwork.NetworkFiles
{
    public class IMBNetworkServer: IMBNetworkEntity
    {
        private Dictionary<int,NativeMBPeer> Peers;
        private readonly int Capacity;
        private readonly List<int> AvailableIndexes;
        private NativeMBPeer CurrentPeer = null;
        private Random rnd;
        private IMBNetworkServer(int capacity)
        {
            Peers = new Dictionary<int,NativeMBPeer>();
            Capacity = capacity;
            AvailableIndexes = Enumerable.Range(1, Capacity).ToList();
            rnd = new Random();
            HandleNetworkPacket = typeof(GameNetwork).GetMethod("HandleNetworkPacketAsServer", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { typeof(NetworkCommunicator) }, null);
        }

        /// <summary>
		/// This method should be called from the server, when a GameNetworkMessage is received.
		/// </summary>
		/// <param name="peer">The peer, who sent the packet.</param>
		/// <param name="packet">The packet in a byte array.</param>
		public void HandleNetworkPacketAsServer(TWNetworkPeer peer, byte[] packet)
        {
            HandleNetworkPacketAsEntity(packet, new object[] { peer.GetNetworkCommunicator() });
        }
        public void HandleNewClientConnect(TWNetworkPeer peer,PlayerConnectionInfo info )
        {
            NetworkCommunicatorExtensions.AddTWNetworkPeer(peer);
            GameNetwork.HandleNewClientConnect(info,false);
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
            var MBPeer = new NativeMBPeer();
            Peers.Add(index,MBPeer);
            NetworkCommunicatorExtensions.AddNativeMBPeerToLastPeer(MBPeer);
            return index;
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
            return Peers.Count + numPlayers <= Capacity;
        }
        internal void EndSingleModuleEvent(bool isReliable)
        {
            CurrentPeer.Communicator.Send(GetBuffer(), (isReliable) ? DeliveryMethodType.Reliable : DeliveryMethodType.Unreliable);
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
                peer.Communicator.Send(GetBuffer(), (isReliable) ? DeliveryMethodType.Reliable : DeliveryMethodType.Unreliable);
            }
            EndModuleEvent();
        }

        #region Static Members

        private static IMBNetworkServer server = null;
        public static IMBNetworkServer Server => server;
        /// <summary>
        /// Should be called, when the server is initialized, but before the GameNetwork.StartMultiplayerOnServer is called.
        /// </summary>
        /// <param name="Capacity">The capacity of the server.</param>
        public static void InitializeServer(int Capacity)
        {
            server = new IMBNetworkServer(Capacity);
            Entity = server;
        }
        /// <summary>
        /// Should be called, when the server is stopped, but after the GameNetwork.TerminateServerSide method is called.
        /// </summary>
        public static void TerminateServer()
        {
            server = null;
            Entity = null;
        }
        #endregion
    }
}
