using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNetwork.NetworkFiles
{
    public interface IServer
    {
        void Start(int port);
        void Stop();
    }
}
