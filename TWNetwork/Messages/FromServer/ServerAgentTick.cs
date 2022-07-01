using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TWNetwork.Messages.FromServer
{
    public class ServerAgentTick
    {
        public Vec3 Position { get; private set; }
        public Agent.MovementControlFlag MovementFlags { get; private set; }
        public Agent.EventControlFlag EventControlFlags { get; private set; }
        public Vec2 MovementInputVector { get; private set; }
        public Vec3 LookDirection { get; private set; }
        public Agent Agent { get; private set; }

        public ServerAgentTick(Agent agent,Vec3 position, Agent.MovementControlFlag movementFlags, Agent.EventControlFlag eventControlFlags, Vec2 movementInputVector, Vec3 lookDirection)
        {
            this.Agent = agent;
            this.Position = position;
            this.MovementFlags = movementFlags;
            this.EventControlFlags = eventControlFlags;
            this.MovementInputVector = movementInputVector;
            this.LookDirection = lookDirection;
        }
    }
}