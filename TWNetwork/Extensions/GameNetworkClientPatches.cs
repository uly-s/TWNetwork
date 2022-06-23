using TaleWorlds.MountAndBlade;
using TWNetworkPatcher;
using System;
namespace TWNetwork
{
    public class GameNetworkClientPatches : HarmonyPatches
    {
        private static MissionClient _missionClient = null;

        public static MissionNetworkEntity Entity => _missionClient;
        public static bool IsInitialized { get; private set; } = false;
        private static INetworkPeer _peer = null;
        public static INetworkPeer Peer 
        {
            get
            { 
                return _peer; 
            } 
            set
            {
                if (IsInitialized)
                {
                    throw new MissionEntityAlreadyInitializedException();
                }
                _peer = value;
            }
        }
        [PatchedMethod(typeof(GameNetwork),nameof(GameNetwork.BeginModuleEventAsClient),true)]
        private void BeginModuleEventAsClient()
        {
            _missionClient.BeginModuleEventAsClient();
        }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.BeginModuleEventAsClientUnreliable), true)]
        private void BeginModuleEventAsClientUnreliable()
        {
            _missionClient.BeginModuleEventAsClientUnreliable();
        }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.EndModuleEventAsClient), true)]
        private void EndModuleEventAsClient()
        {
            _missionClient.EndModuleEventAsClient();
        }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.EndModuleEventAsClientUnreliable), true)]
        private void EndModuleEventAsClientUnreliable()
        {
            _missionClient.EndModuleEventAsClientUnreliable();
        }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.InitializeClientSide), true)]
        private void InitializeClientSide(string serverAddress,int port,int sessionKey,int playerIndex)
        {
            if (IsInitialized || GameNetworkServerPatches.IsInitialized)
                throw new InvalidOperationException();
            if (_peer is null)
                throw new ServerPeerIsNotGivenException();
            _missionClient = new MissionClient(_peer);
            IsInitialized = true;
        }
        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.TerminateClientSide), true)]
        private void TerminateClientSide()
        {
            IsInitialized = false;
            _missionClient = null;
            _peer = null;
        }
        [PatchedMethod(typeof(GameNetwork), "HandleNetworkPacketAsClient", true)]
        private bool HandleNetworkPacketAsClient()
        {
            return _missionClient.HandleNetworkPacketAsClient();
        }

    }
}