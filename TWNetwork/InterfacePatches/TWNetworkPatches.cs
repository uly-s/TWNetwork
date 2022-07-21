using HarmonyLib;
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
	internal class TWNetworkPatches: HarmonyPatches
    {
		private static bool GotTickMessage = false;
		private static MethodInfo OnTickMethod = typeof(MissionState).GetMethod("OnTick",BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		[PatchedMethod(typeof(MissionState), "OnTick", true)]
		private void OnTick(float dt)
		{
			Run = GameNetwork.IsServer || GotTickMessage;
		}


		[PatchedMethod(typeof(MissionNetworkComponent), nameof(MissionNetworkComponent.OnMissionTick), false)]
		private void OnMissionTick(float dt)
		{
			if (GameNetwork.IsServer)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new ServerTick(dt,Mission.Current));
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

		[PatchedMethod(typeof(Agent),nameof(Agent.MovementFlags),true, TWNetworkPatcher.MethodType.Setter)]
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
		[PatchedMethod(typeof(Agent),nameof(Agent.WieldNextWeapon),true)]
		private void WieldNextWeapon(HandIndex weaponIndex, WeaponWieldActionType wieldActionType)
		{
			if (GameNetwork.IsClient && MainAgentControlTickPatch.InMainAgentControlTick && ((Agent)Instance).IsMainAgent)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new WieldNextWeaponRequest(weaponIndex,wieldActionType));
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
				GameNetwork.WriteMessage(new TryToWieldWeaponInSlotRequest(slotIndex, type,isWieldedOnSpawn));
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
				GameNetwork.WriteMessage(new TryToSheathWeaponInSlotRequest(handIndex,type));
				GameNetwork.EndModuleEventAsClient();
			}
			if (GameNetwork.IsServer)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new TryToSheathWeaponInSlot((Agent)Instance,handIndex,type));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
			}
			Run = GameNetwork.IsServer || !MainAgentControlTickPatch.InMainAgentControlTick;
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
			OnTickMethod.Invoke(MissionState.Current, new object[] { serverTick.dt });
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
