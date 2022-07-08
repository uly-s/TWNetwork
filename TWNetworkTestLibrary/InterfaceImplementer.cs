using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace TWNetworkPatcher
{
    public class InterfaceImplementer : RealProxy, IRemotingTypeInfo
    {
        private readonly Type _type;
        private readonly Func<MethodInfo, object> _callback;

        public InterfaceImplementer(Type type, Func<MethodInfo, object> callback) : base(type)
        {
            _callback = callback;
            _type = type;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var call = msg as IMethodCallMessage;

            if (call == null)
                throw new NotSupportedException();

            var method = (MethodInfo)call.MethodBase;

            return new ReturnMessage(_callback(method), null, 0, call.LogicalCallContext, call);
        }

        public bool CanCastTo(Type fromType, object o) => fromType == _type;

        public string TypeName { get; set; }
    }
}
