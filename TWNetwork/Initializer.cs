using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TWNetwork.InterfacePatches;
using TWNetworkPatcher;

namespace TWNetwork
{
    public static class Initializer
    {
        /// <summary>
        /// This method should be called in the OnSubModuleLoad method.
        /// The method replaces the IMBNetwork and IMBPeer objects with our new implementation.
        /// </summary>
        public static void InitInterfaces()
        {
            typeof(MBAPI).GetField("IMBNetwork",BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).SetValue(null,new IMBNetwork().GetTransparentProxy());
            typeof(MBAPI).GetField("IMBPeer", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).SetValue(null, new IMBPeer().GetTransparentProxy());
        }
        /// <summary>
        /// This method applies all the patches that is implemented with the HarmonyPatcher framework in the current AppDomain.
        /// Should be called in the OnSubModuleLoad method.
        /// </summary>
        public static void InitPatches()
        {
            HarmonyPatcher.ApplyPatches();
            new Harmony("TWNetwork.ManualPatches").PatchAll();
        }

        public static void InitNetwork(bool isServer)
        {
            TWNetworkPatches.NetworkIdentifier = (isServer)?NetworkIdentifier.Server:NetworkIdentifier.Client;
        }

        public static void DeleteNetwork()
        {
            TWNetworkPatches.NetworkIdentifier = NetworkIdentifier.None;
        }
    }
}
