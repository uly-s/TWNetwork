using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork
{
    public interface IGameNetworkEntity
    {
        GameNetworkMessage MessageToSend { get; set; }
    }
}
