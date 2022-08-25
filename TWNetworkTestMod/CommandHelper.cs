using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TWNetworkTestMod
{
    static public class CommandHelper
    {
        public static void SpawnAgent()
        {
            var frame = Mission.Current.GetBattleSideInitialSpawnPathFrame(BattleSideEnum.Attacker).ToGroundMatrixFrame();
            var character = Game.Current.ObjectManager.GetObjectTypeList<BasicCharacterObject>()[0];
            var agentBuildData = new AgentBuildData(character).BodyProperties(character.GetBodyPropertiesMax());
            Mission mission = Mission.Current;
            var agentBuildData2 = agentBuildData
                .InitialPosition(frame.origin)
                .InitialDirection((frame.rotation.f.AsVec2)
                .Normalized())
                .TroopOrigin(new BasicBattleAgentOrigin(character))
                .Team(mission.AttackerTeam)
                .NoHorses(true)
                .Equipment(character.Equipment);
            Agent agent = mission.SpawnAgent(agentBuildData2);
        }
    }
}
