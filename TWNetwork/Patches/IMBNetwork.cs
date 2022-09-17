using System;
using System.Reflection;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TWNetwork.Messages.FromClient;
using TWNetwork.Messages.FromServer;
using TWNetwork.NetworkFiles;
using TWNetworkHelper;

namespace TWNetwork.Patches
{
    public class IMBNetwork: InterfaceImplementer
    {
        public IMBNetwork() : base(typeof(MBAPI).GetField("IMBNetwork", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).FieldType)
        {
        }
		private static IServer server = null;
		public static IServer Server { get { return server; } set { if (server == null) server = value; } }

        private static IClient client = null;
        public static IClient Client { get { return client; } set { if (client == null) client = value; } }

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
			IMBNetworkServer.InitializeServer(port,Server);
		}

		private void InitializeClientSide(string serverAddress, int port, int sessionKey, int playerIndex)
		{
			IMBNetworkClient.InitializeClient(serverAddress,port,sessionKey,playerIndex,Client);
		}

		private void TerminateServerSide()
		{
            if (GameNetworkPatches.NetworkIdentifier != NetworkIdentifier.Server)
                return;
            IMBNetworkServer.TerminateServer();
			server = null;
		}

		private void TerminateClientSide()
		{
			if (GameNetworkPatches.NetworkIdentifier != NetworkIdentifier.Client)
				return;
			IMBNetworkClient.TerminateClient();
			client = null;
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
