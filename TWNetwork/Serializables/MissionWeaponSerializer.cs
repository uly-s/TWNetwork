using JetBrains.Annotations;
using ProtoBuf;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class MissionWeaponSerializer
    {
        [ProtoMember(1)]
        public bool IsEmpty { get; set; }
        [ProtoMember(2)]
        public uint? ItemRef { get; set; } = null;
        [ProtoMember(3)]
        public int? RawDataForNetwork { get; set; } = null;
        [ProtoMember(4)]
        public int? ReloadPhase { get; set; } = null;
        [ProtoMember(5)]
        public int? CurrentUsageIndex { get; set; } = null;
        [ProtoMember(6)]
        public bool? Flag { get; set; } = null;
        [ProtoMember(7)]
        public string BannerRef { get; set; } = null;
        [ProtoMember(8)]
        public bool? Flag2 { get; set; } = null;
        [ProtoMember(9)]
        public uint? AmmoWeaponItemRef { get; set; } = null;
        [ProtoMember(10)]
        public int? AmmoWeaponRawDataForNetwork { get; set; } = null;

        public MissionWeaponSerializer()
        {}

        public MissionWeaponSerializer(MissionWeapon weapon)
        {
            IsEmpty = weapon.IsEmpty;
            if (!IsEmpty)
            {
                ItemRef = (weapon.Item != null) ? weapon.Item.Id.InternalValue : 0U;
                RawDataForNetwork = weapon.RawDataForNetwork;
                ReloadPhase = weapon.ReloadPhase;
                CurrentUsageIndex = weapon.CurrentUsageIndex;
                Flag = weapon.Banner != null;
                if ((bool)Flag)
                    BannerRef = weapon.Banner.Serialize();
                Flag2 = !weapon.AmmoWeapon.IsEmpty;
                if ((bool)Flag2)
                {
                    AmmoWeaponItemRef = (weapon.AmmoWeapon.Item != null) ? weapon.AmmoWeapon.Item.Id.InternalValue : 0U;
                    AmmoWeaponRawDataForNetwork = weapon.AmmoWeapon.RawDataForNetwork;
                }
            }
        }

        public static implicit operator MissionWeapon(MissionWeaponSerializer serializer)
        {
            if (serializer.IsEmpty)
                return MissionWeapon.Invalid;
            MBObjectBase mbObjectBase1 = SerializerHelper.GetObjectFromRef((uint)serializer.ItemRef);
            Banner banner = null;
            if ((bool)serializer.Flag)
                banner = new Banner(serializer.BannerRef);
            ItemObject primaryItem = mbObjectBase1 as ItemObject;
            MissionWeapon? ammoWeapon = null;
            if ((bool)serializer.Flag2)
            {
                MBObjectBase mbObjectBase2 = SerializerHelper.GetObjectFromRef((uint)serializer.AmmoWeaponItemRef);
                ItemObject primaryItem2 = mbObjectBase2 as ItemObject;
                ammoWeapon = new MissionWeapon?(new MissionWeapon(primaryItem2, null, banner, (short)serializer.AmmoWeaponRawDataForNetwork));
            }
            return new MissionWeapon(primaryItem, null, banner, (short)serializer.RawDataForNetwork, (short)serializer.ReloadPhase, ammoWeapon)
            {
                CurrentUsageIndex = (int)serializer.CurrentUsageIndex
            };
        }

        public static implicit operator MissionWeaponSerializer(MissionWeapon missionWeapon)
        {
            return new MissionWeaponSerializer(missionWeapon);
        }

    }
}