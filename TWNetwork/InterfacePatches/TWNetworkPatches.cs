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

		[PatchedMethod(typeof(Agent), nameof(Agent.MovementFlags), true, TWNetworkHelper.MethodType.Setter)]
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

		[PatchedMethod(typeof(Agent), nameof(Agent.EventControlFlags), true, TWNetworkHelper.MethodType.Setter)]
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

		[PatchedMethod(typeof(Agent), nameof(Agent.LookDirection), true, TWNetworkHelper.MethodType.Setter)]
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
		[PatchedMethod(typeof(Agent), "BuildAux",true)]
		private void BuildAux()
		{
			var pointer = (UIntPtr)typeof(Agent).GetMethod("GetPtr",BindingFlags.Instance | BindingFlags.NonPublic).Invoke(((Agent)Instance),new object[] { });
			var eyeoffset = ((Agent)Instance).Monster.EyeOffsetWrtHead;
			using (FileStream stream = new FileStream(@"C:\Github Repos\Something.txt", FileMode.Append))
			{
				using (StreamWriter writer = new StreamWriter(stream))
				{
					writer.WriteLine(GameNetwork.IsServer?"Server:":"Client:");
					writer.WriteLine($"UIntPointer: {pointer}");
					writer.WriteLine($"EyeOffset: {eyeoffset.X},{eyeoffset.Y},{eyeoffset.Z},{eyeoffset.w}");
				}
			}
			Run = true;
		}

		//[PatchedMethod(typeof(MissionNetworkComponent), "HandleServerEventCreateAgent", new Type[] { typeof(CreateAgent) },true)]
		//private void HandleServerEventCreateAgent(CreateAgent message)
		//{
		//	BasicCharacterObject character = message.Character;
		//	NetworkCommunicator peer = message.Peer;
		//	MissionPeer missionPeer = (peer != null) ? peer.GetComponent<MissionPeer>() : null;
		//	AgentBuildData agentBuildData = new AgentBuildData(character).MissionPeer(message.IsPlayerAgent ? missionPeer : null).Monster(message.Monster).TroopOrigin(new BasicBattleAgentOrigin(character)).Equipment(message.SpawnEquipment).EquipmentSeed(message.BodyPropertiesSeed);
		//	Vec3 position = message.Position;
		//	AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(position);
		//	Vec2 vec = message.Direction;
		//	vec = vec.Normalized();
		//	AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(vec).MissionEquipment(message.SpawnMissionEquipment).Team(message.Team).Index(message.AgentIndex).MountIndex(message.MountAgentIndex).IsFemale(message.IsFemale).ClothingColor1(message.ClothingColor1).ClothingColor2(message.ClothingColor2);
		//	Formation formation = null;
		//	if (message.Team != null && message.FormationIndex >= 0 && !GameNetwork.IsReplay)
		//	{
		//		formation = message.Team.GetFormation((FormationClass)message.FormationIndex);
		//		agentBuildData3.Formation(formation);
		//	}
		//	agentBuildData3.BodyProperties(message.BodyPropertiesValue);
		//	agentBuildData3.Age((int)agentBuildData3.AgentBodyProperties.Age);
		//	Banner banner = null;
		//	if (formation != null)
		//	{
		//		if (!string.IsNullOrEmpty(formation.BannerCode))
		//		{
		//			if (formation.Banner == null)
		//			{
		//				banner = new Banner(formation.BannerCode, message.Team.Color, message.Team.Color2);
		//				formation.Banner = banner;
		//			}
		//			else
		//			{
		//				banner = formation.Banner;
		//			}
		//		}
		//	}
		//	agentBuildData3.Banner(banner);
		//	Agent mountAgent = Mission.Current.SpawnAgent(agentBuildData3, false, 0).MountAgent;
		//}

		[PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.WriteMessage), new Type[] { typeof(GameNetworkMessage) },false)]
		private void WriteMessage(GameNetworkMessage message)
		{
			InformationManager.DisplayMessage(new InformationMessage($"Sent {message.GetType().Name} message."));
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
