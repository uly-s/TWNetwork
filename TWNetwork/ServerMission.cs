using System;
using TaleWorlds.MountAndBlade;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
namespace TWNetwork
{
		public class ServerMission: GameState
		{
			public IMissionSystemHandler Handler { get; set; }
			public Mission Mission { get; private set; }
			public string MissionName { get; private set; }
			public bool Paused { get; set; }
			protected override void OnFinalize()
			{
				this.Mission.ClearResources(this.Mission.NeedsMemoryCleanup);
				this.Mission = null;
			}
			protected override void OnActivate()
			{
				Mission currentMission = this.Mission;
				if (((currentMission != null) ? currentMission.MissionBehaviors : null) == null)
				{
					return;
				}
				foreach (MissionBehavior missionBehavior in this.Mission.MissionBehaviors)
				{
					missionBehavior.OnMissionActivate();
				}
			}
			protected override void OnDeactivate()
			{
				if (this.Mission == null || this.Mission.MissionBehaviors == null)
				{
					return;
				}
				foreach (MissionBehavior missionBehavior in this.Mission.MissionBehaviors)
				{
					missionBehavior.OnMissionDeactivate();
				}
			}

			protected override void OnIdleTick(float dt)
			{
				if (this.Mission != null && this.Mission.CurrentState == Mission.State.Continuing)
				{
					this.Mission.IdleTick(dt);
				}
			}

			protected override void OnTick(float realDt)
			{
				if (this.Mission.CurrentState == Mission.State.NewlyCreated || this.Mission.CurrentState == Mission.State.Initializing)
				{
					if (this.Mission.CurrentState == Mission.State.NewlyCreated)
					{
						this.Mission.ClearUnreferencedResources(this.Mission.NeedsMemoryCleanup);
					}
					this.TickLoading(realDt);
				}
				else if (this.Mission.CurrentState == Mission.State.Continuing || this.Mission.MissionEnded)
				{
					if (this.MissionFastForwardAmount != 0f)
					{
						this.Mission.FastForwardMission(this.MissionFastForwardAmount, 0.033f);
						this.MissionFastForwardAmount = 0f;
					}
					bool flag = false;
					if (this.MissionEndTime != 0f && this.Mission.CurrentTime > this.MissionEndTime)
					{
						this.Mission.EndMission();
						flag = true;
					}
					if (!flag && (this.Handler == null || this.Handler.RenderIsReady()))
					{
						this.TickMission(realDt);
					}
					if (flag && MBEditor._isEditorMissionOn)
					{
						MBEditor.LeaveEditMissionMode();
						this.TickMission(realDt);
					}
				}
				if (this.Mission.CurrentState == Mission.State.Over)
				{
					if (MBGameManager.Current.IsEnding)
					{
						Game.Current.GameStateManager.CleanStates(0);
						return;
					}
					Game.Current.GameStateManager.PopState(0);
				}
			}

			// Token: 0x06001752 RID: 5970 RVA: 0x00056E60 File Offset: 0x00055060
			private void TickMission(float realDt)
			{
				if (this._firstMissionTickAfterLoading && this.Mission != null && this.Mission.CurrentState == Mission.State.Continuing)
				{
					if (GameNetwork.IsClient)
					{
						MBDebug.Print("Client: I finished loading. Sending confirmation to server.", 0, Debug.DebugColor.White, 17179869184UL);
						GameNetwork.BeginModuleEventAsClient();
						GameNetwork.WriteMessage(new FinishedLoading());
						GameNetwork.EndModuleEventAsClient();
						GameNetwork.SyncRelevantGameOptionsToServer();
					}
					this._firstMissionTickAfterLoading = false;
				}
				if (Game.Current.DeterministicMode)
				{
					Game.Current.ResetRandomGenerator(this._missionTickCount);
				}
				IMissionSystemHandler handler = this.Handler;
				if (handler != null)
				{
					handler.BeforeMissionTick(this.Mission, realDt);
				}
				this.Mission.PauseAITick = false;
				if (GameNetwork.IsSessionActive && this.Mission.ClearSceneTimerElapsedTime < 0f)
				{
					this.Mission.PauseAITick = true;
				}
				float num = realDt;
				if (this.Paused || MBCommon.IsPaused)
				{
					num = 0f;
				}
				else if (this.Mission.FixedDeltaTimeMode)
				{
					num = this.Mission.FixedDeltaTime;
				}
				if (!GameNetwork.IsSessionActive)
				{
					this.Mission.UpdateSceneTimeSpeed();
					float timeSpeed = this.Mission.Scene.TimeSpeed;
					num *= timeSpeed;
				}
				if (this.Mission.ClearSceneTimerElapsedTime < -0.3f && !GameNetwork.IsClientOrReplay)
				{
					this.Mission.ClearAgentActions();
				}
				if (this.Mission.CurrentState == Mission.State.Continuing || this.Mission.MissionEnded)
				{
					if (this.Mission.IsFastForward)
					{
						float num2 = num * 9f;
						while (num2 > 1E-06f)
						{
							if (num2 > 0.1f)
							{
								this.TickMissionAux(0.1f, 0.1f, false);
								if (this.Mission.CurrentState == Mission.State.Over)
								{
									break;
								}
								num2 -= 0.1f;
							}
							else
							{
								if (num2 > 0.0033333334f)
								{
									this.TickMissionAux(num2, num2, false);
								}
								num2 = 0f;
							}
						}
						if (this.Mission.CurrentState != Mission.State.Over)
						{
							this.TickMissionAux(num, realDt, true);
						}
					}
					else
					{
						this.TickMissionAux(num, realDt, true);
					}
				}
				if (this.Handler != null)
				{
					this.Handler.AfterMissionTick(this.Mission, realDt);
				}
				this._missionTickCount++;
				bool deterministicMode = Game.Current.DeterministicMode;
			}

			// Token: 0x06001753 RID: 5971 RVA: 0x00057087 File Offset: 0x00055287
			private void TickMissionAux(float dt, float realDt, bool updateCamera)
			{
				this.Mission.Tick(dt);
				if (this._missionTickCount > 2)
				{
					this.Mission.OnTick(dt, realDt, updateCamera);
				}
			}

			// Token: 0x06001754 RID: 5972 RVA: 0x000570AC File Offset: 0x000552AC
			private void TickLoading(float realDt)
			{
				this._tickCountBeforeLoad++;
				if (!this._missionInitializing && this._tickCountBeforeLoad > 0)
				{
					this.LoadMission();
					Utilities.SetLoadingScreenPercentage(0.01f);
					return;
				}
				if (this._missionInitializing && this.Mission.IsLoadingFinished)
				{
					this.FinishMissionLoading();
				}
			}

			// Token: 0x06001755 RID: 5973 RVA: 0x00057104 File Offset: 0x00055304
			private void LoadMission()
			{
				foreach (MissionBehavior missionBehavior in this.Mission.MissionBehaviors)
				{
					missionBehavior.OnMissionScreenPreLoad();
				}
				Utilities.ClearOldResourcesAndObjects();
				this._missionInitializing = true;
				this.Mission.Initialize();
			}

			// Token: 0x06001756 RID: 5974 RVA: 0x00057170 File Offset: 0x00055370
			private void CreateMission(MissionInitializerRecord rec)
			{
				this.Mission = new Mission(rec, this);
			}

			// Token: 0x06001757 RID: 5975 RVA: 0x00057180 File Offset: 0x00055380
			private Mission HandleOpenNew(string missionName, MissionInitializerRecord rec, InitializeMissionBehaviorsDelegate handler, bool addDefaultMissionBehaviors)
			{
				this.MissionName = missionName;
				this.CreateMission(rec);
				IEnumerable<MissionBehavior> enumerable = handler(this.Mission);
				if (addDefaultMissionBehaviors)
				{
					enumerable = MissionState.AddDefaultMissionBehaviorsTo(this.Mission, enumerable);
				}
				foreach (MissionBehavior missionBehavior in enumerable)
				{
					missionBehavior.OnAfterMissionCreated();
				}
				this.AddBehaviorsToMission(enumerable);
				if (this.Handler != null)
				{
					enumerable = new MissionBehavior[0];
					enumerable = this.Handler.OnAddBehaviors(enumerable, this.Mission, missionName, addDefaultMissionBehaviors);
					this.AddBehaviorsToMission(enumerable);
				}
				return this.Mission;
			}

			// Token: 0x06001758 RID: 5976 RVA: 0x0005722C File Offset: 0x0005542C
			private void AddBehaviorsToMission(IEnumerable<MissionBehavior> behaviors)
			{
				MissionLogic[] logicBehaviors = (from behavior in behaviors.OfType<MissionLogic>()
												 where !(behavior is MissionNetwork)
												 select behavior).ToArray<MissionLogic>();
				MissionBehavior[] otherBehaviors = (from behavior in behaviors
													where behavior != null && !(behavior is MissionNetwork) && !(behavior is MissionLogic)
													select behavior).ToArray<MissionBehavior>();
				MissionNetwork[] networkBehaviors = behaviors.OfType<MissionNetwork>().ToArray<MissionNetwork>();
				this.Mission.InitializeStartingBehaviors(logicBehaviors, otherBehaviors, networkBehaviors);
			}

			// Token: 0x06001759 RID: 5977 RVA: 0x000572AE File Offset: 0x000554AE
			private static bool IsRecordingActive()
			{
				if (GameNetwork.IsServer)
				{
					return MultiplayerOptions.OptionType.EnableMissionRecording.GetBoolValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
				return MissionState.RecordMission && Game.Current.GameType.IsCoreOnlyGameMode;
			}

			// Token: 0x0600175A RID: 5978 RVA: 0x000572D8 File Offset: 0x000554D8
			public static Mission OpenNew(string missionName, MissionInitializerRecord rec, InitializeMissionBehaviorsDelegate handler, bool addDefaultMissionBehaviors = true, bool needsMemoryCleanup = true)
			{
				if (!GameNetwork.IsClientOrReplay && !GameNetwork.IsServer)
				{
					MBCommon.CurrentGameType = (MissionState.IsRecordingActive() ? MBCommon.GameType.SingleRecord : MBCommon.GameType.Single);
				}
				Game.Current.OnMissionIsStarting(missionName, rec);
				MissionState missionState = Game.Current.GameStateManager.CreateState<MissionState>();
				Mission mission = missionState.HandleOpenNew(missionName, rec, handler, addDefaultMissionBehaviors);
				Game.Current.GameStateManager.PushState(missionState, 0);
				mission.NeedsMemoryCleanup = needsMemoryCleanup;
				return mission;
			}

			// Token: 0x0600175B RID: 5979 RVA: 0x00057344 File Offset: 0x00055544
			private static IEnumerable<MissionBehavior> AddDefaultMissionBehaviorsTo(Mission mission, IEnumerable<MissionBehavior> behaviors)
			{
				List<MissionBehavior> list = new List<MissionBehavior>();
				if (GameNetwork.IsSessionActive || GameNetwork.IsReplay)
				{
					list.Add(new MissionNetworkComponent());
				}
				if (MissionState.IsRecordingActive() && !GameNetwork.IsReplay)
				{
					list.Add(new RecordMissionLogic());
				}
				list.Add(new BasicMissionHandler());
				list.Add(new CasualtyHandler());
				list.Add(new AgentCommonAILogic());
				list.Add(new MissionGamepadHapticEffectsHandler());
				return list.Concat(behaviors);
			}

			// Token: 0x0600175C RID: 5980 RVA: 0x000573BC File Offset: 0x000555BC
			private void FinishMissionLoading()
			{
				this._missionInitializing = false;
				this.Mission.Scene.SetOwnerThread();
				Utilities.SetLoadingScreenPercentage(0.4f);
				for (int i = 0; i < 2; i++)
				{
					this.Mission.Tick(0.001f);
				}
				Utilities.SetLoadingScreenPercentage(0.42f);
				IMissionSystemHandler handler = this.Handler;
				if (handler != null)
				{
					handler.OnMissionAfterStarting(this.Mission);
				}
				Utilities.SetLoadingScreenPercentage(0.48f);
				this.Mission.AfterStart();
				Utilities.SetLoadingScreenPercentage(0.56f);
				IMissionSystemHandler handler2 = this.Handler;
				if (handler2 != null)
				{
					handler2.OnMissionLoadingFinished(this.Mission);
				}
				Utilities.SetLoadingScreenPercentage(0.62f);
				this.Mission.Scene.ResumeLoadingRenderings();
			}

			// Token: 0x04000934 RID: 2356
			private const int MissionFastForwardSpeedMultiplier = 10;

			// Token: 0x04000935 RID: 2357
			private bool _missionInitializing;

			// Token: 0x04000936 RID: 2358
			private bool _firstMissionTickAfterLoading = true;

			// Token: 0x04000937 RID: 2359
			private int _tickCountBeforeLoad;

			// Token: 0x04000938 RID: 2360
			public static bool RecordMission;

			// Token: 0x0400093A RID: 2362
			public float MissionFastForwardAmount;

			// Token: 0x0400093B RID: 2363
			public float MissionEndTime;

			// Token: 0x04000940 RID: 2368
			private int _missionTickCount;
		}
}