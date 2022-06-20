using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNetwork
{
    public class MultipleMissionServer : GameNetworkServerBase
    {
        public Missions Missions { get; private set; }
    }
}
