using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TWNetworkPatcher;

namespace TWNetworkTestMod
{
    public class Main: MBSubModuleBase
    {
        IUpdatable updatable = null;
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            HarmonyPatcher.ApplyPatches();
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
            TWNetworkClient client = new TWNetworkClient();
            client.Start("127.0.0.1",15801);
            updatable = client;
        }

        private void CreateServer()
        {
            TWNetworkServer server = new TWNetworkServer();
            server.Start(15801, 2);
            updatable = server;
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            updatable?.Update();
        }
    }
}
