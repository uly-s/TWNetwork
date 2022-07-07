using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TWNetwork.Messages.FromClient;
using TWNetwork.Messages.FromServer;
using TWNetwork.NetworkFiles;
using TWNetworkPatcher;

namespace TWNetwork.InterfacePatches
{
    public class IMBNetwork: HarmonyPatches
    {
		/// <summary>
		/// Should set the capacity if we want to customize, how many players can join the mission. The default is 100 player.
		/// </summary>
		public static int Capacity { get; set; } = 100;
		/// <summary>
		/// Should set the ServerPeer before calling GameNetwork.StartMultiplayerOnClient method.
		/// </summary>
		public static TWNetworkPeer ServerPeer { get; set; }

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "GetMultiplayerDisabled", true)]
		private bool GetMultiplayerDisabled()
		{
			return false;
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "IsDedicatedServer", true)]
		private bool IsDedicatedServer()
		{
			return false;
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "InitializeServerSide",new Type[] { typeof(int) }, true)]
		private void InitializeServerSide(int port)
		{
			IMBNetworkServer.InitializeServer(Capacity);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "InitializeClientSide", new Type[] { typeof(string), typeof(int), typeof(int), typeof(int) }, true)]
		private void InitializeClientSide(string serverAddress, int port, int sessionKey, int playerIndex)
		{
			IMBNetworkClient.InitializeClient(ServerPeer);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "TerminateServerSide", true)]
		private void TerminateServerSide()
		{
			IMBNetworkServer.TerminateServer();
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "TerminateClientSide", true)]
		private void TerminateClientSide()
		{
			IMBNetworkClient.TerminateServer();
		}


		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ServerPing", new Type[] { typeof(string), typeof(int) }, true)]
		private void ServerPing(string serverAddress, int port) { }

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "AddPeerToDisconnect", new Type[] { typeof(int) }, true)]
		void AddPeerToDisconnect(int peer)
		{
			IMBNetworkServer.Server.OnPeerDisconnect(peer);

		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "PrepareNewUdpSession", new Type[] { typeof(int), typeof(int) }, true)]
		void PrepareNewUdpSession(int player, int sessionKey)
		{
			
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "CanAddNewPlayersOnServer", new Type[] { typeof(int) }, true)]
		bool CanAddNewPlayersOnServer(int numPlayers)
		{
			return IMBNetworkServer.Server.CanAddNewPlayers(numPlayers);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "AddNewPlayerOnServer", new Type[] { typeof(bool) }, true)]
		private int AddNewPlayerOnServer(bool serverPlayer)
		{
			return IMBNetworkServer.Server.AddNewPlayer(serverPlayer);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "AddNewBotOnServer", true)]
		private int AddNewBotOnServer()
		{
			return -1;
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "RemoveBotOnServer", new Type[] { typeof(int) }, true)]
		private void RemoveBotOnServer(int botPlayerIndex)
		{
			
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ResetMissionData", true)]
		private void ResetMissionData() { }

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "BeginBroadcastModuleEvent", true)]
		private void BeginBroadcastModuleEvent() 
		{
			IMBNetworkServer.Server.BeginBroadcastModuleEvent();
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "EndBroadcastModuleEvent", new Type[] { typeof(int), typeof(int), typeof(bool) }, true)]
		private void EndBroadcastModuleEvent(int broadcastFlags, int targetPlayer, bool isReliable) 
		{
			IMBNetworkServer.Server.EndBroadcastModuleEvent(broadcastFlags, targetPlayer, isReliable);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ElapsedTimeSinceLastUdpPacketArrived", true)]
		private double ElapsedTimeSinceLastUdpPacketArrived() { return 0; }

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "BeginModuleEventAsClient", new Type[] { typeof(bool) }, true)]
		private void BeginModuleEventAsClient(bool isReliable) 
		{
			IMBNetworkClient.Client.BeginModuleEventAsClient(isReliable);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "EndModuleEventAsClient", new Type[] { typeof(bool) }, true)]
		private void EndModuleEventAsClient(bool isReliable) 
		{
			IMBNetworkClient.Client.EndModuleEventAsClient(isReliable);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ReadIntFromPacket", new Type[] { typeof(CompressionInfo.Integer),typeof(int) }, new bool[] { true, true }, true)]
		private bool ReadIntFromPacket(ref CompressionInfo.Integer compressionInfo, out int output)
		{
			return IMBNetworkEntity.Entity.ReadIntFromPacket(ref compressionInfo, out output);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ReadUintFromPacket", new Type[] { typeof(CompressionInfo.UnsignedInteger), typeof(uint) }, new bool[] { true, true }, true)]
		private bool ReadUintFromPacket(ref CompressionInfo.UnsignedInteger compressionInfo, out uint output) 
		{
			return IMBNetworkEntity.Entity.ReadUintFromPacket(ref compressionInfo, out output);
		}


		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ReadLongFromPacket", new Type[] { typeof(CompressionInfo.LongInteger), typeof(long) }, new bool[] { true, true }, true)]
		private bool ReadLongFromPacket(ref CompressionInfo.LongInteger compressionInfo, out long output)
		{
			return IMBNetworkEntity.Entity.ReadLongFromPacket(ref compressionInfo, out output);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ReadUlongFromPacket", new Type[] { typeof(CompressionInfo.UnsignedLongInteger), typeof(ulong) }, new bool[] { true, true }, true)]
		private bool ReadUlongFromPacket(ref CompressionInfo.UnsignedLongInteger compressionInfo, out ulong output)
		{
			return IMBNetworkEntity.Entity.ReadUlongFromPacket(ref compressionInfo, out output);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ReadFloatFromPacket", new Type[] { typeof(CompressionInfo.Float), typeof(float) }, new bool[] { true, true }, true)]
		private bool ReadFloatFromPacket(ref CompressionInfo.Float compressionInfo, out float output)
		{
			return IMBNetworkEntity.Entity.ReadFloatFromPacket(ref compressionInfo, out output);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ReadStringFromPacket", new Type[] { typeof(bool) }, new bool[] { true }, true)]
		private string ReadStringFromPacket(ref bool bufferReadValid) 
		{
			return IMBNetworkEntity.Entity.ReadStringFromPacket(ref bufferReadValid);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "WriteIntToPacket", new Type[] { typeof(int), typeof(CompressionInfo.Integer) }, new bool[] { false, true }, true)]
		private void WriteIntToPacket(int value, ref CompressionInfo.Integer compressionInfo) 
		{
			IMBNetworkEntity.Entity.WriteIntToPacket(value, ref compressionInfo);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "WriteUintToPacket", new Type[] { typeof(uint), typeof(CompressionInfo.UnsignedInteger) }, new bool[] { false, true }, true)]
		private void WriteUintToPacket(uint value, ref CompressionInfo.UnsignedInteger compressionInfo) 
		{
			IMBNetworkEntity.Entity.WriteUintToPacket(value, ref compressionInfo);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "WriteLongToPacket", new Type[] { typeof(long), typeof(CompressionInfo.LongInteger) }, new bool[] { false, true }, true)]
		private void WriteLongToPacket(long value, ref CompressionInfo.LongInteger compressionInfo) 
		{
			IMBNetworkEntity.Entity.WriteLongToPacket(value, ref compressionInfo);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "WriteUlongToPacket", new Type[] { typeof(ulong), typeof(CompressionInfo.UnsignedLongInteger) }, new bool[] { false, true }, true)]
		private void WriteUlongToPacket(ulong value, ref CompressionInfo.UnsignedLongInteger compressionInfo)
		{
			IMBNetworkEntity.Entity.WriteUlongToPacket(value, ref compressionInfo);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "WriteFloatToPacket", new Type[] { typeof(float), typeof(CompressionInfo.Float) }, new bool[] { false,true }, true)]
		private void WriteFloatToPacket(float value, ref CompressionInfo.Float compressionInfo)
		{
			IMBNetworkEntity.Entity.WriteFloatToPacket(value, ref compressionInfo);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "WriteStringToPacket", new Type[] { typeof(string) }, true)]
		private void WriteStringToPacket(string value)
		{
			IMBNetworkEntity.Entity.WriteStringToPacket(value);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ReadByteArrayFromPacket", new Type[] { typeof(byte[]), typeof(int), typeof(int), typeof(bool) },new bool[] { false,false,false,true}, true)]
		private int ReadByteArrayFromPacket(byte[] buffer, int offset, int bufferCapacity, ref bool bufferReadValid)
		{
			return IMBNetworkEntity.Entity.ReadByteArrayFromPacket(buffer, offset, bufferCapacity,ref bufferReadValid);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "WriteByteArrayToPacket", new Type[] { typeof(byte[]),typeof(int),typeof(int) }, true)]
		private void WriteByteArrayToPacket(byte[] value, int offset, int size)
		{
			IMBNetworkEntity.Entity.WriteByteArrayToPacket(value, offset, size);
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "IncreaseTotalUploadLimit", new Type[] { typeof(int) }, true)]
		private void IncreaseTotalUploadLimit(int value)
		{ 
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ResetDebugVariables", true)]
		private void ResetDebugVariables()
		{ 
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "PrintDebugStats", true)]
		private void PrintDebugStats()
		{ 
		}

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "GetAveragePacketLossRatio", true)]
		private float GetAveragePacketLossRatio() { return 0f; }

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "GetDebugUploadsInBits", new Type[] { typeof(GameNetwork.DebugNetworkPacketStatisticsStruct), typeof(GameNetwork.DebugNetworkPositionCompressionStatisticsStruct) },new bool[] { true,true }, true)]
		private void GetDebugUploadsInBits(ref GameNetwork.DebugNetworkPacketStatisticsStruct networkStatisticsStruct, ref GameNetwork.DebugNetworkPositionCompressionStatisticsStruct posStatisticsStruct) { }

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ResetDebugUploads", true)]
		private void ResetDebugUploads() { }

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "PrintReplicationTableStatistics", true)]
		private void PrintReplicationTableStatistics() { }

		[PatchedMethod(typeof(MBAPI), "IMBNetwork", "ClearReplicationTableStatistics", true)]
		private void ClearReplicationTableStatistics() { }

		[PatchedMethod(typeof(MissionNetworkComponent), nameof(MissionNetworkComponent.OnMissionTick), false)]
		private void OnMissionTick(float dt)
		{
			if (GameNetwork.IsClient && GameNetwork.MyPeer.ControlledAgent != null)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new ClientTick(Agent.Main));
				GameNetwork.EndModuleEventAsClient();
			}
			if (GameNetwork.IsServer)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new ServerTick(Mission.Current));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
			}
		}

		[PatchedMethod(typeof(MissionNetworkComponent), "AddRemoveMessageHandlers", false)]
		private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register(new GameNetworkMessage.ServerMessageHandlerDelegate<ServerTick>(HandleServerEventServerTick));
			}
			if (GameNetwork.IsServer)
			{
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<ClientTick>(HandleClientEventClientTick));
			}
		}

		private static void HandleServerEventServerTick(ServerTick serverTick)
		{
			foreach (ServerAgentTick tick in serverTick.ServerAgentTicks)
			{
				tick.Agent.TeleportToPosition(tick.Position);
				if (tick.Agent.Index != Agent.Main.Index)
				{
					tick.Agent.MovementFlags = tick.MovementFlags;
					tick.Agent.EventControlFlags = tick.EventControlFlags;
					tick.Agent.MovementInputVector = tick.MovementInputVector;
					tick.Agent.LookDirection = tick.LookDirection;
				}
			}
		}

		private static bool HandleClientEventClientTick(NetworkCommunicator networkPeer, ClientTick clientTick)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			Agent controlledAgent = component.ControlledAgent;
			if (controlledAgent != null)
			{
				controlledAgent.MovementFlags = clientTick.MovementFlags;
				controlledAgent.EventControlFlags = clientTick.EventFlags;
				controlledAgent.MovementInputVector = clientTick.MovementInputVector;
				controlledAgent.LookDirection = clientTick.LookDirection;
			}
			return true;
		}

	}
}
