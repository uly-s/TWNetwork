﻿using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork
{
    public static class GameNetworkExtensions
    {
        private static ConcurrentDictionary<INetworkPeer, NetworkCommunicator> _peers = new ConcurrentDictionary<INetworkPeer, NetworkCommunicator>();
        public static NetworkCommunicator GetTWNetworkPeer(INetworkPeer peer)
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
    [HarmonyPatch]
    public static class GameNetworkPatches
    {
        private static GameNetworkServerBase Server;
        private static GameNetworkClient Client;
        private static Type GameNetworkType = typeof(GameNetwork);
        private static Type ServerType = typeof(GameNetworkServerBase);
        private static Type ClientType = typeof(GameNetworkClient);
        private static Dictionary<MethodBase,MethodInfo> methods = new Dictionary<MethodBase,MethodInfo>();

        private static MethodBase GetGameNetworkMethod(string name)
        {
            return GameNetworkType.GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static MethodBase GetGameNetworkMethod(string name,Type [] argstypes)
        {
            return GameNetworkType.GetMethod(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,null, argstypes,null);
        }

        private static MethodInfo GetServerMethod(string name)
        {
            return ServerType.GetMethod(name);
        }

        private static MethodInfo GetServerMethod(string name, Type[] argstypes)
        {
            return ServerType.GetMethod(name,argstypes);
        }

        private static MethodInfo GetClientMethod(string name)
        {
            return ClientType.GetMethod(name);
        }

        private static MethodInfo GetClientMethod(string name, Type[] argstypes)
        {
            return ClientType.GetMethod(name,argstypes,);
        }

        static GameNetworkPatches()
        {
            //Server Side Methods
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.BeginBroadcastModuleEvent)),GetServerMethod(nameof(GameNetworkServerBase.BeginBroadcastModuleEvent)));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.BeginModuleEventAsServerUnreliable),new Type[] { typeof(NetworkCommunicator)}), GetServerMethod(nameof(GameNetworkServerBase.BeginModuleEventAsServerUnreliable),new Type[] { typeof(NetworkCommunicator)}));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.BeginModuleEventAsServerUnreliable),new Type[] { typeof(VirtualPlayer)}), GetServerMethod(nameof(GameNetworkServerBase.BeginModuleEventAsServerUnreliable),new Type[] { typeof(VirtualPlayer)}));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.EndBroadcastModuleEventUnreliable)), GetServerMethod(nameof(GameNetworkServerBase.EndBroadcastModuleEventUnreliable)));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.EndBroadcastModuleEvent)), GetServerMethod(nameof(GameNetworkServerBase.EndBroadcastModuleEvent)));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.BeginModuleEventAsServer),new Type[] { typeof(NetworkCommunicator)}), GetServerMethod(nameof(GameNetworkServerBase.BeginModuleEventAsServer),new Type[] { typeof(NetworkCommunicator)}));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.BeginModuleEventAsServer),new Type[] { typeof(VirtualPlayer)}), GetServerMethod(nameof(GameNetworkServerBase.BeginModuleEventAsServer),new Type[] { typeof(VirtualPlayer)}));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.EndModuleEventAsServer)), GetServerMethod(nameof(GameNetworkServerBase.EndModuleEventAsServer)));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.EndModuleEventAsServerUnreliable)), GetServerMethod(nameof(GameNetworkServerBase.EndModuleEventAsServerUnreliable)));
            methods.Add(GetGameNetworkMethod("InitializeServerSide"), ServerType.GetMethod(nameof(GameNetworkServerBase.InitializeServerSide)));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.AddNewPlayerOnServer)), GetServerMethod(nameof(GameNetworkServerBase.AddNewPlayerOnServer)));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.AddNewPlayersOnServer)), GetServerMethod(nameof(GameNetworkServerBase.AddNewPlayersOnServer)));
            methods.Add(GetGameNetworkMethod("AddPeerToDisconnect"), GetServerMethod(nameof(GameNetworkServerBase.AddPeerToDisconnect)));
            methods.Add(GetGameNetworkMethod("TerminateServerSide"), GetServerMethod(nameof(GameNetworkServerBase.TerminateServerSide)));
            methods.Add(GetGameNetworkMethod("HandleNetworkPacketAsServer"), GetServerMethod(nameof(GameNetworkServerBase.HandleNetworkPacketAsServer)));


            //Client Side Methods
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.BeginModuleEventAsClient)), GetClientMethod(nameof(GameNetworkClient.BeginModuleEventAsClient)));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.BeginModuleEventAsClientUnreliable)), GetClientMethod(nameof(GameNetworkClient.BeginModuleEventAsClientUnreliable)));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.EndModuleEventAsClient)), GetClientMethod(nameof(GameNetworkClient.EndModuleEventAsClient)));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.EndModuleEventAsClientUnreliable)), GetClientMethod(nameof(GameNetworkClient.EndModuleEventAsClientUnreliable)));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.InitializeClientSide)), GetClientMethod(nameof(GameNetworkClient.InitializeClientSide)));
            methods.Add(GetGameNetworkMethod(nameof(GameNetwork.TerminateClientSide)), GetClientMethod(nameof(GameNetworkClient.TerminateClientSide)));
        }
        static IEnumerable<MethodBase> TargetMethods()
        {
            return methods.Keys;
        }

        static bool Prefix(object[] __args, MethodBase __originalMethod)
        {
            var Method = methods[__originalMethod];
            Method.Invoke(((Method.DeclaringType == ServerType)?(object)Server:(object)Client), __args);
            return false;
        }


        //TODO: Make the IGameNetworkServer and IGameNetworkClient a class, which will be able to be used by a server-client system.
        public static void InitializeServer(GameNetworkServerBase server)
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
        public static void InitializeClient(GameNetworkClient client)
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
        [HarmonyPatch]
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
