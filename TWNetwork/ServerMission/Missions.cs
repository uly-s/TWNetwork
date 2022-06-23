using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TWNetwork
{
    public class Missions
    {
        private ConcurrentDictionary<Guid, ServerMission> missions = new ConcurrentDictionary<Guid, ServerMission>();
        public int Count => missions.Count;
        public readonly int Capacity;
        public void TickMissions(float realDt)
        {
            foreach (ServerMission mission in missions.Values)
            {
                mission.OnTick(realDt);
            }
        }
        public Missions(int capacity)
        {
            Capacity = capacity;
        }
        public Mission this[Guid id] 
        {
            get 
            {
                if (!missions.ContainsKey(id))
                {
                    throw new MissionMissingException();
                }
                return missions[id].Mission; 
            }
        }

        public Mission GetMission(Guid id)
        {
            if (!missions.ContainsKey(id))
            {
                throw new MissionMissingException();
            }
            return missions[id].Mission;
        }
        public Mission OpenNew(string missionName, MissionInitializerRecord rec, InitializeMissionBehaviorsDelegate handler, bool addDefaultMissionBehaviors = true, bool needsMemoryCleanup = true)
        {
            ServerMission serverMission = ServerMission.OpenNew(missionName, rec, handler, addDefaultMissionBehaviors, needsMemoryCleanup);
            AddMission(serverMission);
            return serverMission.Mission;
        }
        private void AddMission(ServerMission mission)
        {
            if (missions.Values.Any(serverMission => serverMission == mission))
            {
                throw new MissionAlreadyAddedException();
            }
            missions.TryAdd(mission.ID,mission);
        }

        public void RemoveMission(Guid id)
        {
            if (!missions.ContainsKey(id))
            {
                throw new MissionMissingException();
            }
            if (!missions.TryRemove(id, out ServerMission m))
            {
                throw new MissionNotRemovedException();
            }
        }
        public void RemoveMission(ServerMission mission)
        {
            if (!missions.ContainsKey(mission.ID))
            {
                throw new MissionMissingException();
            }
            if (!missions.TryRemove(mission.ID, out ServerMission m))
            {
                throw new MissionNotRemovedException();
            }
        }

        public void Clear()
        {
            missions.Clear();
        }
    }
}