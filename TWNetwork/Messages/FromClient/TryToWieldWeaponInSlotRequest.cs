using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.Messages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class TryToWieldWeaponInSlotRequest: GameNetworkMessage
    {
        public EquipmentIndex SlotIndex { get; private set; }
        public WeaponWieldActionType Type { get; private set; }
        public bool IsWieldedOnSpawn { get; private set; }

        public TryToWieldWeaponInSlotRequest() { }
        public TryToWieldWeaponInSlotRequest(EquipmentIndex slotIndex, WeaponWieldActionType type,bool isWieldedOnSpawn) 
        {
            SlotIndex = slotIndex;
            Type = type;
            IsWieldedOnSpawn = isWieldedOnSpawn;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Mission;
        }

        protected override string OnGetLogFormat()
        {
            return "TryToWieldWeaponInSlotRequest";
        }

        protected override bool OnRead()
        {
            bool result = true;
            SlotIndex = (EquipmentIndex)ReadIntFromPacket(new CompressionInfo.Integer(0, 4, true), ref result);
            Type = (WeaponWieldActionType)ReadIntFromPacket(new CompressionInfo.Integer(0, 3, true), ref result);
            IsWieldedOnSpawn = ReadBoolFromPacket(ref result);
            return result;
        }

        protected override void OnWrite()
        {
            WriteIntToPacket((int)SlotIndex, new CompressionInfo.Integer(0, 4, true));
            WriteIntToPacket((int)Type, new CompressionInfo.Integer(0,3,true));
            WriteBoolToPacket(IsWieldedOnSpawn);
        }
    }
}