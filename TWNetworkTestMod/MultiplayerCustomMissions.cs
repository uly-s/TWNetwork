using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TWNetworkTestMod
{ 
    [MissionManager]
    public static class MultiplayerCustomMissions
    {
		[MissionMethod]
		public static void OpenTestMission(string scene)
		{
			MissionState.OpenNew("MultiplayerTestMission", new MissionInitializerRecord(scene), delegate (Mission missionController)
			{
				if (GameNetwork.IsServer)
				{
					return new MissionBehavior[]
					{
					};
				}
				return new MissionBehavior[]
				{
				};
			}, true, true);
		}
	}
}
