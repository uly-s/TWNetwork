using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.MountAndBlade;

namespace TWNetwork
{
    public class Missions
    {
        private ConcurrentDictionary<Guid, ServerMission> missions;
        public int Count => missions.Count;
        public void TickMissions()
        {
            foreach (ServerMission mission in missions.Values)
            {
                mission.TickMission();
            }
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

        public void AddMission(Mission mission)
        {
            if (missions.Values.Any(serverMission => serverMission.Mission == mission))
            {
                throw new MissionAlreadyAddedException();
            }
            missions.TryAdd(Guid.NewGuid(), new ServerMission(mission));
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
        public void RemoveMission(Mission mission)
        {
            if (!missions.ContainsKey(mission.ID()))
            {
                throw new MissionMissingException();
            }
            if (!missions.TryRemove(mission.ID(), out ServerMission m))
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