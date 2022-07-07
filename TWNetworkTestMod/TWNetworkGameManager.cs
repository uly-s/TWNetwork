using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.PlatformService;

namespace TWNetworkTestMod
{
	public class TWNetworkGameManager : MBGameManager
	{
		private readonly bool IsServer;
		public TWNetworkGameManager(bool isServer)
		{
			IsServer = isServer;
			MBMusicManager mbmusicManager = MBMusicManager.Current;
			if (mbmusicManager == null)
			{
				return;
			}
			mbmusicManager.PauseMusicManagerSystem();
		}

		protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
		{
			nextStep = GameManagerLoadingSteps.None;
			switch (gameManagerLoadingStep)
			{
				case GameManagerLoadingSteps.PreInitializeZerothStep:
					nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
					return;
				case GameManagerLoadingSteps.FirstInitializeFirstStep:
					MBGameManager.LoadModuleData(false);
					MBDebug.Print("Game creating...", 0, Debug.DebugColor.White, 17592186044416UL);
					MBGlobals.InitializeReferences();
					Game.CreateGame(new MultiplayerGame(), this).DoLoading();
					nextStep = GameManagerLoadingSteps.WaitSecondStep;
					return;
				case GameManagerLoadingSteps.WaitSecondStep:
					MBGameManager.StartNewGame();
					nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
					return;
				case GameManagerLoadingSteps.SecondInitializeThirdState:
					nextStep = (Game.Current.DoLoading() ? GameManagerLoadingSteps.PostInitializeFourthState : GameManagerLoadingSteps.SecondInitializeThirdState);
					return;
				case GameManagerLoadingSteps.PostInitializeFourthState:
					{
						bool flag = true;
						foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
						{
							flag = (flag && mbsubModuleBase.DoLoading(Game.Current));
						}
						nextStep = (flag ? GameManagerLoadingSteps.FinishLoadingFifthStep : GameManagerLoadingSteps.PostInitializeFourthState);
						return;
					}
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
			MBGlobals.InitializeReferences();
			if (IsServer)
			{
				TWNetworkServer server = new TWNetworkServer();
				server.Start(15801, 2);
				Main.updatable = server;
				Module.CurrentModule.StartMultiplayerGame("CustomBattleMission", "mp_skirmish_spawn_test");
			}
			else
			{
				TWNetworkClient client = new TWNetworkClient();
				client.Start("127.0.0.1", 15801);
				Main.updatable = client;
			}
		}

		public override void OnAfterCampaignStart(Game game)
		{
			if (GameNetwork.IsDedicatedServer)
			{
				NetworkMain.InitializeAsDedicatedServer();
				return;
			}
			NetworkMain.Initialize();
		}

		public override void OnNewCampaignStart(Game game, object starterObject)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnMultiplayerGameStart(game, starterObject);
			}
		}

		public override void OnSessionInvitationAccepted(SessionInvitationType sessionInvitationType)
		{
			if (sessionInvitationType == SessionInvitationType.Multiplayer)
			{
				return;
			}
			base.OnSessionInvitationAccepted(sessionInvitationType);
		}
	}
}
