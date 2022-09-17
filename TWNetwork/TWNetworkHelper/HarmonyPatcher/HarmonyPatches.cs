using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TWNetworkHelper
{
    public abstract class HarmonyPatches
    {
        public bool Run { get; protected set; } = false;
        internal object Result { get; private set; } = null;
        private object instance = null;
        protected MethodInfo PatchedMethod { get; private set; }
        protected object Instance 
        { 
            get 
            {
                if (instance is null)
                {
                    throw new NotInstanceMethodException();
                }
                return instance;
            } 
        }
        internal static HarmonyPatches InvokeMethod(MethodInfo PatchedMethod,MethodInfo method, object[] args ,object Instance = null)
        {
            try
            {
                HarmonyPatches obj = (HarmonyPatches)Activator.CreateInstance(method.DeclaringType);
                obj.instance = Instance;
                obj.PatchedMethod = PatchedMethod;
                if (method.ReturnType != typeof(void))
                {
                    obj.Result = method.Invoke(obj, args);
                }
                else
                {
                    method.Invoke(obj, args);
                }
                return (HarmonyPatches)obj;
            }
            catch (TargetInvocationException ex)
            { 
                throw ex.InnerException; 
            }
        }
    }
}
