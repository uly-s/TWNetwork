using System;
using System.Reflection;

namespace TWNetworkPatcher
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PatchedMethodAttribute : Attribute
    {
        public MethodInfo Method { get; private set; }
        public bool IsPrefix { get; private set; }
        public PatchedMethodAttribute(Type type,string methodName,Type[] types,bool IsPrefix)
        {
            Method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,null,types,null);
            this.IsPrefix = IsPrefix;
        }
        public PatchedMethodAttribute(Type type,string methodName,bool IsPrefix)
        {
            Method = type.GetMethod(methodName,BindingFlags.Instance |BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            this.IsPrefix = IsPrefix;
        }
    }
}