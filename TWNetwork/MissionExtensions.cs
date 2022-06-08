using HarmonyLib;
using System;
using System.Collections.Concurrent;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TWNetwork
{
    [HarmonyPatch(typeof(Mission),MethodType.Constructor, new Type[] { typeof(MissionInitializerRecord), typeof(MissionState) })]
    public static class MissionExtensions
    {
        private static ConcurrentDictionary<Mission, Guid> IDs = new ConcurrentDictionary<Mission, Guid>();
        public static Guid ID(this Mission mission)
        {
            return IDs[mission];
        }

        public static void Postfix(Mission __instance)
        {
            if (GameNetwork.IsServer)
            {
                if (!IDs.TryAdd(__instance, Guid.NewGuid()))
                {
                    throw new MissionIDNotAddedException();
                }
            }
        }
        
    }
}
