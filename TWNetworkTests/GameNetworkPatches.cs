using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNetworkPatcher;

namespace TWNetworkTests
{
    public static class GameNetworkPatches
    {
		public static int Count { get; private set; } = 0;

		[PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.BeginModuleEventAsClient))]
		public static void BeginModuleEventAsClient()
		{
			Count++;
		}

		[PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.BeginModuleEventAsClientUnreliable))]
		public static void BeginModuleEventAsClientUnreliable()
		{
			Count++;
		}

		[PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.EndModuleEventAsClient))]
		public static void EndModuleEventAsClient()
		{
			Count++;
		}

		[PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.EndModuleEventAsClientUnreliable))]
		public static void EndModuleEventAsClientUnreliable()
		{
			Count++;
		}

		[PatchedMethod(typeof(GameNetwork), nameof(GameNetwork.GetOne))]
		public static int GetOne()
		{
			Count++;
			return -1;
		}
	}
}
