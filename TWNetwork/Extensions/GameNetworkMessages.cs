using HarmonyLib;
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork
{
    [ProtoContract]
    public class GameNetworkMessages
    {
        [ProtoMember(1)]
        private List<GameNetworkMessage> Messages;

        public GameNetworkMessages() 
        {
            Messages = new List<GameNetworkMessage>();
        }
        
        public bool PopMessage(out GameNetworkMessage message)
        {
            message = null;
            lock(Messages)
            {
                if (Messages.Count == 0)
                    return false;
                message = Messages[0];
                Messages.RemoveAt(0);
                return true;
            }
        }

        public void PushMessage(GameNetworkMessage message)
        {
            lock (Messages)
            {
                Messages.Add(message);
            }
        }
    }
}
