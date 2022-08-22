using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.MissionSpawnHandlers;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;
using TaleWorlds.MountAndBlade.View;
using TWNetworkPatcher;

namespace TWNetworkTestMod
{
    public class TWNetworkCustomBattlePatches: HarmonyPatches
    {
		public static string SceneID { get; private set; }

		

		[PatchedMethod(typeof(BannerlordMissions),nameof(BannerlordMissions.OpenCustomBattleMission),true)]
		private Mission OpenCustomBattleMission(string scene, BasicCharacterObject playerCharacter, CustomBattleCombatant playerParty, CustomBattleCombatant enemyParty, bool isPlayerGeneral, BasicCharacterObject playerSideGeneralCharacter, string sceneLevels = "", string seasonString = "", float timeOfDay = 6f)
		{
			if (GameNetwork.IsSessionActive)
			{
				SceneID = scene;
			}
			SimpleSceneTestWithMission SimpleScene = new SimpleSceneTestWithMission(scene);
			return (Mission)typeof(SimpleSceneTestWithMission).GetField("_mission",BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(SimpleScene);
	}
	}
}
