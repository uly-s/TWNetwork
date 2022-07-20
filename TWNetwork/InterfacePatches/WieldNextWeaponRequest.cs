using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork.InterfacePatches
{
    public sealed class WieldNextWeaponRequest : GameNetworkMessage
    {
        public Agent.HandIndex WeaponIndex { get; internal set; }
        public Agent.WeaponWieldActionType WieldActionType { get; internal set; }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            throw new System.NotImplementedException();
        }

        protected override string OnGetLogFormat()
        {
            throw new System.NotImplementedException();
        }

        protected override bool OnRead()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnWrite()
        {
            throw new System.NotImplementedException();
        }
    }
}