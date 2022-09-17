using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;
using TaleWorlds.PlatformService;
using TWNetwork.Patches;

namespace TWNetworkTestMod
{
	

	public class TWNetworkGameManager : MBGameManager
	{
		public readonly bool IsServer;
		public TWNetworkGameManager(bool isServer)
		{
			IsServer = isServer;
		}

		public static TWNetworkGameManager CurrentManager => Current as TWNetworkGameManager;

		protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
		{
			nextStep = GameManagerLoadingSteps.None;
			switch (gameManagerLoadingStep)
			{
				case GameManagerLoadingSteps.PreInitializeZerothStep:
					MBGameManager.LoadModuleData(false);
					MBGlobals.InitializeReferences();
					Game.CreateGame(new CustomGame(), this).DoLoading();
					nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
					return;
				case GameManagerLoadingSteps.FirstInitializeFirstStep:
					{
						bool flag = true;
						foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
						{
							flag = (flag && mbsubModuleBase.DoLoading(Game.Current));
						}
						nextStep = (flag ? GameManagerLoadingSteps.WaitSecondStep : GameManagerLoadingSteps.FirstInitializeFirstStep);
						return;
					}
				case GameManagerLoadingSteps.WaitSecondStep:
					MBGameManager.StartNewGame();
					nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
					return;
				case GameManagerLoadingSteps.SecondInitializeThirdState:
					nextStep = (Game.Current.DoLoading() ? GameManagerLoadingSteps.PostInitializeFourthState : GameManagerLoadingSteps.SecondInitializeThirdState);
					return;
				case GameManagerLoadingSteps.PostInitializeFourthState:
					nextStep = GameManagerLoadingSteps.FinishLoadingFifthStep;
					return;
				case GameManagerLoadingSteps.FinishLoadingFifthStep:
					nextStep = GameManagerLoadingSteps.None;
					return;
				default:
					return;
			}
		}

		public override void OnLoadFinished()
		{
			base.OnLoadFinished();
			if (IsServer)
			{
				TWNetworkServer s = new TWNetworkServer();
                IMBNetwork.Server = s;
				GameNetwork.StartMultiplayerOnServer(15801);
				Main.updatable = s;
				BannerlordMissions.OpenCustomBattleMission("battle_terrain_001",null,null,null,false,null);
			}
			else
			{
				TWNetworkClient client = new TWNetworkClient();
				IMBNetwork.Client = client; 
				GameNetwork.StartMultiplayerOnClient("127.0.0.1", 15801,1,1);
				Main.updatable = client;
			}
		}

		public override void OnAfterCampaignStart(Game game)
		{
			GameNetwork.InitializeCompressionInfos();
			GameNetwork.Initialize(new TWNetworkHandler());
		}
	}
}
