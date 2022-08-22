using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TWNetwork.Messages.FromClient;
using TWNetwork.Messages.FromServer;
using TWNetworkPatcher;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.InterfacePatches
{
	internal enum NetworkIdentifier { None,Server,Client }
	internal class TWNetworkPatches : HarmonyPatches
	{
		internal static NetworkIdentifier NetworkIdentifier { get; set; } = NetworkIdentifier.None;
		private static bool GotTickMessage = false;
		private static MethodInfo OnTickMethod = typeof(MissionState).GetMethod("OnTick", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		private static List<Type> GameNetworkMessageIdsFromServer => (List<Type>)typeof(GameNetwork).GetField("_gameNetworkMessageIdsFromServer", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null);
		private static List<Type> GameNetworkMessageIdsFromClient => (List<Type>)typeof(GameNetwork).GetField("_gameNetworkMessageIdsFromClient", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null);
		private static Dictionary<int, List<object>> FromServerMessageHandlers => (Dictionary<int, List<object>>)typeof(GameNetwork).GetField("_fromServerMessageHandlers",BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null);
		private static Dictionary<int, List<object>> FromClientMessageHandlers => (Dictionary<int, List<object>>)typeof(GameNetwork).GetField("_fromClientMessageHandlers", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null);
		private bool Read(GameNetworkMessage message)
		{
			return (bool)typeof(GameNetworkMessage).GetMethod("Read", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke(message, new object[] { });
		}


		[PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.IsServer), false, TWNetworkPatcher.MethodType.Getter)]
		private bool get_IsServer()
		{
			return NetworkIdentifier == NetworkIdentifier.Server;
		}

		[PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.IsClient), false, TWNetworkPatcher.MethodType.Getter)]
		private bool get_IsClient()
		{
			return NetworkIdentifier == NetworkIdentifier.Client;
		}


		[PatchedMethod(typeof(MissionNetworkComponent), nameof(MissionNetworkComponent.OnMissionTick), false)]
		private void OnMissionTick(float dt)
		{
			if (GameNetwork.IsServer)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new ServerTick(dt, Mission.Current));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
			}
		}

		[PatchedMethod(typeof(MissionNetworkComponent), "AddRemoveMessageHandlers", false)]
		private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register(new GameNetworkMessage.ServerMessageHandlerDelegate<ServerTick>(HandleServerEventServerTick));
				registerer.Register(new GameNetworkMessage.ServerMessageHandlerDelegate<TryToSheathWeaponInSlot>(HandleServerEventTryToSheathWeaponInSlot));
			}
			if (GameNetwork.IsServer)
			{
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<MovementFlagChangeRequest>(HandleClientEventChangeMovementFlagChangeRequest));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<EventFlagChangeRequest>(HandleClientEventEventFlagChangeRequest));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<LookDirectionChangeRequest>(HandleClientEventLookDirectionChangeRequest));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<WieldNextWeaponRequest>(HandleClientEventWieldNextWeaponRequest));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<TryToWieldWeaponInSlotRequest>(HandleClientEventTryToWieldWeaponInSlotRequest));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<TryToSheathWeaponInSlotRequest>(HandleClientEventTryToSheathWeaponInHandRequest));
			}
		}

		[PatchedMethod(typeof(Agent), nameof(Agent.MovementFlags), true, TWNetworkPatcher.MethodType.Setter)]
		private void set_MovementFlags(MovementControlFlag value)
		{
			if (GameNetwork.IsClient && MainAgentControlTickPatch.InMainAgentControlTick && ((Agent)Instance).IsMainAgent)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new MovementFlagChangeRequest(value));
				GameNetwork.EndModuleEventAsClient();
			}
			Run = GameNetwork.IsServer || GotTickMessage;
		}

		[PatchedMethod(typeof(Agent), nameof(Agent.EventControlFlags), true, TWNetworkPatcher.MethodType.Setter)]
		private void set_EventControlFlags(EventControlFlag value)
		{
			if (GameNetwork.IsClient && MainAgentControlTickPatch.InMainAgentControlTick && ((Agent)Instance).IsMainAgent)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new EventFlagChangeRequest(value));
				GameNetwork.EndModuleEventAsClient();
			}
			Run = GameNetwork.IsServer || GotTickMessage;
		}

		[PatchedMethod(typeof(Agent), nameof(Agent.LookDirection), true, TWNetworkPatcher.MethodType.Setter)]
		private void set_LookDirection(Vec3 value)
		{
			if (GameNetwork.IsClient && MainAgentControlTickPatch.InMainAgentControlTick && ((Agent)Instance).IsMainAgent)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new LookDirectionChangeRequest(value));
				GameNetwork.EndModuleEventAsClient();
			}
			Run = GameNetwork.IsServer || GotTickMessage;
		}
		[PatchedMethod(typeof(Agent), nameof(Agent.WieldNextWeapon), true)]
		private void WieldNextWeapon(HandIndex weaponIndex, WeaponWieldActionType wieldActionType)
		{
			if (GameNetwork.IsClient && MainAgentControlTickPatch.InMainAgentControlTick && ((Agent)Instance).IsMainAgent)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new WieldNextWeaponRequest(weaponIndex, wieldActionType));
				GameNetwork.EndModuleEventAsClient();
			}
			Run = GameNetwork.IsServer || !MainAgentControlTickPatch.InMainAgentControlTick;
		}

		[PatchedMethod(typeof(Agent), nameof(Agent.TryToWieldWeaponInSlot), true)]
		private void TryToWieldWeaponInSlot(EquipmentIndex slotIndex, WeaponWieldActionType type, bool isWieldedOnSpawn)
		{
			if (GameNetwork.IsClient && MainAgentControlTickPatch.InMainAgentControlTick && ((Agent)Instance).IsMainAgent)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new TryToWieldWeaponInSlotRequest(slotIndex, type, isWieldedOnSpawn));
				GameNetwork.EndModuleEventAsClient();
			}
			Run = GameNetwork.IsServer || !MainAgentControlTickPatch.InMainAgentControlTick;
		}

		[PatchedMethod(typeof(Agent), nameof(Agent.TryToSheathWeaponInHand), true)]
		private void TryToSheathWeaponInHand(HandIndex handIndex, WeaponWieldActionType type)
		{
			if (GameNetwork.IsClient && MainAgentControlTickPatch.InMainAgentControlTick && ((Agent)Instance).IsMainAgent)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new TryToSheathWeaponInSlotRequest(handIndex, type));
				GameNetwork.EndModuleEventAsClient();
			}
			if (GameNetwork.IsServer)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new TryToSheathWeaponInSlot((Agent)Instance, handIndex, type));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
			}
			Run = GameNetwork.IsServer || !MainAgentControlTickPatch.InMainAgentControlTick;
		}
		[PatchedMethod(typeof(GameNetwork), "HandleNetworkPacketAsServer", new Type[] { typeof(NetworkCommunicator) },true)]
		private bool HandleNetworkPacketAsServer(NetworkCommunicator networkPeer)
		{
			if (networkPeer == null)
			{
				Debug.Print("networkPeer == null", 0, Debug.DebugColor.White, 17592186044416UL);
				return false;
			}
			bool flag = true;
			try
			{
				int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.NetworkComponentEventTypeFromClientCompressionInfo, ref flag);
				if (flag)
				{
					if (num >= 0 && num < GameNetworkMessageIdsFromClient.Count)
					{
						GameNetworkMessage gameNetworkMessage = Activator.CreateInstance(GameNetworkMessageIdsFromClient[num]) as GameNetworkMessage;
						gameNetworkMessage.MessageId = num;
						InformationManager.DisplayMessage(new InformationMessage(gameNetworkMessage.GetType().Name));
						flag = Read(gameNetworkMessage);
						if (flag)
						{
							List<object> list;
							if (FromClientMessageHandlers.TryGetValue(num, out list))
							{
								foreach (object obj in list)
								{
									Delegate method = obj as Delegate;
									flag = (flag && (bool)method.DynamicInvokeWithLog(new object[]
									{
								networkPeer,
								gameNetworkMessage
									}));
									if (!flag)
									{
										break;
									}
								}
								if (list.Count == 0)
								{
									Debug.FailedAssert("Handler not found for network message " + gameNetworkMessage, "C:\\Develop\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\GameNetwork.cs", "HandleNetworkPacketAsServer", 620);
									flag = false;
								}
							}
							else
							{
								Debug.FailedAssert("Unknown network messageId " + gameNetworkMessage, "C:\\Develop\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\GameNetwork.cs", "HandleNetworkPacketAsServer", 626);
								flag = false;
							}
						}
					}
					else
					{
						Debug.FailedAssert("Handler not found for network message id: " + num.ToString(), "C:\\Develop\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\GameNetwork.cs", "HandleNetworkPacketAsServer", 633);
						flag = false;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Print("error " + ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				return false;
			}
			return flag;
		}

		[PatchedMethod(typeof(GameNetwork), "HandleNetworkPacketAsClient", true)]
        private bool HandleNetworkPacketAsClient()
        {
            bool flag = true;
            int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo, ref flag);
            if (flag && num >= 0 && num < GameNetworkMessageIdsFromServer.Count)
            {
                GameNetworkMessage gameNetworkMessage = Activator.CreateInstance(GameNetworkMessageIdsFromServer[num]) as GameNetworkMessage;
                gameNetworkMessage.MessageId = num;
                Debug.Print("Reading message: " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.White, 17179869184UL);
				InformationManager.DisplayMessage(new InformationMessage(gameNetworkMessage.GetType().Name));
				flag = Read(gameNetworkMessage);
                if (flag)
                {
                    List<object> list;
                    if (FromServerMessageHandlers.TryGetValue(num, out list))
                    {
                        foreach (object obj in list)
                        {
                            try
                            {
                                (obj as Delegate).DynamicInvokeWithLog(new object[]
                                {
                                gameNetworkMessage
                                });
                            }
                            catch
                            {
                                Debug.Print("Exception in handler of " + num.ToString(), 0, Debug.DebugColor.White, 17179869184UL);
                                Debug.Print("Exception in handler of " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.Red, 17179869184UL);
                                throw;
                            }
                        }
                        if (list.Count == 0)
                        {
                            Debug.Print("No message handler found for " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.Red, 17179869184UL);
                        }
                    }
                    else
                    {
                        Debug.Print("Invalid messageId " + num.ToString(), 0, Debug.DebugColor.White, 17179869184UL);
                        Debug.Print("Invalid messageId " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.White, 17179869184UL);
                    }
                }
                else
                {
                    Debug.Print("Invalid message read for: " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.White, 17179869184UL);
                }
            }
            else
            {
                Debug.Print("Invalid message id read: " + num, 0, Debug.DebugColor.White, 17179869184UL);
            }
            return flag;
        }

        private static void HandleServerEventTryToSheathWeaponInSlot(TryToSheathWeaponInSlot message)
		{
			message.AgentRef.TryToSheathWeaponInHand(message.HandIndex,message.Type);
		}

		private static void HandleServerEventServerTick(ServerTick serverTick)
		{
			GotTickMessage = true;
			foreach (ServerAgentTick tick in serverTick.ServerAgentTicks)
			{
				tick.Agent.TeleportToPosition(tick.Position);
				tick.Agent.MovementFlags = tick.MovementFlags;
				tick.Agent.EventControlFlags = tick.EventControlFlags;
				tick.Agent.MovementInputVector = tick.MovementInputVector;
				tick.Agent.LookDirection = tick.LookDirection;
			}
			GotTickMessage = false;
		}

		private static bool HandleClientEventChangeMovementFlagChangeRequest(NetworkCommunicator networkPeer, MovementFlagChangeRequest changeMovementFlag)
		{
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.MovementFlags = changeMovementFlag.MovementFlag;
			}
			return true;
		}

		private static bool HandleClientEventEventFlagChangeRequest(NetworkCommunicator networkPeer, EventFlagChangeRequest changeEventFlag)
		{
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.EventControlFlags = changeEventFlag.EventFlag;
			}
			return true;
		}

		private static bool HandleClientEventWieldNextWeaponRequest(NetworkCommunicator networkPeer, WieldNextWeaponRequest wieldNextWeaponRequest)
		{
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.WieldNextWeapon(wieldNextWeaponRequest.WeaponIndex,wieldNextWeaponRequest.WieldActionType);
			}
			return true;
		}

		private static bool HandleClientEventTryToWieldWeaponInSlotRequest(NetworkCommunicator networkPeer, TryToWieldWeaponInSlotRequest tryToWieldWeaponInSlotRequest)
		{
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.TryToWieldWeaponInSlot(tryToWieldWeaponInSlotRequest.SlotIndex, tryToWieldWeaponInSlotRequest.Type,tryToWieldWeaponInSlotRequest.IsWieldedOnSpawn);
			}
			return true;
		}

		private static bool HandleClientEventTryToSheathWeaponInHandRequest(NetworkCommunicator networkPeer, TryToSheathWeaponInSlotRequest tryToSheathWeaponInSlotRequest)
		{
			if(networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.TryToSheathWeaponInHand(tryToSheathWeaponInSlotRequest.HandIndex, tryToSheathWeaponInSlotRequest.Type);
			}
			return true;
		}


		private static bool HandleClientEventLookDirectionChangeRequest(NetworkCommunicator networkPeer, LookDirectionChangeRequest lookDirectionChangeRequest)
		{
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.LookDirection = lookDirectionChangeRequest.LookDirection;
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(MissionMainAgentController),"ControlTick")]
	internal class MainAgentControlTickPatch
	{
		public static bool InMainAgentControlTick { get; private set; } = false;

		static void Prefix() 
		{
			InMainAgentControlTick = true;
		}

		static void Postfix()
		{
			InMainAgentControlTick = false;
		}
	}
}
