using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace MultiplayerBattle.Messages.Serializables
{
    /// <summary>
    /// This class is a serializable class by ProtoBuf-net library and 
    /// it's responsibility is to serialize and deserialize an Agent reference,
    /// which has been already spawned in the current Mission.
    /// </summary>
    [ProtoContract]
    public class AgentSerializer
    {
        /// <value>
        /// The <c>AgentIndex</c> property represents the index
        /// for one of the Agents in a Mission.
        /// </value>
        [ProtoMember(1)]
        public int AgentIndex { get; set; }

        /// <summary>
        /// This is the default constructor and it is required for ProtoBuf to serialize the object.
        /// </summary>
        public AgentSerializer()
        { }

        /// <summary>
        /// This is a constructor with an Agent object as a parameter, we only call this constructor.
        /// </summary>
        /// <param name="agent">
        /// This is the Agent object we will send over the internet.
        /// </param>
        public AgentSerializer(Agent agent)
        {
            AgentIndex = agent.Index;
        }

        /// <summary>
        /// This is a method to return the Agent object from the AgentIndex.
        /// </summary>
        /// <returns>
        /// The return value is the Agent object, which has been sent to our computer.
        /// </returns>
        public static implicit operator Agent(AgentSerializer serializer)
        {
            return Mission.Current.FindAgentWithIndex(serializer.AgentIndex);
        }

        public static implicit operator AgentSerializer(Agent agent)
        {
            return new AgentSerializer(agent);
        }
        
    }
}
