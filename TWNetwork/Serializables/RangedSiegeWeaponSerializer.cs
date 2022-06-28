using ProtoBuf;
using TaleWorlds.MountAndBlade;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class RangedSiegeWeaponSerializer
    {
        [ProtoMember(1)]
        public MissionObjectSerializer RangedSiegeWeaponRef { get; set; }
        public RangedSiegeWeaponSerializer() { }
        public RangedSiegeWeaponSerializer(RangedSiegeWeapon rangedSiegeWeapon)
        {
            RangedSiegeWeaponRef = rangedSiegeWeapon;
        }

        public static implicit operator RangedSiegeWeaponSerializer(RangedSiegeWeapon rangedSiegeWeapon)
        {
            return new RangedSiegeWeaponSerializer(rangedSiegeWeapon);
        }

        public static implicit operator RangedSiegeWeapon(RangedSiegeWeaponSerializer serializer)
        {
            return (RangedSiegeWeapon)(MissionObject)serializer.RangedSiegeWeaponRef;
        }
    }
}
