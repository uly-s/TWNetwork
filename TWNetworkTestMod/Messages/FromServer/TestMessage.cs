using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetworkTestMod.Messages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class TestMessage: GameNetworkMessage
    {
        public int NumberOn3Bits { get; private set; }

        public TestMessage(int num)
        {
            NumberOn3Bits = num;
        }

        public TestMessage()
        { }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.General;
        }

        protected override string OnGetLogFormat()
        {
            return "TestMessage";
        }

        protected override bool OnRead()
        {
            bool result = true;
            NumberOn3Bits = ReadIntFromPacket(new CompressionInfo.Integer(0, 3), ref result);
            return result;
        }

        protected override void OnWrite()
        {
            WriteIntToPacket(NumberOn3Bits,new CompressionInfo.Integer(0,3));
        }
    }
}
