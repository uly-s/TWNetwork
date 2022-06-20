using HarmonyLib;
using System;
using System.Collections.Concurrent;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork
{
    [HarmonyPatch(typeof(NetworkCommunicator),MethodType.Constructor)]
    public static class NetworkCommunicatorPatchAndExtensions
    {
        private static ConcurrentDictionary<NetworkCommunicator, ConcurrentQueue<GameNetworkMessage>> Messages = new ConcurrentDictionary<NetworkCommunicator, ConcurrentQueue<GameNetworkMessage>>();
        
        public static GameNetworkMessage PopMessage(this NetworkCommunicator communicator)
        {
            GameNetworkMessage message;
            if(!Messages[communicator].TryDequeue(out message))
            {
                return null;   
            }
            return message;
        }

        public static void PushMessage(this NetworkCommunicator communicator,GameNetworkMessage message)
        {
            if (!Messages.ContainsKey(communicator))
            {
                throw new InvalidOperationException("NetworkCommunicator is not added to the Dictionary.");
            }
            Messages[communicator].Enqueue(message);
        }
        private static void Postfix(NetworkCommunicator __instance)
        {
            Messages.TryAdd(__instance,new ConcurrentQueue<GameNetworkMessage>());
        }
    }
}
