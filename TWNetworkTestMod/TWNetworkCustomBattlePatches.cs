using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.MissionSpawnHandlers;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;
using TWNetworkPatcher;

namespace TWNetworkTestMod
{
    public class TWNetworkCustomBattlePatches: HarmonyPatches
    {
		public static string SceneID { get; private set; }
		public static string SeasonString { get; private set; }
		public static int TimeOfDay { get; private set; }
		public static string SceneLevels { get; private set; }

		[PatchedMethod(typeof(GameNetwork), "StartMultiplayer", false)]
		private void StartMultiplayer() 
		{
			GameNetwork.AddNetworkComponent<TWNetworkComponent>();
		}

		[PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.EndMultiplayer), false)]
		private void EndMultiplayer()
		{
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<TWNetworkComponent>());
		}


		[PatchedMethod(typeof(CustomBattleMenuVM),nameof(CustomBattleMenuVM.ExecuteStart),false)]
        private void ExecuteStart()
        {
            TWNetworkServer server = new TWNetworkServer();
            server.Start(15801, 2);
            Main.updatable = server;
        }
		[PatchedMethod(typeof(MissionState),"FinishMissionLoading",false)]
		private void FinishMissionLoading()
		{
			MissionState.Current.Paused = true;
		}

		[PatchedMethod(typeof(TeamAIGeneral),nameof(TeamAIGeneral.OnUnitAddedToFormationForTheFirstTime),false)]
        private void OnUnitAddedToFormationForTheFirstTime(Formation formation)
		{
			if (GameNetwork.IsServer)
			{
				formation.AI.AddAiBehavior(new BehaviorAdvance(formation));
				formation.AI.AddAiBehavior(new BehaviorCautiousAdvance(formation));
				formation.AI.AddAiBehavior(new BehaviorCavalryScreen(formation));
				formation.AI.AddAiBehavior(new BehaviorDefend(formation));
				formation.AI.AddAiBehavior(new BehaviorDefensiveRing(formation));
				formation.AI.AddAiBehavior(new BehaviorFireFromInfantryCover(formation));
				formation.AI.AddAiBehavior(new BehaviorFlank(formation));
				formation.AI.AddAiBehavior(new BehaviorHoldHighGround(formation));
				formation.AI.AddAiBehavior(new BehaviorHorseArcherSkirmish(formation));
				formation.AI.AddAiBehavior(new BehaviorMountedSkirmish(formation));
				formation.AI.AddAiBehavior(new BehaviorProtectFlank(formation));
				formation.AI.AddAiBehavior(new BehaviorScreenedSkirmish(formation));
				formation.AI.AddAiBehavior(new BehaviorSkirmish(formation));
				formation.AI.AddAiBehavior(new BehaviorSkirmishBehindFormation(formation));
				formation.AI.AddAiBehavior(new BehaviorSkirmishLine(formation));
				formation.AI.AddAiBehavior(new BehaviorVanguard(formation));
			}
		}

		[PatchedMethod(typeof(BannerlordMissions),nameof(BannerlordMissions.OpenCustomBattleMission),true)]
		private Mission OpenCustomBattleMission(string scene, BasicCharacterObject playerCharacter, CustomBattleCombatant playerParty, CustomBattleCombatant enemyParty, bool isPlayerGeneral, BasicCharacterObject playerSideGeneralCharacter, string sceneLevels = "", string seasonString = "", float timeOfDay = 6f)
		{
			if (GameNetwork.IsSessionActive)
			{
				SceneID = scene;
				SeasonString = seasonString;
				TimeOfDay = (int)timeOfDay;
				SceneLevels = sceneLevels;
			}
			if (GameNetwork.IsServer)
			{
				BattleSideEnum playerSide = playerParty.Side;
				bool isPlayerAttacker = playerSide == BattleSideEnum.Attacker;
				IMissionTroopSupplier[] troopSuppliers = new IMissionTroopSupplier[2];
				CustomBattleTroopSupplier customBattleTroopSupplier = new CustomBattleTroopSupplier(playerParty, true, isPlayerGeneral);
				troopSuppliers[(int)playerParty.Side] = customBattleTroopSupplier;
				CustomBattleTroopSupplier customBattleTroopSupplier2 = new CustomBattleTroopSupplier(enemyParty, false, false);
				troopSuppliers[(int)enemyParty.Side] = customBattleTroopSupplier2;
				bool isPlayerSergeant = !isPlayerGeneral;
				return MissionState.OpenNew("CustomBattle", new MissionInitializerRecord(scene)
				{
					DoNotUseLoadingScreen = false,
					PlayingInCampaignMode = false,
					AtmosphereOnCampaign = CreateAtmosphereInfoForMission(seasonString, (int)timeOfDay),
					SceneLevels = sceneLevels,
					TimeOfDay = timeOfDay,
					AtlasGroup = 2
				}, (Mission missionController) =>
					new MissionBehavior[] 
					{
						new MultiplayerTimerComponent(),
						new MissionAgentSpawnLogic(troopSuppliers, playerSide, MissionAgentSpawnLogic.BattleSizeType.Battle),
						new BattlePowerCalculationLogic(),
						new CustomBattleAgentLogic(),
						new CustomBattleMissionSpawnHandler((!isPlayerAttacker) ? playerParty : enemyParty, isPlayerAttacker ? playerParty : enemyParty),
						new MissionOptionsComponent(),
						new BattleEndLogic(),
						new MissionCombatantsLogic(null, playerParty, (!isPlayerAttacker) ? playerParty : enemyParty, isPlayerAttacker ? playerParty : enemyParty, Mission.MissionTeamAITypeEnum.FieldBattle, isPlayerSergeant),
						new BattleObserverMissionLogic(),
						new AgentHumanAILogic(),
						new AgentVictoryLogic(),
						new MissionAgentPanicHandler(),
						new BattleMissionAgentInteractionLogic(),
						new AgentMoraleInteractionLogic(),
						new AssignPlayerRoleInTeamMissionController(isPlayerGeneral, isPlayerSergeant, false, isPlayerSergeant ? Enumerable.Repeat<string>(playerCharacter.StringId, 1).ToList<string>() : new List<string>(), FormationClass.NumberOfRegularFormations),
						new CreateBodyguardMissionBehavior((isPlayerAttacker & isPlayerGeneral) ? playerCharacter.GetName() : ((isPlayerAttacker & isPlayerSergeant) ? playerSideGeneralCharacter.GetName() : null), (!isPlayerAttacker & isPlayerGeneral) ? playerCharacter.GetName() : ((!isPlayerAttacker & isPlayerSergeant) ? playerSideGeneralCharacter.GetName() : null), null, null, true),
						new EquipmentControllerLeaveLogic(),
						new MissionHardBorderPlacer(),
						new MissionBoundaryPlacer(),
						new MissionBoundaryCrossingHandler(),
						new HighlightsController(),
						new BattleHighlightsController(),
						new DeploymentMissionController(isPlayerAttacker),
						new BattleDeploymentHandler(isPlayerAttacker)
					}, true, true);
			}
			if (GameNetwork.IsClient)
			{
				return MissionState.OpenNew("CustomBattle", new MissionInitializerRecord(scene)
				{
					DoNotUseLoadingScreen = false,
					PlayingInCampaignMode = false,
					AtmosphereOnCampaign = CreateAtmosphereInfoForMission(seasonString, (int)timeOfDay),
					SceneLevels = sceneLevels,
					TimeOfDay = timeOfDay,
					AtlasGroup = 2
				}, (Mission missionController) =>
					new MissionBehavior[]
					{
						new MultiplayerTimerComponent(),
						new MissionHardBorderPlacer(),
						new MissionBoundaryPlacer(),
						new MissionBoundaryCrossingHandler(),
					}, true, true);
			}
			Run = true;
			return null;
	}

		private AtmosphereInfo CreateAtmosphereInfoForMission(string seasonId, int timeOfDay)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add("spring", 0);
			dictionary.Add("summer", 1);
			dictionary.Add("fall", 2);
			dictionary.Add("winter", 3);
			int season = 0;
			dictionary.TryGetValue(seasonId, out season);
			Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
			dictionary2.Add(6, "TOD_06_00_SemiCloudy");
			dictionary2.Add(12, "TOD_12_00_SemiCloudy");
			dictionary2.Add(15, "TOD_04_00_SemiCloudy");
			dictionary2.Add(18, "TOD_03_00_SemiCloudy");
			dictionary2.Add(22, "TOD_01_00_SemiCloudy");
			string atmosphereName = "field_battle";
			dictionary2.TryGetValue(timeOfDay, out atmosphereName);
			return new AtmosphereInfo
			{
				AtmosphereName = atmosphereName,
				TimeInfo = new TimeInformation
				{
					Season = season
				}
			};
		}

	}
}
