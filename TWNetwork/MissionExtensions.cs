using HarmonyLib;
using System;
using System.Collections.Concurrent;
using TaleWorlds.MountAndBlade;

namespace TWNetwork
{
    public static class MissionExtensions
    {
        private static ConcurrentDictionary<Mission, Guid> IDs = new ConcurrentDictionary<Mission, Guid>();
        public static Guid ID(this Mission mission)
        {
            return IDs[mission];
        }
        public static Mission CreateMission()
        {
            
        }
    }
}
