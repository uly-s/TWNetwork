using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TWNetwork.Messages.FromClient;
using TWNetwork.Messages.FromServer;
using TWNetwork.NetworkFiles;
using TWNetworkPatcher;

namespace TWNetwork.InterfacePatches
{
    public class IMBNetwork: InterfaceImplementer
    {
        public IMBNetwork() : base(typeof(MBAPI).GetField("IMBNetwork").FieldType)
        {
        }

        /// <summary>
        /// Should set the capacity if we want to customize, how many players can join the mission. The default is 100 player.
        /// </summary>
        public static int Capacity { get; set; } = 100;
		/// <summary>
		/// Should set the ServerPeer before calling GameNetwork.StartMultiplayerOnClient method.
		/// </summary>
		public static TWNetworkPeer ServerPeer { get; set; }
		private bool GetMultiplayerDisabled()
		{
			return false;
		}
		private bool IsDedicatedServer()
		{
			return false;
		}
		private void InitializeServerSide(int port)
		{
			IMBNetworkServer.InitializeServer(Capacity);
		}

		private void InitializeClientSide(string serverAddress, int port, int sessionKey, int playerIndex)
		{
			IMBNetworkClient.InitializeClient(ServerPeer);
		}

		private void TerminateServerSide()
		{
			IMBNetworkServer.TerminateServer();
		}

		private void TerminateClientSide()
		{
			IMBNetworkClient.TerminateServer();
		}

		private void ServerPing(string serverAddress, int port) { }

		void AddPeerToDisconnect(int peer)
		{
			IMBNetworkServer.Server.OnPeerDisconnect(peer);

		}

		void PrepareNewUdpSession(int player, int sessionKey)
		{
			
		}

		bool CanAddNewPlayersOnServer(int numPlayers)
		{
			return IMBNetworkServer.Server.CanAddNewPlayers(numPlayers);
		}

		private int AddNewPlayerOnServer(bool serverPlayer)
		{
			return IMBNetworkServer.Server.AddNewPlayer(serverPlayer);
		}

		private int AddNewBotOnServer()
		{
			return -1;
		}

		private void RemoveBotOnServer(int botPlayerIndex)
		{
			
		}

		private void ResetMissionData() { }

		private void BeginBroadcastModuleEvent() 
		{
			IMBNetworkServer.Server.BeginBroadcastModuleEvent();
		}

		private void EndBroadcastModuleEvent(int broadcastFlags, int targetPlayer, bool isReliable) 
		{
			IMBNetworkServer.Server.EndBroadcastModuleEvent(broadcastFlags, targetPlayer, isReliable);
		}

		private double ElapsedTimeSinceLastUdpPacketArrived() { return 0; }

		private void BeginModuleEventAsClient(bool isReliable) 
		{
			IMBNetworkClient.Client.BeginModuleEventAsClient(isReliable);
		}

		private void EndModuleEventAsClient(bool isReliable) 
		{
			IMBNetworkClient.Client.EndModuleEventAsClient(isReliable);
		}

		private bool ReadIntFromPacket(ref CompressionInfo.Integer compressionInfo, out int output)
		{
			return IMBNetworkEntity.Entity.ReadIntFromPacket(ref compressionInfo, out output);
		}

		private bool ReadUintFromPacket(ref CompressionInfo.UnsignedInteger compressionInfo, out uint output) 
		{
			return IMBNetworkEntity.Entity.ReadUintFromPacket(ref compressionInfo, out output);
		}

		private bool ReadLongFromPacket(ref CompressionInfo.LongInteger compressionInfo, out long output)
		{
			return IMBNetworkEntity.Entity.ReadLongFromPacket(ref compressionInfo, out output);
		}

		private bool ReadUlongFromPacket(ref CompressionInfo.UnsignedLongInteger compressionInfo, out ulong output)
		{
			return IMBNetworkEntity.Entity.ReadUlongFromPacket(ref compressionInfo, out output);
		}

		private bool ReadFloatFromPacket(ref CompressionInfo.Float compressionInfo, out float output)
		{
			return IMBNetworkEntity.Entity.ReadFloatFromPacket(ref compressionInfo, out output);
		}

		private string ReadStringFromPacket(ref bool bufferReadValid) 
		{
			return IMBNetworkEntity.Entity.ReadStringFromPacket(ref bufferReadValid);
		}

		private void WriteIntToPacket(int value, ref CompressionInfo.Integer compressionInfo) 
		{
			IMBNetworkEntity.Entity.WriteIntToPacket(value, ref compressionInfo);
		}

		private void WriteUintToPacket(uint value, ref CompressionInfo.UnsignedInteger compressionInfo) 
		{
			IMBNetworkEntity.Entity.WriteUintToPacket(value, ref compressionInfo);
		}

		private void WriteLongToPacket(long value, ref CompressionInfo.LongInteger compressionInfo) 
		{
			IMBNetworkEntity.Entity.WriteLongToPacket(value, ref compressionInfo);
		}

		private void WriteUlongToPacket(ulong value, ref CompressionInfo.UnsignedLongInteger compressionInfo)
		{
			IMBNetworkEntity.Entity.WriteUlongToPacket(value, ref compressionInfo);
		}

		private void WriteFloatToPacket(float value, ref CompressionInfo.Float compressionInfo)
		{
			IMBNetworkEntity.Entity.WriteFloatToPacket(value, ref compressionInfo);
		}

		private void WriteStringToPacket(string value)
		{
			IMBNetworkEntity.Entity.WriteStringToPacket(value);
		}

		private int ReadByteArrayFromPacket(byte[] buffer, int offset, int bufferCapacity, ref bool bufferReadValid)
		{
			return IMBNetworkEntity.Entity.ReadByteArrayFromPacket(buffer, offset, bufferCapacity,ref bufferReadValid);
		}

		private void WriteByteArrayToPacket(byte[] value, int offset, int size)
		{
			IMBNetworkEntity.Entity.WriteByteArrayToPacket(value, offset, size);
		}

		private void IncreaseTotalUploadLimit(int value)
		{ 
		}

		private void ResetDebugVariables()
		{ 
		}

		private void PrintDebugStats()
		{ 
		}

		private float GetAveragePacketLossRatio() { return 0f; }

		private void GetDebugUploadsInBits(ref GameNetwork.DebugNetworkPacketStatisticsStruct networkStatisticsStruct, ref GameNetwork.DebugNetworkPositionCompressionStatisticsStruct posStatisticsStruct) { }

		private void ResetDebugUploads() { }

		private void PrintReplicationTableStatistics() { }
		private void ClearReplicationTableStatistics() { }
	}
}
