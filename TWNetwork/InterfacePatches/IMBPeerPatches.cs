using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TWNetworkPatcher;

namespace TWNetwork.Extensions
{
	public class IMBPeerPatches : HarmonyPatches
	{
		private static readonly ConcurrentDictionary<int, NativeMBPeer> Peers = new ConcurrentDictionary<int, NativeMBPeer>();
		private static readonly ConstructorInfo MBTeamCtr = typeof(MBTeam).GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, null, new Type[] { typeof(Mission), typeof(int) }, null);
		private static NativeMBPeer CurrentPeer = null;
		private static bool IsReliable = false;
		private static BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		private static Type MBNetworkPeer = Type.GetType("TaleWorlds.MountAndBlade.MBNetworkPeer");
		public static ConstructorInfo MBNetworkPeer_Ctr = MBNetworkPeer.GetConstructor(Flags,null,new Type[] { typeof(NetworkCommunicator) },null);
		public static object IMBPeer => typeof(MBAPI).GetField("IMBPeer",Flags).GetValue(null);
		public static MethodInfo m_SetUserData = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("SetUserData", Flags,null,new Type[] { typeof(int), MBNetworkPeer },null);
		public static MethodInfo m_SetControlledAgent = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("SetControlledAgent", Flags);
		public static MethodInfo m_SetTeam = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("SetTeam", Flags);
		public static MethodInfo m_IsActive = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("IsActive", Flags);
		public static MethodInfo m_SetIsSynchronized = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("SetIsSynchronized", Flags);
		public static MethodInfo m_GetIsSynchronized = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("GetIsSynchronized", Flags);
		public static MethodInfo m_SendExistingObjects = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("SendExistingObjects", Flags);
		public static MethodInfo m_BeginModuleEvent = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("BeginModuleEvent", Flags);
		public static MethodInfo m_EndModuleEvent = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("EndModuleEvent", Flags);
		public static MethodInfo m_GetAveragePingInMilliseconds = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("GetAveragePingInMilliseconds", Flags);
		public static MethodInfo m_GetAverageLossPercent = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("GetAverageLossPercent", Flags);
		public static MethodInfo m_SetRelevantGameOptions = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("SetRelevantGameOptions", Flags);
		public static MethodInfo m_GetReversedHost = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("GetReversedHost", Flags);
		public static MethodInfo m_GetHost = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("GetHost", Flags);
		public static MethodInfo m_GetPort = typeof(MBAPI).GetField("IMBPeer", Flags).FieldType.GetMethod("GetPort", Flags);
		private static MissionNetworkEntity Entity => (GameNetwork.IsServer)?GameNetworkServerPatches.Entity:(GameNetwork.IsClient)?GameNetworkClientPatches.Entity:null;
		[PatchedMethod(typeof(MBAPI), "IMBPeer", "SetUserData",new string[] { "System.Int32", "TaleWorlds.MountAndBlade.MBNetworkPeer" },true)]
		private void SetUserData(int index, object data)
		{
			if (!Peers.TryAdd(index, new NativeMBPeer((NetworkCommunicator)typeof(GameNetwork).Assembly.GetType("MBNetworkPeer").GetProperty("NetworkPeer").GetValue(data))))
				throw new InvalidOperationException();
		}

		[PatchedMethod(typeof(GameNetwork),"IMBPeer", "SetControlledAgent",new Type[] { typeof(int), typeof(UIntPtr), typeof(int) }, true)]
		private void SetControlledAgent(int index, UIntPtr missionPointer, int agentIndex)
		{
			Peers[index].ControlledAgent = Peers[index].Communicator.GetMission().FindAgentWithIndex(agentIndex);
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "SetTeam", new Type[] { typeof(int), typeof(int) }, true)]
		private void SetTeam(int index, int teamIndex)
		{
			Mission mission = Peers[index].Communicator.GetMission();
			Peers[index].SetTeam(mission.Teams.Find((MBTeam)MBTeamCtr.Invoke(new object[] { mission, teamIndex })));
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "IsActive", new Type[] { typeof(int) }, true)]
		private bool IsActive(int index)
		{
			return Peers[index].IsActive;
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "SetIsSynchronized", new Type[] { typeof(int), typeof(bool) }, true)]
		private void SetIsSynchronized(int index, bool value)
		{
			Peers[index].IsSynchronized = value;
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "GetIsSynchronized", new Type[] { typeof(int) }, true)]
		private bool GetIsSynchronized(int index)
		{
			return Peers[index].IsSynchronized;
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "SendExistingObjects", new Type[] { typeof(int), typeof(UIntPtr) }, true)]
		private void SendExistingObjects(int index, UIntPtr missionPointer)
		{
			//TODO: implement
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "BeginModuleEvent", new Type[] { typeof(int), typeof(bool) }, true)]
		private void BeginModuleEvent(int index, bool isReliable)
		{
			CurrentPeer = Peers[index];
			IsReliable = isReliable;
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "EndModuleEvent", new Type[] { typeof(bool) }, true)]
		private void EndModuleEvent(bool isReliable)
		{
			CurrentPeer.Communicator.Send(Entity.MessagesToSend,(isReliable)?DeliveryMethodType.Reliable:DeliveryMethodType.Unreliable);
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "GetAveragePingInMilliseconds", new Type[] { typeof(int) }, true)]
		private double GetAveragePingInMilliseconds(int index)
		{
			return Peers[index].AveragePingInMilliSeconds;
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "GetAverageLossPercent", new Type[] { typeof(int) }, true)]
		private double GetAverageLossPercent(int index)
		{
			return Peers[index].AverageLossPercent;
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "SetRelevantGameOptions", new Type[] { typeof(int), typeof(bool), typeof(bool) }, true)]
		private void SetRelevantGameOptions(int index, bool sendMeBloodEvents, bool sendMeSoundEvents)
		{
			Peers[index].SetRelevantGameOptions(sendMeBloodEvents, sendMeSoundEvents);
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "GetReversedHost", new Type[] { typeof(int) }, true)]
		private uint GetReversedHost(int index) 
		{
			return Peers[index].ReversedHost;
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "GetHost", new Type[] { typeof(int) }, true)]
		private uint GetHost(int index)
		{
			return Peers[index].Host;
		}

		[PatchedMethod(typeof(GameNetwork), "IMBPeer", "GetPort", new Type[] { typeof(int) }, true)]
		private ushort GetPort(int index)
		{
			return Peers[index].Port;
		}
	}
}
