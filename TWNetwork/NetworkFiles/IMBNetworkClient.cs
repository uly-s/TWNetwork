using System.Reflection;
using TaleWorlds.MountAndBlade;
using TWNetwork.Patches;

namespace TWNetwork.NetworkFiles
{
    public class IMBNetworkClient: IMBNetworkEntity
    {
        private readonly TWNetworkPeer ServerPeer;
        private readonly IClient NetworkClient;
        private IMBNetworkClient(string serverAddress, int port, int sessionKey, int playerIndex,IClient c)
        {
            NetworkClient = c;
            HandleNetworkPacket = typeof(GameNetwork).GetMethod("HandleNetworkPacketAsClient", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            ServerPeer = NetworkClient.Connect(serverAddress,port);
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
            HandleNetworkPacketAsEntity(packet, new object[] { });
        }

        #region Static Members

        private static IMBNetworkClient client = null;
        public static IMBNetworkClient Client => client;
        /// <summary>
        /// Should be called, when the client is initialized, but before the GameNetwork.StartMultiplayerOnClient is called.
        /// </summary>
        /// <param name="Capacity">The capacity of the server.</param>
        public static void InitializeClient(string serverAddress, int port, int sessionKey, int playerIndex,IClient c)
        {
            client = new IMBNetworkClient(serverAddress,port,sessionKey,playerIndex,c);
            Entity = client;

        }
        /// <summary>
        /// Should be called, when the client is stopped/disconnected, but after the GameNetwork.TerminateClientSide method is called.
        /// </summary>
        public static void TerminateClient()
        {
            client.NetworkClient.Disconnect();
            client = null;
            Entity = null;
            GameNetworkPatches.NetworkIdentifier = NetworkIdentifier.None;
        }
        #endregion
    }
}
