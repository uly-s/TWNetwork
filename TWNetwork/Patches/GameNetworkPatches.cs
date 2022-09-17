using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TWNetworkHelper;

namespace TWNetwork.Patches
{
    internal enum NetworkIdentifier { None, Server, Client }
    internal class GameNetworkPatches : HarmonyPatches
    {
        internal static NetworkIdentifier NetworkIdentifier { get; set; } = NetworkIdentifier.None;

        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.IsServer), false, TWNetworkHelper.MethodType.Getter)]
        private bool get_IsServer()
        {
            return NetworkIdentifier == NetworkIdentifier.Server;
        }

        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.IsClient), false, TWNetworkHelper.MethodType.Getter)]
        private bool get_IsClient()
        {
            return NetworkIdentifier == NetworkIdentifier.Client;
        }

        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.StartMultiplayerOnServer), new Type[] { typeof(int) }, true)]
        private void StartMultiplayerOnServer(int port)
        {
            if (NetworkIdentifier != NetworkIdentifier.None)
                return;
            GameNetwork.ClientPeerIndex = -1;
            typeof(GameNetwork).GetMethod("InitializeServerSide",BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Invoke(null,new object[] { port });
            typeof(GameNetwork).GetMethod("StartMultiplayer", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Invoke(null, new object[] { });
            NetworkIdentifier = NetworkIdentifier.Server;
        }

        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.StartMultiplayerOnClient), new Type[] { typeof(string), typeof(int), typeof(int), typeof(int) }, true)]
        private void StartMultiplayerOnClient(string serverAddress,int port,int sessionKey,int playerIndex)
        {
            if (NetworkIdentifier != NetworkIdentifier.None)
                return;
            GameNetwork.ClientPeerIndex = playerIndex;
            GameNetwork.InitializeClientSide(serverAddress, port, sessionKey, playerIndex);
            typeof(GameNetwork).GetMethod("StartMultiplayer", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Invoke(null, new object[] { });
            GameNetwork.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
            NetworkIdentifier = NetworkIdentifier.Client;

        }
    }
}
