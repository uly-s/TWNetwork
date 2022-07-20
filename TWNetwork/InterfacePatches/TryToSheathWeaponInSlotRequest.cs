using TaleWorlds.MountAndBlade;

namespace TWNetwork.InterfacePatches
{
    internal class TryToSheathWeaponInSlotRequest
    {
        public Agent.WeaponWieldActionType Type { get; internal set; }
        public Agent.HandIndex HandIndex { get; internal set; }
    }
}