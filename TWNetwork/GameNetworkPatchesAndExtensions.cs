using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork
{
    public static class GameNetworkExtensions
    {
        private static ConcurrentDictionary<object, NetworkCommunicator> _peers = new ConcurrentDictionary<object, NetworkCommunicator>();
        public static NetworkCommunicator GetTWNetworkPeer(object peer)
        {
            if (!GameNetwork.IsServer)
            {
                throw new InvalidOperationException("This method can only be used by the server!");
            }
            return _peers[peer];
        }

        public static T GetNetworkPeer<T>(NetworkCommunicator communicator) where T : class
        {
            if (!GameNetwork.IsServer)
            {
                throw new InvalidOperationException("This method can only be used by the server!");
            }
            foreach (var key in _peers.Keys)
            {
                if (_peers[key] == communicator)
                {
                    return (T)key;
                }
            }
            return null;
        }
    }
    public static class GameNetworkPatches
    {
        private static IGameNetworkServer Server;
        private static IGameNetworkClient Client;

        public static void InitializeServer(IGameNetworkServer server)
        {
            if (Client is null && Server is null)
            {
                Server = server;
                Client = null;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        public static void InitializeClient(IGameNetworkClient client)
        {
            if (Client is null && Server is null)
            {
                Client = client;
                Server = null;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static void Delete()
        {
            Server = null;
            Client = null;
        }
        [HarmonyPatch(typeof(GameNetwork))]
        [HarmonyPatch("HandleNetworkPacketAsServer")]
        class HandleNetworkPacketAsServerPatch
        {
            private static List<Type> _gameNetworkMessageIdsFromClient = ((List<Type>)typeof(GameNetwork).GetField("_gameNetworkMessageIdsFromClient", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null));
            private static Dictionary<int, List<object>> _fromClientMessageHandlers = ((Dictionary<int, List<object>>)typeof(GameNetwork).GetField("_fromClientMessageHandlers", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null));
            private bool Prefix(NetworkCommunicator networkPeer, bool __result)
            {
                if (networkPeer == null)
                {
                    MBDebug.Print("networkPeer == null", 0, Debug.DebugColor.White, 17592186044416UL);
                    __result = false;
                }
                else
                {
                    var msg = networkPeer.PopMessage();
                    bool flag = true;
                    if (msg.MessageId >= 0 && msg.MessageId < _gameNetworkMessageIdsFromClient.Count)
                    {
                        while (!(msg is null))
                        {
                            List<object> list;
                            if (_fromClientMessageHandlers.TryGetValue(msg.MessageId, out list))
                            {
                                foreach (object obj in list)
                                {
                                    Delegate method = obj as Delegate;
                                    flag = flag && ((bool)method.DynamicInvokeWithLog(new object[]
                                    {
                                networkPeer,
                                msg
                                    }));
                                    if (!flag)
                                    {
                                        break;
                                    }
                                }
                                if (list.Count == 0)
                                {
                                    flag = false;
                                }
                            }
                            else
                            {
                                flag = false;
                            }
                            msg = networkPeer.PopMessage();
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                    __result = flag;
                }
                return false;
            }
        }
        [HarmonyPatch(typeof(GameNetwork),"IsClient",MethodType.Getter)]
        class IsClientPatch 
        {
            private static bool Prefix(bool __result)
            {
                __result = !(Client is null);
                return false;
            }
        }
        [HarmonyPatch(typeof(GameNetwork), "IsServer", MethodType.Getter)]
        class IsServerPatch
        {
            private static bool Prefix(bool __result)
            {
                __result = !(Server is null);
                return false;
            }
        }

        [HarmonyPatch(typeof(GameNetworkMessage))]
        [HarmonyPatch("Write")]
        class WriteMessagePatch
        {
            private static bool Prefix(GameNetworkMessage __instance)
            {
                if (GameNetwork.IsServer)
                {
                    Server.MessageToSend = __instance;
                }
                if (GameNetwork.IsClient)
                {
                    Client.MessageToSend = __instance;
                }
                return false;
            }
        }
    }

}
