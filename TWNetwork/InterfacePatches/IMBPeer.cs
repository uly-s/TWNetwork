using System;
using System.Reflection;
using TaleWorlds.MountAndBlade;
using TWNetwork.InterfacePatches;
using TWNetwork.NetworkFiles;
using TWNetworkPatcher;

namespace TWNetwork.InterfacePatches
{
    public class IMBPeer : InterfaceImplementer
	{
		private static readonly ConstructorInfo MBTeamCtr = typeof(MBTeam).GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, null, new Type[] { typeof(Mission), typeof(int) }, null);
		private static BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		private static Type MBNetworkPeer = Type.GetType("TaleWorlds.MountAndBlade.MBNetworkPeer");
		public static ConstructorInfo MBNetworkPeer_Ctr = MBNetworkPeer.GetConstructor(Flags,null,new Type[] { typeof(NetworkCommunicator) },null);
		public static object IMBPeerObject => typeof(MBAPI).GetField("IMBPeer",Flags).GetValue(null);
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

        public IMBPeer() : base(typeof(MBAPI).GetField("IMBPeer").FieldType)
        {
        }

        private void SetUserData(int index, object data)
		{
			IMBNetworkServer.Server.GetPeer(index).SetCommunicator((NetworkCommunicator)typeof(GameNetwork).Assembly.GetType("MBNetworkPeer").GetProperty("NetworkPeer").GetValue(data));
		}

		private void SetControlledAgent(int index, UIntPtr missionPointer, int agentIndex)
		{
			IMBNetworkServer.Server.GetPeer(index).ControlledAgent = Mission.Current.FindAgentWithIndex(agentIndex);
		}

		private void SetTeam(int index, int teamIndex)
		{
			IMBNetworkServer.Server.GetPeer(index).SetTeam(Mission.Current.Teams.Find((MBTeam)MBTeamCtr.Invoke(new object[] { Mission.Current, teamIndex })));
		}

		private bool IsActive(int index)
		{
			return IMBNetworkServer.Server.GetPeer(index).IsActive;
		}

		private void SetIsSynchronized(int index, bool value)
		{
			IMBNetworkServer.Server.GetPeer(index).IsSynchronized = value;
		}

		private bool GetIsSynchronized(int index)
		{
			return IMBNetworkServer.Server.GetPeer(index).IsSynchronized;
		}

		private void SendExistingObjects(int index, UIntPtr missionPointer)
		{
			//TODO: implement
		}

		private void BeginModuleEvent(int index, bool isReliable)
		{
			IMBNetworkServer.Server.BeginSingleModuleEvent(index, isReliable);
		}

		private void EndModuleEvent(bool isReliable)
		{
			IMBNetworkServer.Server.EndSingleModuleEvent(isReliable);
		}

		private double GetAveragePingInMilliseconds(int index)
		{
			return IMBNetworkServer.Server.GetPeer(index).AveragePingInMilliSeconds;
		}

		private double GetAverageLossPercent(int index)
		{
			return IMBNetworkServer.Server.GetPeer(index).AverageLossPercent;
		}

		private void SetRelevantGameOptions(int index, bool sendMeBloodEvents, bool sendMeSoundEvents)
		{
			IMBNetworkServer.Server.GetPeer(index).SetRelevantGameOptions(sendMeBloodEvents, sendMeSoundEvents);
		}

		private uint GetReversedHost(int index) 
		{
			return IMBNetworkServer.Server.GetPeer(index).ReversedHost;
		}

		private uint GetHost(int index)
		{
			return IMBNetworkServer.Server.GetPeer(index).Host;
		}
		private ushort GetPort(int index)
		{
			return IMBNetworkServer.Server.GetPeer(index).Port;
		}
	}
}
