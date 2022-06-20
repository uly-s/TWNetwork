using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNetwork
{
    public class SingleMissionServer: GameNetworkServerBase
    {
        public ServerMission Mission { get; private set; }
    }
}
