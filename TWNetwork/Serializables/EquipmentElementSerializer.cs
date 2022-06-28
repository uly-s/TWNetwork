using ProtoBuf;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class EquipmentElementSerializer
    {
        [ProtoMember(1)]
        public uint ItemRef { get; set; }
        [ProtoMember(2)]
        public uint CosmeticItemRef { get; set; }

        public EquipmentElementSerializer()
        { }

        public EquipmentElementSerializer(EquipmentElement item)
        {
            ItemRef = SerializerHelper.GetReferenceFromObject(item.Item);
            CosmeticItemRef= SerializerHelper.GetReferenceFromObject(item.CosmeticItem);
        }

        public static implicit operator EquipmentElement(EquipmentElementSerializer serializer)
        {
            MBObjectBase mbobjectBase = SerializerHelper.GetObjectFromRef(serializer.ItemRef);
            MBObjectBase mbobjectBase2 = SerializerHelper.GetObjectFromRef(serializer.CosmeticItemRef);
            return new EquipmentElement(mbobjectBase as ItemObject, null, mbobjectBase2 as ItemObject, false);
        }

        public static implicit operator EquipmentElementSerializer(EquipmentElement equipmentElement)
        {
            return new EquipmentElementSerializer(equipmentElement);
        }
    }
}