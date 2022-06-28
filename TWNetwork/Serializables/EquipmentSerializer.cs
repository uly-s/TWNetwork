using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class EquipmentSerializer
    {
        [ProtoMember(1)]
        public List<EquipmentElementSerializer> Equipments { get; set; }

        public EquipmentSerializer(Equipment equipment)
        {
            Equipments = new List<EquipmentElementSerializer>(7);
            for (EquipmentIndex equipmentIndex = EquipmentIndex.NumAllWeaponSlots; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
            {
                Equipments.Add(equipment.GetEquipmentFromSlot(equipmentIndex));
            }
        }

        public EquipmentSerializer() 
        {
        }

        public static implicit operator EquipmentSerializer(Equipment equipment)
        {
            return new EquipmentSerializer(equipment);
        }

        public static implicit operator Equipment(EquipmentSerializer serializer)
        {
            Equipment SpawnEquipment = new Equipment();
            for (EquipmentIndex equipmentIndex = EquipmentIndex.NumAllWeaponSlots; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
            {
                SpawnEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, serializer.Equipments[(int)(equipmentIndex - 5)]);
            }
            return SpawnEquipment;
        }
    }
}
