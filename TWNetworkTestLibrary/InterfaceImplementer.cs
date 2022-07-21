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
    public abstract class InterfaceImplementer : RealProxy, IRemotingTypeInfo
    {
        private readonly Type ImplementedInterfaceType;
        private readonly Dictionary<MethodInfo, MethodInfo> Methods;

        private bool MethodsEquals(MethodInfo method1,MethodInfo method2)
        {
            if (method1.Name != method2.Name || method1.ReturnType != method2.ReturnType || method1.GetParameters().Length != method2.GetParameters().Length)
                return false;
            ParameterInfo[] Params1 = method1.GetParameters();
            ParameterInfo[] Params2 = method2.GetParameters();
            for (int i=0;i<Params1.Length;i++)
            {
                if (Params1[i].ParameterType != Params2[i].ParameterType && Params1[i].ParameterType != typeof(object) && Params2[i].ParameterType != typeof(object))
                    return false;
            }
            return true;
        }
        protected InterfaceImplementer(Type implementedInterfaceType) : base(implementedInterfaceType)
        {
            if (!implementedInterfaceType.IsInterface || !implementedInterfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).All(method1 => this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).ToList().Find(method2 => MethodsEquals(method1,method2)) != null))
                throw new InvalidOperationException("The given type should be an interface and should implement all the methods that the interface implements.");
            ImplementedInterfaceType = implementedInterfaceType;
            Methods = new Dictionary<MethodInfo, MethodInfo>();
            foreach (MethodInfo m in ImplementedInterfaceType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                Methods.Add(m,this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).ToList().Find(m2 => MethodsEquals(m,m2)));
            }
        }

        private object DispatchFunction(MethodInfo Method,object[] args)
        {
            return Methods[Method].Invoke(this, args);
        }

        public override IMessage Invoke(IMessage msg)
        {
            var call = msg as IMethodCallMessage;

            if (call == null)
                throw new NotSupportedException();

            var method = (MethodInfo)call.MethodBase;

            return new ReturnMessage(DispatchFunction(method,call.Args), null, 0, call.LogicalCallContext, call);
        }

        public bool CanCastTo(Type fromType, object o) => fromType == ImplementedInterfaceType;

        public string TypeName { get; set; }
    }
}
