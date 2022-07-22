using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;
using TWNetworkPatcher;

namespace TWNetworkTestMod
{
    public class TWNetworkCustomBattlePatches: HarmonyPatches
    {
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
}
