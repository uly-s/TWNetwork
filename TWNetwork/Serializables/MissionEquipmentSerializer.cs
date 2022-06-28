using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class MissionEquipmentSerializer
    {
        [ProtoMember(1)]
        public List<MissionWeaponSerializer> Equipments { get; set; }

        public MissionEquipmentSerializer(MissionEquipment missionEquipment)
        {
            Equipments = new List<MissionWeaponSerializer>(5);
            for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
            {
                Equipments.Add(missionEquipment[equipmentIndex]);
            }
        }

        public MissionEquipmentSerializer() { }

        public static implicit operator MissionEquipmentSerializer(MissionEquipment missionEquipment)
        {
            return new MissionEquipmentSerializer(missionEquipment);
        }

        public static implicit operator MissionEquipment(MissionEquipmentSerializer serializer)
        {
            MissionEquipment SpawnMissionEquipment = new MissionEquipment();
            for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
            {
                SpawnMissionEquipment[equipmentIndex] = serializer.Equipments[(int)equipmentIndex];
            }
            return SpawnMissionEquipment;
        }
    }
}
