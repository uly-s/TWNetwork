﻿using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.Messages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class ServerTick : GameNetworkMessage
    {
        private List<ServerAgentTick> serverAgentTicks = null;
        public IReadOnlyList<ServerAgentTick> ServerAgentTicks => serverAgentTicks;

        public ServerTick(Mission current)
        {
            foreach (Agent agent in current.Agents)
            {
                serverAgentTicks = new List<ServerAgentTick>();
                serverAgentTicks.Add(new ServerAgentTick(agent,agent.Position,agent.MovementFlags,agent.EventControlFlags,agent.LookDirection,agent.MovementInputVector));
            }
        }

        public ServerTick() { }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.Mission;
        }

        protected override string OnGetLogFormat()
        {
            return "ServerTick";
        }

        protected override bool OnRead()
        {
            bool result = true;
            serverAgentTicks = new List<ServerAgentTick>();
            int count = ReadIntFromPacket(new CompressionInfo.Integer(0, 15), ref result);
            for (int i = 0; i < count; i++)
            {
                Agent agent = ReadAgentReferenceFromPacket(ref result);
                Vec3 position = ReadVec3FromPacket(CompressionInfo.Float.FullPrecision,ref result);
                MovementControlFlag movementFlags = (MovementControlFlag)ReadUintFromPacket(CompressionGenericExtended.EventControlFlagCompressionInfo,ref result);
                EventControlFlag eventFlags = (EventControlFlag)ReadUintFromPacket(CompressionGenericExtended.MovementFlagCompressionInfo, ref result);
                Vec3 lookDirection = ReadVec3FromPacket(CompressionInfo.Float.FullPrecision,ref result);
                Vec2 movementInputVector = ReadVec2FromPacket(CompressionInfo.Float.FullPrecision, ref result);
                serverAgentTicks.Add(new ServerAgentTick(agent,position,movementFlags,eventFlags,lookDirection,movementInputVector));
            }
            return result;
        }

        protected override void OnWrite()
        {
            int count = (serverAgentTicks is null) ? 0 : serverAgentTicks.Count;
            WriteIntToPacket(count,new CompressionInfo.Integer(0,15));
            for (int i = 0; i < count; i++)
            {
                WriteAgentReferenceToPacket(serverAgentTicks[i].Agent);
                WriteVec3ToPacket(serverAgentTicks[i].Position, CompressionInfo.Float.FullPrecision);
                WriteUintToPacket((uint)serverAgentTicks[i].EventControlFlags, CompressionGenericExtended.EventControlFlagCompressionInfo);
                WriteUintToPacket((uint)serverAgentTicks[i].MovementFlags, CompressionGenericExtended.MovementFlagCompressionInfo);
                WriteVec3ToPacket(serverAgentTicks[i].LookDirection, CompressionInfo.Float.FullPrecision);
                WriteVec2ToPacket(serverAgentTicks[i].MovementInputVector, CompressionInfo.Float.FullPrecision);
            }
        }
    }
}