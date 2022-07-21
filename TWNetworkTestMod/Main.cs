using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TWNetwork;
using TWNetworkPatcher;

namespace TWNetworkTestMod
{
    public class Main: MBSubModuleBase
    {
        public static IUpdatable updatable = null;
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Initializer.InitInterfaces();
            Initializer.InitPatches();
            TaleWorlds.MountAndBlade.Module.CurrentModule.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("CustomBattleMission"));
            FieldInfo splashScreen = TaleWorlds.MountAndBlade.Module.CurrentModule.GetType().GetField("_splashScreenPlayed", BindingFlags.Instance | BindingFlags.NonPublic);
            splashScreen.SetValue(TaleWorlds.MountAndBlade.Module.CurrentModule, true);
            TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(new InitialStateOption("CreateServerOption", new TextObject("Create Server"), 9990,
                 CreateServer,
                () => (false, new TextObject(""))));
            TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(new InitialStateOption("JoinServerOption", new TextObject("Join Server"), 9990,
                JoinServer,
               () => (false, new TextObject(""))));

        }

        private void JoinServer()
        {
            MBGameManager.StartNewGame(new TWNetworkGameManager(false));
        }

        private void CreateServer()
        {
            MBGameManager.StartNewGame(new TWNetworkGameManager(true));
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            updatable?.Update();
        }
    }
}
