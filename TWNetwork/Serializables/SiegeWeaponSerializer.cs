using ProtoBuf;
using TaleWorlds.MountAndBlade;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class SiegeWeaponSerializer
    {
        [ProtoMember(1)]
        public MissionObjectSerializer SiegeWeaponRef { get; set; }
        public SiegeWeaponSerializer() { }
        public SiegeWeaponSerializer(SiegeWeapon siegeWeapon) 
        {
            SiegeWeaponRef = siegeWeapon;
        }

        public static implicit operator SiegeWeaponSerializer(SiegeWeapon siegeWeapon)
        {
            return new SiegeWeaponSerializer(siegeWeapon);
        }

        public static implicit operator SiegeWeapon(SiegeWeaponSerializer serializer)
        {
            return (SiegeWeapon)(MissionObject)serializer.SiegeWeaponRef;
        }
    }
}
