using System;
using TaleWorlds.MountAndBlade;

namespace TWNetwork.NetworkFiles
{
    internal class NativeMBPeer
    {
        public NetworkCommunicator Communicator { get; private set; }
        public Agent ControlledAgent {get; set; } //Automatically set.
        public bool IsActive { get; private set; } //I don't know where to set it.
        public bool IsSynchronized { get; set; } //Automatically set
        public ushort Port { get; private set; } //IDK
        public uint Host { get; private set; } //IDK
        public uint ReversedHost { get; private set; } //IDK
        public double AverageLossPercent => 0; //Should be 0 because we use reliable UDP for everything.
        public double AveragePingInMilliSeconds { get; private set; } //Calculate somewhere at sending tick message

        private Team team;
        private SendableOptions relevantGameOptions = new SendableOptions();

        public NativeMBPeer()
        {}

        public void SetCommunicator(NetworkCommunicator communicator)
        {
            Communicator = communicator;
        }

        public void SetTeam(Team team)
        {
            this.team = team;
        }

        internal void SetRelevantGameOptions(bool sendMeBloodEvents, bool sendMeSoundEvents)
        {
            relevantGameOptions.SendBlood = sendMeBloodEvents;
            relevantGameOptions.SendSoundEffects = sendMeSoundEvents;
        }

        internal void BeginModuleEvent(bool isReliable)
        {
            throw new NotImplementedException();
        }
    }
}