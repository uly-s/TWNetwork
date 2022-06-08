using System;
using TaleWorlds.MountAndBlade;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using System.Reflection;

namespace TWNetwork
{
		public class ServerMission
		{
			public Mission Mission { get; private set; }
			public string MissionName { get; private set; }
			public bool Paused { get; set; }

			public Guid ID => (!(Mission is null))?Mission.ID():Guid.Empty;

			private MethodInfo IdleTick = typeof(Mission).GetMethod("IdleTick",BindingFlags.NonPublic | BindingFlags.Instance);
			private MethodInfo SyncRelevantGameOptionsToServer = typeof(GameNetwork).GetMethod("SyncRelevantGameOptionsToServer", BindingFlags.Static | BindingFlags.NonPublic);
			private MethodInfo UpdateSceneTimeSpeed = typeof(Mission).GetMethod("UpdateSceneTimeSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
			private MethodInfo Tick = typeof(Mission).GetMethod("Tick", BindingFlags.NonPublic | BindingFlags.Instance);
			private static PropertyInfo NeedsMemoryCleanUp = typeof(Mission).GetProperty("NeedsMemoryCleanup");
			public void OnFinalize()
			{
				this.Mission.ClearResources(this.Mission.NeedsMemoryCleanup);
				this.Mission = null;
			}
			public void OnActivate()
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

			public void OnDeactivate()
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
			
			public void OnIdleTick(float dt)
			{
				if (this.Mission != null && this.Mission.CurrentState == Mission.State.Continuing)
				{
					IdleTick.Invoke(Mission, new object[] { dt });
				}
			}

			public void OnTick(float realDt)
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
					if (!flag)
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
						SyncRelevantGameOptionsToServer.Invoke(null, new object[] { });
					}
					this._firstMissionTickAfterLoading = false;
				}
				if (Game.Current.DeterministicMode)
				{
					Game.Current.ResetRandomGenerator(this._missionTickCount);
				}
				this.Mission.PauseAITick = false;
				if (GameNetwork.IsSessionActive && this.Mission.ClearSceneTimerElapsedTime < 0f)
				{
					this.Mission.PauseAITick = true;
				}
				float num = realDt;
				if (this.Paused)
				{
					num = 0f;
				}
				else if (this.Mission.FixedDeltaTimeMode)
				{
					num = this.Mission.FixedDeltaTime;
				}
				if (!GameNetwork.IsSessionActive)
				{
					UpdateSceneTimeSpeed.Invoke(Mission,new object[] { });
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
				this._missionTickCount++;
				bool deterministicMode = Game.Current.DeterministicMode;
			}
			private void TickMissionAux(float dt, float realDt, bool updateCamera)
			{
				Tick.Invoke(Mission,new object[] { dt });
				if (this._missionTickCount > 2)
				{
					this.Mission.OnTick(dt, realDt, updateCamera);
				}
			}
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

			private void CreateMission(MissionInitializerRecord rec)
			{
				this.Mission = new Mission(rec, null);
			}

			private Mission HandleOpenNew(string missionName, MissionInitializerRecord rec, InitializeMissionBehaviorsDelegate handler, bool addDefaultMissionBehaviors)
			{
				this.MissionName = missionName;
				this.CreateMission(rec);
				IEnumerable<MissionBehavior> enumerable = handler(this.Mission);
				if (addDefaultMissionBehaviors)
				{
					enumerable = ServerMission.AddDefaultMissionBehaviorsTo(this.Mission, enumerable);
				}
				foreach (MissionBehavior missionBehavior in enumerable)
				{
					missionBehavior.OnAfterMissionCreated();
				}
			this.AddBehaviorsToMission(enumerable);
				return this.Mission;
			}
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
			private static bool IsRecordingActive()
			{
				if (GameNetwork.IsServer)
				{
					return MultiplayerOptions.OptionType.EnableMissionRecording.GetBoolValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
				return MissionState.RecordMission && Game.Current.GameType.IsCoreOnlyGameMode;
			}

			public static ServerMission OpenNew(string missionName, MissionInitializerRecord rec, InitializeMissionBehaviorsDelegate handler, bool addDefaultMissionBehaviors = true, bool needsMemoryCleanup = true)
			{
				if (!GameNetwork.IsClientOrReplay && !GameNetwork.IsServer)
				{
					MBCommon.CurrentGameType = (ServerMission.IsRecordingActive() ? MBCommon.GameType.SingleRecord : MBCommon.GameType.Single);
				}
				Game.Current.OnMissionIsStarting(missionName, rec);
				ServerMission serverMission = new ServerMission();
				Mission mission = serverMission.HandleOpenNew(missionName, rec, handler, addDefaultMissionBehaviors);
				NeedsMemoryCleanUp.SetValue(mission, needsMemoryCleanup);
				return serverMission;
			}

			private static IEnumerable<MissionBehavior> AddDefaultMissionBehaviorsTo(Mission mission, IEnumerable<MissionBehavior> behaviors)
			{
				List<MissionBehavior> list = new List<MissionBehavior>();
				if (GameNetwork.IsSessionActive || GameNetwork.IsReplay)
				{
					list.Add(new MissionNetworkComponent());
				}
				if (ServerMission.IsRecordingActive() && !GameNetwork.IsReplay)
				{
					list.Add(new RecordMissionLogic());
				}
				list.Add(new BasicMissionHandler());
				list.Add(new CasualtyHandler());
				list.Add(new AgentCommonAILogic());
				list.Add(new MissionGamepadHapticEffectsHandler());
				return list.Concat(behaviors);
			}

			private void FinishMissionLoading()
			{
				this._missionInitializing = false;
				this.Mission.Scene.SetOwnerThread();
				for (int i = 0; i < 2; i++)
				{
					Tick.Invoke(Mission, new object[] { 0.001f });
				}
				this.Mission.AfterStart();
				this.Mission.Scene.ResumeLoadingRenderings();
			}

			private const int MissionFastForwardSpeedMultiplier = 10;

			private bool _missionInitializing;

			private bool _firstMissionTickAfterLoading = true;

			private int _tickCountBeforeLoad;

			public static bool RecordMission;

			public float MissionFastForwardAmount;

			public float MissionEndTime;

			private int _missionTickCount;
		}
}