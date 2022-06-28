using ProtoBuf;
using TaleWorlds.MountAndBlade;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class UsableMachineSerializer
    {
        [ProtoMember(1)]
        public MissionObjectSerializer UsableMachineRef { get; set; }
        public UsableMachineSerializer() { }
        public UsableMachineSerializer(UsableMachine usableMachine)
        {
            UsableMachineRef = usableMachine;
        }

        public static implicit operator UsableMachineSerializer(UsableMachine usableMachine)
        {
            return new UsableMachineSerializer(usableMachine);
        }

        public static implicit operator UsableMachine(UsableMachineSerializer serializer)
        {
            return (UsableMachine)(MissionObject)serializer.UsableMachineRef;
        }
    }
}
