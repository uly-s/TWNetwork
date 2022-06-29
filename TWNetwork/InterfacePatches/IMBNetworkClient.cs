using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TWNetwork.Extensions;

namespace TWNetwork.InterfacePatches
{
    internal class IMBNetworkClient: IMBNetworkEntity
    {
        private readonly INetworkPeer ServerPeer;
        private IMBNetworkClient(INetworkPeer serverPeer)
        {
            ServerPeer = serverPeer;
            HandleNetworkPacket = typeof(GameNetwork).GetMethod("HandleNetworkPacketAsClient", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }
        public void BeginModuleEventAsClient(bool isReliable)
        {
            BeginModuleEvent();
        }
        public void EndModuleEventAsClient(bool isReliable)
        {
            ServerPeer.SendRaw(GetBuffer(), (isReliable) ? DeliveryMethodType.Reliable : DeliveryMethodType.Unreliable);
            EndModuleEvent();
        }

        /// <summary>
		/// This method should be called from the client, when a GameNetworkMessage is received.
		/// </summary>
		/// <param name="packet">The packet in a byte array.</param>
		public void HandleNetworkPacketAsClient(byte[] packet)
        {
            OnReceivePacket(packet);
            while ((bool)HandleNetworkPacket?.Invoke(null, new object[] { })) { }
        }

        #region Static Members

        private static IMBNetworkClient client = null;
        public static IMBNetworkClient Client => client;
        /// <summary>
        /// Should be called, when the client is initialized, but before the GameNetwork.StartMultiplayerOnClient is called.
        /// </summary>
        /// <param name="Capacity">The capacity of the server.</param>
        public static void InitializeClient(INetworkPeer ServerPeer)
        {
            client = new IMBNetworkClient(ServerPeer);
            Entity = client;

        }
        /// <summary>
        /// Should be called, when the client is stopped/disconnected, but after the GameNetwork.TerminateClientSide method is called.
        /// </summary>
        public static void TerminateServer()
        {
            client = null;
            Entity = null;
        }
        #endregion
    }
}
