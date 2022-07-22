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

        public IMBPeer() : base(typeof(MBAPI).GetField("IMBPeer",BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).FieldType)
        {
        }

        private void SetUserData(int index, object data)
		{
			NativeMBPeer peer = IMBNetworkServer.Server.GetPeer(index);
			Type type = data.GetType();
			PropertyInfo prop = type.GetProperty("NetworkPeer");
			NetworkCommunicator communicator = (NetworkCommunicator)prop.GetValue(data);
			peer.SetCommunicator(communicator);
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
