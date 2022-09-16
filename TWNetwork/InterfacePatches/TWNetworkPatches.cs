using HarmonyLib;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TWNetwork.Messages.FromClient;
using TWNetwork.Messages.FromServer;
using TWNetworkHelper;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.InterfacePatches
{
	internal enum NetworkIdentifier { None,Server,Client }
	internal class TWNetworkPatches : HarmonyPatches
	{
		internal static NetworkIdentifier NetworkIdentifier { get; set; } = NetworkIdentifier.None;

        [PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.IsServer), false, TWNetworkHelper.MethodType.Getter)]
		private bool get_IsServer()
		{
			return NetworkIdentifier == NetworkIdentifier.Server;
		}

		[PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.IsClient), false, TWNetworkHelper.MethodType.Getter)]
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
                GameNetwork.WriteMessage(new ServerTick(Mission.Current));
                GameNetwork.EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
            }
            if (GameNetwork.IsClient && Agent.Main != null)
			{
                GameNetwork.BeginModuleEventAsClientUnreliable();
                GameNetwork.WriteMessage(new MainAgentTick(Agent.Main.EventControlFlags,Agent.Main.LookDirection,Agent.Main.MovementFlags,Agent.Main.MovementInputVector));
                GameNetwork.EndModuleEventAsClientUnreliable();
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
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<MainAgentTick>(HandleClientEventMainAgentTick));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<WieldNextWeaponRequest>(HandleClientEventWieldNextWeaponRequest));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<TryToWieldWeaponInSlotRequest>(HandleClientEventTryToWieldWeaponInSlotRequest));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<TryToSheathWeaponInSlotRequest>(HandleClientEventTryToSheathWeaponInHandRequest));
			}
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
		
		[PatchedMethod(typeof(LobbyClient), nameof(LobbyClient.IsInGame), false,TWNetworkHelper.MethodType.Getter)]
		private bool get_IsInGame()
		{
			return true;
		}

		[PatchedMethod(typeof(Agent), nameof(Agent.LockAgentReplicationTableDataWithCurrentReliableSequenceNo),new Type[] { typeof(NetworkCommunicator) },true)]
		private void LockAgentReplicationTableDataWithCurrentReliableSequenceNo(NetworkCommunicator peer)
		{
			Run = false;
		}
        private static void HandleServerEventTryToSheathWeaponInSlot(TryToSheathWeaponInSlot message)
		{
			message.AgentRef.TryToSheathWeaponInHand(message.HandIndex,message.Type);
		}

		private static void HandleServerEventServerTick(ServerTick serverTick)
		{
				foreach (ServerAgentTick tick in serverTick.ServerAgentTicks)
				{
					if (tick.Agent != Agent.Main)
					{
						if (tick.Position.Distance(tick.Agent.Position) > 0.3f)
						{
							tick.Agent.TeleportToPosition(tick.Position);
						}
						tick.Agent.MovementFlags = tick.MovementFlags;
						tick.Agent.EventControlFlags = tick.EventControlFlags;
						tick.Agent.LookDirection = tick.LookDirection;
						tick.Agent.MovementInputVector = tick.MovementInputVector;
					}
					else if (tick.Agent == Agent.Main && tick.Position.Distance(Agent.Main.Position) > 0.3f)
					{
						Agent.Main.TeleportToPosition(tick.Position);
					}
				}
		}
		private static bool HandleClientEventMainAgentTick(NetworkCommunicator networkPeer, MainAgentTick message)
		{
            if (networkPeer.ControlledAgent != null)
            {
                networkPeer.ControlledAgent.MovementFlags = message.MovementFlag;
				networkPeer.ControlledAgent.EventControlFlags = message.EventFlag;
				networkPeer.ControlledAgent.LookDirection = message.LookDirection;
				networkPeer.ControlledAgent.MovementInputVector = message.MovementInputVector;
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
