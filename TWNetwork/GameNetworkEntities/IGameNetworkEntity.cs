using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork
{
    public abstract class MissionNetworkEntity
    {
        protected GameNetworkMessage MessageToSend { get; set; }
    }
}
