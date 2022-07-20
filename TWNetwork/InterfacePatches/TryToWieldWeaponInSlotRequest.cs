using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TWNetwork.InterfacePatches
{
    internal class TryToWieldWeaponInSlotRequest
    {
        public EquipmentIndex SlotIndex { get; internal set; }
        public Agent.WeaponWieldActionType Type { get; internal set; }
        public bool IsWieldedOnSpawn { get; internal set; }
    }
}