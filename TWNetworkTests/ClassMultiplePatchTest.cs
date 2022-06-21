using HarmonyLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TWNetworkPatcher;

namespace TWNetworkTests
{
    [TestClass]
    public class ClassMultiplePatchTest
    {
        [TestMethod]
        public void TestGameNetworkPatches()
        {
            StaticPrefixSkipPatcher.ApplyPatches();
            GameNetwork.BeginModuleEventAsClientUnreliable();
            GameNetwork.EndModuleEventAsClientUnreliable();
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.EndModuleEventAsClient();
            Assert.IsTrue(GameNetworkPatches.Count == 4 && GameNetwork.Count == 0);
            int result = GameNetwork.GetOne();
            Assert.IsTrue(result == -1 && GameNetwork.Count == 0 && GameNetworkPatches.Count == 5);
        }
    }
}
