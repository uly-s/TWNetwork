using System;
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
		private static readonly Dictionary<int, NativeMBPeer> Peers = new Dictionary<int, NativeMBPeer>();


		[PatchedMethod(typeof(GameNetwork),"IMBPeer", "SetUserData",new Type[] { typeof(int),null },new string[] { null,"MBNetworkPeer"}, true)]
		private void SetUserData(int index, object data)
		{
			
		}

		// Token: 0x06001A47 RID: 6727
		[EngineMethod("set_controlled_agent", false)]
		private void SetControlledAgent(int index, UIntPtr missionPointer, int agentIndex);

		// Token: 0x06001A48 RID: 6728
		[EngineMethod("set_team", false)]
		private void SetTeam(int index, int teamIndex);

		// Token: 0x06001A49 RID: 6729
		[EngineMethod("is_active", false)]
		private bool IsActive(int index);

		// Token: 0x06001A4A RID: 6730
		[EngineMethod("set_is_synchronized", false)]
		private void SetIsSynchronized(int index, bool value);

		// Token: 0x06001A4B RID: 6731
		[EngineMethod("get_is_synchronized", false)]
		private bool GetIsSynchronized(int index);

		// Token: 0x06001A4C RID: 6732
		[EngineMethod("send_existing_objects", false)]
		private void SendExistingObjects(int index, UIntPtr missionPointer);

		// Token: 0x06001A4D RID: 6733
		[EngineMethod("begin_module_event", false)]
		private void BeginModuleEvent(int index, bool isReliable);

		// Token: 0x06001A4E RID: 6734
		[EngineMethod("end_module_event", false)]
		private void EndModuleEvent(bool isReliable);

		// Token: 0x06001A4F RID: 6735
		[EngineMethod("get_average_ping_in_milliseconds", false)]
		private double GetAveragePingInMilliseconds(int index);

		// Token: 0x06001A50 RID: 6736
		[EngineMethod("get_average_loss_percent", false)]
		private double GetAverageLossPercent(int index);

		// Token: 0x06001A51 RID: 6737
		[EngineMethod("set_relevant_game_options", false)]
		private void SetRelevantGameOptions(int index, bool sendMeBloodEvents, bool sendMeSoundEvents);

		// Token: 0x06001A52 RID: 6738
		[EngineMethod("get_reversed_host", false)]
		private uint GetReversedHost(int index);

		// Token: 0x06001A53 RID: 6739
		[EngineMethod("get_host", false)]
		private uint GetHost(int index);

		// Token: 0x06001A54 RID: 6740
		[EngineMethod("get_port", false)]
		private ushort GetPort(int index);
	}
}
