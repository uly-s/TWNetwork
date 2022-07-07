using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TWNetworkPatcher;

namespace TWNetworkTestMod
{
    internal class MultiplayerGameModePatches: HarmonyPatches
    {
        [PatchedMethod(typeof(MissionBasedMultiplayerGameMode),nameof(MissionBasedMultiplayerGameMode.StartMultiplayerGame),new Type[] { typeof(string) },false)]
        private void StartMultiplayerGame(string scene)
        {
            if (((MissionBasedMultiplayerGameMode)Instance).Name == "CustomBattleMission")
            {
                MultiplayerCustomMissions.OpenTestMission(scene);
            }
        }
    }
}
