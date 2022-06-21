using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace TWNetwork
{
    internal class ImplementedMBAPI
    {
        private static Type IMBNetworkType = typeof(GameNetwork).Assembly.GetType("IMBNetwork");
    }
}
